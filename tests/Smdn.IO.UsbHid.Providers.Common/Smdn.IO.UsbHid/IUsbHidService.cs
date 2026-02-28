// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Threading;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

using Smdn.IO.UsbHid.DependencyInjection;

namespace Smdn.IO.UsbHid;

[TestFixture]
public class IUsbHidServiceTests {
  [Test]
  public void Dispose()
  {
    var services = new ServiceCollection();

    services.AddUsbHidProvider();

    using var provider = services.BuildServiceProvider();
    using var usbHidService = provider.GetRequiredService<IUsbHidService>();

    Assert.That(
      () => usbHidService.Dispose(),
      Throws.Nothing
    );
    Assert.That(
      () => usbHidService.Dispose(),
      Throws.Nothing,
      "dispose again"
    );
    Assert.That(
      async () => await usbHidService.DisposeAsync(),
      Throws.Nothing,
      "dispose async"
    );

    Assert.That(
      () => usbHidService.GetDevices(),
      Throws.TypeOf<ObjectDisposedException>(),
      "disposed object"
    );
  }

  [Test]
  public void DisposeAsync()
  {
    var services = new ServiceCollection();

    services.AddUsbHidProvider();

    using var provider = services.BuildServiceProvider();
    using var usbHidService = provider.GetRequiredService<IUsbHidService>();

    Assert.That(
      async () => await usbHidService.DisposeAsync(),
      Throws.Nothing
    );
    Assert.That(
      async () => await usbHidService.DisposeAsync(),
      Throws.Nothing,
      "dispose again"
    );
    Assert.That(
      () => usbHidService.Dispose(),
      Throws.Nothing,
      "dispose sync"
    );

    Assert.That(
      () => usbHidService.GetDevices(),
      Throws.TypeOf<ObjectDisposedException>(),
      "disposed object"
    );
  }

  [Test]
  public void GetDevices()
  {
    var services = new ServiceCollection();

    services.AddUsbHidProvider();

    using var provider = services.BuildServiceProvider();
    using var usbHidService = provider.GetRequiredService<IUsbHidService>();

    IReadOnlyList<IUsbHidDevice>? devices = null;

    Assert.That(
      () => devices = usbHidService.GetDevices(),
      Throws.Nothing
    );
    Assert.That(devices, Is.Not.Null);

    foreach (var device in devices) {
      device.Dispose();
    }
  }

  [Test]
  public void GetDevices_CancellationRequested()
  {
    var services = new ServiceCollection();

    services.AddUsbHidProvider();

    using var cts = new CancellationTokenSource();
    using var provider = services.BuildServiceProvider();
    using var usbHidService = provider.GetRequiredService<IUsbHidService>();

    cts.Cancel();

    var cancellationToken = cts.Token;

    Assert.That(
      () => _ = usbHidService.GetDevices(cancellationToken),
      Throws
        .InstanceOf<OperationCanceledException>()
        .With
        .Property(nameof(OperationCanceledException.CancellationToken))
        .EqualTo(cancellationToken)
    );
  }
}
