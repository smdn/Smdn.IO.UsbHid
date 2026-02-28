// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#if !LIBUSBDOTNET_V3
using System;
#if SYSTEM_RUNTIME_INTEROPSERVICES_NATIVELIBRARY
using System.Collections.Generic;
#endif
using System.IO;
#if NETFRAMEWORK
using System.Linq;
#endif
#if NETFRAMEWORK || SYSTEM_RUNTIME_INTEROPSERVICES_NATIVELIBRARY
using System.Runtime.InteropServices;
#endif

using NUnit.Framework;

using MonoLibUsb;

namespace Smdn.IO.UsbHid;

[SetUpFixture]
public class LibUsbLifecycle {
  private class LibUsbSession : ILibUsbSession {
#if SYSTEM_RUNTIME_INTEROPSERVICES_NATIVELIBRARY
    private static readonly string[] LibUsbFileNameCandidates = [
      "libusb-1.0.so.0",
      "libusb-1.0.so",
      "libusb-1.0.dylib",
      "libusb-1.0.0.dylib",
      "libusb-1.0",
      "libusb-1.0.0"
    ];
    private static readonly string[] LibUsbInstallDirectoryCandidates = [
      "/usr/lib/",
      "/usr/local/lib/",
      "/opt/homebrew/lib/",
    ];
#endif

    private static bool IsRunningOnWindows()
#if NETFRAMEWORK
      => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#else
      => OperatingSystem.IsWindows();
#endif

    public void Initialize(LibUsbDotNetOptions options)
    {
      if (!IsRunningOnWindows())
        InitializeDllImportResolver();
    }

    private void InitializeDllImportResolver()
    {
#if SYSTEM_RUNTIME_INTEROPSERVICES_NATIVELIBRARY
      const string LibUsbDllName = "libusb-1.0";

      NativeLibrary.SetDllImportResolver(
        assembly: typeof(MonoLibUsb.MonoUsbApi).Assembly,
        resolver: (libraryName, assembly, searchPath) => {
          if (!LibUsbDllName.Equals(libraryName, StringComparison.OrdinalIgnoreCase))
            return IntPtr.Zero;

          var searchDirectories = new HashSet<string>();

          if (Environment.GetEnvironmentVariable("LD_LIBRARY_PATH") is string ldLibraryPath)
            searchDirectories.UnionWith(ldLibraryPath.Split(Path.PathSeparator));

          if (Environment.GetEnvironmentVariable("DYLD_LIBRARY_PATH") is string dyldLibraryPath)
            searchDirectories.UnionWith(dyldLibraryPath.Split(Path.PathSeparator));

          if (AppContext.GetData("NATIVE_DLL_SEARCH_DIRECTORIES") is string nativeDllSearchDirectories)
            searchDirectories.UnionWith(nativeDllSearchDirectories.Split(Path.PathSeparator));

          foreach (var possibleInstallDirectory in LibUsbInstallDirectoryCandidates) {
            if (Directory.Exists(possibleInstallDirectory))
              searchDirectories.Add(possibleInstallDirectory);
          }

          foreach (var searchDirectory in searchDirectories) {
            if (searchDirectory.Length == 0)
              continue;

            foreach (var libUsbFileName in LibUsbFileNameCandidates) {
              var pathToNativeLibrary = Path.Combine(searchDirectory, libUsbFileName);

              if (NativeLibrary.TryLoad(pathToNativeLibrary, out var handleOfLibUsb))
                return handleOfLibUsb;
            }
          }

          foreach (var libUsbFileName in LibUsbFileNameCandidates) {
            if (searchPath.HasValue && NativeLibrary.TryLoad(libUsbFileName, assembly, searchPath, out var handleOfLibUsbWithSearchPath))
              return handleOfLibUsbWithSearchPath;

            if (NativeLibrary.TryLoad(libUsbFileName, out var handleOfLibUsb2))
              return handleOfLibUsb2;
          }

          return IntPtr.Zero;
        }
      );
#endif
    }

    public void Dispose()
    {
      if (IsRunningOnWindows())
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
