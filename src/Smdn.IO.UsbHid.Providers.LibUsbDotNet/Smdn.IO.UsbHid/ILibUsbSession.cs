// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#if LIBUSBDOTNET_V3
#error This file was written for LibUsbDotNet v2. It cannot be built for v3.
#endif
using System;

namespace Smdn.IO.UsbHid;

public interface ILibUsbSession : IDisposable {
  void Initialize(LibUsbDotNetOptions options);
}
