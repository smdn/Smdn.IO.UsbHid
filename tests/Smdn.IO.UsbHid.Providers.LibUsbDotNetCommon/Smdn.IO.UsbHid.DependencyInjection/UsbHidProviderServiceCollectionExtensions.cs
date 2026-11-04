// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Smdn.IO.UsbHid.DependencyInjection;

// The methods in this class implement the partial methods defined in
// `Smdn.IO.UsbHid.Providers.Common`. Specifically, they add `HidSharpUsbHidService`
// as an implementation of `IUsbHidService`, which is used in common test code.
internal static partial class UsbHidProviderServiceCollectionExtensions {
  public static partial IServiceCollection AddUsbHidProvider(
    this IServiceCollection services
  )
    => services
#if LIBUSBDOTNET_V3
        .AddLibUsbDotNetV3UsbHid
#else
        .AddLibUsbNullSession()
        .AddLibUsbDotNetUsbHid
#endif
        (
          configure: static (_, options) => {
            options.DebugLevel = LogLevel.None;
          }
        );

  public static partial IServiceCollection AddUsbHidProvider(
    this IServiceCollection services,
    string serviceKey
  )
    => services
#if LIBUSBDOTNET_V3
        .AddLibUsbDotNetV3UsbHid
#else
        .AddLibUsbNullSession()
        .AddLibUsbDotNetUsbHid
#endif
        (
          serviceKey: serviceKey,
          configure: static (_, options) => {
            options.DebugLevel = LogLevel.None;
          }
        );

  public static partial IServiceCollection AddUsbHidProvider<TServiceKey>(
    this IServiceCollection services,
    TServiceKey serviceKey,
    Func<TServiceKey, string?> selectOptionsNameForServiceKey
  )
    => services
#if LIBUSBDOTNET_V3
        .AddLibUsbDotNetV3UsbHid
#else
        .AddLibUsbNullSession()
        .AddLibUsbDotNetUsbHid
#endif
        (
          serviceKey: serviceKey,
          selectOptionsNameForServiceKey: selectOptionsNameForServiceKey,
          configure: static (_, options) => {
            options.DebugLevel = LogLevel.None;
          }
        );

  public static partial IServiceCollection AddUsbHidProvider(
    this IServiceCollection services,
    Action<UsbHidServiceBuilder<object?>> configure
  )
    => services
#if LIBUSBDOTNET_V3
        .AddLibUsbDotNetV3UsbHid
#else
        .AddLibUsbNullSession()
        .AddLibUsbDotNetUsbHid
#endif
        (
          configure: configure is null
            ? null!
            : (builder, options) => {
              options.DebugLevel = LogLevel.None;
              configure(builder);
            }
        );

  public static partial IServiceCollection AddUsbHidProvider(
    this IServiceCollection services,
    string serviceKey,
    Action<UsbHidServiceBuilder<string>> configure
  )
    => services
#if LIBUSBDOTNET_V3
        .AddLibUsbDotNetV3UsbHid
#else
        .AddLibUsbNullSession()
        .AddLibUsbDotNetUsbHid
#endif
        (
          serviceKey: serviceKey,
          configure: configure is null
            ? null!
            : (builder, options) => {
              options.DebugLevel = LogLevel.None;
              configure(builder);
            }
        );

  public static partial IServiceCollection AddUsbHidProvider<TServiceKey>(
    this IServiceCollection services,
    TServiceKey serviceKey,
    Func<TServiceKey, string?> selectOptionsNameForServiceKey,
    Action<UsbHidServiceBuilder<TServiceKey>> configure
  )
    => services
#if LIBUSBDOTNET_V3
        .AddLibUsbDotNetV3UsbHid
#else
        .AddLibUsbNullSession()
        .AddLibUsbDotNetUsbHid
#endif
        (
          serviceKey: serviceKey,
          selectOptionsNameForServiceKey: selectOptionsNameForServiceKey,
          configure: configure is null
            ? null!
            : (builder, options) => {
              options.DebugLevel = LogLevel.None;
              configure(builder);
            }
        );
}
