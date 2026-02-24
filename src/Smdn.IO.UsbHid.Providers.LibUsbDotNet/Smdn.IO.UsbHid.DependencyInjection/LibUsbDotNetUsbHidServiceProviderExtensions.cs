// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#pragma warning disable SA1008, SA1110

#if LIBUSBDOTNET_V3
#pragma warning disable SA1649 // warning SA1649: File name should match first type name
#endif

using System;

using Polly;
using Polly.Registry;

namespace Smdn.IO.UsbHid.DependencyInjection;

public static class
#if LIBUSBDOTNET_V3
LibUsbDotNetV3UsbHidServiceProviderExtensions
#else
LibUsbDotNetUsbHidServiceProviderExtensions
#endif
{
  [CLSCompliant(false)] // ResiliencePipelineProvider is CLS incompliant
  public static ResiliencePipelineProvider<string>?
#if LIBUSBDOTNET_V3
  GetResiliencePipelineProviderForLibUsbDotNetV3UsbHidService
#else
  GetResiliencePipelineProviderForLibUsbDotNetUsbHidService
#endif
  (
    this IServiceProvider serviceProvider,
    object? serviceKey
  )
    => (serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider))).GetKeyedResiliencePipelineProvider<string>(
      serviceKey: serviceKey,
      typeOfKeyPair: typeof(
#if LIBUSBDOTNET_V3
        LibUsbDotNetV3ResiliencePipelineKeyPair<>
#else
        LibUsbDotNetResiliencePipelineKeyPair<>
#endif
      )
    );
}
