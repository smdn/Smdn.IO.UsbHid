// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Polly;
using Polly.Registry;

namespace Smdn.IO.UsbHid.DependencyInjection;

public static class HidApiDotNetUsbHidServiceProviderExtensions {
  [CLSCompliant(false)] // ResiliencePipelineProvider is CLS incompliant
  public static ResiliencePipelineProvider<string>?
  GetResiliencePipelineProviderForHidApiDotNetUsbHidService(
    this IServiceProvider serviceProvider,
    object? serviceKey
  )
    => (serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider))).GetKeyedResiliencePipelineProvider<string>(
      serviceKey: serviceKey,
      typeOfKeyPair: typeof(HidApiDotNetResiliencePipelineKeyPair<>)
    );
}
