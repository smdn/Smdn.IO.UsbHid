// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using NUnit.Framework;

namespace Smdn.IO.UsbHid.DependencyInjection;

[TestFixture]
public class CommonUsbHidServiceBuilderTests {
  private class PseudoLoggerFactory : ILoggerFactory {
    public bool CreateCalled { get; private set; }
    public string? CreatedLoggerCategoryName { get; private set; }

    public void AddProvider(ILoggerProvider provider) { }
    public ILogger CreateLogger(string categoryName)
    {
      CreateCalled = true;
      CreatedLoggerCategoryName = categoryName;

      return NullLogger.Instance;
    }

    public void Dispose() { }
  }

  [Test]
  public void Build()
  {
    var services = new ServiceCollection();

    services.AddUsbHidProvider();

    var provider = services.BuildServiceProvider();
    var builder = provider.GetUsbHidServiceBuilder();

    using var service = builder.Build(provider);

    Assert.That(service, Is.Not.Null);
  }

  [Test]
  public void Build_ServiceProviderNull()
  {
    var services = new ServiceCollection();

    services.AddUsbHidProvider();

    var provider = services.BuildServiceProvider();
    var builder = provider.GetUsbHidServiceBuilder();

    Assert.That(
      () => builder.Build(null!),
      Throws.ArgumentNullException.ParamName.EqualTo("serviceProvider")
    );
  }

  [Test]
  public void Build_UsesKeyedLoggerFactory()
  {
    const string ServiceKey = nameof(ServiceKey);

    var defaultLoggerFactory = new PseudoLoggerFactory();
    var keyedLoggerFactory = new PseudoLoggerFactory();

    var services = new ServiceCollection();

    services.AddSingleton<ILoggerFactory>(defaultLoggerFactory);
    services.AddKeyedSingleton<ILoggerFactory>(ServiceKey, keyedLoggerFactory);

    services.AddUsbHidProvider(ServiceKey);

    var provider = services.BuildServiceProvider();
    var builder = provider.GetUsbHidServiceBuilder(ServiceKey);
    using var usbHidService = builder.Build(provider);

    // Attempts to get the device to ensure invoking ILoggerFactory.CreateLogger.
    // However, since the call will not be made if the device does not exist, the following assertion cannot be verified.
    using var usbHidDevice = usbHidService.FindDevice(vendorId: null, productId: null, predicate: static _ => true);

    if (usbHidDevice is null) {
      Assert.Ignore("No USB HID device found; cannot verify that the keyed ILoggerFactory is used");
    }
    else {
      Assert.That(defaultLoggerFactory.CreateCalled, Is.False, "default ILoggerFactory must not be used");
      Assert.That(keyedLoggerFactory.CreateCalled, Is.True, "keyed ILoggerFactory must be used");

      Assert.That(keyedLoggerFactory.CreatedLoggerCategoryName, Is.EqualTo(usbHidDevice.GetType().FullName));
    }
  }

  [Test]
  public void Build_UsesDefaultLoggerFactory()
  {
    var defaultLoggerFactory = new PseudoLoggerFactory();
    var services = new ServiceCollection();

    services.AddSingleton<ILoggerFactory>(defaultLoggerFactory);

    services.AddUsbHidProvider();

    var provider = services.BuildServiceProvider();
    var builder = provider.GetUsbHidServiceBuilder();
    using var usbHidService = builder.Build(provider);

    // Attempts to get the device to ensure invoking ILoggerFactory.CreateLogger.
    // However, since the call will not be made if the device does not exist, the following assertion cannot be verified.
    using var usbHidDevice = usbHidService.FindDevice(vendorId: null, productId: null, predicate: static _ => true);

    if (usbHidDevice is null) {
      Assert.Ignore("No USB HID device found; cannot verify that the keyed ILoggerFactory is used");
    }
    else {
      Assert.That(defaultLoggerFactory.CreateCalled, Is.True, "default ILoggerFactory must be used");
      Assert.That(defaultLoggerFactory.CreatedLoggerCategoryName, Is.EqualTo(usbHidDevice.GetType().FullName));
    }
  }
}
