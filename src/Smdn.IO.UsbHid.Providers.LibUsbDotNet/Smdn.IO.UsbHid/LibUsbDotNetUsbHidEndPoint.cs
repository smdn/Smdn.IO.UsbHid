// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#if LIBUSBDOTNET_V3
#error This file was written for LibUsbDotNet v2. It cannot be built for v3.
#endif

using System;
using System.Buffers;
#if !SYSTEM_OPERATINGSYSTEM_ISOSPLATFORM
using System.Runtime.InteropServices;
#endif
using System.Threading;
using System.Threading.Tasks;

using LibUsbDotNet;

namespace Smdn.IO.UsbHid;

/// <summary>
/// An implementation of <see cref="IUsbHidEndPoint"/> that uses
/// <see cref="UsbEndpointReader"/> and <see cref="UsbEndpointWriter"/> of
/// LibUsbDotNet as the backend.
/// </summary>
public sealed class LibUsbDotNetUsbHidEndPoint : IUsbHidEndPoint<UsbEndpointReader, UsbEndpointWriter> {
  private const int LengthOfReportId = 1;

  private readonly bool shouldDisposeDevice;

  private LibUsbDotNetUsbHidDevice? device;

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

  internal LibUsbDotNetUsbHidEndPoint(
    LibUsbDotNetUsbHidDevice device,
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
      var rentBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length);

      buffer.CopyTo(rentBuffer.AsSpan());

      try {
        _ = writer.Write(
          buffer: rentBuffer,
          offset: 0,
          count: buffer.Length,
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
  /// <remarks>
  /// This implementation performs a synchronous write, as the underlying
  /// <see cref="UsbEndpointWriter"/> does not support asynchronous operations.
  /// </remarks>
  public ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
  {
    Write(buffer.Span, cancellationToken);

    return default;
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

    return ReadCore(
      thisEndPoint: this,
      reader: ReadEndPoint,
      buffer: buffer,
      timeout: readEndPointTimeoutInMilliseconds
    );

    static int ReadCore(
      LibUsbDotNetUsbHidEndPoint thisEndPoint,
      UsbEndpointReader reader,
      Span<byte> buffer,
      int timeout
    )
    {
      var rentBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length);

      try {
        // TODO: The current implementation assumes that the report ID is 0
        // and that the read data does not include the report ID.
        _ = reader.Read(
          buffer: rentBuffer,
          offset: LengthOfReportId,
          count: buffer.Length - LengthOfReportId,
          timeout: timeout,
          out var transferLength
        );

        rentBuffer.AsSpan(0, transferLength).CopyTo(buffer);

        return CalculateActualReadLength(
          endPoint: thisEndPoint,
          requestedReadLengthExcludeReportId: buffer.Length,
          actualReadLength: transferLength
        );
      }
      finally {
        if (rentBuffer is not null)
          ArrayPool<byte>.Shared.Return(rentBuffer);
      }
    }
  }

  /// <inheritdoc/>
  /// <remarks>
  /// <para>
  /// The first byte of the <paramref name="buffer"/>, which is for the Report ID, is ignored
  /// in the current implementation.
  /// </para>
  /// <para>
  /// This implementation performs a synchronous write, as the underlying
  /// <see cref="UsbEndpointReader"/> does not support asynchronous operations.
  /// </para>
  /// </remarks>
  public ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken)
#pragma warning disable SA1114
#if SYSTEM_THREADING_TASKS_VALUETASK_FROMRESULT
    => ValueTask.FromResult<int>(
#else
    => new(
#endif
#pragma warning disable CA2000
      result: Read(buffer.Span, cancellationToken)
    );

  private static int CalculateActualReadLength(
    LibUsbDotNetUsbHidEndPoint endPoint,
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
