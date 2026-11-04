// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

using HidApi;

namespace Smdn.IO.UsbHid;

/// <summary>
/// An implementation of <see cref="IUsbHidEndPoint"/> that uses
/// <see cref="Device"/> of HidApi.Net as the backend.
/// </summary>
public sealed class HidApiDotNetUsbHidEndPoint : IUsbHidEndPoint<Device, Device> {
  private const int LengthOfReportId = 1;

  private readonly bool shouldDisposeDevice;

  private HidApiDotNetUsbHidDevice? device;
  private Device? underlyingDevice;

  /// <inheritdoc/>
  public IUsbHidDevice Device => device ?? throw new ObjectDisposedException(GetType().Name);

  private bool IsDisposed => device is null;

  /// <inheritdoc/>
  public bool CanWrite => !IsDisposed && WriteEndPoint is not null;

  /// <inheritdoc/>
  public bool CanRead => !IsDisposed && ReadEndPoint is not null;

  /// <inheritdoc/>
  [CLSCompliant(false)]
  public Device? ReadEndPoint => underlyingDevice;

  /// <inheritdoc/>
  [CLSCompliant(false)]
  public Device? WriteEndPoint => underlyingDevice;

  internal HidApiDotNetUsbHidEndPoint(
    HidApiDotNetUsbHidDevice device,
    Device underlyingDevice,
    bool shouldDisposeDevice
  )
  {
    this.device = device ?? throw new ArgumentNullException(nameof(device));
    this.underlyingDevice = underlyingDevice ?? throw new ArgumentNullException(nameof(underlyingDevice));
    this.shouldDisposeDevice = shouldDisposeDevice;
  }

  [MemberNotNull(nameof(device))]
  [MemberNotNull(nameof(underlyingDevice))]
  private void ThrowIfDisposed()
  {
    if (device is null || underlyingDevice is null)
      throw new ObjectDisposedException(GetType().FullName);
  }

  /// <inheritdoc/>
  public void Dispose()
  {
    if (shouldDisposeDevice) {
      device?.Dispose();
      device = null;
    }

    underlyingDevice?.Dispose();
    underlyingDevice = null;
  }

  /// <inheritdoc/>
  /// <remarks>
  /// This implementation performs a synchronous disposal, as the underlying
  /// <see cref="Device"/> does not support asynchronous disposal.
  /// </remarks>
  public async ValueTask DisposeAsync()
  {
    if (shouldDisposeDevice && device is not null) {
      await device.DisposeAsync().ConfigureAwait(false);
      device = null;
    }

    underlyingDevice?.Dispose();
    underlyingDevice = null;
  }

  /// <inheritdoc/>
  public void Write(ReadOnlySpan<byte> buffer, CancellationToken cancellationToken)
  {
    if (buffer.IsEmpty)
      return;

    ThrowIfDisposed();

    cancellationToken.ThrowIfCancellationRequested();

    underlyingDevice.Write(buffer);
  }

  /// <inheritdoc/>
  /// <remarks>
  /// This implementation performs a synchronous write, as the underlying
  /// <see cref="Device"/> does not support asynchronous operations.
  /// </remarks>
  public ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
  {
    Write(buffer.Span, cancellationToken);

    return default;
  }

  /// <inheritdoc/>
  public int Read(Span<byte> buffer, CancellationToken cancellationToken)
  {
    if (buffer.IsEmpty)
      return 0;

    ThrowIfDisposed();

    cancellationToken.ThrowIfCancellationRequested();

    // TODO: The current implementation assumes that the report ID is 0
    // and that the read data does not include the report ID.
    var readLength = underlyingDevice.Read(buffer.Slice(LengthOfReportId));

    return readLength == 0
      ? 0 // no data available
      : readLength + LengthOfReportId;
  }

  /// <inheritdoc/>
  /// <remarks>
  /// This implementation performs a synchronous write, as the underlying
  /// <see cref="Device"/> does not support asynchronous operations.
  /// </remarks>
  public ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken)
    => new(
      result: Read(buffer.Span, cancellationToken)
    );

  public override string? ToString()
    => $"{GetType().FullName} (Device='{device}', ReadEndPoint='{ReadEndPoint}', WriteEndPoint='{WriteEndPoint}')";
}
