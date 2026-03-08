// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
#if !(SYSTEM_IO_STREAM_READ_SPAN_OF_BYTE || SYSTEM_IO_STREAM_READASYNC_MEMORY_OF_BYTE || SYSTEM_IO_STREAM_WRITE_READONLYSPAN_OF_BYTE || SYSTEM_IO_STREAM_WRITE_READONLYSPAN_OF_BYTE)
using System.Buffers;
using System.Runtime.InteropServices;
#endif
using System.Threading;
using System.Threading.Tasks;

using HidSharp;

namespace Smdn.IO.UsbHid;

/// <summary>
/// An implementation of <see cref="IUsbHidEndPoint"/> that
/// uses <see cref="HidStream"/> of HidSharp as the backend.
/// </summary>
public sealed class HidSharpUsbHidEndPoint : IUsbHidEndPoint<HidStream, HidStream> {
  private readonly bool shouldDisposeDevice;

  private HidSharpUsbHidDevice? device;

  /// <inheritdoc/>
  public IUsbHidDevice Device => device ?? throw new ObjectDisposedException(GetType().Name);

  private HidStream? endPointStream;
  private HidStream EndPointStream => endPointStream ?? throw new ObjectDisposedException(GetType().Name);

  /// <inheritdoc/>
  public bool CanRead => EndPointStream.CanRead;

  /// <inheritdoc/>
  public bool CanWrite => EndPointStream.CanWrite;

  /// <inheritdoc/>
  [CLSCompliant(false)]
  public HidStream? ReadEndPoint => EndPointStream;

  /// <inheritdoc/>
  [CLSCompliant(false)]
  public HidStream? WriteEndPoint => EndPointStream;

  private int MaxOutputReportLength => EndPointStream.Device.GetMaxOutputReportLength();
  private int MaxInputReportLength => EndPointStream.Device.GetMaxInputReportLength();

  internal HidSharpUsbHidEndPoint(
    HidSharpUsbHidDevice device,
    HidStream stream,
    bool shouldDisposeDevice
  )
  {
    this.device = device ?? throw new ArgumentNullException(nameof(device));
    endPointStream = stream ?? throw new ArgumentNullException(nameof(stream));
    this.shouldDisposeDevice = shouldDisposeDevice;
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

    endPointStream?.Dispose();
    endPointStream = null;
  }

  /// <inheritdoc/>
#if NET || NETSTANDARD2_1_OR_GREATER
  public async ValueTask DisposeAsync()
  {
    if (shouldDisposeDevice && device is not null) {
      await device.DisposeAsync().ConfigureAwait(false);
      device = null;
    }

    if (endPointStream is not null) {
      await endPointStream.DisposeAsync().ConfigureAwait(false);
      endPointStream = null;
    }
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
    if (MaxOutputReportLength < buffer.Length)
      throw new ArgumentException($"length of the buffer must be less than or equals to maximum output report length ({MaxOutputReportLength})", nameof(buffer));

    ThrowIfDisposed();

    cancellationToken.ThrowIfCancellationRequested();

#if SYSTEM_IO_STREAM_WRITE_READONLYSPAN_OF_BYTE
    // Stream.Write(ReadOnlySpan<byte>) does not take CancellationToken
    EndPointStream.Write(buffer);
#else
    var len = buffer.Length;
    var buf = ArrayPool<byte>.Shared.Rent(MaxOutputReportLength);

    try {
      buffer.CopyTo(buf);

      EndPointStream.Write(buf, 0, len);
    }
    finally {
      ArrayPool<byte>.Shared.Return(buf);
    }
#endif
  }

#if SYSTEM_IO_STREAM_WRITEASYNC_READONLYMEMORY_OF_BYTE
  /// <inheritdoc/>
#else
  /// <inheritdoc/>
  /// <remarks>
  /// This implementation performs a synchronous write, as the
  /// underlying <see cref="HidStream"/> does not
  /// support asynchronous operations.
  /// </remarks>
#endif
  public ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
  {
    if (MaxOutputReportLength < buffer.Length)
      throw new ArgumentException($"length of the buffer must be less than or equals to maximum output report length ({MaxOutputReportLength})", nameof(buffer));

    ThrowIfDisposed();

    cancellationToken.ThrowIfCancellationRequested();

#if SYSTEM_IO_STREAM_WRITEASYNC_READONLYMEMORY_OF_BYTE
    return EndPointStream.WriteAsync(buffer, cancellationToken);
#else
    var len = buffer.Length;
    var buf = ArrayPool<byte>.Shared.Rent(MaxOutputReportLength);

    try {
      buffer.CopyTo(buf);

      EndPointStream.Write(buf, 0, len);
    }
    finally {
      ArrayPool<byte>.Shared.Return(buf);
    }

#if SYSTEM_THREADING_TASKS_VALUETASK_COMPLETEDTASK
    return ValueTask.CompletedTask;
#else
    return default;
#endif
#endif
  }

  /// <inheritdoc/>
  public int Read(Span<byte> buffer, CancellationToken cancellationToken)
  {
    if (MaxInputReportLength < buffer.Length)
      throw new ArgumentException($"length of the buffer must be less than or equals to maximum input report length ({MaxInputReportLength})", nameof(buffer));

    ThrowIfDisposed();

    cancellationToken.ThrowIfCancellationRequested();

#if SYSTEM_IO_STREAM_READ_SPAN_OF_BYTE
    // Stream.Read(Span<byte>) does not take CancellationToken
    return EndPointStream.Read(buffer);
#else
    var len = buffer.Length;
    var buf = ArrayPool<byte>.Shared.Rent(MaxInputReportLength);

    try {
      var ret = EndPointStream.Read(buf, 0, len);

      buf.AsSpan(0, ret).CopyTo(buffer);

      return ret;
    }
    finally {
      ArrayPool<byte>.Shared.Return(buf);
    }
#endif
  }

#if SYSTEM_IO_STREAM_READASYNC_MEMORY_OF_BYTE
  /// <inheritdoc/>
#else
  /// <inheritdoc/>
  /// <remarks>
  /// This implementation performs a synchronous read, as the
  /// underlying <see cref="HidStream"/> does not
  /// support asynchronous operations.
  /// </remarks>
#endif
  public ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken)
  {
    if (MaxInputReportLength < buffer.Length)
      throw new ArgumentException($"length of the buffer must be less than or equals to maximum input report length ({MaxInputReportLength})", nameof(buffer));

    ThrowIfDisposed();

    cancellationToken.ThrowIfCancellationRequested();

#if SYSTEM_IO_STREAM_READASYNC_MEMORY_OF_BYTE
    return EndPointStream.ReadAsync(buffer, cancellationToken);
#else
    var len = buffer.Length;
    byte[]? rentBuffer = null;

    if (!MemoryMarshal.TryGetArray<byte>(buffer, out var buf)) {
      rentBuffer = ArrayPool<byte>.Shared.Rent(MaxInputReportLength);

      buf = new ArraySegment<byte>(rentBuffer, 0, len);
    }

    try {
      return
#pragma warning disable SA1114
#if SYSTEM_THREADING_TASKS_VALUETASK_FROMRESULT
        ValueTask.FromResult<int>(
#else
        new(
#endif
#pragma warning restore SA1114
          result: EndPointStream.Read(buf.Array ?? throw new InvalidOperationException("destination array is null"), buf.Offset, buf.Count)
        );
    }
    finally {
      if (rentBuffer != null) {
        buf.AsMemory().CopyTo(buffer);

        ArrayPool<byte>.Shared.Return(rentBuffer);
      }
    }
#endif
  }

  public override string? ToString()
    => $"{GetType().FullName} (EndPointImplementation='{EndPointStream}')";
}
