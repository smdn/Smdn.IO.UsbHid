// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Microsoft.Extensions.DependencyInjection;

namespace Smdn.IO.UsbHid.DependencyInjection;

internal static class UsbHidProviderServiceCollectionExtensions {
  public static partial IServiceCollection AddUsbHidProvider(
    this IServiceCollection services
  );
}
