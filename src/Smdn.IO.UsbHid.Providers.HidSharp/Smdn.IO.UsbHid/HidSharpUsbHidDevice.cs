// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES || SYSTEM_DIAGNOSTICS_CODEANALYSIS_MEMBERNOTNULLATTRIBUTE
using System.Diagnostics.CodeAnalysis;
#endif
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using HidSharp;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Polly;
using Polly.Registry;

namespace Smdn.IO.UsbHid;

/// <summary>
/// An implementation of <see cref="IUsbHidDevice"/> that uses
/// <see cref="HidDevice"/> of HidSharp as the backend.
/// </summary>
public sealed partial class HidSharpUsbHidDevice : IUsbHidDevice<HidDevice> {
  /// <summary>
  /// Gets the key to retrieve the <see cref="ResiliencePipeline"/> for
  /// the <see cref="OpenEndPointAsync(bool, bool, bool, CancellationToken)"/> method
  /// from the <see cref="ResiliencePipelineProvider{TKey}"/>.
  /// </summary>
  public static string ResiliencePipelineKeyForOpenEndPoint { get; } = nameof(HidSharpUsbHidDevice) + "." + nameof(resiliencePipelineOpenEndPoint);

  private const int EventIdOpenEndPoint = 10;
  private const int EventIdGetDeviceInfo = 20;

  private HidDevice? underlyingDevice;

  /// <inheritdoc/>
  [CLSCompliant(false)]
  public HidDevice UnderlyingDevice => underlyingDevice ?? throw new ObjectDisposedException(GetType().FullName);

  /// <inheritdoc/>
  public int VendorId => UnderlyingDevice.VendorID;

  /// <inheritdoc/>
  public int ProductId => UnderlyingDevice.ProductID;

  private readonly ILogger logger;
  private readonly ResiliencePipeline resiliencePipelineOpenEndPoint;

  internal HidSharpUsbHidDevice(
    HidDevice device,
    ResiliencePipelineProvider<string>? resiliencePipelineProvider,
    ILogger? logger
  )
  {
    underlyingDevice = device ?? throw new ArgumentNullException(nameof(device));
    this.logger = logger ?? NullLogger.Instance;

    ResiliencePipeline? resiliencePipelineOpenEndPoint = null;

    _ = resiliencePipelineProvider?.TryGetPipeline(
      ResiliencePipelineKeyForOpenEndPoint,
      out resiliencePipelineOpenEndPoint
    );

    this.resiliencePipelineOpenEndPoint = resiliencePipelineOpenEndPoint ?? ResiliencePipeline.Empty;
  }

#if SYSTEM_DIAGNOSTICS_CODEANALYSIS_MEMBERNOTNULLATTRIBUTE
  [MemberNotNull(nameof(underlyingDevice))]
#endif
  private void ThrowIfDisposed()
  {
    if (underlyingDevice is null)
      throw new ObjectDisposedException(GetType().FullName);
  }

  /// <inheritdoc/>
  /// <remarks>
  /// This implementation may additionally suppress <see cref="IOException"/> or
  /// <see cref="UnauthorizedAccessException"/> and return <see langword="false"/>.
  /// </remarks>
  public bool TryGetProductName(
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
    [NotNullWhen(true)]
#endif
    out string? productName
  )
  {
    productName = default;

    try {
      productName = UnderlyingDevice.GetProductName();
    }
    catch (IOException ex) {
      LogUsbHidGetDeviceInfo(ex, nameof(IOException), "product name");

      return false;
    }
    catch (UnauthorizedAccessException ex) {
      LogUsbHidGetDeviceInfo(ex, nameof(UnauthorizedAccessException), "product name");

      return false;
    }

    return productName is not null;
  }

  /// <inheritdoc/>
  /// <remarks>
  /// This implementation may additionally suppress <see cref="IOException"/> or
  /// <see cref="UnauthorizedAccessException"/> and return <see langword="false"/>.
  /// </remarks>
  public bool TryGetManufacturer(
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
    [NotNullWhen(true)]
#endif
    out string? manufacturer
  )
  {
    manufacturer = default;

    try {
      manufacturer = UnderlyingDevice.GetManufacturer();
    }
    catch (IOException ex) {
      LogUsbHidGetDeviceInfo(ex, nameof(IOException), "manufacturer name");

      return false;
    }
    catch (UnauthorizedAccessException ex) {
      LogUsbHidGetDeviceInfo(ex, nameof(UnauthorizedAccessException), "manufacturer name");

      return false;
    }

    return manufacturer is not null;
  }

  /// <inheritdoc/>
  /// <remarks>
  /// This implementation may additionally suppress <see cref="IOException"/> or
  /// <see cref="UnauthorizedAccessException"/> and return <see langword="false"/>.
  /// </remarks>
  public bool TryGetSerialNumber(
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
    [NotNullWhen(true)]
#endif
    out string? serialNumber
  )
  {
    serialNumber = default;

    try {
      serialNumber = UnderlyingDevice.GetSerialNumber();
    }
    catch (IOException ex) {
      LogUsbHidGetDeviceInfo(ex, nameof(IOException), "serial number");

      return false;
    }
    catch (UnauthorizedAccessException ex) {
      LogUsbHidGetDeviceInfo(ex, nameof(UnauthorizedAccessException), "serial number");

      return false;
    }

    return serialNumber is not null;
  }

  [LoggerMessage(
    EventId = EventIdGetDeviceInfo,
    Level = LogLevel.Trace,
    Message = "{ExceptionName} occurred while getting {DeviceInfo}."
  )]
  private partial void LogUsbHidGetDeviceInfo(Exception ex, string exceptionName, string deviceInfo);

  /// <inheritdoc/>
  public bool TryGetDeviceIdentifier(
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
    [NotNullWhen(true)]
#endif
    out string? deviceIdentifier
  )
    => (deviceIdentifier = UnderlyingDevice.DevicePath) is not null;

  /// <inheritdoc/>
  public void Dispose()
  {
    // HidDevice does not implement IDisposable
    underlyingDevice = null;
  }

  /// <inheritdoc/>
  public ValueTask DisposeAsync()
  {
    // HidDevice does not implement IDisposable
    underlyingDevice = null;

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

    var resilienceContext = ResilienceContextPool.Shared.Get(cancellationToken);

    try {
      return resiliencePipelineOpenEndPoint.Execute(
        callback: ctx => {
          LogUsbHidOpenEndPoint(UnderlyingDevice);

          return new HidSharpUsbHidEndPoint(this, UnderlyingDevice.Open(), shouldDisposeDevice);
        },
        context: resilienceContext
      );
    }
    finally {
      ResilienceContextPool.Shared.Return(resilienceContext);
    }
  }

  /// <inheritdoc/>
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

    return OpenEndPointCoreAsync();

    async ValueTask<IUsbHidEndPoint> OpenEndPointCoreAsync()
    {
      var resilienceContext = ResilienceContextPool.Shared.Get(cancellationToken);

      try {
        return await resiliencePipelineOpenEndPoint.ExecuteAsync(
          callback: ctx => {
            LogUsbHidOpenEndPoint(UnderlyingDevice);

            var endPoint = new HidSharpUsbHidEndPoint(this, UnderlyingDevice.Open(), shouldDisposeDevice);

            return
#if SYSTEM_THREADING_TASKS_VALUETASK_FROMRESULT
              ValueTask.FromResult(endPoint);
#else
              new ValueTask<IUsbHidEndPoint>(result: endPoint);
#endif
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
    Message = "Attempt to open endpoint ({Device})"
  )]
  private partial void LogUsbHidOpenEndPoint(HidDevice device);

  /// <inheritdoc/>
  public override string? ToString()
    => $"{GetType().FullName} (UnderlyingDevice='{UnderlyingDevice}')";
}
