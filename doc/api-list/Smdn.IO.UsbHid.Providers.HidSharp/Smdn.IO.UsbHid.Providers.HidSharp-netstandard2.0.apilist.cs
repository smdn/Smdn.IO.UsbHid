// Smdn.IO.UsbHid.Providers.HidSharp.dll (Smdn.IO.UsbHid.Providers.HidSharp-1.0.0-preview3)
//   Name: Smdn.IO.UsbHid.Providers.HidSharp
//   AssemblyVersion: 1.0.0.0
//   InformationalVersion: 1.0.0-preview3+f1cc8729011e4f92a8ec7f2d2964048385e37942
//   TargetFramework: .NETStandard,Version=v2.0
//   Configuration: Release
//   Metadata: RepositoryUrl=https://github.com/smdn/Smdn.IO.UsbHid
//   Metadata: RepositoryBranch=main
//   Metadata: RepositoryCommit=f1cc8729011e4f92a8ec7f2d2964048385e37942
//   Referenced assemblies:
//     HidSharp, Version=2.1.0.0, Culture=neutral
//     Microsoft.Bcl.AsyncInterfaces, Version=8.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51
//     Microsoft.Extensions.DependencyInjection.Abstractions, Version=8.0.0.0, Culture=neutral, PublicKeyToken=adb9793829ddae60
//     Microsoft.Extensions.Logging.Abstractions, Version=6.0.0.0, Culture=neutral, PublicKeyToken=adb9793829ddae60
//     Polly.Core, Version=8.0.0.0, Culture=neutral, PublicKeyToken=c8a3ffc3f8f825cc
//     Polly.Extensions, Version=8.0.0.0, Culture=neutral, PublicKeyToken=c8a3ffc3f8f825cc
//     Smdn.Extensions.Polly.KeyedRegistry, Version=1.2.0.0, Culture=neutral
//     Smdn.IO.UsbHid.Abstractions, Version=1.0.0.0, Culture=neutral
//     System.Buffers, Version=4.0.2.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51
//     System.Memory, Version=4.0.1.2, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51
//     System.Threading.Tasks.Extensions, Version=4.2.0.1, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51
//     netstandard, Version=2.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51
#nullable enable annotations

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using HidSharp;
using Polly;
using Polly.DependencyInjection;
using Polly.Registry;
using Polly.Retry;
using Smdn.IO.UsbHid;
using Smdn.IO.UsbHid.DependencyInjection;

namespace Smdn.IO.UsbHid {
  public sealed class HidSharpUsbHidDevice : IUsbHidDevice<HidDevice> {
    public static string ResiliencePipelineKeyForOpenEndPoint { get; } = "HidSharpUsbHidDevice.resiliencePipelineOpenEndPoint";

    public int ProductId { get; }
    public HidDevice UnderlyingDevice { get; }
    public int VendorId { get; }

    public void Dispose() {}
    public ValueTask DisposeAsync() {}
    public IUsbHidEndPoint OpenEndPoint(bool openOutEndPoint, bool openInEndPoint, bool shouldDisposeDevice, CancellationToken cancellationToken) {}
    public ValueTask<IUsbHidEndPoint> OpenEndPointAsync(bool openOutEndPoint, bool openInEndPoint, bool shouldDisposeDevice, CancellationToken cancellationToken) {}
    public override string? ToString() {}
    public bool TryGetDeviceIdentifier(out string? deviceIdentifier) {}
    public bool TryGetManufacturer(out string? manufacturer) {}
    public bool TryGetProductName(out string? productName) {}
    public bool TryGetSerialNumber(out string? serialNumber) {}
  }

  public sealed class HidSharpUsbHidEndPoint : IUsbHidEndPoint<HidStream, HidStream> {
    public bool CanRead { get; }
    public bool CanWrite { get; }
    public IUsbHidDevice Device { get; }
    public HidStream? ReadEndPoint { get; }
    public HidStream? WriteEndPoint { get; }

    public void Dispose() {}
    public ValueTask DisposeAsync() {}
    public int Read(Span<byte> buffer, CancellationToken cancellationToken) {}
    public ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken) {}
    public override string? ToString() {}
    public void Write(ReadOnlySpan<byte> buffer, CancellationToken cancellationToken) {}
    public ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken) {}
  }
}

namespace Smdn.IO.UsbHid.DependencyInjection {
  public static class HidSharpServiceCollectionExtensions {
    public static IServiceCollection AddHidSharpUsbHid(this IServiceCollection services) {}
    public static IServiceCollection AddHidSharpUsbHid(this IServiceCollection services, Action<HidSharpUsbHidServiceBuilder<object?>> configure) {}
    public static IServiceCollection AddHidSharpUsbHid(this IServiceCollection services, string serviceKey) {}
    public static IServiceCollection AddHidSharpUsbHid(this IServiceCollection services, string serviceKey, Action<HidSharpUsbHidServiceBuilder<string>> configure) {}
    public static IServiceCollection AddHidSharpUsbHid<TServiceKey>(this IServiceCollection services, TServiceKey serviceKey, Func<TServiceKey, string?> selectOptionsNameForServiceKey) {}
    public static IServiceCollection AddHidSharpUsbHid<TServiceKey>(this IServiceCollection services, TServiceKey serviceKey, Func<TServiceKey, string?> selectOptionsNameForServiceKey, Action<HidSharpUsbHidServiceBuilder<TServiceKey>> configure) {}
  }

  public static class HidSharpUsbHidServiceBuilderExtensions {
    public static HidSharpUsbHidServiceBuilder<TServiceKey> AddResiliencePipelineForOpenEndPoint<TServiceKey>(this HidSharpUsbHidServiceBuilder<TServiceKey> builder) {}
    public static HidSharpUsbHidServiceBuilder<TServiceKey> AddResiliencePipelineForOpenEndPoint<TServiceKey>(this HidSharpUsbHidServiceBuilder<TServiceKey> builder, Action<ResiliencePipelineBuilder, AddResiliencePipelineContext<HidSharpResiliencePipelineKeyPair<TServiceKey>>> configure) {}
    public static HidSharpUsbHidServiceBuilder<TServiceKey> AddResiliencePipelineForOpenEndPoint<TServiceKey>(this HidSharpUsbHidServiceBuilder<TServiceKey> builder, RetryStrategyOptions retryOptions) {}
  }

  public sealed class HidSharpUsbHidServiceBuilder<TServiceKey> : UsbHidServiceBuilder<TServiceKey> {
    public override IUsbHidService Build(IServiceProvider serviceProvider) {}
  }

  public static class HidSharpUsbHidServiceProviderExtensions {
    public static ResiliencePipelineProvider<string>? GetResiliencePipelineProviderForHidSharpUsbHidService(this IServiceProvider serviceProvider, object? serviceKey) {}
  }

  public readonly record struct HidSharpResiliencePipelineKeyPair<TServiceKey> {
    public HidSharpResiliencePipelineKeyPair(TServiceKey serviceKey, string pipelineKey) {}

    public string PipelineKey { get; }
    public TServiceKey ServiceKey { get; }

    public override string ToString() {}
  }
}
// API list generated by Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks v1.8.2.0.
// Smdn.Reflection.ReverseGenerating.ListApi.Core v1.6.2.0 (https://github.com/smdn/Smdn.Reflection.ReverseGenerating)
