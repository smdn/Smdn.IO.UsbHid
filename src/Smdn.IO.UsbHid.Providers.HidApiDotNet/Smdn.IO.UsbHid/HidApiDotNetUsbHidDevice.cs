// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

using HidApi;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Polly;
using Polly.Registry;

namespace Smdn.IO.UsbHid;

/// <summary>
/// An implementation of <see cref="IUsbHidDevice"/> that uses
/// <see cref="DeviceInfo"/> of HidApi.Net as the backend.
/// </summary>
public sealed partial class HidApiDotNetUsbHidDevice : IUsbHidDevice<DeviceInfo> {
  /// <summary>
  /// Gets the key to retrieve the <see cref="ResiliencePipeline"/> for the
  /// <see cref="OpenEndPointAsync(bool, bool, bool, CancellationToken)"/> method
  /// from the <see cref="ResiliencePipelineProvider{TKey}"/>.
  /// </summary>
  public static string ResiliencePipelineKeyForOpenEndPoint { get; } = nameof(HidApiDotNetUsbHidDevice) + "." + nameof(resiliencePipelineOpenEndPoint);

  private const int EventIdOpenEndPoint = 10;

  private bool disposed;

  /// <inheritdoc/>
  [CLSCompliant(false)]
  public DeviceInfo UnderlyingDevice { get; } // ?? throw new ObjectDisposedException(GetType().FullName);

  /// <inheritdoc/>
  public int VendorId => UnderlyingDevice.VendorId;

  /// <inheritdoc/>
  public int ProductId => UnderlyingDevice.ProductId;

  private readonly ResiliencePipeline resiliencePipelineOpenEndPoint;
  private readonly ILogger logger;

  internal HidApiDotNetUsbHidDevice(
    DeviceInfo deviceInfo,
    ResiliencePipelineProvider<string>? resiliencePipelineProvider,
    ILogger? logger
  )
  {
    UnderlyingDevice = deviceInfo;
    this.logger = logger ?? NullLogger.Instance;

    ResiliencePipeline? resiliencePipelineOpenEndPoint = null;

    _ = resiliencePipelineProvider?.TryGetPipeline(
      ResiliencePipelineKeyForOpenEndPoint,
      out resiliencePipelineOpenEndPoint
    );

    this.resiliencePipelineOpenEndPoint = resiliencePipelineOpenEndPoint ?? ResiliencePipeline.Empty;
  }

  private void ThrowIfDisposed()
  {
    if (disposed)
      throw new ObjectDisposedException(GetType().FullName);
  }

  /// <inheritdoc/>
  public bool TryGetProductName(
    [NotNullWhen(true)] out string? productName
  )
    => (productName = UnderlyingDevice.ProductString) is not null;

  /// <inheritdoc/>
  public bool TryGetManufacturer(
    [NotNullWhen(true)] out string? manufacturer
  )
    => (manufacturer = UnderlyingDevice.ManufacturerString) is not null;

  /// <inheritdoc/>
  public bool TryGetSerialNumber(
    [NotNullWhen(true)] out string? serialNumber
  )
    => (serialNumber = UnderlyingDevice.SerialNumber) is not null;

  /// <inheritdoc/>
  public bool TryGetDeviceIdentifier(
    [NotNullWhen(true)] out string? deviceIdentifier
  )
    => (deviceIdentifier = UnderlyingDevice.Path) is not null;

  /// <inheritdoc/>
  public void Dispose()
  {
    // nothing to do
    disposed = true;
  }

  /// <inheritdoc/>
  public ValueTask DisposeAsync()
  {
    // nothing to do
    disposed = true;

    return default;
  }

  /// <inheritdoc/>
  public IUsbHidEndPoint OpenEndPoint(
    bool openOutEndPoint,
    bool openInEndPoint,
    bool shouldDisposeDevice,
    CancellationToken cancellationToken
  )
  {
    if (!openOutEndPoint && !openInEndPoint)
      throw new InvalidOperationException("At least one of the IN or OUT endpoints must be opened.");

    ThrowIfDisposed();

    cancellationToken.ThrowIfCancellationRequested();

    var resilienceContext = ResilienceContextPool.Shared.Get(cancellationToken);

    try {
      return resiliencePipelineOpenEndPoint.Execute<IUsbHidEndPoint>(
        callback: ctx => {
          LogHidApiDotNetOpenEndPoint(UnderlyingDevice.Path);

          return new HidApiDotNetUsbHidEndPoint(this, UnderlyingDevice.ConnectToDevice(), shouldDisposeDevice);
        },
        context: resilienceContext
      );
    }
    finally {
      ResilienceContextPool.Shared.Return(resilienceContext);
    }
  }

  /// <inheritdoc/>
  /// <remarks>
  /// This implementation performs a synchronous open, as the underlying
  /// HidApi.Net APIs do not support asynchronous operations for opening endpoints.
  /// </remarks>
  public ValueTask<IUsbHidEndPoint> OpenEndPointAsync(
    bool openOutEndPoint,
    bool openInEndPoint,
    bool shouldDisposeDevice,
    CancellationToken cancellationToken
  )
  {
    if (!openOutEndPoint && !openInEndPoint)
      throw new InvalidOperationException("At least one of the IN or OUT endpoints must be opened.");

    ThrowIfDisposed();

    cancellationToken.ThrowIfCancellationRequested();

    return OpenEndPointAsyncCore();

    async ValueTask<IUsbHidEndPoint> OpenEndPointAsyncCore()
    {
      var resilienceContext = ResilienceContextPool.Shared.Get(cancellationToken);

      try {
        return await resiliencePipelineOpenEndPoint.ExecuteAsync<IUsbHidEndPoint>(
          callback: ctx => {
            LogHidApiDotNetOpenEndPoint(UnderlyingDevice.Path);

            return new(
              new HidApiDotNetUsbHidEndPoint(this, UnderlyingDevice.ConnectToDevice(), shouldDisposeDevice)
            );
          },
          context: resilienceContext
        ).ConfigureAwait(false);
      }
      finally {
        ResilienceContextPool.Shared.Return(resilienceContext);
      }
    }
  }

  [LoggerMessage(
    EventId = EventIdOpenEndPoint,
    Level = LogLevel.Debug,
    Message = "Attempt to open endpoint ({Path})"
  )]
  private partial void LogHidApiDotNetOpenEndPoint(string path);

  /// <inheritdoc/>
  public override string? ToString()
    => $"{GetType().FullName} (UnderlyingDevice='{UnderlyingDevice}')";
}
