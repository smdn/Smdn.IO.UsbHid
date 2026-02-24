// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#if LIBUSBDOTNET_V3
#pragma warning disable SA1649 // warning SA1649: File name should match first type name
#endif

using System;

using Microsoft.Extensions.DependencyInjection;

using Polly;
using Polly.DependencyInjection;

namespace Smdn.IO.UsbHid.DependencyInjection;

internal static class
#if LIBUSBDOTNET_V3
LibUsbDotNetV3UsbHidServiceCollectionExtensions
#else
LibUsbDotNetUsbHidServiceCollectionExtensions
#endif
{
  private static
#if LIBUSBDOTNET_V3
  LibUsbDotNetV3ResiliencePipelineKeyPair
#else
  LibUsbDotNetResiliencePipelineKeyPair
#endif
  <TServiceKey>
  CreateResiliencePipelineKeyPair<TServiceKey>(TServiceKey serviceKey, string pipelineKey)
    => new(serviceKey, pipelineKey);

  // /* non-public */ [CLSCompliant(false)] // RetryStrategyOptions is not CLS compliant
  public static IServiceCollection AddResiliencePipelineForOpenEndPoint(
    this IServiceCollection services,
    Action<ResiliencePipelineBuilder, AddResiliencePipelineContext<string>> configure
  )
  {
    if (services is null)
      throw new ArgumentNullException(nameof(services));

    services
      .AddResiliencePipeline(
        key:
#if LIBUSBDOTNET_V3
          LibUsbDotNetV3UsbHidDevice.ResiliencePipelineKeyForOpenEndPoint,
#else
          LibUsbDotNetUsbHidDevice.ResiliencePipelineKeyForOpenEndPoint,
#endif
        configure: configure
      );

    return services;
  }

  // /* non-public */ [CLSCompliant(false)] // RetryStrategyOptions is not CLS compliant
  public static IServiceCollection AddResiliencePipelineForOpenEndPoint<TServiceKey>(
    this IServiceCollection services,
    TServiceKey serviceKey,
    Action<
      ResiliencePipelineBuilder,
      AddResiliencePipelineContext<
#if LIBUSBDOTNET_V3
        LibUsbDotNetV3ResiliencePipelineKeyPair<TServiceKey>
#else
        LibUsbDotNetResiliencePipelineKeyPair<TServiceKey>
#endif
      >
    > configure
  )
  {
    if (services is null)
      throw new ArgumentNullException(nameof(services));

    services
      .AddResiliencePipeline(
        serviceKey: serviceKey,
        pipelineKey:
#if LIBUSBDOTNET_V3
          LibUsbDotNetV3UsbHidDevice.ResiliencePipelineKeyForOpenEndPoint,
#else
          LibUsbDotNetUsbHidDevice.ResiliencePipelineKeyForOpenEndPoint,
#endif
        createResiliencePipelineKeyPair: CreateResiliencePipelineKeyPair,
        configure: configure
      );

    return services;
  }
}
