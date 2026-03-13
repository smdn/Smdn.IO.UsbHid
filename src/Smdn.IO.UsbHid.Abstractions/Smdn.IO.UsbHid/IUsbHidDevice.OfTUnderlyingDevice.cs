// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

namespace Smdn.IO.UsbHid;

/// <summary>
/// Provides a mechanism to abstract and operate USB HID devices,
/// and provides a property for accessing the underlying device object used by the backend library.
/// </summary>
/// <typeparam name="TUnderlyingDevice">
/// The type of the <see cref="UnderlyingDevice"/> property, which represents the
/// implementation-dependent underlying device object.
/// </typeparam>
public interface IUsbHidDevice<TUnderlyingDevice> : IUsbHidDevice where TUnderlyingDevice : notnull {
  /// <summary>
  /// Gets the implementation-dependent underlying device object.
  /// </summary>
  TUnderlyingDevice UnderlyingDevice { get; }
}
