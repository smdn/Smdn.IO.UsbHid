// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Threading;

using NUnit.Framework;

namespace Smdn.IO.UsbHid.Abstractions;

[TestFixture]
public class NullUsbHidDeviceTests {
  [Test]
  public void Instance()
  {
    var instance = NullUsbHidDevice.Instance;

    Assert.That(instance, Is.Not.Null);
    Assert.That(instance, Is.SameAs(NullUsbHidDevice.Instance));
  }

  [Test]
  public void UnderlyingDevice()
  {
    var underlyingDevice = NullUsbHidDevice.Instance.UnderlyingDevice;

    Assert.That(underlyingDevice, Is.Not.Null);
    Assert.That(underlyingDevice, Is.SameAs(NullUsbHidDevice.Instance.UnderlyingDevice));
  }

  [Test]
  public void VendorId()
    => Assert.That(NullUsbHidDevice.Instance.VendorId, Is.Default);

  [Test]
  public void ProductId()
    => Assert.That(NullUsbHidDevice.Instance.ProductId, Is.Default);

  [Test]
  public void Dispose()
  {
    Assert.That(NullUsbHidDevice.Instance.Dispose, Throws.Nothing);
    Assert.That(NullUsbHidDevice.Instance.Dispose, Throws.Nothing, "dispose again");

    Assert.That(() => NullUsbHidDevice.Instance.OpenEndPoint(), Throws.Nothing, "still alive");
    Assert.That(async () => await NullUsbHidDevice.Instance.OpenEndPointAsync(), Throws.Nothing, "still alive");
  }

  [Test]
  public void DisposeAsync()
  {
    Assert.That(
      async () => await NullUsbHidDevice.Instance.DisposeAsync(),
      Throws.Nothing
    );
    Assert.That(
      async () => await NullUsbHidDevice.Instance.DisposeAsync(),
      Throws.Nothing,
      "dispose again"
    );

    Assert.That(() => NullUsbHidDevice.Instance.OpenEndPoint(), Throws.Nothing, "still alive");
    Assert.That(async () => await NullUsbHidDevice.Instance.OpenEndPointAsync(), Throws.Nothing, "still alive");
  }

  [Test]
  public void TryGetProductName()
  {
    string? productName = default;

    Assert.That(
      NullUsbHidDevice.Instance.TryGetProductName(out productName),
      Is.False
    );
    Assert.That(productName, Is.Default);
  }

  [Test]
  public void TryGetManufacturer()
  {
    string? manufacturer = default;

    Assert.That(
      NullUsbHidDevice.Instance.TryGetManufacturer(out manufacturer),
      Is.False
    );
    Assert.That(manufacturer, Is.Default);
  }

  [Test]
  public void TryGetSerialNumber()
  {
    string? serialNumber = default;

    Assert.That(
      NullUsbHidDevice.Instance.TryGetSerialNumber(out serialNumber),
      Is.False
    );
    Assert.That(serialNumber, Is.Default);
  }

  [Test]
  public void TryGetDeviceIdentifier()
  {
    string? deviceIdentifier = default;

    Assert.That(
      NullUsbHidDevice.Instance.TryGetDeviceIdentifier(out deviceIdentifier),
      Is.False
    );
    Assert.That(deviceIdentifier, Is.Default);
  }

  [Test]
  public void OpenEndPoint(
    [Values] bool openOutEndPoint,
    [Values] bool openInEndPoint,
    [Values] bool shouldDisposeDevice
  )
  {
    IUsbHidEndPoint? endPoint = default;

    Assert.That(
      () => endPoint = NullUsbHidDevice.Instance.OpenEndPoint(
        openOutEndPoint: openOutEndPoint,
        openInEndPoint: openInEndPoint,
        shouldDisposeDevice: shouldDisposeDevice,
        cancellationToken: default
      ),
      Throws.Nothing
    );
    Assert.That(endPoint, Is.Not.Null);
    Assert.That(endPoint, Is.SameAs(NullUsbHidEndPoint.Instance));
  }

  [Test]
  public void OpenEndPoint_CancellationRequested()
  {
    IUsbHidEndPoint? endPoint = default;

    using var cts = new CancellationTokenSource();

    cts.Cancel();

    Assert.That(
      () => endPoint = NullUsbHidDevice.Instance.OpenEndPoint(
        openOutEndPoint: default,
        openInEndPoint: default,
        shouldDisposeDevice: default,
        cancellationToken: cts.Token
      ),
      Throws.Nothing
    );

    Assert.That(endPoint, Is.Not.Null);
    Assert.That(endPoint, Is.SameAs(NullUsbHidEndPoint.Instance));
  }

  [Test]
  public void OpenEndPointAsync(
    [Values] bool openOutEndPoint,
    [Values] bool openInEndPoint,
    [Values] bool shouldDisposeDevice
  )
  {
    IUsbHidEndPoint? endPoint = default;

    Assert.That(
      async () => endPoint = await NullUsbHidDevice.Instance.OpenEndPointAsync(
        openOutEndPoint: openOutEndPoint,
        openInEndPoint: openInEndPoint,
        shouldDisposeDevice: shouldDisposeDevice,
        cancellationToken: default
      ).ConfigureAwait(false),
      Throws.Nothing
    );
    Assert.That(endPoint, Is.Not.Null);
    Assert.That(endPoint, Is.SameAs(NullUsbHidEndPoint.Instance));
  }

  [Test]
  public void OpenEndPointAsync()
  {
    IUsbHidEndPoint? endPoint = default;

    using var cts = new CancellationTokenSource();

    cts.Cancel();

    Assert.That(
      async () => endPoint = await NullUsbHidDevice.Instance.OpenEndPointAsync(
        openOutEndPoint: default,
        openInEndPoint: default,
        shouldDisposeDevice: default,
        cancellationToken: cts.Token
      ).ConfigureAwait(false),
      Throws.Nothing
    );
    Assert.That(endPoint, Is.Not.Null);
    Assert.That(endPoint, Is.SameAs(NullUsbHidEndPoint.Instance));
  }
}
