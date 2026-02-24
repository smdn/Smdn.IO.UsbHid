// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#pragma warning disable CA1812

#if LIBUSBDOTNET_V3
#error This file was written for LibUsbDotNet v2. It cannot be built for v3.
#endif
using System;
#if SYSTEM_RUNTIME_INTEROPSERVICES_NATIVELIBRARY
using System.Runtime.InteropServices;
#endif

using LibUsbDotNet;
using LibUsbDotNet.Main;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using MonoLibUsb;

namespace Smdn.IO.UsbHid;

internal sealed partial class LibUsbDefaultSession(IServiceProvider? serviceProvider) : ILibUsbSession {
  private readonly ILogger logger =
    (ILogger?)serviceProvider?.GetService<ILoggerFactory>()?.CreateLogger<LibUsbDefaultSession>() ??
    NullLogger.Instance;

  public void Initialize(
    LibUsbDotNetOptions options
  )
  {
    if (!UsbDevice.IsLinux)
      return;

#if SYSTEM_RUNTIME_INTEROPSERVICES_NATIVELIBRARY
    var libUsbDllImportResolver = options.LibUsbDllImportResolver;

    if (libUsbDllImportResolver is null && !string.IsNullOrEmpty(options.LibUsbLibraryPath)) {
      // the value specified for the `dllName` parameter used in [DllImport] for the
      // native API declared in MonoLibUsb.MonoUsbApi
      const string LibUsbDllName = "libusb-1.0";

      // create a libusb resolver that resolves to the specified library path
      libUsbDllImportResolver = (libraryName, assembly, searchPath) => {
        if (LibUsbDllName.Equals(libraryName, StringComparison.OrdinalIgnoreCase)) {
          if (NativeLibrary.TryLoad(options.LibUsbLibraryPath, out var handleOfLibUsb))
            return handleOfLibUsb;
        }

        return IntPtr.Zero;
      };
    }

    if (libUsbDllImportResolver is not null) {
      try {
        NativeLibrary.SetDllImportResolver(
          assembly: typeof(MonoUsbApi).Assembly,
          resolver: libUsbDllImportResolver
        );
      }
      catch (ArgumentException ex) {
        LogSetDllImportResolverFailed(ex);
      }
      catch (InvalidOperationException ex) {
        LogSetDllImportResolverFailed(ex);
      }
    }
#endif

    try {
      // attempt to open libusb-1.0 session handle
      MonoUsbEventHandler.Init();

      // set debug level for this session
      if (!MonoUsbEventHandler.SessionHandle.IsInvalid) {
        MonoUsbApi.SetDebug(
          MonoUsbEventHandler.SessionHandle,
          options.LibUsbDebugLevel
        );
      }
    }
    catch (UsbException ex) {
      LogSetDebugFailed(ex);
    }
  }

#if SYSTEM_RUNTIME_INTEROPSERVICES_NATIVELIBRARY
  [LoggerMessage(
    EventId = 0,
    Level = LogLevel.Warning,
    Message = "Calling SetDllImportResolver failed."
  )]
  private partial void LogSetDllImportResolverFailed(Exception ex);
#endif

  [LoggerMessage(
    EventId = 1,
    Level = LogLevel.Warning,
    Message = "Failed to set log level."
  )]
  private partial void LogSetDebugFailed(Exception ex);

  public void Dispose()
  {
    if (!UsbDevice.IsLinux)
      return;

    // close libusb-1.0 session handle
    MonoUsbEventHandler.Exit();
  }
}
