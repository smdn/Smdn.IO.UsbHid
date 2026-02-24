// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Polly.Telemetry;

using Smdn.IO.UsbHid;
using Smdn.IO.UsbHid.DependencyInjection;

var services = new ServiceCollection();

services
  .AddLibUsbDotNetV3UsbHid(
    configure: (builder, options) => {
      // Add a Polly resilience pipeline used when opening HID device endpoints
      builder.AddResiliencePipelineForOpenEndPoint(
        retryOptions: new() {
          MaxRetryAttempts = 1,
          Delay = TimeSpan.FromMilliseconds(500),
        }
      );

      options.DebugLevel = LogLevel.Information;
    }
  )
  .AddLogging(
    builder => builder
      .AddSimpleConsole(static options => options.SingleLine = true)
      // Specifies the log level output by LibUsbDotNetV3UsbHidDevice
      .AddFilter(typeof(LibUsbDotNetV3UsbHidDevice).FullName, LogLevel.Debug)
      // Specifies the log level output by Polly
      .AddFilter("Polly", LogLevel.Warning)
  );

using var serviceProvider = services.BuildServiceProvider();
var usbHidService = serviceProvider.GetRequiredService<IUsbHidService>();

foreach (var device in usbHidService.GetDevices()) {
  Console.WriteLine($"{device.VendorId:X4}:{device.ProductId:X4}");

  // Open the device endpoint
  await using var endPoint = await device.OpenEndPointAsync(shouldDisposeDevice: true);
  // If an exception occurs, the registered ResiliencePipeline
  // will perform recovery processing

  // endPoint.ReadAsync(...);
  // endPoint.WriteAsync(...);

  await endPoint.DisposeAsync();
}
