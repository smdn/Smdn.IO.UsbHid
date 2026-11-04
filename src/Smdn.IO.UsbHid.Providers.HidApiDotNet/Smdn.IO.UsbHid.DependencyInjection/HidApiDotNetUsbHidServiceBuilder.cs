// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Smdn.IO.UsbHid.DependencyInjection;

/// <summary>
/// A builder for <see cref="HidApiDotNetUsbHidService"/>.
/// </summary>
[CLSCompliant(false)]
public sealed class HidApiDotNetUsbHidServiceBuilder<TServiceKey> : UsbHidServiceBuilder<TServiceKey> {
  internal HidApiDotNetUsbHidServiceBuilder(
    IServiceCollection services,
    TServiceKey serviceKey,
    Func<TServiceKey, string?> selectOptionsNameForServiceKey
  ) : base(
      services: services,
      serviceKey: serviceKey,
      selectOptionsNameForServiceKey: selectOptionsNameForServiceKey
    )
  {
  }

  /// <inheritdoc/>
  public override IUsbHidService Build(
    IServiceProvider serviceProvider
  )
  {
#if false // for future extension
    var options = (serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider)))
      .GetRequiredService<IOptionsMonitor<HidApiDotNetOptions>>()
      .Get(name: GetOptionsName());
#else
    if (serviceProvider is null)
      throw new ArgumentNullException(nameof(serviceProvider));
#endif

    return new HidApiDotNetUsbHidService(
#if false // for future extension
      options: options,
#endif
      // IHidApiSession is a non-keyed singleton, and all
      // HidApiDotNetUsbHidService instances share it
      hidApiSession: serviceProvider.GetRequiredService<IHidApiSession>(),
      resiliencePipelineProvider: serviceProvider
        .GetResiliencePipelineProviderForHidApiDotNetUsbHidService(
          serviceKey: ServiceKey
        ),
      loggerFactory:
        // Attempt to get the ILoggerFactory with the specified service key
        serviceProvider.GetKeyedService<ILoggerFactory>(ServiceKey) ??
        // After that, attempt to get the default ILoggerFactory
        serviceProvider.GetService<ILoggerFactory>()
    );
  }
}
