// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
namespace Smdn.IO.UsbHid.Abstractions;

/// <summary>
/// Represents a null object implementation for the <see cref="IUsbHidEndPoint{TReadEndPoint, TWriteEndPoint}.WriteEndPoint"/>
/// property that performs no operations.
/// </summary>
/// <remarks>
/// This class implements the Null Object pattern. All methods are designed to
/// be "no-op" (no operation), returning default values or indicating failure
/// without throwing exceptions.
/// </remarks>
public sealed class NullUsbHidUnderlyingWriteEndPoint {
  public static NullUsbHidUnderlyingWriteEndPoint Instance { get; } = new();

  private NullUsbHidUnderlyingWriteEndPoint()
  {
    // prohibit instance creation
  }
}
