// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

using Polly.Retry;
using Polly.Registry;

namespace Smdn.IO.UsbHid.DependencyInjection;

[TestFixture]
public class LibUsbDotNetUsbHidServiceProviderExtensionsTests {
  [Test]
  public void GetResiliencePipelineProviderForLibUsbDotNetUsbHidService_ServiceProviderNull()
    => Assert.That(
      () => (null as IServiceProvider)!
#if LIBUSBDOTNET_V3
        .GetResiliencePipelineProviderForLibUsbDotNetV3UsbHidService
#else
        .GetResiliencePipelineProviderForLibUsbDotNetUsbHidService
#endif
        ("service-key"),
      Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("serviceProvider")
    );

  [Test]
  public void GetResiliencePipelineProviderForLibUsbDotNetUsbHidService()
  {
    const string ServiceKey = nameof(ServiceKey);

    var services = new ServiceCollection();

    services
#if LIBUSBDOTNET_V3
      .AddLibUsbDotNetV3UsbHid
#else
      .AddLibUsbNullSession()
      .AddLibUsbDotNetUsbHid
#endif
      (
        ServiceKey,
        (builder, options) => {
          builder.AddResiliencePipelineForOpenEndPoint(
            new RetryStrategyOptions()
          );
        }
      );

    services
#if LIBUSBDOTNET_V3
      .AddLibUsbDotNetV3UsbHid
#else
      .AddLibUsbDotNetUsbHid
#endif
      (
        (builder, options) => {
          builder.AddResiliencePipelineForOpenEndPoint(
            new RetryStrategyOptions()
          );
        }
      );

    using var provider = services.BuildServiceProvider();

    Assert.That(
      provider
#if LIBUSBDOTNET_V3
        .GetResiliencePipelineProviderForLibUsbDotNetV3UsbHidService
#else
        .GetResiliencePipelineProviderForLibUsbDotNetUsbHidService
#endif
        (ServiceKey),
      Is.Not.Null
    );

    Assert.That(
      provider
#if LIBUSBDOTNET_V3
        .GetResiliencePipelineProviderForLibUsbDotNetV3UsbHidService
#else
        .GetResiliencePipelineProviderForLibUsbDotNetUsbHidService
#endif
        (null),
      Is.Not.Null
    );
  }
}
