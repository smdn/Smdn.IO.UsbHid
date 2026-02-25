// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
#if SYSTEM_RUNTIME_INTEROPSERVICES_NATIVELIBRARY
using System.Runtime.InteropServices;
#endif

using Microsoft.Extensions.Logging;

using NUnit.Framework;

namespace Smdn.IO.UsbHid.DependencyInjection;

[TestFixture]
public class LibUsbDotNetOptionsTests {
  private static System.Collections.IEnumerable YieldTestCases_Configure()
  {
    yield return new object?[] {
      0,
      TimeSpan.FromSeconds(0),
      TimeSpan.FromSeconds(1),
      LogLevel.Debug
#if SYSTEM_RUNTIME_INTEROPSERVICES_NATIVELIBRARY
      ,
      null,
      null,
#endif
    };
    yield return new object?[] {
      1,
      TimeSpan.FromSeconds(2),
      TimeSpan.FromSeconds(3),
      LogLevel.Warning
#if SYSTEM_RUNTIME_INTEROPSERVICES_NATIVELIBRARY
      ,
      "lib.so",
      new DllImportResolver(
        (libraryName, assembly, searchPath) => throw new NotImplementedException()
      )
#endif
    };
  }

  [TestCaseSource(nameof(YieldTestCases_Configure))]
  public void Configure(
    int readBufferSize,
    TimeSpan readTimeout,
    TimeSpan writeTimeout,
    LogLevel debugLevel
#if SYSTEM_RUNTIME_INTEROPSERVICES_NATIVELIBRARY
    ,
    string? libUsbLibraryPath,
    DllImportResolver? libUsbDllImportResolver
#endif
  )
  {
    var baseOptions = new LibUsbDotNetOptions() {
      ReadEndPointBufferSize = readBufferSize,
      ReadEndPointTimeout = readTimeout,
      WriteEndPointTimeout = writeTimeout,
      DebugLevel = debugLevel,
#if SYSTEM_RUNTIME_INTEROPSERVICES_NATIVELIBRARY
      LibUsbLibraryPath = libUsbLibraryPath,
      LibUsbDllImportResolver = libUsbDllImportResolver,
#endif
    };

    var options = new LibUsbDotNetOptions();

    Assert.That(
      options.Configure(baseOptions),
      Is.SameAs(options)
    );

    Assert.That(options.ReadEndPointBufferSize, Is.EqualTo(baseOptions.ReadEndPointBufferSize));
    Assert.That(options.ReadEndPointTimeout, Is.EqualTo(baseOptions.ReadEndPointTimeout));
    Assert.That(options.WriteEndPointTimeout, Is.EqualTo(baseOptions.WriteEndPointTimeout));
    Assert.That(options.DebugLevel, Is.EqualTo(baseOptions.DebugLevel));
#if SYSTEM_RUNTIME_INTEROPSERVICES_NATIVELIBRARY
    Assert.That(options.LibUsbLibraryPath, Is.EqualTo(baseOptions.LibUsbLibraryPath));
    Assert.That(options.LibUsbDllImportResolver, Is.SameAs(baseOptions.LibUsbDllImportResolver));
#endif
  }
}