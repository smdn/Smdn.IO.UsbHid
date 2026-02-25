// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

using NUnit.Framework;

namespace Smdn.IO.UsbHid.DependencyInjection;

[TestFixture]
public class LibUsbDotNetV3OptionsTests {
  private static System.Collections.IEnumerable YieldTestCases_Configure()
  {
    yield return new object[] { TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1), LogLevel.Debug };
    yield return new object[] { TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(3), LogLevel.Warning };
  }

  [TestCaseSource(nameof(YieldTestCases_Configure))]
  public void Configure(TimeSpan readTimeout, TimeSpan writeTimeout, LogLevel debugLevel)
  {
    var baseOptions = new LibUsbDotNetV3Options() {
      ReadEndPointTimeout = readTimeout,
      WriteEndPointTimeout = writeTimeout,
      DebugLevel = debugLevel,
    };

    var options = new LibUsbDotNetV3Options();

    Assert.That(
      options.Configure(baseOptions),
      Is.SameAs(options)
    );

    Assert.That(options.ReadEndPointTimeout, Is.EqualTo(baseOptions.ReadEndPointTimeout));
    Assert.That(options.WriteEndPointTimeout, Is.EqualTo(baseOptions.WriteEndPointTimeout));
    Assert.That(options.DebugLevel, Is.EqualTo(baseOptions.DebugLevel));
  }
}