// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Microsoft.Extensions.DependencyInjection;

using Polly;
using Polly.DependencyInjection;

namespace Smdn.IO.UsbHid.DependencyInjection;

internal static class HidApiDotNetUsbHidServiceCollectionExtensions {
  private static HidApiDotNetResiliencePipelineKeyPair<TServiceKey>
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
        key: HidApiDotNetUsbHidDevice.ResiliencePipelineKeyForOpenEndPoint,
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
        HidApiDotNetResiliencePipelineKeyPair<TServiceKey>
      >
    > configure
  )
  {
    if (services is null)
      throw new ArgumentNullException(nameof(services));

    services
      .AddResiliencePipeline(
        serviceKey: serviceKey,
        pipelineKey: HidApiDotNetUsbHidDevice.ResiliencePipelineKeyForOpenEndPoint,
        createResiliencePipelineKeyPair: CreateResiliencePipelineKeyPair,
        configure: configure
      );

    return services;
  }
}
