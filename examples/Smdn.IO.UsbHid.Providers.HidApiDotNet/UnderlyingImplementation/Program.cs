// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using Microsoft.Extensions.DependencyInjection;

using Smdn.IO.UsbHid;
using Smdn.IO.UsbHid.DependencyInjection;

var services = new ServiceCollection();

services.AddHidApiDotNetUsbHid();

var serviceProvider = services.BuildServiceProvider();

var usbHidService = serviceProvider.GetRequiredService<IUsbHidService>();

const int VendorId = 0x04d8;
const int ProductId = 0x00dd;

using IUsbHidDevice d = usbHidService.GetDevices(VendorId, ProductId).First();

// By casting from `IUsbHidDevice` to `IUsbHidDevice<TDevice>`,
// the implementation type of the backend library becomes accessible.
var device = (IUsbHidDevice<HidApi.DeviceInfo>)d;

// It is possible to access the DeviceInfo object via the
// UnderlyingDevice property when using HidApi.Net.
// This allows you to access HidApi.Net-specific APIs.
HidApi.DeviceInfo hidApiDeviceInfo = device.UnderlyingDevice;

Console.WriteLine("Path: {0}", hidApiDeviceInfo.Path);
Console.WriteLine("BusType: {0}", hidApiDeviceInfo.BusType);
Console.WriteLine("Usage: {0}", hidApiDeviceInfo.Usage);
Console.WriteLine("UsagePage: {0}", hidApiDeviceInfo.UsagePage);
Console.WriteLine("InterfaceNumber: {0}", hidApiDeviceInfo.InterfaceNumber);
Console.WriteLine("ReleaseNumber: {0}", hidApiDeviceInfo.ReleaseNumber);
