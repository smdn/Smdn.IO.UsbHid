// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Microsoft.Extensions.DependencyInjection;

using Polly.Registry;

namespace Smdn.IO.UsbHid.DependencyInjection;

// The methods in this class implement the partial methods defined in
// `Smdn.IO.UsbHid.Providers.Common`. Specifically, retrieve the `LibUsbDotNetUsbHidServiceBuilder` or
// `LibUsbDotNetV3UsbHidServiceBuilder` from `IServiceProvider`, which is
// used in the common test code.
internal static partial class UsbHidProviderServiceProviderExtensions {
  public static partial UsbHidServiceBuilder<object?> GetUsbHidServiceBuilder(
    this IServiceProvider provider
  )
    => provider.GetRequiredService<
#if LIBUSBDOTNET_V3
      LibUsbDotNetV3UsbHidServiceBuilder<object?>
#else
      LibUsbDotNetUsbHidServiceBuilder<object?>
#endif
    >();

  public static partial UsbHidServiceBuilder<TServiceKey> GetUsbHidServiceBuilder<TServiceKey>(
    this IServiceProvider provider,
    TServiceKey serviceKey
  )
    => provider.GetRequiredKeyedService<
#if LIBUSBDOTNET_V3
      LibUsbDotNetV3UsbHidServiceBuilder<TServiceKey>
#else
      LibUsbDotNetUsbHidServiceBuilder<TServiceKey>
#endif
    >(serviceKey);

  public static partial ResiliencePipelineProvider<string>? GetResiliencePipelineProviderForUsbHidService(
    this IServiceProvider provider,
    object? serviceKey
  )
    => provider.
#if LIBUSBDOTNET_V3
      GetResiliencePipelineProviderForLibUsbDotNetV3UsbHidService
#else
      GetResiliencePipelineProviderForLibUsbDotNetUsbHidService
#endif
      (serviceKey);
}
