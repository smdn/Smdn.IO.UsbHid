// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Microsoft.Extensions.DependencyInjection;

using Polly;
using Polly.Registry.KeyedRegistry;

namespace Smdn.IO.UsbHid.DependencyInjection;

public readonly record struct HidApiDotNetResiliencePipelineKeyPair<TServiceKey> : IResiliencePipelineKeyPair<TServiceKey, string> {
  /// <summary>
  /// Gets a key of <typeparamref name="TServiceKey"/> type specified when the <see cref="ResiliencePipeline"/>
  /// is registered to the <see cref="IServiceCollection"/>.
  /// </summary>
  public TServiceKey ServiceKey { get; }

  /// <summary name="pipelineKey">
  /// Gets a key for <see cref="ResiliencePipeline"/> referenced by <see cref="HidApiDotNetUsbHidService"/>.
  /// </summary>
  public string PipelineKey { get; }

  public HidApiDotNetResiliencePipelineKeyPair(
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
