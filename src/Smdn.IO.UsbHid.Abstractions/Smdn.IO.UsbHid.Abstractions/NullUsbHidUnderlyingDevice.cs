// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
namespace Smdn.IO.UsbHid.Abstractions;

/// <summary>
/// Represents a null object implementation for the <see cref="IUsbHidDevice{T}.UnderlyingDevice"/>
/// property that performs no operations.
/// </summary>
/// <remarks>
/// This class implements the Null Object pattern. All methods are designed to
/// be "no-op" (no operation), returning default values or indicating failure
/// without throwing exceptions.
/// </remarks>
public sealed class NullUsbHidUnderlyingDevice {
  public static NullUsbHidUnderlyingDevice Instance { get; } = new();

  private NullUsbHidUnderlyingDevice()
  {
    // prohibit instance creation
  }
}
