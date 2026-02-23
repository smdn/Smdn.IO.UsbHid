using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Smdn.IO.UsbHid;
using Smdn.IO.UsbHid.DependencyInjection;

var services = new ServiceCollection();

// Register the IUsbHidService using LibUsbDotNet as the implementation provider
services.AddLibUsbDotNetUsbHid(
  configure: static (builder, options) => {
    // Specify the log level for output to stdout by LibUsbDotNet
    options.DebugLevel = LogLevel.Information;
  }
);

var serviceProvider = services.BuildServiceProvider();

// Request IUsbHidService from IServiceProvider
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

  // Open the device endpoint to send and receive HID reports
  using IUsbHidEndPoint endPoint = device.OpenEndPoint(shouldDisposeDevice: true);

  // await endPoint.ReadAsync(...);
  // await endPoint.WriteAsync(...);

  // Dispose the endpoint
  await endPoint.DisposeAsync();
  // Specifying true for `shouldDisposeDevice` when calling the OpenEndPoint()
  // method will also dispose of the original IUsbHidDevice along with
  // calling IUsbHidEndPoint.Dispose()
}
