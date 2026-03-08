// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Threading;

using NUnit.Framework;

namespace Smdn.IO.UsbHid.Abstractions;

[TestFixture]
public class NullUsbHidEndPointTests {
  [Test]
  public void Instance()
  {
    var instance = NullUsbHidEndPoint.Instance;

    Assert.That(instance, Is.Not.Null);
    Assert.That(instance, Is.SameAs(NullUsbHidEndPoint.Instance));
  }

  [Test]
  public void Device()
  {
    var device = NullUsbHidEndPoint.Instance.Device;

    Assert.That(device, Is.Not.Null);
    Assert.That(device, Is.SameAs(NullUsbHidEndPoint.Instance.Device));
  }

  [Test]
  public void ReadEndPoint()
  {
    var readEndPoint = NullUsbHidEndPoint.Instance.ReadEndPoint;

    Assert.That(readEndPoint, Is.Not.Null);
    Assert.That(readEndPoint, Is.SameAs(NullUsbHidEndPoint.Instance.ReadEndPoint));
  }

  [Test]
  public void WriteEndPoint()
  {
    var writeEndPoint = NullUsbHidEndPoint.Instance.WriteEndPoint;

    Assert.That(writeEndPoint, Is.Not.Null);
    Assert.That(writeEndPoint, Is.SameAs(NullUsbHidEndPoint.Instance.WriteEndPoint));
  }

  [Test]
  public void CanRead()
    => Assert.That(NullUsbHidEndPoint.Instance.CanRead, Is.True);

  [Test]
  public void CanWrite()
    => Assert.That(NullUsbHidEndPoint.Instance.CanWrite, Is.True);

  [Test]
  public void Dispose()
  {
    Assert.That(NullUsbHidEndPoint.Instance.Dispose, Throws.Nothing);
    Assert.That(NullUsbHidEndPoint.Instance.Dispose, Throws.Nothing, "dispose again");

    Assert.That(() => NullUsbHidEndPoint.Instance.Read(default), Throws.Nothing, "still alive");
    Assert.That(async () => await NullUsbHidEndPoint.Instance.ReadAsync(default), Throws.Nothing, "still alive");

    Assert.That(() => NullUsbHidEndPoint.Instance.Write(default), Throws.Nothing, "still alive");
    Assert.That(async () => await NullUsbHidEndPoint.Instance.WriteAsync(default), Throws.Nothing, "still alive");
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

    Assert.That(() => NullUsbHidEndPoint.Instance.Read(default), Throws.Nothing, "still alive");
    Assert.That(async () => await NullUsbHidEndPoint.Instance.ReadAsync(default), Throws.Nothing, "still alive");

    Assert.That(() => NullUsbHidEndPoint.Instance.Write(default), Throws.Nothing, "still alive");
    Assert.That(async () => await NullUsbHidEndPoint.Instance.WriteAsync(default), Throws.Nothing, "still alive");
  }

  [Test]
  public void Read()
  {
    var buffer = new byte[] { 0, 1, 2, 3 };
    var length = 0;

    Assert.That(
      () => length = NullUsbHidEndPoint.Instance.Read(buffer),
      Throws.Nothing
    );
    Assert.That(length, Is.Zero);
    Assert.That(buffer, Is.EqualTo(new byte[] { 0, 1, 2, 3 }).AsCollection);
  }


  [Test]
  public void Read_CancellationRequested()
  {
    using var cts = new CancellationTokenSource();

    cts.Cancel();

    var length = 0;

    Assert.That(
      () => length = NullUsbHidEndPoint.Instance.Read(default, cts.Token),
      Throws.Nothing
    );
    Assert.That(length, Is.Zero);
  }

  [Test]
  public void ReadAsync()
  {
    var buffer = new byte[] { 0, 1, 2, 3 };
    var length = 0;

    Assert.That(
      async () => length = await NullUsbHidEndPoint.Instance.ReadAsync(buffer).ConfigureAwait(false),
      Throws.Nothing
    );
    Assert.That(length, Is.Zero);
    Assert.That(buffer, Is.EqualTo(new byte[] { 0, 1, 2, 3 }).AsCollection);
  }

  [Test]
  public void ReadAsync_CancellationRequested()
  {
    using var cts = new CancellationTokenSource();

    cts.Cancel();

    var length = 0;

    Assert.That(
      async () => length = await NullUsbHidEndPoint.Instance.ReadAsync(default, cts.Token).ConfigureAwait(false),
      Throws.Nothing
    );
    Assert.That(length, Is.Zero);
  }

  [Test]
  public void Write()
  {
    Assert.That(
      () => NullUsbHidEndPoint.Instance.Write([0, 1, 2, 3]),
      Throws.Nothing
    );
  }

  [Test]
  public void Write_CancellationRequested()
  {
    using var cts = new CancellationTokenSource();

    cts.Cancel();

    Assert.That(
      () => NullUsbHidEndPoint.Instance.Write(default, cts.Token),
      Throws.Nothing
    );
  }

  [Test]
  public void WriteAsync()
  {
    Assert.That(
      async () => await NullUsbHidEndPoint.Instance.WriteAsync(new byte[] { 0, 1, 2, 3 }).ConfigureAwait(false),
      Throws.Nothing
    );
  }

  [Test]
  public void WriteAsync_CancellationRequested()
  {
    using var cts = new CancellationTokenSource();

    cts.Cancel();

    Assert.That(
      async () => await NullUsbHidEndPoint.Instance.WriteAsync(default, cts.Token).ConfigureAwait(false),
      Throws.Nothing
    );
  }
}
