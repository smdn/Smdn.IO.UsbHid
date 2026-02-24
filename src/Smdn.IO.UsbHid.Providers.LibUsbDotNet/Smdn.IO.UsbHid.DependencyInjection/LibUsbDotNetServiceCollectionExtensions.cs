// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#pragma warning disable SA1008, SA1110

#if LIBUSBDOTNET_V3
#pragma warning disable SA1649 // warning SA1649: File name should match first type name
#endif

using System;

using Microsoft.Extensions.DependencyInjection;
#if !LIBUSBDOTNET_V3
using Microsoft.Extensions.DependencyInjection.Extensions;
#endif

namespace Smdn.IO.UsbHid.DependencyInjection;

public static class
#if LIBUSBDOTNET_V3
LibUsbDotNetV3ServiceCollectionExtensions
#else
LibUsbDotNetServiceCollectionExtensions
#endif
{
#pragma warning disable IDE0060
  private static void ConfigureNothing<TServiceKey>(
#if LIBUSBDOTNET_V3
    LibUsbDotNetV3UsbHidServiceBuilder<TServiceKey> builder,
    LibUsbDotNetV3Options options
#else
    LibUsbDotNetUsbHidServiceBuilder<TServiceKey> builder,
    LibUsbDotNetOptions options
#endif
  )
  {
    // do nothing
  }
#pragma warning restore IDE0060

  [CLSCompliant(false)]
  public static IServiceCollection
#if LIBUSBDOTNET_V3
  AddLibUsbDotNetV3UsbHid
#else
  AddLibUsbDotNetUsbHid
#endif
  (
    this IServiceCollection services
  )
#if LIBUSBDOTNET_V3
    => AddLibUsbDotNetV3UsbHid
#else
    => AddLibUsbDotNetUsbHid
#endif
    <object?>(
      services: services ?? throw new ArgumentNullException(nameof(services)),
      serviceKey: null,
      selectOptionsNameForServiceKey: static _ => string.Empty /* Options.DefaultName */,
      configure: ConfigureNothing
    );

  [CLSCompliant(false)]
  public static IServiceCollection
#if LIBUSBDOTNET_V3
  AddLibUsbDotNetV3UsbHid
#else
  AddLibUsbDotNetUsbHid
#endif
  (
    this IServiceCollection services,
    Action<
#if LIBUSBDOTNET_V3
      LibUsbDotNetV3UsbHidServiceBuilder<object?>,
      LibUsbDotNetV3Options
#else
      LibUsbDotNetUsbHidServiceBuilder<object?>,
      LibUsbDotNetOptions
#endif
    > configure
  )
#if LIBUSBDOTNET_V3
    => AddLibUsbDotNetV3UsbHid
#else
    => AddLibUsbDotNetUsbHid
#endif
    (
      services: services ?? throw new ArgumentNullException(nameof(services)),
      serviceKey: null,
      selectOptionsNameForServiceKey: static _ => string.Empty /* Options.DefaultName */,
      configure: configure ?? throw new ArgumentNullException(nameof(configure))
    );

  [CLSCompliant(false)]
  public static IServiceCollection
#if LIBUSBDOTNET_V3
  AddLibUsbDotNetV3UsbHid
#else
  AddLibUsbDotNetUsbHid
#endif
  (
    this IServiceCollection services,
    string serviceKey
  )
#if LIBUSBDOTNET_V3
    => AddLibUsbDotNetV3UsbHid
#else
    => AddLibUsbDotNetUsbHid
#endif
    (
      services: services ?? throw new ArgumentNullException(nameof(services)),
      serviceKey: serviceKey,
      selectOptionsNameForServiceKey: static key => key,
      configure: ConfigureNothing
    );

  [CLSCompliant(false)]
  public static IServiceCollection
#if LIBUSBDOTNET_V3
  AddLibUsbDotNetV3UsbHid
#else
  AddLibUsbDotNetUsbHid
#endif
  (
    this IServiceCollection services,
    string serviceKey,
    Action<
#if LIBUSBDOTNET_V3
      LibUsbDotNetV3UsbHidServiceBuilder<string>,
      LibUsbDotNetV3Options
#else
      LibUsbDotNetUsbHidServiceBuilder<string>,
      LibUsbDotNetOptions
#endif
    > configure
  )
#if LIBUSBDOTNET_V3
    => AddLibUsbDotNetV3UsbHid
#else
    => AddLibUsbDotNetUsbHid
#endif
    (
      services: services ?? throw new ArgumentNullException(nameof(services)),
      serviceKey: serviceKey,
      selectOptionsNameForServiceKey: static key => key,
      configure: configure ?? throw new ArgumentNullException(nameof(configure))
    );

  [CLSCompliant(false)]
  public static IServiceCollection
#if LIBUSBDOTNET_V3
  AddLibUsbDotNetV3UsbHid
#else
  AddLibUsbDotNetUsbHid
#endif
  <TServiceKey>
  (
    this IServiceCollection services,
    TServiceKey serviceKey,
    Func<TServiceKey, string?> selectOptionsNameForServiceKey
  )
#if LIBUSBDOTNET_V3
    => AddLibUsbDotNetV3UsbHid
#else
    => AddLibUsbDotNetUsbHid
#endif
    (
      services: services ?? throw new ArgumentNullException(nameof(services)),
      serviceKey: serviceKey,
      selectOptionsNameForServiceKey: selectOptionsNameForServiceKey ?? throw new ArgumentNullException(nameof(selectOptionsNameForServiceKey)),
      configure: ConfigureNothing
    );

  [CLSCompliant(false)]
  public static IServiceCollection
#if LIBUSBDOTNET_V3
  AddLibUsbDotNetV3UsbHid
#else
  AddLibUsbDotNetUsbHid
#endif
  <TServiceKey>
  (
    this IServiceCollection services,
    TServiceKey serviceKey,
    Func<TServiceKey, string?> selectOptionsNameForServiceKey,
    Action<
#if LIBUSBDOTNET_V3
      LibUsbDotNetV3UsbHidServiceBuilder<TServiceKey>,
      LibUsbDotNetV3Options
#else
      LibUsbDotNetUsbHidServiceBuilder<TServiceKey>,
      LibUsbDotNetOptions
#endif
    > configure
  )
  {
    if (services is null)
      throw new ArgumentNullException(nameof(services));
    if (selectOptionsNameForServiceKey is null)
      throw new ArgumentNullException(nameof(selectOptionsNameForServiceKey));
    if (configure is null)
      throw new ArgumentNullException(nameof(configure));

    var builder = new
#if LIBUSBDOTNET_V3
      LibUsbDotNetV3UsbHidServiceBuilder<TServiceKey>
#else
      LibUsbDotNetUsbHidServiceBuilder<TServiceKey>
#endif
      (
        services,
        serviceKey,
        selectOptionsNameForServiceKey
      );
    var configuredOptions = new
#if LIBUSBDOTNET_V3
      LibUsbDotNetV3Options();
#else
      LibUsbDotNetOptions();
#endif

    configure(builder, configuredOptions);

    _ = services.Configure<
#if LIBUSBDOTNET_V3
      LibUsbDotNetV3Options
#else
      LibUsbDotNetOptions
#endif
    >(
      name: builder.GetOptionsName(),
      configureOptions: options => options.Configure(configuredOptions)
    );

#if !LIBUSBDOTNET_V3
    // register ILibUsbSession as non-keyed singleton service to be shared
    // across all IUsbHidService instances
    //
    // if an ILibUsbSession implementation has not been explicitly registered
    // prior to this, register LibUsbDefaultSession as the ILibUsbSession implementation
    services.TryAddSingleton<ILibUsbSession, LibUsbDefaultSession>();
#endif

    services.Add(
      ServiceDescriptor.KeyedSingleton/* <LibUsbDotNetServiceBuilder<TServiceKey>> */(
        serviceKey: builder.ServiceKey,
        implementationFactory: (_, _) => builder
      )
    );

    services.Add(
      ServiceDescriptor.KeyedSingleton/* <IUsbService> */(
        serviceKey: builder.ServiceKey,
        static (serviceProvider, serviceKey)
          => serviceProvider
            .GetRequiredKeyedService<
#if LIBUSBDOTNET_V3
              LibUsbDotNetV3UsbHidServiceBuilder<TServiceKey>
#else
              LibUsbDotNetUsbHidServiceBuilder<TServiceKey>
#endif
            >(serviceKey)
            .Build(serviceProvider)
      )
    );

    return services;
  }
}
