// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#if LIBUSBDOTNET_V3
#error This file was written for LibUsbDotNet v2. It cannot be built for v3.
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using LibUsbDotNet;
using LibUsbDotNet.Descriptors;
using LibUsbDotNet.Main;

using Microsoft.Extensions.Logging;

using Polly.Registry;

namespace Smdn.IO.UsbHid;

/// <summary>
/// An implementation of <see cref="IUsbHidService"/> that uses LibUsbDotNet as the backend.
/// </summary>
internal sealed class LibUsbDotNetUsbHidService : IUsbHidService {
  /// <summary>
  /// Gets the <see cref="LibUsbDotNetOptions"/> used by this instance.
  /// </summary>
  public LibUsbDotNetOptions Options { get; }

  private readonly ResiliencePipelineProvider<string>? resiliencePipelineProvider;
  private readonly ILoggerFactory? loggerFactory;

  public LibUsbDotNetUsbHidService(
    LibUsbDotNetOptions options,
    ILibUsbSession libUsbSession,
    ResiliencePipelineProvider<string>? resiliencePipelineProvider,
    ILoggerFactory? loggerFactory
  )
  {
    Options = options ?? throw new ArgumentNullException(nameof(options));

    libUsbSession.Initialize(options);

    this.loggerFactory = loggerFactory;
    this.resiliencePipelineProvider = resiliencePipelineProvider;
  }

  /// <inheritdoc/>
  public IReadOnlyList<IUsbHidDevice> GetDevices(
    CancellationToken cancellationToken = default
  )
  {
    cancellationToken.ThrowIfCancellationRequested();

    var list = new List<IUsbHidDevice>(); // TODO: best initial capacity

    foreach (UsbRegistry registry in UsbDevice.AllDevices) {
#pragma warning disable CA2000
      if (!registry.Open(out var device))
        continue;
#pragma warning restore CA2000

      if (device.Configs.SelectMany(static c => c.InterfaceInfoList).Any(static i => i.Descriptor.Class == ClassCodeType.Hid)) {
        list.Add(
          new LibUsbDotNetUsbHidDevice(
            service: this,
            device: device,
            resiliencePipelineProvider: resiliencePipelineProvider,
            logger: loggerFactory?.CreateLogger<LibUsbDotNetUsbHidDevice>()
          )
        );
      }
    }

    return list;
  }

  /// <inheritdoc/>
  public void Dispose()
  {
    // nothing to do
  }

  /// <inheritdoc/>
  public ValueTask DisposeAsync()
    => default; // nothing to do

  public override string? ToString()
    => GetType().Assembly.GetName().Name;
}
