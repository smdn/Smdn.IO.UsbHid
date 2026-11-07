// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Microsoft.Extensions.DependencyInjection;

namespace Smdn.IO.UsbHid.DependencyInjection;

// The methods in this class implement the partial methods defined in
// `Smdn.IO.UsbHid.Providers.Common`. Specifically, they add `HidSharpUsbHidService`
// as an implementation of `IUsbHidService`, which is used in common test code.
internal static partial class UsbHidProviderServiceCollectionExtensions {
  public static partial IServiceCollection AddUsbHidProvider(
    this IServiceCollection services
  )
    => services.AddHidApiDotNetUsbHid();

  public static partial IServiceCollection AddUsbHidProvider(
    this IServiceCollection services,
    string serviceKey
  )
    => services.AddHidApiDotNetUsbHid(
      serviceKey: serviceKey
    );

  public static partial IServiceCollection AddUsbHidProvider<TServiceKey>(
    this IServiceCollection services,
    TServiceKey serviceKey,
    Func<TServiceKey, string?> selectOptionsNameForServiceKey
  )
    => services.AddHidApiDotNetUsbHid(
      serviceKey: serviceKey,
      selectOptionsNameForServiceKey: selectOptionsNameForServiceKey
    );

  public static partial IServiceCollection AddUsbHidProvider(
    this IServiceCollection services,
    Action<UsbHidServiceBuilder<object?>> configure
  )
    => services.AddHidApiDotNetUsbHid(
      configure: configure is null
        ? null!
        : builder => configure(builder)
    );

  public static partial IServiceCollection AddUsbHidProvider(
    this IServiceCollection services,
    string serviceKey,
    Action<UsbHidServiceBuilder<string>> configure
  )
    => services.AddHidApiDotNetUsbHid(
      serviceKey: serviceKey,
      configure: configure is null
        ? null!
        : builder => configure(builder)
    );

  public static partial IServiceCollection AddUsbHidProvider<TServiceKey>(
    this IServiceCollection services,
    TServiceKey serviceKey,
    Func<TServiceKey, string?> selectOptionsNameForServiceKey,
    Action<UsbHidServiceBuilder<TServiceKey>> configure
  )
    => services.AddHidApiDotNetUsbHid(
      serviceKey: serviceKey,
      selectOptionsNameForServiceKey: selectOptionsNameForServiceKey,
      configure: configure is null
        ? null!
        : builder => configure(builder)
    );
}
