// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

namespace Smdn.IO.UsbHid.DependencyInjection;

[TestFixture]
public class CommonServiceCollectionExtensionsTests {
  [Test]
  public void AddUsbHidProvider_ServicesNull()
  {
    const string ServiceKey = nameof(ServiceKey);

    Assert.That(
      () => (null as IServiceCollection)!.AddUsbHidProvider(),
      Throws.ArgumentNullException.ParamName.EqualTo("services")
    );
    Assert.That(
      () => (null as IServiceCollection)!.AddUsbHidProvider(
        ServiceKey
      ),
      Throws.ArgumentNullException.ParamName.EqualTo("services")
    );
    Assert.That(
      () => (null as IServiceCollection)!.AddUsbHidProvider(
        ServiceKey,
        static key => key
      ),
      Throws.ArgumentNullException.ParamName.EqualTo("services")
    );

    Assert.That(
      () => (null as IServiceCollection)!.AddUsbHidProvider(
        static builder => throw new InvalidOperationException("Never be invoked")
      ),
      Throws.ArgumentNullException.ParamName.EqualTo("services")
    );
    Assert.That(
      () => (null as IServiceCollection)!.AddUsbHidProvider(
        ServiceKey,
        static builder => throw new InvalidOperationException("Never be invoked")
      ),
      Throws.ArgumentNullException.ParamName.EqualTo("services")
    );
    Assert.That(
      () => (null as IServiceCollection)!.AddUsbHidProvider(
        ServiceKey,
        static key => key,
        static builder => throw new InvalidOperationException("Never be invoked")
      ),
      Throws.ArgumentNullException.ParamName.EqualTo("services")
    );
  }

  [Test]
  public void AddUsbHidProvider_ConfigureNull()
  {
    const string ServiceKey = nameof(ServiceKey);

    var services = new ServiceCollection();

    Assert.That(
      () => services.AddUsbHidProvider(
        configure: null!
      ),
      Throws.ArgumentNullException.ParamName.EqualTo("configure")
    );
    Assert.That(
      () => services.AddUsbHidProvider(
        ServiceKey,
        configure: null!
      ),
      Throws.ArgumentNullException.ParamName.EqualTo("configure")
    );
    Assert.That(
      () => services.AddUsbHidProvider(
        ServiceKey,
        static key => key,
        configure: null!
      ),
      Throws.ArgumentNullException.ParamName.EqualTo("configure")
    );
  }

  [Test]
  public void AddUsbHidProvider_SelectOptionsNameForServiceKeyNull()
  {
    const string ServiceKey = nameof(ServiceKey);

    var services = new ServiceCollection();

    Assert.That(
      () => services.AddUsbHidProvider(
        ServiceKey,
        selectOptionsNameForServiceKey: null!
      ),
      Throws.ArgumentNullException.ParamName.EqualTo("selectOptionsNameForServiceKey")
    );
    Assert.That(
      () => services.AddUsbHidProvider(
        ServiceKey,
        selectOptionsNameForServiceKey: null!,
        static builder => throw new InvalidOperationException("Never be invoked")
      ),
      Throws.ArgumentNullException.ParamName.EqualTo("selectOptionsNameForServiceKey")
    );
  }

  [Test]
  public void AddUsbHidProvider()
  {
    var services = new ServiceCollection();

    services.AddUsbHidProvider();

    var provider = services.BuildServiceProvider();
    using var usbHidService = provider.GetService<IUsbHidService>();

    Assert.That(usbHidService, Is.Not.Null);
  }

  [Test]
  public void AddUsbHidProvider_WithServiceKey_OfStringKey()
  {
    const string ServiceKey = nameof(ServiceKey);

    var services = new ServiceCollection();

    services.AddUsbHidProvider(ServiceKey);

    var provider = services.BuildServiceProvider();
    using var usbHidService = provider.GetKeyedService<IUsbHidService>(ServiceKey);

    Assert.That(usbHidService, Is.Not.Null);
  }

  [Test]
  public void AddUsbHidProvider_WithServiceKey_OfNonStringKey()
  {
    const int ServiceKey = 1;

    var services = new ServiceCollection();

    services.AddUsbHidProvider(
      ServiceKey,
      static key => key.ToString(provider: null)
    );

    var provider = services.BuildServiceProvider();
    using var usbHidService = provider.GetKeyedService<IUsbHidService>(ServiceKey);

    Assert.That(usbHidService, Is.Not.Null);
  }

  [Test]
  public void AddUsbHidProvider_Configure()
  {
    var services = new ServiceCollection();
    var configureCalled = false;

    services.AddUsbHidProvider(
      builder => {
        configureCalled = true;

        Assert.That(builder, Is.Not.Null);
        Assert.That(builder.Services, Is.SameAs(services));
        Assert.That(builder.ServiceKey, Is.Null);
      }
    );

    var provider = services.BuildServiceProvider();

    Assert.That(configureCalled, Is.True);

    using var usbHidService = provider.GetService<IUsbHidService>();

    Assert.That(usbHidService, Is.Not.Null);
  }

  [Test]
  public void AddUsbHidProvider_Configure_WithServiceKey_OfStringKey()
  {
    const string ServiceKey = nameof(ServiceKey);

    var services = new ServiceCollection();
    var configureCalled = false;

    services.AddUsbHidProvider(
      serviceKey: ServiceKey,
      builder => {
        configureCalled = true;

        Assert.That(builder, Is.Not.Null);
        Assert.That(builder.Services, Is.SameAs(services));
        Assert.That(builder.ServiceKey, Is.EqualTo(ServiceKey));
      }
    );

    var provider = services.BuildServiceProvider();

    Assert.That(configureCalled, Is.True);

    using var usbHidService = provider.GetKeyedService<IUsbHidService>(ServiceKey);

    Assert.That(usbHidService, Is.Not.Null);
  }

  [Test]
  public void AddUsbHidProvider_Configure_WithServiceKey_OfNonStringKey()
  {
    const int ServiceKey = 1;

    var services = new ServiceCollection();
    var configureCalled = false;

    services.AddUsbHidProvider(
      serviceKey: ServiceKey,
      static key => key.ToString(provider: null),
      builder => {
        configureCalled = true;

        Assert.That(builder, Is.Not.Null);
        Assert.That(builder.Services, Is.SameAs(services));
        Assert.That(builder.ServiceKey, Is.EqualTo(ServiceKey));
      }
    );

    var provider = services.BuildServiceProvider();

    Assert.That(configureCalled, Is.True);

    using var usbHidService = provider.GetKeyedService<IUsbHidService>(ServiceKey);

    Assert.That(usbHidService, Is.Not.Null);
  }
}
