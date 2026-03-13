// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

namespace Smdn.IO.UsbHid;

/// <summary>
/// Provides a mechanism to abstract reading from and writing to USB HID report endpoints,
/// and provides properties for accessing the underlying endpoint objects used by
/// the backend library.
/// </summary>
/// <typeparam name="TUnderlyingReadEndPoint">
/// The type of the <see cref="ReadEndPoint"/> property, which represents the
/// implementation-dependent underlying endpoint object for reading.
/// </typeparam>
/// <typeparam name="TUnderlyingWriteEndPoint">
/// The type of the <see cref="WriteEndPoint"/> property, which represents the
/// implementation-dependent underlying endpoint object for writing.
/// </typeparam>
public interface IUsbHidEndPoint<TUnderlyingReadEndPoint, TUnderlyingWriteEndPoint> : IUsbHidEndPoint {
  /// <summary>
  /// Gets the implementation-dependent underlying endpoint object for reading.
  /// </summary>
  TUnderlyingReadEndPoint? ReadEndPoint { get; }

  /// <summary>
  /// Gets the implementation-dependent underlying endpoint object for writing.
  /// </summary>
  TUnderlyingWriteEndPoint? WriteEndPoint { get; }
}
