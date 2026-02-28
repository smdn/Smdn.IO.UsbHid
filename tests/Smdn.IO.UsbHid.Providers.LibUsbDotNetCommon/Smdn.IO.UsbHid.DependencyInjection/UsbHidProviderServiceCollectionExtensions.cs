// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Microsoft.Extensions.DependencyInjection;

namespace Smdn.IO.UsbHid.DependencyInjection;

internal static class UsbHidProviderServiceCollectionExtensions {
  public static IServiceCollection AddUsbHidProvider(
    this IServiceCollection services
  )
    => services
#if LIBUSBDOTNET_V3
        .AddLibUsbDotNetV3UsbHid();
#else
        .AddLibUsbNullSession()
        .AddLibUsbDotNetUsbHid();
#endif
}
