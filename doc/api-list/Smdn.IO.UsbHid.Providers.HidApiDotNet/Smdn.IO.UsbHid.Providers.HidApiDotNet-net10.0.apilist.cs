// Smdn.IO.UsbHid.Providers.HidApiDotNet.dll (Smdn.IO.UsbHid.Providers.HidApiDotNet-1.0.0)
//   Name: Smdn.IO.UsbHid.Providers.HidApiDotNet
//   AssemblyVersion: 1.0.0.0
//   InformationalVersion: 1.0.0+99531df9f0c4dd1a64011b27e47d324b2b3c004f
//   TargetFramework: .NETCoreApp,Version=v10.0
//   Configuration: Release
//   Metadata: IsTrimmable=True
//   Metadata: RepositoryUrl=https://github.com/smdn/Smdn.IO.UsbHid
//   Metadata: RepositoryBranch=main
//   Metadata: RepositoryCommit=99531df9f0c4dd1a64011b27e47d324b2b3c004f
//   Referenced assemblies:
//     HidApi.Net, Version=1.2.0.0, Culture=neutral
//     Microsoft.Extensions.DependencyInjection.Abstractions, Version=8.0.0.0, Culture=neutral, PublicKeyToken=adb9793829ddae60
//     Microsoft.Extensions.Logging.Abstractions, Version=6.0.0.0, Culture=neutral, PublicKeyToken=adb9793829ddae60
//     Polly.Core, Version=8.0.0.0, Culture=neutral, PublicKeyToken=c8a3ffc3f8f825cc
//     Polly.Extensions, Version=8.0.0.0, Culture=neutral, PublicKeyToken=c8a3ffc3f8f825cc
//     Smdn.Extensions.Polly.KeyedRegistry, Version=1.2.0.0, Culture=neutral
//     Smdn.IO.UsbHid.Abstractions, Version=1.0.0.0, Culture=neutral
//     System.Collections, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
//     System.ComponentModel, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
//     System.Linq, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
//     System.Runtime, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
#nullable enable annotations

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using HidApi;
using Polly;
using Polly.DependencyInjection;
using Polly.Registry;
using Polly.Retry;
using Smdn.IO.UsbHid;
using Smdn.IO.UsbHid.DependencyInjection;

namespace Smdn.IO.UsbHid {
  public interface IHidApiSession : IDisposable {
  }

  public sealed class HidApiDotNetUsbHidDevice : IUsbHidDevice<DeviceInfo> {
    public static string ResiliencePipelineKeyForOpenEndPoint { get; } = "HidApiDotNetUsbHidDevice.resiliencePipelineOpenEndPoint";

    public int ProductId { get; }
    public DeviceInfo UnderlyingDevice { get; }
    public int VendorId { get; }

    public void Dispose() {}
    public ValueTask DisposeAsync() {}
    public IUsbHidEndPoint OpenEndPoint(bool openOutEndPoint, bool openInEndPoint, bool shouldDisposeDevice, CancellationToken cancellationToken) {}
    public ValueTask<IUsbHidEndPoint> OpenEndPointAsync(bool openOutEndPoint, bool openInEndPoint, bool shouldDisposeDevice, CancellationToken cancellationToken) {}
    public override string? ToString() {}
    public bool TryGetDeviceIdentifier([NotNullWhen(true)] out string? deviceIdentifier) {}
    public bool TryGetManufacturer([NotNullWhen(true)] out string? manufacturer) {}
    public bool TryGetProductName([NotNullWhen(true)] out string? productName) {}
    public bool TryGetSerialNumber([NotNullWhen(true)] out string? serialNumber) {}
  }

  public sealed class HidApiDotNetUsbHidEndPoint : IUsbHidEndPoint<Device, Device> {
    public bool CanRead { get; }
    public bool CanWrite { get; }
    public IUsbHidDevice Device { get; }
    public Device? ReadEndPoint { get; }
    public Device? WriteEndPoint { get; }

    public void Dispose() {}
    public async ValueTask DisposeAsync() {}
    public int Read(Span<byte> buffer, CancellationToken cancellationToken) {}
    public ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken) {}
    public override string? ToString() {}
    public void Write(ReadOnlySpan<byte> buffer, CancellationToken cancellationToken) {}
    public ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken) {}
  }
}

namespace Smdn.IO.UsbHid.DependencyInjection {
  public static class HidApiDotNetServiceCollectionExtensions {
    public static IServiceCollection AddHidApiDotNetUsbHid(this IServiceCollection services) {}
    public static IServiceCollection AddHidApiDotNetUsbHid(this IServiceCollection services, Action<HidApiDotNetUsbHidServiceBuilder<object?>> configure) {}
    public static IServiceCollection AddHidApiDotNetUsbHid(this IServiceCollection services, string serviceKey) {}
    public static IServiceCollection AddHidApiDotNetUsbHid(this IServiceCollection services, string serviceKey, Action<HidApiDotNetUsbHidServiceBuilder<string>> configure) {}
    public static IServiceCollection AddHidApiDotNetUsbHid<TServiceKey>(this IServiceCollection services, TServiceKey serviceKey, Func<TServiceKey, string?> selectOptionsNameForServiceKey) {}
    public static IServiceCollection AddHidApiDotNetUsbHid<TServiceKey>(this IServiceCollection services, TServiceKey serviceKey, Func<TServiceKey, string?> selectOptionsNameForServiceKey, Action<HidApiDotNetUsbHidServiceBuilder<TServiceKey>> configure) {}
  }

  public static class HidApiDotNetUsbHidServiceBuilderExtensions {
    public static HidApiDotNetUsbHidServiceBuilder<TServiceKey> AddResiliencePipelineForOpenEndPoint<TServiceKey>(this HidApiDotNetUsbHidServiceBuilder<TServiceKey> builder, Action<ResiliencePipelineBuilder, AddResiliencePipelineContext<HidApiDotNetResiliencePipelineKeyPair<TServiceKey>>> configure) {}
    public static HidApiDotNetUsbHidServiceBuilder<TServiceKey> AddResiliencePipelineForOpenEndPoint<TServiceKey>(this HidApiDotNetUsbHidServiceBuilder<TServiceKey> builder, RetryStrategyOptions retryOptions) {}
  }

  public sealed class HidApiDotNetUsbHidServiceBuilder<TServiceKey> : UsbHidServiceBuilder<TServiceKey> {
    public override IUsbHidService Build(IServiceProvider serviceProvider) {}
  }

  public static class HidApiDotNetUsbHidServiceProviderExtensions {
    public static ResiliencePipelineProvider<string>? GetResiliencePipelineProviderForHidApiDotNetUsbHidService(this IServiceProvider serviceProvider, object? serviceKey) {}
  }

  public readonly record struct HidApiDotNetResiliencePipelineKeyPair<TServiceKey> {
    public HidApiDotNetResiliencePipelineKeyPair(TServiceKey serviceKey, string pipelineKey) {}

    public string PipelineKey { get; }
    public TServiceKey ServiceKey { get; }

    public override string ToString() {}
  }
}
// API list generated by Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks v1.8.2.0.
// Smdn.Reflection.ReverseGenerating.ListApi.Core v1.6.2.0 (https://github.com/smdn/Smdn.Reflection.ReverseGenerating)
