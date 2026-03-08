// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using NUnit.Framework;

namespace Smdn.IO.UsbHid.Abstractions;

[TestFixture]
public class NullUsbHidUnderlyingReadEndPointTests {
  [Test]
  public void Instance()
  {
    var instance = NullUsbHidUnderlyingReadEndPoint.Instance;

    Assert.That(instance, Is.Not.Null);
    Assert.That(instance, Is.SameAs(NullUsbHidUnderlyingReadEndPoint.Instance));
  }
}
