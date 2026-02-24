// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#if LIBUSBDOTNET_V3
#pragma warning disable SA1649 // warning SA1649: File name should match first type name
#endif

using System;

using Polly;
using Polly.DependencyInjection;
using Polly.Retry;

namespace Smdn.IO.UsbHid.DependencyInjection;

public static class
#if LIBUSBDOTNET_V3
LibUsbDotNetV3UsbHidServiceBuilderExtensions
#else
LibUsbDotNetUsbHidServiceBuilderExtensions
#endif
{
  [CLSCompliant(false)]
  public static
#if LIBUSBDOTNET_V3
  LibUsbDotNetV3UsbHidServiceBuilder<TServiceKey>
#else
  LibUsbDotNetUsbHidServiceBuilder<TServiceKey>
#endif
  AddResiliencePipelineForOpenEndPoint<TServiceKey>(
    this
#if LIBUSBDOTNET_V3
    LibUsbDotNetV3UsbHidServiceBuilder<TServiceKey>
#else
    LibUsbDotNetUsbHidServiceBuilder<TServiceKey>
#endif
      builder,
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
  public static
#if LIBUSBDOTNET_V3
  LibUsbDotNetV3UsbHidServiceBuilder<TServiceKey>
#else
  LibUsbDotNetUsbHidServiceBuilder<TServiceKey>
#endif
  AddResiliencePipelineForOpenEndPoint<TServiceKey>(
    this
#if LIBUSBDOTNET_V3
    LibUsbDotNetV3UsbHidServiceBuilder<TServiceKey>
#else
    LibUsbDotNetUsbHidServiceBuilder<TServiceKey>
#endif
      builder,
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
    _ = (builder ?? throw new ArgumentNullException(nameof(builder))).Services.AddResiliencePipelineForOpenEndPoint(
      serviceKey: builder.ServiceKey,
      configure: configure ?? throw new ArgumentNullException(nameof(configure))
    );

    return builder;
  }
}
