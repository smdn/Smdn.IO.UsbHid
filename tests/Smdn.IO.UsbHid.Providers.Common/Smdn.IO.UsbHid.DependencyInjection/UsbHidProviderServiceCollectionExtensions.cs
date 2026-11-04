// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Microsoft.Extensions.DependencyInjection;

namespace Smdn.IO.UsbHid.DependencyInjection;

// For partial methods declared in this class, the code for adding the
// USB HID service (IUsbHidService) is expected to be implemented in the
// test assembly for each individual provider library. The common test
// code written in this directory performs tests using the IUsbHidService
// added by this method.
internal static partial class UsbHidProviderServiceCollectionExtensions {
  public static partial IServiceCollection AddUsbHidProvider(
    this IServiceCollection services
  );

  public static partial IServiceCollection AddUsbHidProvider(
    this IServiceCollection services,
    string serviceKey
  );

  public static partial IServiceCollection AddUsbHidProvider<TServiceKey>(
    this IServiceCollection services,
    TServiceKey serviceKey,
    Func<TServiceKey, string?> selectOptionsNameForServiceKey
  );

  public static partial IServiceCollection AddUsbHidProvider(
    this IServiceCollection services,
    Action<UsbHidServiceBuilder<object?>> configure
  );

  public static partial IServiceCollection AddUsbHidProvider(
    this IServiceCollection services,
    string serviceKey,
    Action<UsbHidServiceBuilder<string>> configure
  );

  public static partial IServiceCollection AddUsbHidProvider<TServiceKey>(
    this IServiceCollection services,
    TServiceKey serviceKey,
    Func<TServiceKey, string?> selectOptionsNameForServiceKey,
    Action<UsbHidServiceBuilder<TServiceKey>> configure
  );
}
