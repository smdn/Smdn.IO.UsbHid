// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Polly;
using Polly.DependencyInjection;
using Polly.Retry;

namespace Smdn.IO.UsbHid.DependencyInjection;

public static class HidApiDotNetUsbHidServiceBuilderExtensions {
  [CLSCompliant(false)]
  public static HidApiDotNetUsbHidServiceBuilder<TServiceKey>
  AddResiliencePipelineForOpenEndPoint<TServiceKey>(
    this HidApiDotNetUsbHidServiceBuilder<TServiceKey> builder,
    RetryStrategyOptions retryOptions
  )
  {
    if (retryOptions is null)
      throw new ArgumentNullException(nameof(retryOptions));

    return AddResiliencePipelineForOpenEndPoint(
      builder: builder ?? throw new ArgumentNullException(nameof(builder)),
      configure: (pipelineBuilder, _) => {
        pipelineBuilder.AddRetry(retryOptions);
      }
    );
  }

  [CLSCompliant(false)]
  public static HidApiDotNetUsbHidServiceBuilder<TServiceKey>
  AddResiliencePipelineForOpenEndPoint<TServiceKey>(
    this HidApiDotNetUsbHidServiceBuilder<TServiceKey> builder,
    Action<
      ResiliencePipelineBuilder,
      AddResiliencePipelineContext<
        HidApiDotNetResiliencePipelineKeyPair<TServiceKey>
      >
    > configure
  )
  {
    _ = (builder ?? throw new ArgumentNullException(nameof(builder))).Services.AddResiliencePipelineForOpenEndPoint(
      serviceKey: builder.ServiceKey,
      configure: configure ?? throw new ArgumentNullException(nameof(configure))
    );

    return builder;
  }
}
