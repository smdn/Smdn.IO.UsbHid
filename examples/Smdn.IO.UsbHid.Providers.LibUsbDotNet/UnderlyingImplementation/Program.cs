// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System.Linq;

using Microsoft.Extensions.DependencyInjection;

using Smdn.IO.UsbHid;
using Smdn.IO.UsbHid.DependencyInjection;

var services = new ServiceCollection();

services.AddLibUsbDotNetUsbHid();

using var serviceProvider = services.BuildServiceProvider();
var usbHidService = serviceProvider.GetRequiredService<IUsbHidService>();

const int VendorId = 0x04d8;
const int ProductId = 0x00dd;

using IUsbHidDevice d = usbHidService.GetDevices(VendorId, ProductId).First();

// By casting from `IUsbHidDevice` to `IUsbHidDevice<TDevice>`,
// the implementation type of the backend library becomes accessible.
var device = (IUsbHidDevice<LibUsbDotNet.UsbDevice>)d;

// It is possible to access the UsbDevice object via the
// UnderlyingDevice property when using LibUsbDotNet.
// This allows you to access LibUsbDotNet-specific APIs.
LibUsbDotNet.UsbDevice usbDevice = device.UnderlyingDevice;

if (usbDevice.Open()) {
  Console.WriteLine("DevicePath: {0}", usbDevice.DevicePath);
  Console.WriteLine("Product: {0}", usbDevice.Info.ProductString);
  Console.WriteLine("Manufacturer: {0}", usbDevice.Info.ManufacturerString);
  Console.WriteLine("SerialNumber: {0}", usbDevice.Info.SerialString);
}

