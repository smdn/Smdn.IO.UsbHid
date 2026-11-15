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

// Get and list all devices with a specific Vendor ID and Product ID
foreach (IUsbHidDevice device in usbHidService.GetDevices(VendorId, ProductId)) {
  Console.WriteLine($"{device.VendorId:X4}:{device.ProductId:X4}");

  if (device.TryGetProductName(out var productName))
    Console.WriteLine($"  {productName}");

  if (device.TryGetManufacturer(out var manufacturer))
    Console.WriteLine($"  {manufacturer}");

  if (device.TryGetSerialNumber(out var serialNumber))
    Console.WriteLine($"  {serialNumber}");
}
