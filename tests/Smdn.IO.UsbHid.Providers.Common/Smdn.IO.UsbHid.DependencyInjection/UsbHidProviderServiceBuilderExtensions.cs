// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using Polly.Retry;

namespace Smdn.IO.UsbHid.DependencyInjection;

// For the partial methods declared in this class, the code for adding a
// ResiliencePipe to UsbHidServiceBuilder is expected to be implemented in
// the test assembly for each individual provider library. The common test
// code written in this directory runs tests using the ResiliencePipe added
// or retrieved by this method.
internal static partial class UsbHidProviderServiceBuilderExtensions {
  public static partial UsbHidServiceBuilder<TServiceKey> AddResiliencePipelineForOpenEndPoint<TServiceKey>(
    this UsbHidServiceBuilder<TServiceKey> builder,
    RetryStrategyOptions retryOptions
  );
}
