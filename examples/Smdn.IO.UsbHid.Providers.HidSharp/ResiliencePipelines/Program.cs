// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Polly.Telemetry;

using Smdn.IO.UsbHid;
using Smdn.IO.UsbHid.DependencyInjection;

var services = new ServiceCollection();

services
  .AddHidSharpUsbHid(
    configure: builder => {
      // Add a Polly resilience pipeline used when opening HID device endpoints
      builder.AddResiliencePipelineForOpenEndPoint(
        retryOptions: new() {
          MaxRetryAttempts = 1,
          Delay = TimeSpan.FromMilliseconds(500),
        }
      );
    }
  )
  .AddLogging(
    builder => builder
      .AddSimpleConsole(static options => options.SingleLine = true)
      // Specifies the log level output by HidSharpUsbHidDevice
      .AddFilter(typeof(HidSharpUsbHidDevice).FullName, LogLevel.Debug)
      // Specifies the log level output by Polly
      .AddFilter("Polly", LogLevel.Warning)
  );

var usbHidService =  services.BuildServiceProvider().GetRequiredService<IUsbHidService>();

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
