// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

using Polly.Registry;
using Polly.Retry;

namespace Smdn.IO.UsbHid.DependencyInjection;

// Common test cases are defined in `CommonUsbHidServiceBuilderTests`.
// Provider-specific test cases should be defined in this class.
[TestFixture]
public class HidApiDotNetUsbHidServiceBuilderExtensionsTests {
  [Test]
  public void AddResiliencePipelineForOpenEndPoint_BuilderNull()
  {
    HidApiDotNetUsbHidServiceBuilder<string> nullBuilder = null!;

    Assert.That(
      () => nullBuilder.AddResiliencePipelineForOpenEndPoint(retryOptions: new RetryStrategyOptions()),
      Throws.ArgumentNullException.ParamName.EqualTo("builder")
    );
    Assert.That(
      () => nullBuilder.AddResiliencePipelineForOpenEndPoint(configure: (builder, context) => { }),
      Throws.ArgumentNullException.ParamName.EqualTo("builder")
    );
  }

  [Test]
  public void AddResiliencePipelineForOpenEndPoint_RetryOptionsNull()
  {
    var services = new ServiceCollection();

    services.AddHidApiDotNetUsbHid();

    var provider = services.BuildServiceProvider();
    var builder = provider.GetRequiredService<HidApiDotNetUsbHidServiceBuilder<object?>>();

    Assert.That(
      () => builder.AddResiliencePipelineForOpenEndPoint(retryOptions: null!),
      Throws.ArgumentNullException.ParamName.EqualTo("retryOptions")
    );
  }

  [Test]
  public void AddResiliencePipelineForOpenEndPoint_ConfigureNull()
  {
    var services = new ServiceCollection();

    services.AddHidApiDotNetUsbHid();

    var provider = services.BuildServiceProvider();
    var builder = provider.GetRequiredService<HidApiDotNetUsbHidServiceBuilder<object?>>();

    Assert.That(
      () => builder.AddResiliencePipelineForOpenEndPoint(configure: null!),
      Throws.ArgumentNullException.ParamName.EqualTo("configure")
    );
  }

  [Test]
  public void AddResiliencePipelineForOpenEndPoint()
  {
    const string ServiceKey = nameof(ServiceKey);
    var services = new ServiceCollection();

    services.AddHidApiDotNetUsbHid(
      ServiceKey,
      builder => builder.AddResiliencePipelineForOpenEndPoint(
        new RetryStrategyOptions()
      )
    );

    var provider = services.BuildServiceProvider();
    var pipelineProvider = provider.GetRequiredKeyedService<ResiliencePipelineProvider<string>>(ServiceKey);

    Assert.That(
      pipelineProvider.TryGetPipeline(
        HidApiDotNetUsbHidDevice.ResiliencePipelineKeyForOpenEndPoint,
        out var pipeline
      ),
      Is.True
    );
    Assert.That(pipeline, Is.Not.Null);
  }
}
