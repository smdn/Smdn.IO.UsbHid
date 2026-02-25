// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
#if SYSTEM_DIAGNOSTICS_CODEANALYSIS_MEMBERNOTNULLATTRIBUTE
using System.Diagnostics.CodeAnalysis;
#endif
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using LibUsbDotNet;
using LibUsbDotNet.LibUsb;

using Microsoft.Extensions.Logging;

using Polly.Registry;

namespace Smdn.IO.UsbHid;

/// <summary>
/// An implementation of <see cref="IUsbHidService"/> that uses LibUsbDotNet as the backend.
/// </summary>
internal sealed class LibUsbDotNetV3UsbHidService : IUsbHidService {
  /// <summary>
  /// Gets the <see cref="LibUsbDotNetV3Options"/> used by this instance.
  /// </summary>
  public LibUsbDotNetV3Options Options { get; }

  private readonly ResiliencePipelineProvider<string>? resiliencePipelineProvider;
  private readonly ILoggerFactory? loggerFactory;
  private UsbContext context;

  public LibUsbDotNetV3UsbHidService(
    LibUsbDotNetV3Options options,
    ResiliencePipelineProvider<string>? resiliencePipelineProvider,
    ILoggerFactory? loggerFactory
  )
  {
    Options = options ?? throw new ArgumentNullException(nameof(options));

    this.loggerFactory = loggerFactory;
    this.resiliencePipelineProvider = resiliencePipelineProvider;

    context = new UsbContext();
    context.SetDebugLevel(Options.LibUsbDotNetDebugLevel);
  }

  /// <inheritdoc/>
  public IReadOnlyList<IUsbHidDevice> GetDevices(
    CancellationToken cancellationToken = default
  )
  {
    ThrowIfDisposed();

    cancellationToken.ThrowIfCancellationRequested();

    var deviceList = context.List();
    var list = new List<IUsbHidDevice>(capacity: deviceList.Count);

    foreach (var device in deviceList.OfType<UsbDevice>()) {
      if (device.Configs.SelectMany(static c => c.Interfaces).Any(static i => i.Class == ClassCode.Hid)) {
        list.Add(
          new LibUsbDotNetV3UsbHidDevice(
            service: this,
            device: device,
            resiliencePipelineProvider: resiliencePipelineProvider,
            logger: loggerFactory?.CreateLogger<LibUsbDotNetV3UsbHidDevice>()
          )
        );
      }
    }

    return list;
  }

#if SYSTEM_DIAGNOSTICS_CODEANALYSIS_MEMBERNOTNULLATTRIBUTE
  [MemberNotNull(nameof(context))]
#endif
  private void ThrowIfDisposed()
  {
    if (context is null)
      throw new ObjectDisposedException(GetType().FullName);
  }

  /// <inheritdoc/>
  public void Dispose()
  {
    context?.Dispose();
    context = null!;
  }

  /// <inheritdoc/>
  /// <remarks>
  /// This implementation performs a synchronous disposal, as the
  /// underlying <see cref="UsbContext"/> does not support asynchronous disposal.
  /// </remarks>
  public ValueTask DisposeAsync()
  {
    // UsbContext does not implement IAsyncDisposable
    context?.Dispose();
    context = null!;

    return default;
  }

  public override string? ToString()
    => GetType().Assembly.GetName().Name;
}
