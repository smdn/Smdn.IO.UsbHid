// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using HidApi;

using Microsoft.Extensions.Logging;

using Polly.Registry;

namespace Smdn.IO.UsbHid;

/// <summary>
/// An implementation of <see cref="IUsbHidService"/> that uses HidApi.Net as the backend.
/// </summary>
internal sealed class HidApiDotNetUsbHidService : IUsbHidService {
#if false // for future extension
  /// <summary>
  /// Gets the <see cref="HidApiDotNetOptions"/> used by this instance.
  /// </summary>
  public HidApiDotNetOptions Options { get; }
#endif

  private readonly ResiliencePipelineProvider<string>? resiliencePipelineProvider;
  private readonly ILoggerFactory? loggerFactory;

  private bool isDisposed;

  public HidApiDotNetUsbHidService(
#if false // for future extension
    HidApiDotNetOptions options,
#endif
    IHidApiSession hidApiSession,
    ResiliencePipelineProvider<string>? resiliencePipelineProvider,
    ILoggerFactory? loggerFactory
  )
  {
#if false // for future extension
    Options = options ?? throw new ArgumentNullException(nameof(options));

    hidApiSession.Initialize(options);
#endif

    this.loggerFactory = loggerFactory;
    this.resiliencePipelineProvider = resiliencePipelineProvider;
  }

  /// <inheritdoc/>
  public IReadOnlyList<IUsbHidDevice> GetDevices(
    CancellationToken cancellationToken = default
  )
  {
    if (isDisposed)
      throw new ObjectDisposedException(GetType().FullName);

    cancellationToken.ThrowIfCancellationRequested();

    return Hid
      .Enumerate()
      .Select(
        d => new HidApiDotNetUsbHidDevice(
          deviceInfo: d,
          resiliencePipelineProvider: resiliencePipelineProvider,
          logger: loggerFactory?.CreateLogger<HidApiDotNetUsbHidDevice>()
        )
      )
      .ToList();
  }

  /// <inheritdoc/>
  public void Dispose()
  {
    isDisposed = true;
  }

  /// <inheritdoc/>
  public ValueTask DisposeAsync()
  {
    isDisposed = true;

    return default;
  }

  public override string? ToString()
    => GetType().Assembly.GetName().Name;
}
