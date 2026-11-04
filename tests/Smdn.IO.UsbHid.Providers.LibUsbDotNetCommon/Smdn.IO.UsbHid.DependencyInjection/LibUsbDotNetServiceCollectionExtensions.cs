// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NUnit.Framework;

namespace Smdn.IO.UsbHid.DependencyInjection;

// Common test cases are defined in `CommonServiceCollectionExtensionsTests`.
// Provider-specific test cases should be defined in this class.
[TestFixture]
public class LibUsbDotNetServiceCollectionExtensionsTests {
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
