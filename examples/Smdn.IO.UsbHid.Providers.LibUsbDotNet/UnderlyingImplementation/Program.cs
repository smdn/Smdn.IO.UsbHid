// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System.Linq;

using Microsoft.Extensions.DependencyInjection;

using Smdn.IO.UsbHid;
using Smdn.IO.UsbHid.DependencyInjection;

var services = new ServiceCollection();

services.AddLibUsbDotNetUsbHid();

var serviceProvider = services.BuildServiceProvider();

var usbHidService = serviceProvider.GetRequiredService<IUsbHidService>();

const int VendorId = 0x04d8;
const int ProductId = 0x00dd;

using IUsbHidDevice d = usbHidService.GetDevices(VendorId, ProductId).First();

// By casting from `IUsbHidDevice` to `IUsbHidDevice<TDevice>`,
// the implementation type of the backend library becomes accessible.
var device = (IUsbHidDevice<LibUsbDotNet.LibUsb.UsbDevice>)d;

// It is possible to access the UsbDevice object via the
// DeviceImplementation property when using LibUsbDotNet.
// This allows you to access LibUsbDotNet-specific APIs.
LibUsbDotNet.LibUsb.UsbDevice usbDevice = device.DeviceImplementation;

usbDevice.TryOpen();

Console.WriteLine("LocationId: {0}", usbDevice.LocationId);
Console.WriteLine("Product: {0}", usbDevice.Descriptor.Product);
Console.WriteLine("Manufacturer: {0}", usbDevice.Descriptor.Manufacturer);
Console.WriteLine("SerialNumber: {0}", usbDevice.Descriptor.SerialNumber);
