[![GitHub license](https://img.shields.io/github/license/smdn/Smdn.IO.UsbHid)](https://github.com/smdn/Smdn.IO.UsbHid/blob/main/LICENSE.txt)
[![tests/main](https://img.shields.io/github/actions/workflow/status/smdn/Smdn.IO.UsbHid/test.yml?branch=main&label=tests%2Fmain)](https://github.com/smdn/Smdn.IO.UsbHid/actions/workflows/test.yml)
[![CodeQL](https://github.com/smdn/Smdn.IO.UsbHid/actions/workflows/codeql-analysis.yml/badge.svg?branch=main)](https://github.com/smdn/Smdn.IO.UsbHid/actions/workflows/codeql-analysis.yml)

# Smdn.IO.UsbHid.Abstractions
[![NuGet Smdn.IO.UsbHid.Abstractions](https://img.shields.io/nuget/v/Smdn.IO.UsbHid.Abstractions.svg)](https://www.nuget.org/packages/Smdn.IO.UsbHid.Abstractions/)

Provides a unified and abstracted API for interacting with USB HID devices, specifically focusing on HID report I/O operations.

This library provides a unified interface for interacting with USB HID devices in .NET. It defines core abstractions including `IUsbHidService`, `IUsbHidDevice`, and `IUsbHidEndPoint` to represent device discovery and report I/O operations.

The design decouples application logic from specific hardware access libraries through a dependency injection-friendly structure using `UsbHidServiceBuilder`. Extension methods are included to simplify common tasks such as finding specific devices by VID/PID and opening endpoints. It serves as the common foundation for the `Smdn.IO.UsbHid.*` libraries, ensuring a consistent API regardless of the chosen backend.



# Smdn.IO.UsbHid.Providers.HidSharp
[![NuGet Smdn.IO.UsbHid.Providers.HidSharp](https://img.shields.io/nuget/v/Smdn.IO.UsbHid.Providers.HidSharp)](https://www.nuget.org/packages/Smdn.IO.UsbHid.Providers.HidSharp/)

A backend provider for `Smdn.IO.UsbHid.*` that utilizes the [HIDSharp](https://github.com/SeekHisKingdom/HIDSharp) library for hardware communication.

It maps the common interfaces to HIDSharp's `HidDevice` and `HidStream`, and it supports standard DI registration via the `AddHidSharpUsbHid` extension methods. The implementation allows for the integration of [Polly](https://github.com/App-vNext/Polly) resilience pipelines for the endpoint opening process through the `HidSharpUsbHidServiceBuilder`. While adhering to the unified API, the underlying `HidDevice` instance remains accessible via the `IUsbHidDevice.UnderlyingDevice` property for library-specific operations.



# Smdn.IO.UsbHid.Providers.LibUsbDotNet
[![NuGet Smdn.IO.UsbHid.Providers.LibUsbDotNet](https://img.shields.io/nuget/v/Smdn.IO.UsbHid.Providers.LibUsbDotNet)](https://www.nuget.org/packages/Smdn.IO.UsbHid.Providers.LibUsbDotNet/)

A backend provider for `Smdn.IO.UsbHid.*` that utilizes the [LibUsbDotNet](https://github.com/LibUsbDotNet/LibUsbDotNet) library for hardware communication.

It offers specific configuration through `LibUsbDotNetOptions`, allowing users to define timeouts and buffer sizes for HID report transfers. The library maps `IUsbHidDevice` to `UsbDevice` and handles HID reports using LibUsbDotNet's endpoint reader and writer classes. Similar to other providers, it supports DI-based service registration and the configuration of [Polly](https://github.com/App-vNext/Polly) resilience pipelines for device access.






# Getting started and usage examples
More examples can be found in following examples directory.

- [Smdn.IO.UsbHid.Providers.HidSharp examples](examples/Smdn.IO.UsbHid.Providers.HidSharp)
- [Smdn.IO.UsbHid.Providers.LibUsbDotNet examples](examples/Smdn.IO.UsbHid.Providers.LibUsbDotNet)

# Troubleshooting
## DllImport resolving
LibUsbDotNet do DllImport-ing a shared library with the filename `libusb-1.0.so.0`.

If the libusb's .so filename installed on your system is different from that, use the [NativeLibrary.SetDllImportResolver()](https://docs.microsoft.com/ja-jp/dotnet/api/system.runtime.interopservices.nativelibrary.setdllimportresolver) to load installed .so file like below.

```sh
$ find /lib/ -name "libusb-*.so*"
/lib/x86_64-linux-gnu/libusb-1.0.so.x.y.z
/lib/i386-linux-gnu/libusb-1.0.so.x.y.z
```

```cs
using System.Runtime.InteropServices;

static void Main() {
  // libusb.so filename which is installed on your system
  const string fileNameLibUsb = "libusb-1.0.so.x.y.z";

  NativeLibrary.SetDllImportResolver(
    typeof(global::LibUsbDotNet.LibUsb.UsbDevice).Assembly,
    (libraryName, assembly, searchPath) => {
      if (string.Equals(libraryName, "libusb-1.0.so.0", StringComparison.OrdinalIgnoreCase)) {
        if (NativeLibrary.TryLoad(fileNameLibUsb, out var handleOfLibUsb))
          return handleOfLibUsb;
      }

      return IntPtr.Zero;
    }
  );

  // your codes here
    ︙
    ︙
}
```

# For contributors
Contributions are appreciated!

If there's a feature you would like to add or a bug you would like to fix, please read [Contribution guidelines](./CONTRIBUTING.md) and create an Issue or Pull Request.

IssueやPull Requestを送る際は、[Contribution guidelines](./CONTRIBUTING.md)をご覧頂ください。　可能なら英語が望ましいですが、日本語で構いません。


# Notice
<!-- #pragma section-start NupkgReadmeFile_Notice -->
## License
This project is licensed under the terms of the [MIT License](./LICENSE.txt).

## Credits
This project uses the following components. See [ThirdPartyNotices.md](./ThirdPartyNotices.md) for detail.

- [LibUsbDotNet/LibUsbDotNet](https://github.com/LibUsbDotNet/LibUsbDotNet)
- [SeekHisKingdom/HIDSharp](https://github.com/SeekHisKingdom/HIDSharp)
- [App-vNext/Polly](https://github.com/App-vNext/Polly)
<!-- #pragma section-end NupkgReadmeFile_Notice -->
