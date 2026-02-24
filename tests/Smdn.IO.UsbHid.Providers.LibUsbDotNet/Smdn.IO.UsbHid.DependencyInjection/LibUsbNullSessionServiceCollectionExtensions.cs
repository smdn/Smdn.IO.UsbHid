// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#if !LIBUSBDOTNET_V3
using System;

using Microsoft.Extensions.DependencyInjection;

namespace Smdn.IO.UsbHid.DependencyInjection;

internal static class LibUsbNullSessionServiceCollectionExtensions {
  private class LibUsbNullSession : ILibUsbSession {
    public static LibUsbNullSession Instance { get; } = new();

    public void Initialize(LibUsbDotNetOptions options)
    {
      // do nothing
    }

    public void Dispose()
    {
      // do nothing
    }
  }

  // The initialization and termination of libusb must be handled by the
  // LibUsbLifecycle class rather than the IServiceProvider for each
  // test case, so replace ILibUsbSession with an empty implementation.
  public static IServiceCollection AddLibUsbNullSession(
    this IServiceCollection services
  )
  {
#pragma warning disable CA1510
    if (services is null)
      throw new ArgumentNullException(nameof(services));
#pragma warning restore CA1510

    services.AddSingleton<ILibUsbSession>(LibUsbNullSession.Instance);

    return services;
  }
}
#endif
