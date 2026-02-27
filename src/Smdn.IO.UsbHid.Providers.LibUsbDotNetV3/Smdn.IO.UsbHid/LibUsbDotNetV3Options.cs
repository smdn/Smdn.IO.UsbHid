// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Microsoft.Extensions.Logging;

namespace Smdn.IO.UsbHid;

/// <summary>
/// A class that represents the options for the Route B session configurations.
/// </summary>
// To enable the addition of options in future versions, exposes as a class rather than a struct.
public sealed class LibUsbDotNetV3Options {
  [CLSCompliant(false)]
  public LogLevel DebugLevel { get; set; }

#pragma warning disable IDE0360 // Property accessor can be simplified
#pragma warning disable SA1513 // Closing brace should be followed by blank line
  public TimeSpan ReadEndPointTimeout {
    get => field;
    set =>
      field = TimeSpan.Zero <= value
        ? value
        : throw new ArgumentOutOfRangeException(
            paramName: nameof(ReadEndPointTimeout),
            actualValue: value,
            message: "must be zero or positive value"
          );
  } = TimeSpan.FromSeconds(10);

  public TimeSpan WriteEndPointTimeout {
    get => field;
    set =>
      field = TimeSpan.Zero <= value
        ? value
        : throw new ArgumentOutOfRangeException(
            paramName: nameof(WriteEndPointTimeout),
            actualValue: value,
            message: "must be zero or positive value"
          );
  } = TimeSpan.FromSeconds(10);
#pragma warning restore SA1513
#pragma warning restore IDE0360

  internal LibUsbDotNet.LogLevel LibUsbDotNetDebugLevel => DebugLevel switch {
    LogLevel.Trace or LogLevel.Debug => LibUsbDotNet.LogLevel.Debug,
    LogLevel.Information => LibUsbDotNet.LogLevel.Info,
    LogLevel.Warning => LibUsbDotNet.LogLevel.Warning,
    LogLevel.Error => LibUsbDotNet.LogLevel.Error,
    LogLevel.Critical => LibUsbDotNet.LogLevel.Error,
    LogLevel.None or _ => LibUsbDotNet.LogLevel.None,
  };

  /// <summary>
  /// Configure this instance to have the same values as the instance passed as an argument.
  /// </summary>
  /// <param name="baseOptions">
  /// A <see cref="LibUsbDotNetV3Options"/> that holds the values that are used to configure this instance.
  /// </param>
  /// <exception cref="ArgumentNullException">
  /// <paramref name="baseOptions"/> is <see langword="null"/>.
  /// </exception>
  /// <returns>
  /// The current <see cref="LibUsbDotNetV3Options"/> so that additional calls can be chained.
  /// </returns>
  public LibUsbDotNetV3Options Configure(LibUsbDotNetV3Options baseOptions)
  {
    if (baseOptions is null)
      throw new ArgumentNullException(nameof(baseOptions));

    ReadEndPointTimeout = baseOptions.ReadEndPointTimeout;
    WriteEndPointTimeout = baseOptions.WriteEndPointTimeout;
    DebugLevel = baseOptions.DebugLevel;

    return this;
  }
}
