// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Polly.Registry;

namespace Smdn.IO.UsbHid.DependencyInjection;

// For partial methods declared in this class, the code for retrieving the
// USB HID service (UsbHidServiceBuilder) is expected to be implemented in
// the test assembly for each individual provider library. The common test
// code written in this directory performs tests using the UsbHidServiceBuilder
// retrieved by this method.
internal static partial class UsbHidProviderServiceProviderExtensions {
  public static partial UsbHidServiceBuilder<object?> GetUsbHidServiceBuilder(
    this IServiceProvider provider
  );

  public static partial UsbHidServiceBuilder<TServiceKey> GetUsbHidServiceBuilder<TServiceKey>(
    this IServiceProvider provider,
    TServiceKey serviceKey
  );

  public static partial ResiliencePipelineProvider<string>? GetResiliencePipelineProviderForUsbHidService(
    this IServiceProvider provider,
    object? serviceKey
  );
}
