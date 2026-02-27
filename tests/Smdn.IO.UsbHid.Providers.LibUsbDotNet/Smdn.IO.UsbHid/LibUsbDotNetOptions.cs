// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
#if SYSTEM_RUNTIME_INTEROPSERVICES_NATIVELIBRARY
using System.Runtime.InteropServices;
#endif

using Microsoft.Extensions.Logging;

using NUnit.Framework;

namespace Smdn.IO.UsbHid;

[TestFixture]
public class LibUsbDotNetOptionsTests {
  [Test]
  public void ReadEndPointBufferSize_OutOfRange(
    [Values(0, -1)] int value
  )
    => Assert.That(
      () => new LibUsbDotNetOptions() { ReadEndPointBufferSize = value },
      Throws
        .TypeOf<ArgumentOutOfRangeException>()
        .With
        .Property(nameof(ArgumentOutOfRangeException.ParamName))
        .EqualTo(nameof(LibUsbDotNetOptions.ReadEndPointBufferSize))
    );

  [Test]
  public void ReadEndPointBufferSize(
    [Values(1, 2, 0x100)] int value
  )
  {
    var options = new LibUsbDotNetOptions();

    Assert.That(
      () => options.ReadEndPointBufferSize = value,
      Throws.Nothing
    );
    Assert.That(options.ReadEndPointBufferSize, Is.EqualTo(value));
  }

  [Test]
  public void ReadEndPointTimeout_OutOfRange(
    [Values(-1, -60)] int timeout
  )
    => Assert.That(
      () => new LibUsbDotNetOptions() { ReadEndPointTimeout = TimeSpan.FromSeconds(timeout) },
      Throws
        .TypeOf<ArgumentOutOfRangeException>()
        .With
        .Property(nameof(ArgumentOutOfRangeException.ParamName))
        .EqualTo(nameof(LibUsbDotNetOptions.ReadEndPointTimeout))
    );

  [Test]
  public void ReadEndPointTimeout(
    [Values(0, 1, 60)] int timeout
  )
  {
    var options = new LibUsbDotNetOptions();

    Assert.That(
      () => options.ReadEndPointTimeout = TimeSpan.FromSeconds(timeout),
      Throws.Nothing
    );
    Assert.That(options.ReadEndPointTimeout, Is.EqualTo(TimeSpan.FromSeconds(timeout)));
  }

  [Test]
  public void WriteEndPointTimeout_OutOfRange(
    [Values(-1, -60)] int timeout
  )
    => Assert.That(
      () => new LibUsbDotNetOptions() { WriteEndPointTimeout = TimeSpan.FromSeconds(timeout) },
      Throws
        .TypeOf<ArgumentOutOfRangeException>()
        .With
        .Property(nameof(ArgumentOutOfRangeException.ParamName))
        .EqualTo(nameof(LibUsbDotNetOptions.WriteEndPointTimeout))
    );

  [Test]
  public void WriteEndPointTimeout(
    [Values(0, 1, 60)] int timeout
  )
  {
    var options = new LibUsbDotNetOptions();

    Assert.That(
      () => options.WriteEndPointTimeout = TimeSpan.FromSeconds(timeout),
      Throws.Nothing
    );
    Assert.That(options.WriteEndPointTimeout, Is.EqualTo(TimeSpan.FromSeconds(timeout)));
  }

  private static System.Collections.IEnumerable YieldTestCases_Configure()
  {
    yield return new object?[] {
      1,
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
      2,
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
