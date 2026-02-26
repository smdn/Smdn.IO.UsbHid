// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#if !LIBUSBDOTNET_V3
using System;
#if NETFRAMEWORK || SYSTEM_RUNTIME_INTEROPSERVICES_NATIVELIBRARY
using System.Runtime.InteropServices;
#endif

using NUnit.Framework;

using MonoLibUsb;

namespace Smdn.IO.UsbHid.DependencyInjection;

[SetUpFixture]
public class LibUsbLifecycle {
  private class LibUsbSession : ILibUsbSession {
#if SYSTEM_RUNTIME_INTEROPSERVICES_NATIVELIBRARY
    private static readonly string[] LibUsbFileNameCandidates = [
      "libusb-1.0.so.0",
      "libusb-1.0.dylib",
      "libusb-1.0.0.dylib",
      "libusb-1.0",
      "libusb-1.0.0"
    ];
#endif

    public void Initialize(LibUsbDotNetOptions options)
    {
#if SYSTEM_RUNTIME_INTEROPSERVICES_NATIVELIBRARY
      if (OperatingSystem.IsWindows())
        return;

      const string LibUsbDllName = "libusb-1.0";

      NativeLibrary.SetDllImportResolver(
        assembly: typeof(MonoLibUsb.MonoUsbApi).Assembly,
        resolver: (libraryName, assembly, searchPath) => {
          if (LibUsbDllName.Equals(libraryName, StringComparison.OrdinalIgnoreCase)) {
            foreach (var libUsbFileName in LibUsbFileNameCandidates) {
              if (NativeLibrary.TryLoad(libUsbFileName, assembly, searchPath, out var handleOfLibUsb))
                return handleOfLibUsb;
            }
          }

          return IntPtr.Zero;
        }
      );
#endif
    }

    public void Dispose()
    {
#if NETFRAMEWORK
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
#else
      if (OperatingSystem.IsWindows())
#endif
        return;

      MonoUsbEventHandler.Exit();
    }
  }

  private readonly LibUsbSession assemblyLevelLibUsbSession = new();

  [OneTimeSetUp]
  public void SetUpLibUsb10()
  {
    assemblyLevelLibUsbSession.Initialize(null!);
  }

  [OneTimeTearDown]
  public void TearDownLibUsb10()
  {
    assemblyLevelLibUsbSession.Dispose();
  }
}
#endif
