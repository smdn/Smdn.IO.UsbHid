// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#if LIBUSBDOTNET_V3
#error This file was written for LibUsbDotNet v2. It cannot be built for v3.
#endif

using System;
#if SYSTEM_RUNTIME_INTEROPSERVICES_NATIVELIBRARY
using System.Runtime.InteropServices;
#endif

using Microsoft.Extensions.Logging;

namespace Smdn.IO.UsbHid;

/// <summary>
/// A class that represents the options for the Route B session configurations.
/// </summary>
// To enable the addition of options in future versions, exposes as a class rather than a struct.
public sealed class LibUsbDotNetOptions {
  [CLSCompliant(false)]
  public LogLevel DebugLevel { get; set; }

#pragma warning disable IDE0360 // Property accessor can be simplified
#pragma warning disable SA1513 // Closing brace should be followed by blank line
  public int ReadEndPointBufferSize {
    get => field;
    set => field = value; // TODO: validation
  } = 0x100;

  public TimeSpan ReadEndPointTimeout {
    get => field;
    set => field = value; // TODO: validation
  } = TimeSpan.FromSeconds(10);

  public TimeSpan WriteEndPointTimeout {
    get => field;
    set => field = value; // TODO: validation
  } = TimeSpan.FromSeconds(10);
#pragma warning restore SA1513
#pragma warning restore IDE0360

  internal int LibUsbDebugLevel => DebugLevel switch {
    LogLevel.Trace or LogLevel.Debug => 3,
    LogLevel.Information => 3,
    LogLevel.Warning => 2,
    LogLevel.Error => 1,
    LogLevel.Critical => 1,
    LogLevel.None or _ => 0,
  };

#if SYSTEM_RUNTIME_INTEROPSERVICES_NATIVELIBRARY
  /// <summary>
  /// Gets or sets the library path for <c>libusb-1.0</c> referenced in native API calls.
  /// If a non-<see langword="null"/> null or non-empty string is specified, configure the
  /// default resolver that resolves to this path using <see cref="NativeLibrary.SetDllImportResolver"/>.
  /// </summary>
  /// <remarks>
  /// If the <see cref="LibUsbDllImportResolver"/> property is set to a value other than <see langword="null"/>,
  /// that resolver will be used prioritized.
  /// </remarks>
  /// <seealso cref="LibUsbDllImportResolver"/>
  public string? LibUsbLibraryPath { get; set; }

  /// <summary>
  /// Gets or sets the <see cref="DllImportResolver"/> for <c>libusb-1.0</c> referenced in native API calls.
  /// If a non-<see langword="null"/> value is specified, it is registered as the resolver
  /// that resolves <c>libusb-1.0</c> using <see cref="NativeLibrary.SetDllImportResolver"/>.
  /// </summary>
  /// <remarks>
  /// The setting of this property is prioritized over the setting of <see cref="LibUsbLibraryPath"/>.
  /// </remarks>
  /// <seealso cref="LibUsbLibraryPath"/>
  public DllImportResolver? LibUsbDllImportResolver { get; set; }
#endif

  /// <summary>
  /// Configure this instance to have the same values as the instance passed as an argument.
  /// </summary>
  /// <param name="baseOptions">
  /// A <see cref="LibUsbDotNetOptions"/> that holds the values that are used to configure this instance.
  /// </param>
  /// <exception cref="ArgumentNullException">
  /// <paramref name="baseOptions"/> is <see langword="null"/>.
  /// </exception>
  /// <returns>
  /// The current <see cref="LibUsbDotNetOptions"/> so that additional calls can be chained.
  /// </returns>
  public LibUsbDotNetOptions Configure(LibUsbDotNetOptions baseOptions)
  {
    if (baseOptions is null)
      throw new ArgumentNullException(nameof(baseOptions));

    DebugLevel = baseOptions.DebugLevel;
#if SYSTEM_RUNTIME_INTEROPSERVICES_NATIVELIBRARY
    LibUsbLibraryPath = baseOptions.LibUsbLibraryPath;
    LibUsbDllImportResolver = baseOptions.LibUsbDllImportResolver;
#endif

    return this;
  }
}
