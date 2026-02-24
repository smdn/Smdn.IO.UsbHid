// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#pragma warning disable SA1008, SA1110

#if LIBUSBDOTNET_V3
#pragma warning disable SA1649 // warning SA1649: File name should match first type name
#endif

using System;

using Microsoft.Extensions.DependencyInjection;

using Polly;
using Polly.Registry.KeyedRegistry;

namespace Smdn.IO.UsbHid.DependencyInjection;

#pragma warning disable IDE0055
public readonly record struct
#if LIBUSBDOTNET_V3
LibUsbDotNetV3ResiliencePipelineKeyPair<TServiceKey> :
  IEquatable<LibUsbDotNetV3ResiliencePipelineKeyPair<TServiceKey>>,
#else
LibUsbDotNetResiliencePipelineKeyPair<TServiceKey> :
  IEquatable<LibUsbDotNetResiliencePipelineKeyPair<TServiceKey>>,
#endif
  IResiliencePipelineKeyPair<TServiceKey, string>
#pragma warning restore IDE0055
{
  /// <summary>
  /// Gets a key of <typeparamref name="TServiceKey"/> type specified when the <see cref="ResiliencePipeline"/>
  /// is registered to the <see cref="IServiceCollection"/>.
  /// </summary>
  public TServiceKey ServiceKey { get; }

#if LIBUSBDOTNET_V3
  /// <summary name="pipelineKey">
  /// Gets a key for <see cref="ResiliencePipeline"/> referenced by <see cref="LibUsbDotNetV3UsbHidService"/>.
  /// </summary>
#else
  /// <summary name="pipelineKey">
  /// Gets a key for <see cref="ResiliencePipeline"/> referenced by <see cref="LibUsbDotNetUsbHidService"/>.
  /// </summary>
#endif
  public string PipelineKey { get; }

  public
#if LIBUSBDOTNET_V3
  LibUsbDotNetV3ResiliencePipelineKeyPair
#else
  LibUsbDotNetResiliencePipelineKeyPair
#endif
  (
    TServiceKey serviceKey,
    string pipelineKey
  )
  {
    if (pipelineKey is null)
      throw new ArgumentNullException(nameof(pipelineKey));
    if (string.IsNullOrEmpty(pipelineKey))
      throw new ArgumentException(message: "must be non-empty string", paramName: nameof(pipelineKey));

    ServiceKey = serviceKey;
    PipelineKey = pipelineKey;
  }

  public override string ToString()
    => $"{{{ServiceKey}:{PipelineKey}}}";
}
