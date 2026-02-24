// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

using Polly;
using Polly.DependencyInjection;
using Polly.Registry;
using Polly.Retry;

namespace Smdn.IO.UsbHid.DependencyInjection;

[TestFixture]
public class LibUsbDotNetUsbHidServiceBuilderExtensionsTests {
  [Test]
  public void AddResiliencePipelineForOpenEndPoint_BuilderNull()
  {
#if LIBUSBDOTNET_V3
    LibUsbDotNetV3UsbHidServiceBuilder
#else
    LibUsbDotNetUsbHidServiceBuilder
#endif
    <string> nullBuilder = null!;

    Assert.That(
      () => nullBuilder.AddResiliencePipelineForOpenEndPoint(retryOptions: new RetryStrategyOptions()),
      Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("builder")
    );
    Assert.That(
      () => nullBuilder.AddResiliencePipelineForOpenEndPoint(configure: (builder, context) => { }),
      Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("builder")
    );
  }

  [Test]
  public void AddResiliencePipelineForOpenEndPoint_RetryOptionsNull()
  {
    var services = new ServiceCollection();

    services
#if LIBUSBDOTNET_V3
      .AddLibUsbDotNetV3UsbHid
#else
      .AddLibUsbNullSession()
      .AddLibUsbDotNetUsbHid
#endif
      ();

    using var provider = services.BuildServiceProvider();
    var builder = provider.GetRequiredService<
#if LIBUSBDOTNET_V3
      LibUsbDotNetV3UsbHidServiceBuilder
#else
      LibUsbDotNetUsbHidServiceBuilder
#endif
      <object?>
    >();

    Assert.That(
      () => builder.AddResiliencePipelineForOpenEndPoint(retryOptions: null!),
      Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("retryOptions")
    );
  }

  [Test]
  public void AddResiliencePipelineForOpenEndPoint_ConfigureNull()
  {
    var services = new ServiceCollection();

    services
#if LIBUSBDOTNET_V3
      .AddLibUsbDotNetV3UsbHid
#else
      .AddLibUsbNullSession()
      .AddLibUsbDotNetUsbHid
#endif
      ();

    using var provider = services.BuildServiceProvider();
    var builder = provider.GetRequiredService<
#if LIBUSBDOTNET_V3
      LibUsbDotNetV3UsbHidServiceBuilder
#else
      LibUsbDotNetUsbHidServiceBuilder
#endif
      <object?>
    >();

    Assert.That(
      () => builder.AddResiliencePipelineForOpenEndPoint(configure: null!),
      Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("configure")
    );
  }

  [Test]
  public void AddResiliencePipelineForOpenEndPoint()
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

    using var provider = services.BuildServiceProvider();
    var pipelineProvider = provider.GetRequiredKeyedService<ResiliencePipelineProvider<string>>(ServiceKey);

    Assert.That(
      pipelineProvider.TryGetPipeline(
#if LIBUSBDOTNET_V3
        LibUsbDotNetV3UsbHidDevice
#else
        LibUsbDotNetUsbHidDevice
#endif
          .ResiliencePipelineKeyForOpenEndPoint,
        out var pipeline
      ),
      Is.True
    );
    Assert.That(pipeline, Is.Not.Null);
  }
}
