// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using NUnit.Framework;

namespace Smdn.IO.UsbHid.Abstractions;

[TestFixture]
public class NullUsbHidUnderlyingDeviceTests {
  [Test]
  public void Instance()
  {
    var instance = NullUsbHidUnderlyingDevice.Instance;

    Assert.That(instance, Is.Not.Null);
    Assert.That(instance, Is.SameAs(NullUsbHidUnderlyingDevice.Instance));
  }
}
