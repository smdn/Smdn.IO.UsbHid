// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using HidApi;

namespace Smdn.IO.UsbHid;

// Since HidApiDefaultSession is never instantiated directly, warning CA1812 is generated;
// however, because it may be added to the IServiceCollection as the default implementation
// via TryAddSingleton<IHidApiSession, HidApiDefaultSession>(), suppress this warning.
#pragma warning disable CA1812 // Avoid uninstantiated internal classes
internal sealed class HidApiDefaultSession() : IHidApiSession {
#pragma warning restore CA1812
  public void Dispose()
    => Hid.Exit(); // Finalize HIDAPI library
}
