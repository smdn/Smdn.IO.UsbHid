// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using System.Buffers;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using LibUsbDotNet.LibUsb;

namespace Smdn.IO.UsbHid;

/// <summary>
/// An implementation of <see cref="IUsbHidEndPoint"/> that uses
/// <see cref="UsbEndpointReader"/> and <see cref="UsbEndpointWriter"/> of
/// LibUsbDotNet as the backend.
/// </summary>
public sealed class LibUsbDotNetV3UsbHidEndPoint : IUsbHidEndPoint<UsbEndpointReader, UsbEndpointWriter> {
  private const int LengthOfReportId = 1;

  private readonly bool shouldDisposeDevice;

  private LibUsbDotNetV3UsbHidDevice? device;

  /// <inheritdoc/>
  public IUsbHidDevice Device => device ?? throw new ObjectDisposedException(GetType().Name);

  private bool IsDisposed => device is null;

  /// <inheritdoc/>
  public bool CanWrite => !IsDisposed && WriteEndPoint is not null;

  /// <inheritdoc/>
  public bool CanRead => !IsDisposed && ReadEndPoint is not null;

  /// <inheritdoc/>
  [CLSCompliant(false)]
  public UsbEndpointWriter? WriteEndPoint { get; private set; }

  /// <inheritdoc/>
  [CLSCompliant(false)]
  public UsbEndpointReader? ReadEndPoint { get; private set; }

  private readonly int maxOutEndPointPacketSize;
  private readonly int maxInEndPointPacketSize;

  private readonly int writeEndPointTimeoutInMilliseconds;
  private readonly int readEndPointTimeoutInMilliseconds;

  /// <summary>
  /// Gets or sets the value indicating whether the actual read length
  /// includes the report ID length (1 byte).
  /// </summary>
  /// <value>
  /// <see langword="true"/> if the report ID is determined to be included;
  /// <see langword="false"/> if it is determined not to be included;
  /// <see langword="null"/> if it cannot be determined either way.
  /// </value>
  /// <remarks>
  /// On Windows, the actual length read and set in the `transferred` pointer
  /// parameter of `libusb_bulk_transfer` and `libusb_interrupt_transfer`
  /// may include the length of the report ID. If a read operation on an
  /// endpoint determines that the read length includes the report ID,
  /// this value is set to `true`.
  /// </remarks>
  private bool? AssumesActualReadLengthIncludesReportId { get; set; }

  internal LibUsbDotNetV3UsbHidEndPoint(
    LibUsbDotNetV3UsbHidDevice device,
    UsbEndpointWriter? endPointWriter,
    int maxOutEndPointPacketSize,
    TimeSpan writeEndPointTimeout,
    UsbEndpointReader? endPointReader,
    int maxInEndPointPacketSize,
    TimeSpan readEndPointTimeout,
    bool shouldDisposeDevice
  )
  {
    this.device = device ?? throw new ArgumentNullException(nameof(device));
    ReadEndPoint = endPointReader;
    WriteEndPoint = endPointWriter;
    this.maxInEndPointPacketSize = maxInEndPointPacketSize;
    readEndPointTimeoutInMilliseconds = (int)readEndPointTimeout.TotalMilliseconds;
    this.maxOutEndPointPacketSize = maxOutEndPointPacketSize;
    writeEndPointTimeoutInMilliseconds = (int)writeEndPointTimeout.TotalMilliseconds;
    this.shouldDisposeDevice = shouldDisposeDevice;

    // As of now, on platforms other than Windows, it is considered that the
    // read length does not include the report ID if the ID is 0.
    if (!IsRunningOnWindows())
      AssumesActualReadLengthIncludesReportId = false;

    static bool IsRunningOnWindows()
      =>
#if SYSTEM_OPERATINGSYSTEM_ISOSPLATFORM
        OperatingSystem.IsWindows();
#else
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#endif
  }

  private void ThrowIfDisposed()
  {
    if (device is null)
      throw new ObjectDisposedException(GetType().FullName);
  }

  /// <inheritdoc/>
  public void Dispose()
  {
    if (shouldDisposeDevice) {
      device?.Dispose();
      device = null;
    }

    // UsbEndpointWriter/UsbEndpointReader does not implement IDisposable
    WriteEndPoint = null;
    ReadEndPoint = null;
  }

  /// <inheritdoc/>
  /// <remarks>
  /// This implementation performs a synchronous disposal, as the
  /// underlying <see cref="UsbEndpointReader"/> and <see cref="UsbEndpointWriter"/>
  /// do not support asynchronous disposal.
  /// </remarks>
#if NET || NETSTANDARD2_1_OR_GREATER
  public async ValueTask DisposeAsync()
  {
    if (shouldDisposeDevice && device is not null) {
      await device.DisposeAsync().ConfigureAwait(false);
      device = null;
    }

    // UsbEndpointWriter/UsbEndpointReader does not implement IDisposable
    WriteEndPoint = null;
    ReadEndPoint = null;
  }
#else
  public ValueTask DisposeAsync()
  {
    Dispose();

    return default;
  }
#endif

  /// <inheritdoc/>
  public void Write(ReadOnlySpan<byte> buffer, CancellationToken cancellationToken)
  {
    if (buffer.IsEmpty)
      return;

    if (LengthOfReportId + maxOutEndPointPacketSize < buffer.Length)
      throw new ArgumentException($"length of the buffer must be less than or equals to maximum output packet length ({maxOutEndPointPacketSize})", nameof(buffer));

    if (buffer[0] == 0x00) // If the report ID is 0, send only the payload
      buffer = buffer.Slice(LengthOfReportId);

    ThrowIfDisposed();

    if (WriteEndPoint is null)
      throw new InvalidOperationException("can not write");

    cancellationToken.ThrowIfCancellationRequested();

    WriteCore(
      WriteEndPoint,
      buffer,
      writeEndPointTimeoutInMilliseconds
    );

    static void WriteCore(
      UsbEndpointWriter writer,
      ReadOnlySpan<byte> buffer,
      int timeout
    )
    {
      // since UsbEndpointWriter.Write() requires Span<byte> rather than ReadOnlySpan<byte>,
      // copy the data to be written into a separately allocated Span<byte>
      var rentBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length);
      var buf = rentBuffer.AsSpan(0, buffer.Length);

      buffer.CopyTo(buf);

      try {
        _ = writer.Write(
          buffer: buf,
          timeout: timeout,
          transferLength: out var transferLength
        );
      }
      finally {
        if (rentBuffer is not null)
          ArrayPool<byte>.Shared.Return(rentBuffer);
      }
    }
  }

  /// <inheritdoc/>
  public ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
  {
    if (buffer.IsEmpty)
      return default;

    if (LengthOfReportId + maxOutEndPointPacketSize < buffer.Length)
      throw new ArgumentException($"length of the buffer must be less than or equals to maximum output packet length ({maxOutEndPointPacketSize})", nameof(buffer));

    if (buffer.Span[0] == 0x00) // If the report ID is 0, send only the payload
      buffer = buffer.Slice(LengthOfReportId);

    ThrowIfDisposed();

    if (WriteEndPoint is null)
      throw new InvalidOperationException("can not write");

    cancellationToken.ThrowIfCancellationRequested();

    return WriteAsyncCore(
      WriteEndPoint,
      buffer,
      writeEndPointTimeoutInMilliseconds
    );

    static async ValueTask WriteAsyncCore(
      UsbEndpointWriter writer,
      ReadOnlyMemory<byte> buffer,
      int timeout
    )
    {
      byte[]? rentBuffer = null;

      if (!MemoryMarshal.TryGetArray(buffer, out var bufferSegment)) {
        rentBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length);

        bufferSegment = new ArraySegment<byte>(rentBuffer, 0, buffer.Length);

        buffer.CopyTo(bufferSegment.AsMemory(0, buffer.Length));
      }

      try {
        _ = await writer.WriteAsync(
          buffer: bufferSegment.Array,
          offset: bufferSegment.Offset,
          length: bufferSegment.Count,
          timeout: timeout
        ).ConfigureAwait(false);
      }
      finally {
        if (rentBuffer is not null)
          ArrayPool<byte>.Shared.Return(rentBuffer);
      }
    }
  }

  /// <inheritdoc/>
  /// <remarks>
  /// The first byte of the <paramref name="buffer"/>, which is for the Report ID, is ignored
  /// in the current implementation.
  /// </remarks>
  public int Read(Span<byte> buffer, CancellationToken cancellationToken)
  {
    if (buffer.IsEmpty)
      return 0;

    if (LengthOfReportId + maxInEndPointPacketSize < buffer.Length)
      throw new ArgumentException($"length of the buffer must be less than or equals to maximum input packet length ({maxInEndPointPacketSize})", nameof(buffer));

    ThrowIfDisposed();

    if (ReadEndPoint is null)
      throw new InvalidOperationException("can not read");

    cancellationToken.ThrowIfCancellationRequested();

    // TODO: The current implementation assumes that the report ID is 0
    // and that the read data does not include the report ID.
    buffer = buffer.Slice(LengthOfReportId);

    _ = ReadEndPoint.Read(
      buffer: buffer,
      timeout: readEndPointTimeoutInMilliseconds,
      out var transferLength
    );

    return CalculateActualReadLength(
      endPoint: this,
      requestedReadLengthExcludeReportId: buffer.Length,
      actualReadLength: transferLength
    );
  }

  /// <inheritdoc/>
  /// <remarks>
  /// The first byte of the <paramref name="buffer"/>, which is for the Report ID, is ignored
  /// in the current implementation.
  /// </remarks>
  public ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken)
  {
    if (buffer.IsEmpty) {
      return
#if SYSTEM_THREADING_TASKS_VALUETASK_FROMRESULT
        ValueTask.FromResult(result: 0);
#else
        new(result: 0);
#endif
    }

    if (LengthOfReportId + maxInEndPointPacketSize < buffer.Length)
      throw new ArgumentException($"length of the buffer must be less than or equals to maximum input packet length ({maxInEndPointPacketSize})", nameof(buffer));

    ThrowIfDisposed();

    if (ReadEndPoint is null)
      throw new InvalidOperationException("can not read");

    cancellationToken.ThrowIfCancellationRequested();

    return ReadAsyncCore(
      thisEndPoint: this,
      reader: ReadEndPoint,
      buffer: buffer,
      timeout: readEndPointTimeoutInMilliseconds
    );

    static async ValueTask<int> ReadAsyncCore(
      LibUsbDotNetV3UsbHidEndPoint thisEndPoint,
      UsbEndpointReader reader,
      Memory<byte> buffer,
      int timeout
    )
    {
      // TODO: The current implementation assumes that the report ID is 0
      // and that the read data does not include the report ID.
      buffer = buffer.Slice(LengthOfReportId);

      var (_, transferLength) = await reader.ReadAsync(
        buffer: buffer,
        timeout: timeout
      ).ConfigureAwait(false);

      return CalculateActualReadLength(
        endPoint: thisEndPoint,
        requestedReadLengthExcludeReportId: buffer.Length,
        actualReadLength: transferLength
      );
    }
  }

  private static int CalculateActualReadLength(
    LibUsbDotNetV3UsbHidEndPoint endPoint,
    int requestedReadLengthExcludeReportId,
    int actualReadLength
  )
  {
    if (!endPoint.AssumesActualReadLengthIncludesReportId.HasValue) {
      // If the actual read length is equal to '(Report ID + requested read
      // data length)', the report ID is considered to be included in the
      // actual read length (e.g., if a request was made to read a 64-byte
      // payload and the actual read length turned out to be 65 bytes).
      if (LengthOfReportId + requestedReadLengthExcludeReportId == actualReadLength)
        endPoint.AssumesActualReadLengthIncludesReportId = true;
    }

    if (endPoint.AssumesActualReadLengthIncludesReportId ?? false)
      return actualReadLength;
    else
      return LengthOfReportId + actualReadLength;
  }

  public override string? ToString()
    => $"{GetType().FullName} (Device='{device}', ReadEndPoint='{ReadEndPoint}', WriteEndPoint='{WriteEndPoint}')";
}
