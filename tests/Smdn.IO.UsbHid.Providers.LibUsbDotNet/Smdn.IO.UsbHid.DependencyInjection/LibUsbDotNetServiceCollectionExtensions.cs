// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NUnit.Framework;

namespace Smdn.IO.UsbHid.DependencyInjection;

[TestFixture]
public class LibUsbDotNetServiceCollectionExtensionsTests {
  [Test]
  public void AddLibUsbDotNetUsbHid_ServicesNull()
  {
    Assert.That(
      () => (null as IServiceCollection)!
#if LIBUSBDOTNET_V3
        .AddLibUsbDotNetV3UsbHid
#else
        .AddLibUsbDotNetUsbHid
#endif
        (),
      Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("services")
    );
    Assert.That(
      () => (null as IServiceCollection)!
#if LIBUSBDOTNET_V3
        .AddLibUsbDotNetV3UsbHid
#else
        .AddLibUsbDotNetUsbHid
#endif
        ((builder, options) => { }),
      Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("services")
    );
    Assert.That(
      () => (null as IServiceCollection)!
#if LIBUSBDOTNET_V3
        .AddLibUsbDotNetV3UsbHid
#else
        .AddLibUsbDotNetUsbHid
#endif
        ("key"),
      Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("services")
    );
    Assert.That(
      () => (null as IServiceCollection)!
#if LIBUSBDOTNET_V3
        .AddLibUsbDotNetV3UsbHid
#else
        .AddLibUsbDotNetUsbHid
#endif
        ("key", (builder, options) => { }),
      Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("services")
    );
    Assert.That(
      () => (null as IServiceCollection)!
#if LIBUSBDOTNET_V3
        .AddLibUsbDotNetV3UsbHid
#else
        .AddLibUsbDotNetUsbHid
#endif
        <string>("key", static key => key),
      Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("services")
    );
    Assert.That(
      () => (null as IServiceCollection)!
#if LIBUSBDOTNET_V3
        .AddLibUsbDotNetV3UsbHid
#else
        .AddLibUsbDotNetUsbHid
#endif
        <string>("key", static key => key, (builder, options) => { }),
      Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("services")
    );
  }

  [Test]
  public void AddLibUsbDotNetUsbHid_ConfigureNull()
  {
    var services = new ServiceCollection();

    Assert.That(
      () => services
#if LIBUSBDOTNET_V3
        .AddLibUsbDotNetV3UsbHid
#else
        .AddLibUsbDotNetUsbHid
#endif
        (configure: null!),
      Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("configure")
    );
    Assert.That(
      () => services
#if LIBUSBDOTNET_V3
        .AddLibUsbDotNetV3UsbHid
#else
        .AddLibUsbDotNetUsbHid
#endif
        ("key", configure: null!),
      Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("configure")
    );
    Assert.That(
      () => services
#if LIBUSBDOTNET_V3
        .AddLibUsbDotNetV3UsbHid
#else
        .AddLibUsbDotNetUsbHid
#endif
        <string>("key", static key => key, configure: null!),
      Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("configure")
    );
  }

  [Test]
  public void AddLibUsbDotNetUsbHid_SelectOptionsNameForServiceKeyNull()
  {
    var services = new ServiceCollection();

    Assert.That(
      () => services
#if LIBUSBDOTNET_V3
        .AddLibUsbDotNetV3UsbHid
#else
        .AddLibUsbDotNetUsbHid
#endif
        ("key", selectOptionsNameForServiceKey: null!),
      Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("selectOptionsNameForServiceKey")
    );
    Assert.That(
      () => services
#if LIBUSBDOTNET_V3
        .AddLibUsbDotNetV3UsbHid
#else
        .AddLibUsbDotNetUsbHid
#endif
        ("key", selectOptionsNameForServiceKey: null!, (builder, options) => { }),
      Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("selectOptionsNameForServiceKey")
    );
  }

  [Test]
  public void AddLibUsbDotNetUsbHid()
  {
    var services = new ServiceCollection();

    services
#if LIBUSBDOTNET_V3
      .AddLibUsbDotNetV3UsbHid
#else
      .AddLibUsbNullSession()
      .AddLibUsbDotNetUsbHid
#endif
      (
        static (_, options) => {
          options.DebugLevel = LogLevel.None;
        }
      );

    using var provider = services.BuildServiceProvider();
    var usbHidService = provider.GetService<IUsbHidService>();

    Assert.That(usbHidService, Is.Not.Null);
  }

  [Test]
  public void AddLibUsbDotNetUsbHid_WithServiceKey_OfStringKey()
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
        static (_, options) => {
          options.DebugLevel = LogLevel.None;
        }
      );

    using var provider = services.BuildServiceProvider();
    var usbHidService = provider.GetKeyedService<IUsbHidService>(ServiceKey);

    Assert.That(usbHidService, Is.Not.Null);
  }

  [Test]
  public void AddLibUsbDotNetUsbHid_WithServiceKey_OfNonStringKey()
  {
    const int ServiceKey = 1;
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
        static key => key.ToString(provider: null),
        static (_, options) => {
          options.DebugLevel = LogLevel.None;
        }
      );

    using var provider = services.BuildServiceProvider();
    var usbHidService = provider.GetKeyedService<IUsbHidService>(ServiceKey);

    Assert.That(usbHidService, Is.Not.Null);
  }

  [Test]
  public void AddLibUsbDotNetUsbHid_Configure()
  {
    var services = new ServiceCollection();
    var configureCalled = false;

    services.AddLogging();

    services
#if LIBUSBDOTNET_V3
      .AddLibUsbDotNetV3UsbHid
#else
      .AddLibUsbNullSession()
      .AddLibUsbDotNetUsbHid
#endif
      (
        (builder, options) => {
          configureCalled = true;

          Assert.That(builder, Is.Not.Null);
          Assert.That(options, Is.Not.Null);

          options.DebugLevel = LogLevel.Warning;
        }
      );

    using var provider = services.BuildServiceProvider();
    var resolvedOptions = provider.GetRequiredService<
      IOptions<
#if LIBUSBDOTNET_V3
        LibUsbDotNetV3Options
#else
        LibUsbDotNetOptions
#endif
      >
    >().Value;

    Assert.That(configureCalled, Is.True);
    Assert.That(resolvedOptions.DebugLevel, Is.EqualTo(LogLevel.Warning));
  }
}
