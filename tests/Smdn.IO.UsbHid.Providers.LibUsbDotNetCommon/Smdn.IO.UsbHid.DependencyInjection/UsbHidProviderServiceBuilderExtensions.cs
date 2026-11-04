// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Polly.Retry;

namespace Smdn.IO.UsbHid.DependencyInjection;

// The methods in this class implement the partial methods defined in
// `Smdn.IO.UsbHid.Providers.Common`. Specifically, adding the ResiliencePipeline
// to UsbHidServiceBuilder, which is used in the common test code.
internal static partial class UsbHidProviderServiceBuilderExtensions {
  public static partial UsbHidServiceBuilder<TServiceKey> AddResiliencePipelineForOpenEndPoint<TServiceKey>(
    this UsbHidServiceBuilder<TServiceKey> builder,
    RetryStrategyOptions retryOptions
  )
#if LIBUSBDOTNET_V3
    => builder is LibUsbDotNetV3UsbHidServiceBuilder<TServiceKey> b
#else
    => builder is LibUsbDotNetUsbHidServiceBuilder<TServiceKey> b
#endif
      ? b.AddResiliencePipelineForOpenEndPoint(retryOptions)
      : throw new InvalidOperationException("Unexpected type of UsbHidServiceBuilder.");
}
