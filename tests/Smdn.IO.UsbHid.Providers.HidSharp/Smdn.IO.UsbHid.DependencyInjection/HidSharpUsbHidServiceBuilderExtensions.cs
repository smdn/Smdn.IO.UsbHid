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
public class HidSharpUsbHidServiceBuilderExtensionsTests {
  [Test]
  public void AddResiliencePipelineForOpenEndPoint_BuilderNull()
  {
    HidSharpUsbHidServiceBuilder<string> nullBuilder = null!;

    Assert.That(
      () => nullBuilder.AddResiliencePipelineForOpenEndPoint(),
      Throws.ArgumentNullException.ParamName.EqualTo("builder")
    );
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

    services.AddHidSharpUsbHid();

    var provider = services.BuildServiceProvider();
    var builder = provider.GetRequiredService<HidSharpUsbHidServiceBuilder<object?>>();

    Assert.That(
      () => builder.AddResiliencePipelineForOpenEndPoint(retryOptions: null!),
      Throws.ArgumentNullException.ParamName.EqualTo("retryOptions")
    );
  }

  [Test]
  public void AddResiliencePipelineForOpenEndPoint_ConfigureNull()
  {
    var services = new ServiceCollection();

    services.AddHidSharpUsbHid();

    var provider = services.BuildServiceProvider();
    var builder = provider.GetRequiredService<HidSharpUsbHidServiceBuilder<object?>>();

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

    services.AddHidSharpUsbHid(
      ServiceKey,
      builder => {
        builder.AddResiliencePipelineForOpenEndPoint(
          new RetryStrategyOptions()
        );
      }
    );

    var provider = services.BuildServiceProvider();
    var pipelineProvider = provider.GetRequiredKeyedService<ResiliencePipelineProvider<string>>(ServiceKey);

    Assert.That(
      pipelineProvider.TryGetPipeline(
        HidSharpUsbHidDevice.ResiliencePipelineKeyForOpenEndPoint,
        out var pipeline
      ),
      Is.True
    );
    Assert.That(pipeline, Is.Not.Null);
  }

  [Test]
  public void AddResiliencePipelineForOpenEndPoint_WithDefaultOption()
  {
    const string ServiceKey = nameof(ServiceKey);
    var services = new ServiceCollection();

    services.AddHidSharpUsbHid(
      ServiceKey,
      builder => builder.AddResiliencePipelineForOpenEndPoint()
    );

    var provider = services.BuildServiceProvider();
    var pipelineProvider = provider.GetRequiredKeyedService<ResiliencePipelineProvider<string>>(ServiceKey);

    Assert.That(
      pipelineProvider.TryGetPipeline(
        HidSharpUsbHidDevice.ResiliencePipelineKeyForOpenEndPoint,
        out var pipeline
      ),
      Is.True
    );
    Assert.That(pipeline, Is.Not.Null);
  }
}
