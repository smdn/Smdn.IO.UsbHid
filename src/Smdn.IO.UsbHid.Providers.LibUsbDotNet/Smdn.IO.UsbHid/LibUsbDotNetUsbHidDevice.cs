// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#if LIBUSBDOTNET_V3
#error This file was written for LibUsbDotNet v2. It cannot be built for v3.
#endif

using System;
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
using System.Diagnostics.CodeAnalysis;
#endif
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using LibUsbDotNet;
using LibUsbDotNet.Descriptors;
using LibUsbDotNet.Info;
using LibUsbDotNet.Main;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Polly;
using Polly.Registry;

using Smdn.IO.UsbHid.Logging;

namespace Smdn.IO.UsbHid;

/// <summary>
/// An implementation of <see cref="IUsbHidDevice"/> that uses
/// <see cref="UsbDevice"/> of LibUsbDotNet as the backend.
/// </summary>
public sealed partial class LibUsbDotNetUsbHidDevice : IUsbHidDevice<UsbDevice> {
  /// <summary>
  /// Gets the key to retrieve the <see cref="ResiliencePipeline"/> for the
  /// <see cref="OpenEndPointAsync(bool, bool, bool, CancellationToken)"/> method
  /// from the <see cref="ResiliencePipelineProvider{TKey}"/>.
  /// </summary>
  public static string ResiliencePipelineKeyForOpenEndPoint { get; } = nameof(LibUsbDotNetUsbHidDevice) + "." + nameof(resiliencePipelineOpenEndPoint);

  private readonly LibUsbDotNetUsbHidService service;

  private UsbDevice? underlyingDevice;

  /// <inheritdoc/>
  [CLSCompliant(false)]
  public UsbDevice UnderlyingDevice => underlyingDevice ?? throw new ObjectDisposedException(GetType().FullName);

  /// <inheritdoc/>
  public int VendorId => UnderlyingDevice.Info.Descriptor.VendorID;

  /// <inheritdoc/>
  public int ProductId => UnderlyingDevice.Info.Descriptor.ProductID;

  private readonly ResiliencePipeline resiliencePipelineOpenEndPoint;
  private readonly ILogger logger;

  internal LibUsbDotNetUsbHidDevice(
    LibUsbDotNetUsbHidService service,
    UsbDevice device,
    ResiliencePipelineProvider<string>? resiliencePipelineProvider,
    ILogger? logger
  )
  {
    this.service = service ?? throw new ArgumentNullException(nameof(service));
    underlyingDevice = device ?? throw new ArgumentNullException(nameof(device));
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
    if (underlyingDevice is null)
      throw new ObjectDisposedException(GetType().FullName);
  }

  /// <inheritdoc/>
  /// <remarks>
  /// If the device is not open, this method attempts to open it.
  /// </remarks>
  public bool TryGetProductName(
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
    [NotNullWhen(true)]
#endif
    out string? productName
  )
  {
    productName = default;

    if (!UnderlyingDevice.IsOpen && !UnderlyingDevice.Open())
      return false;

    return (productName = UnderlyingDevice.Info.ProductString) is not null;
  }

  /// <inheritdoc/>
  /// <remarks>
  /// If the device is not open, this method attempts to open it.
  /// </remarks>
  public bool TryGetManufacturer(
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
    [NotNullWhen(true)]
#endif
    out string? manufacturer
  )
  {
    manufacturer = default;

    if (!UnderlyingDevice.IsOpen && !UnderlyingDevice.Open())
      return false;

    return (manufacturer = UnderlyingDevice.Info.ManufacturerString) is not null;
  }

  /// <inheritdoc/>
  /// <remarks>
  /// If the device is not open, this method attempts to open it.
  /// </remarks>
  public bool TryGetSerialNumber(
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
    [NotNullWhen(true)]
#endif
    out string? serialNumber
  )
  {
    serialNumber = default;

    if (!UnderlyingDevice.IsOpen && !UnderlyingDevice.Open())
      return false;

    return (serialNumber = UnderlyingDevice.Info.SerialString) is not null;
  }

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
    _ = underlyingDevice?.Close();
    underlyingDevice = null;
  }

  /// <inheritdoc/>
  /// <remarks>
  /// This implementation performs a synchronous disposal, as the
  /// underlying <see cref="UsbDevice"/> does not support asynchronous disposal.
  /// </remarks>
  public ValueTask DisposeAsync()
  {
    // UsbDevice does not implement IAsyncDisposable
    _ = underlyingDevice?.Close();
    underlyingDevice = null;

    return default;
  }

  private LibUsbDotNetUsbHidEndPoint OpenEndPointCore(
    bool openOutEndPoint,
    bool openInEndPoint,
    bool shouldDisposeDevice,
    CancellationToken cancellationToken
  )
  {
    if (!openOutEndPoint && !openInEndPoint)
      throw new InvalidOperationException("At least one of the IN or OUT endpoints must be opened.");

    UsbConfigInfo? config = null;
    UsbInterfaceInfo? hidInterface = null;
    UsbEndpointInfo? outEndpoint = null;
    UsbEndpointInfo? inEndpoint = null;

    const byte EndpointAddressInOutBitMask  = 0b_1000_0000;
    const byte EndpointAddressIn            = 0b_1000_0000;
    const byte EndpointAddressOut           = 0b_0000_0000;
    // const byte EndpointAddressNumberMask    = 0b_0000_0111;
    const byte AttributesTransferTypeMask   = 0b_0000_0011;

    foreach (var cfg in UnderlyingDevice.Configs) {
      foreach (var iface in cfg.InterfaceInfoList) {
        if (iface.Descriptor.Class == ClassCodeType.Hid) {
          config = cfg;
          hidInterface = iface;
          outEndpoint = iface.EndpointInfoList.FirstOrDefault(
            static ep => (ep.Descriptor.EndpointID & EndpointAddressInOutBitMask) == EndpointAddressOut
          );
          inEndpoint = iface.EndpointInfoList.FirstOrDefault(
            static ep => (ep.Descriptor.EndpointID & EndpointAddressInOutBitMask) == EndpointAddressIn
          );

          break;
        }
      }

      if (hidInterface is not null)
        break;
    }

    if (config is null)
      throw new UsbHidException("USB configuration not found");
    if (hidInterface is null)
      throw new UsbHidException("HID interface not found");
    if (openOutEndPoint && outEndpoint is null)
      throw new UsbHidException("HID OUT endpoint not found");
    if (openInEndPoint && inEndpoint is null)
      throw new UsbHidException("HID IN endpoint not found");

    cancellationToken.ThrowIfCancellationRequested();

    if (!UnderlyingDevice.IsOpen) {
      LogUsbHidOpenEndPointAttemptToOpen(UnderlyingDevice.DevicePath);

      UnderlyingDevice.Open();
    }

    // try set configuration
    if (UnderlyingDevice is IUsbDevice wholeUsbDevice) {
      ReadOnlySpan<byte> configs = config.Descriptor.ConfigID == 0
        ? [0]
        : [config.Descriptor.ConfigID, 0 /* fallback */];

      foreach (var cfg in configs) {
        try {
          LogUsbHidOpenEndPointAttemptToSetConfiguration(hidInterface.Descriptor.InterfaceID, hidInterface.InterfaceString, cfg);

          if (!wholeUsbDevice.SetConfiguration(cfg))
            continue;
        }
        catch (Exception ex) {
          LogUsbHidOpenEndPointSetConfigurationFailed(ex, hidInterface.Descriptor.InterfaceID, hidInterface.InterfaceString, cfg);
          throw; // unexpected
        }
      }

      // try claim HID interface
      try {
        wholeUsbDevice.ClaimInterface(hidInterface.Descriptor.InterfaceID);
      }
      catch (Exception ex) {
        LogUsbHidOpenEndPointClaimInterfaceFailed(ex, hidInterface.Descriptor.InterfaceID, hidInterface.InterfaceString);
        throw;
      }
    }

    // open HID endpoint
    return new(
      device: this,
      endPointWriter: openOutEndPoint
        ? UnderlyingDevice.OpenEndpointWriter(
            writeEndpointID: (WriteEndpointID)outEndpoint!.Descriptor.EndpointID,
            endpointType: (EndpointType)(outEndpoint.Descriptor.Attributes & AttributesTransferTypeMask)
          )
        : null,
      maxOutEndPointPacketSize: openInEndPoint
        ? outEndpoint!.Descriptor.MaxPacketSize
        : 0,
      writeEndPointTimeout: service.Options.WriteEndPointTimeout,
      endPointReader: openInEndPoint
        ? UnderlyingDevice.OpenEndpointReader(
            readBufferSize: service.Options.ReadEndPointBufferSize,
            readEndpointID: (ReadEndpointID)inEndpoint!.Descriptor.EndpointID,
            endpointType: (EndpointType)(inEndpoint.Descriptor.Attributes & AttributesTransferTypeMask)
          )
        : null,
      maxInEndPointPacketSize: openInEndPoint
        ? inEndpoint!.Descriptor.MaxPacketSize
        : 0,
      readEndPointTimeout: service.Options.ReadEndPointTimeout,
      shouldDisposeDevice: shouldDisposeDevice
    );
  }

  [LoggerMessage(
    EventId = EventIds.OpenEndPointAttemptToOpen,
    Level = Microsoft.Extensions.Logging.LogLevel.Debug,
    Message = "Attempt to open device (Device: #{Device})"
  )]
  private partial void LogUsbHidOpenEndPointAttemptToOpen(
    string device
  );

  [LoggerMessage(
    EventId = EventIds.OpenEndPointAttemptToSetConfiguration,
    Level = Microsoft.Extensions.Logging.LogLevel.Debug,
    Message = "Attempt to set configuration (HID interface #{Number}, {Iface}): Configuration #{Configuration}."
  )]
  private partial void LogUsbHidOpenEndPointAttemptToSetConfiguration(
    int number,
    string iface,
    int configuration
  );

  [LoggerMessage(
    EventId = EventIds.OpenEndPointSetConfigurationFailed,
    Level = Microsoft.Extensions.Logging.LogLevel.Critical,
    Message = "Attempt to open endpoint (HID interface #{Number}, {Iface}): Set configuration #{Configuration} failed."
  )]
  private partial void LogUsbHidOpenEndPointSetConfigurationFailed(
    Exception ex,
    int number,
    string iface,
    int configuration
  );

  [LoggerMessage(
    EventId = EventIds.OpenEndPointClaimInterfaceFailed,
    Level = Microsoft.Extensions.Logging.LogLevel.Critical,
    Message = "Open endpoint failed (HID interface #{Number}, {Iface}): Claim interface #{Number} failed."
  )]
  private partial void LogUsbHidOpenEndPointClaimInterfaceFailed(
    Exception ex,
    int number,
    string iface
  );

  /// <inheritdoc/>
  public IUsbHidEndPoint OpenEndPoint(
    bool openOutEndPoint,
    bool openInEndPoint,
    bool shouldDisposeDevice,
    CancellationToken cancellationToken
  )
  {
    ThrowIfDisposed();

    cancellationToken.ThrowIfCancellationRequested();

    var resilienceContext = ResilienceContextPool.Shared.Get(cancellationToken);

    try {
      return resiliencePipelineOpenEndPoint.Execute(
        callback: ctx => OpenEndPointCore(
          openOutEndPoint: openOutEndPoint,
          openInEndPoint: openInEndPoint,
          shouldDisposeDevice: shouldDisposeDevice,
          cancellationToken: cancellationToken
        ),
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
  /// LibUsbDotNet APIs do not support asynchronous operations for opening endpoints.
  /// </remarks>
  public ValueTask<IUsbHidEndPoint> OpenEndPointAsync(
    bool openOutEndPoint,
    bool openInEndPoint,
    bool shouldDisposeDevice,
    CancellationToken cancellationToken
  )
  {
    ThrowIfDisposed();

    cancellationToken.ThrowIfCancellationRequested();

    return OpenEndPointAsyncCore();

    async ValueTask<IUsbHidEndPoint> OpenEndPointAsyncCore()
    {
      var resilienceContext = ResilienceContextPool.Shared.Get(cancellationToken);

      try {
        return await resiliencePipelineOpenEndPoint.ExecuteAsync(
          callback: ctx =>
#pragma warning disable SA1114
#if SYSTEM_THREADING_TASKS_VALUETASK_FROMRESULT
            ValueTask.FromResult<IUsbHidEndPoint>(
#else
            new ValueTask<IUsbHidEndPoint>(
#endif
#pragma warning disable CA2000
              result: OpenEndPointCore(
                openOutEndPoint: openOutEndPoint,
                openInEndPoint: openInEndPoint,
                shouldDisposeDevice: shouldDisposeDevice,
                cancellationToken: cancellationToken
              )
#pragma warning restore CA2000
            ),
#pragma warning restore SA1114
          context: resilienceContext
        ).ConfigureAwait(false);
      }
      finally {
        ResilienceContextPool.Shared.Return(resilienceContext);
      }
    }
  }

  /// <inheritdoc/>
  public override string? ToString()
    => $"{GetType().FullName} (UnderlyingDevice='{UnderlyingDevice}')";
}
