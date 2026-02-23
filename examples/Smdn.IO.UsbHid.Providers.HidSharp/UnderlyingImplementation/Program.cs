// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System.Linq;

using Microsoft.Extensions.DependencyInjection;

using Smdn.IO.UsbHid;
using Smdn.IO.UsbHid.DependencyInjection;

var services = new ServiceCollection();

services.AddHidSharpUsbHid();

var serviceProvider = services.BuildServiceProvider();

var usbHidService = serviceProvider.GetRequiredService<IUsbHidService>();

const int VendorId = 0x04d8;
const int ProductId = 0x00dd;

using IUsbHidDevice d = usbHidService.GetDevices(VendorId, ProductId).First();

// By casting from `IUsbHidDevice` to `IUsbHidDevice<TDevice>`,
// the implementation type of the backend library becomes accessible.
var device = (IUsbHidDevice<HidSharp.HidDevice>)d;

// It is possible to access the HidDevice object via the
// DeviceImplementation property when using HidSharp.
// This allows you to access HidSharp-specific APIs.
HidSharp.HidDevice hidDevice = device.DeviceImplementation;

Console.WriteLine("DevicePath: {0}", hidDevice.DevicePath);
Console.WriteLine("ProductName: {0}", hidDevice.GetProductName());
Console.WriteLine("SerialNumber: {0}", hidDevice.GetSerialNumber());
