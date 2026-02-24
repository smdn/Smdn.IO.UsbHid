// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#pragma warning disable SA1008, SA1110, SA1114

#if LIBUSBDOTNET_V3
#pragma warning disable SA1649 // warning SA1649: File name should match first type name
#endif

using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Smdn.IO.UsbHid.DependencyInjection;

#if LIBUSBDOTNET_V3
/// <summary>
/// A builder for <see cref="LibUsbDotNetV3UsbHidService"/>.
/// </summary>
#else
/// <summary>
/// A builder for <see cref="LibUsbDotNetUsbHidService"/>.
/// </summary>
#endif
[CLSCompliant(false)]

#pragma warning disable IDE0055
public sealed class
#if LIBUSBDOTNET_V3
LibUsbDotNetV3UsbHidServiceBuilder<TServiceKey>
#else
LibUsbDotNetUsbHidServiceBuilder<TServiceKey>
#endif
  : UsbHidServiceBuilder<TServiceKey>
{
#pragma warning restore IDE0055
  internal
#if LIBUSBDOTNET_V3
  LibUsbDotNetV3UsbHidServiceBuilder
#else
  LibUsbDotNetUsbHidServiceBuilder
#endif
  (
    IServiceCollection services,
    TServiceKey serviceKey,
    Func<TServiceKey, string?> selectOptionsNameForServiceKey
  ) : base(
      services: services,
      serviceKey: serviceKey,
      selectOptionsNameForServiceKey: selectOptionsNameForServiceKey
    )
  {
  }

  /// <inheritdoc/>
  public override IUsbHidService Build(
    IServiceProvider serviceProvider
  )
  {
    var options = (serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider)))
      .GetRequiredService<
        IOptionsMonitor<
#if LIBUSBDOTNET_V3
          LibUsbDotNetV3Options
#else
          LibUsbDotNetOptions
#endif
        >
      >()
      .Get(name: GetOptionsName());

    return new
#if LIBUSBDOTNET_V3
      LibUsbDotNetV3UsbHidService
#else
      LibUsbDotNetUsbHidService
#endif
      (
        options: options,
#if !LIBUSBDOTNET_V3
        // ILibUsbSession is a non-keyed singleton, and all
        // LibUsbDotNetUsbHidService instances share it
        libUsbSession: serviceProvider.GetRequiredService<ILibUsbSession>(),
#endif
        resiliencePipelineProvider: serviceProvider
#if LIBUSBDOTNET_V3
          .GetResiliencePipelineProviderForLibUsbDotNetV3UsbHidService
#else
          .GetResiliencePipelineProviderForLibUsbDotNetUsbHidService
#endif
          (
            serviceKey: ServiceKey
          ),
        loggerFactory:
          // Attempt to get the ILoggerFactory with the specified service key
          serviceProvider.GetKeyedService<ILoggerFactory>(ServiceKey) ??
          // After that, attempt to get the default ILoggerFactory
          serviceProvider.GetService<ILoggerFactory>()
      );
  }
}
