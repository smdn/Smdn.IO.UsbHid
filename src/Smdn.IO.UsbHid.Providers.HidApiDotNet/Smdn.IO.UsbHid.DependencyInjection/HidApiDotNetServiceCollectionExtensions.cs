// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Smdn.IO.UsbHid.DependencyInjection;

public static class HidApiDotNetServiceCollectionExtensions {
#pragma warning disable IDE0060
  private static void ConfigureNothing<TServiceKey>(
    HidApiDotNetUsbHidServiceBuilder<TServiceKey> builder
  )
  {
    // do nothing
  }
#pragma warning restore IDE0060

  [CLSCompliant(false)]
  public static IServiceCollection AddHidApiDotNetUsbHid(
    this IServiceCollection services
  )
    => AddHidApiDotNetUsbHid<object?>(
      services: services ?? throw new ArgumentNullException(nameof(services)),
      serviceKey: null,
      selectOptionsNameForServiceKey: static _ => string.Empty /* Options.DefaultName */,
      configure: ConfigureNothing
    );

  [CLSCompliant(false)]
  public static IServiceCollection AddHidApiDotNetUsbHid(
    this IServiceCollection services,
    Action<HidApiDotNetUsbHidServiceBuilder<object?>> configure
  )
    => AddHidApiDotNetUsbHid(
      services: services ?? throw new ArgumentNullException(nameof(services)),
      serviceKey: null,
      selectOptionsNameForServiceKey: static _ => string.Empty /* Options.DefaultName */,
      configure: configure ?? throw new ArgumentNullException(nameof(configure))
    );

  [CLSCompliant(false)]
  public static IServiceCollection AddHidApiDotNetUsbHid(
    this IServiceCollection services,
    string serviceKey
  )
    => AddHidApiDotNetUsbHid(
      services: services ?? throw new ArgumentNullException(nameof(services)),
      serviceKey: serviceKey,
      selectOptionsNameForServiceKey: static key => key,
      configure: ConfigureNothing
    );

  [CLSCompliant(false)]
  public static IServiceCollection AddHidApiDotNetUsbHid(
    this IServiceCollection services,
    string serviceKey,
    Action<HidApiDotNetUsbHidServiceBuilder<string>> configure
  )
    => AddHidApiDotNetUsbHid(
      services: services ?? throw new ArgumentNullException(nameof(services)),
      serviceKey: serviceKey,
      selectOptionsNameForServiceKey: static key => key,
      configure: configure ?? throw new ArgumentNullException(nameof(configure))
    );

  [CLSCompliant(false)]
  public static IServiceCollection AddHidApiDotNetUsbHid<TServiceKey>(
    this IServiceCollection services,
    TServiceKey serviceKey,
    Func<TServiceKey, string?> selectOptionsNameForServiceKey
  )
    => AddHidApiDotNetUsbHid(
      services: services ?? throw new ArgumentNullException(nameof(services)),
      serviceKey: serviceKey,
      selectOptionsNameForServiceKey: selectOptionsNameForServiceKey ?? throw new ArgumentNullException(nameof(selectOptionsNameForServiceKey)),
      configure: ConfigureNothing
    );

  [CLSCompliant(false)]
  public static IServiceCollection AddHidApiDotNetUsbHid<TServiceKey>(
    this IServiceCollection services,
    TServiceKey serviceKey,
    Func<TServiceKey, string?> selectOptionsNameForServiceKey,
    Action<HidApiDotNetUsbHidServiceBuilder<TServiceKey>> configure
  )
  {
    if (services is null)
      throw new ArgumentNullException(nameof(services));
    if (selectOptionsNameForServiceKey is null)
      throw new ArgumentNullException(nameof(selectOptionsNameForServiceKey));
    if (configure is null)
      throw new ArgumentNullException(nameof(configure));

    var builder = new HidApiDotNetUsbHidServiceBuilder<TServiceKey>(
        services,
        serviceKey,
        selectOptionsNameForServiceKey
      );

#if false // for future extension
    var configuredOptions = new HidApiDotNetOptions();

    configure(builder, configuredOptions);

    _ = services.Configure<HidApiDotNetOptions>(
      name: builder.GetOptionsName(),
      configureOptions: options => options.Configure(configuredOptions)
    );
#else
    configure(builder);
#endif

    // register IHidApiSession as non-keyed singleton service to be shared
    // across all IUsbHidService instances
    //
    // if an IHidApiSession implementation has not been explicitly registered
    // prior to this, register HidApiDefaultSession as the IHidApiSession implementation
    services.TryAddSingleton<IHidApiSession, HidApiDefaultSession>();

    services.Add(
      ServiceDescriptor.KeyedSingleton/* <HidApiDotNetServiceBuilder<TServiceKey>> */(
        serviceKey: builder.ServiceKey,
        implementationFactory: (_, _) => builder
      )
    );

    services.Add(
      ServiceDescriptor.KeyedSingleton/* <IUsbService> */(
        serviceKey: builder.ServiceKey,
        static (serviceProvider, serviceKey)
          => serviceProvider
            .GetRequiredKeyedService<HidApiDotNetUsbHidServiceBuilder<TServiceKey>>(serviceKey)
            .Build(serviceProvider)
      )
    );

    return services;
  }
}
