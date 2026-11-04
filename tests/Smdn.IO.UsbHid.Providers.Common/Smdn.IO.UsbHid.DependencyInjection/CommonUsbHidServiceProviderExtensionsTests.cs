// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

using Polly.Registry;
using Polly.Retry;

namespace Smdn.IO.UsbHid.DependencyInjection;

[TestFixture]
public class CommonUsbHidServiceProviderExtensionsTests {
  [Test]
  public void GetResiliencePipelineProviderForUsbHidService_ServiceProviderNull()
  {
    const string ServiceKey = nameof(ServiceKey);

    Assert.That(
      () => (null as IServiceProvider)!.GetResiliencePipelineProviderForUsbHidService(ServiceKey),
      Throws.ArgumentNullException.ParamName.EqualTo("serviceProvider")
    );
    Assert.That(
      () => (null as IServiceProvider)!.GetResiliencePipelineProviderForUsbHidService(null),
      Throws.ArgumentNullException.ParamName.EqualTo("serviceProvider")
    );
  }

  [Test]
  public void GetResiliencePipelineProviderForUsbHidService()
  {
    const string ServiceKey = nameof(ServiceKey);

    var services = new ServiceCollection();

    services.AddUsbHidProvider(
      ServiceKey,
      builder => builder.AddResiliencePipelineForOpenEndPoint(
        new RetryStrategyOptions()
      )
    );
    services.AddUsbHidProvider(
      builder => builder.AddResiliencePipelineForOpenEndPoint(
        new RetryStrategyOptions()
      )
    );

    var provider = services.BuildServiceProvider();
    ResiliencePipelineProvider<string>? keyedResiliencePipelineProvider = null;
    ResiliencePipelineProvider<string>? resiliencePipelineProvider = null;

    Assert.That(
      () => keyedResiliencePipelineProvider = provider.GetResiliencePipelineProviderForUsbHidService(ServiceKey),
      Throws.Nothing
    );
    Assert.That(
      keyedResiliencePipelineProvider,
      Is.Not.Null
    );

    Assert.That(
      () => resiliencePipelineProvider = provider.GetResiliencePipelineProviderForUsbHidService(null),
      Throws.Nothing
    );
    Assert.That(
      resiliencePipelineProvider,
      Is.Not.Null
    );

    Assert.That(
      keyedResiliencePipelineProvider,
      Is.Not.SameAs(resiliencePipelineProvider)
    );
  }

  [Test]
  public void GetResiliencePipelineProviderForUsbHidService_NoResiliencePipelineProviderAvailable()
  {
    const string ServiceKey = nameof(ServiceKey);

    var services = new ServiceCollection();

    services.AddUsbHidProvider(ServiceKey);
    services.AddUsbHidProvider();

    var provider = services.BuildServiceProvider();

    Assert.That(
      provider.GetResiliencePipelineProviderForUsbHidService(ServiceKey),
      Is.Null
    );

    Assert.That(
      provider.GetResiliencePipelineProviderForUsbHidService(null),
      Is.Null
    );
  }
}
