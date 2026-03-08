// Smdn.IO.UsbHid.Providers.LibUsbDotNetV3.dll (Smdn.IO.UsbHid.Providers.LibUsbDotNetV3-1.0.0-preview3)
//   Name: Smdn.IO.UsbHid.Providers.LibUsbDotNetV3
//   AssemblyVersion: 1.0.0.0
//   InformationalVersion: 1.0.0-preview3+f1cc8729011e4f92a8ec7f2d2964048385e37942
//   TargetFramework: .NETStandard,Version=v2.1
//   Configuration: Release
//   Metadata: RepositoryUrl=https://github.com/smdn/Smdn.IO.UsbHid
//   Metadata: RepositoryBranch=main
//   Metadata: RepositoryCommit=f1cc8729011e4f92a8ec7f2d2964048385e37942
//   Referenced assemblies:
//     LibUsbDotNet, Version=3.0.0.0, Culture=neutral, PublicKeyToken=c677239abe1e02a9
//     Microsoft.Extensions.DependencyInjection.Abstractions, Version=8.0.0.0, Culture=neutral, PublicKeyToken=adb9793829ddae60
//     Microsoft.Extensions.Logging.Abstractions, Version=6.0.0.0, Culture=neutral, PublicKeyToken=adb9793829ddae60
//     Microsoft.Extensions.Options, Version=6.0.0.0, Culture=neutral, PublicKeyToken=adb9793829ddae60
//     Polly.Core, Version=8.0.0.0, Culture=neutral, PublicKeyToken=c8a3ffc3f8f825cc
//     Polly.Extensions, Version=8.0.0.0, Culture=neutral, PublicKeyToken=c8a3ffc3f8f825cc
//     Smdn.Extensions.Polly.KeyedRegistry, Version=1.2.0.0, Culture=neutral
//     Smdn.IO.UsbHid.Abstractions, Version=1.0.0.0, Culture=neutral
//     netstandard, Version=2.1.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51
#nullable enable annotations

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using LibUsbDotNet.LibUsb;
using Polly;
using Polly.DependencyInjection;
using Polly.Registry;
using Polly.Retry;
using Smdn.IO.UsbHid;
using Smdn.IO.UsbHid.DependencyInjection;

namespace Smdn.IO.UsbHid {
  public sealed class LibUsbDotNetV3Options {
    public LibUsbDotNetV3Options() {}

    public LogLevel DebugLevel { get; set; }
    public TimeSpan ReadEndPointTimeout { get; set; }
    public TimeSpan WriteEndPointTimeout { get; set; }

    public LibUsbDotNetV3Options Configure(LibUsbDotNetV3Options baseOptions) {}
  }

  public sealed class LibUsbDotNetV3UsbHidDevice : IUsbHidDevice<UsbDevice> {
    public static string ResiliencePipelineKeyForOpenEndPoint { get; } = "LibUsbDotNetV3UsbHidDevice.resiliencePipelineOpenEndPoint";

    public int ProductId { get; }
    public UsbDevice UnderlyingDevice { get; }
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

  public sealed class LibUsbDotNetV3UsbHidEndPoint : IUsbHidEndPoint<UsbEndpointReader, UsbEndpointWriter> {
    public bool CanRead { get; }
    public bool CanWrite { get; }
    public IUsbHidDevice Device { get; }
    public UsbEndpointReader? ReadEndPoint { get; }
    public UsbEndpointWriter? WriteEndPoint { get; }

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
  public static class LibUsbDotNetV3ServiceCollectionExtensions {
    public static IServiceCollection AddLibUsbDotNetV3UsbHid(this IServiceCollection services) {}
    public static IServiceCollection AddLibUsbDotNetV3UsbHid(this IServiceCollection services, Action<LibUsbDotNetV3UsbHidServiceBuilder<object?>, LibUsbDotNetV3Options> configure) {}
    public static IServiceCollection AddLibUsbDotNetV3UsbHid(this IServiceCollection services, string serviceKey) {}
    public static IServiceCollection AddLibUsbDotNetV3UsbHid(this IServiceCollection services, string serviceKey, Action<LibUsbDotNetV3UsbHidServiceBuilder<string>, LibUsbDotNetV3Options> configure) {}
    public static IServiceCollection AddLibUsbDotNetV3UsbHid<TServiceKey>(this IServiceCollection services, TServiceKey serviceKey, Func<TServiceKey, string?> selectOptionsNameForServiceKey) {}
    public static IServiceCollection AddLibUsbDotNetV3UsbHid<TServiceKey>(this IServiceCollection services, TServiceKey serviceKey, Func<TServiceKey, string?> selectOptionsNameForServiceKey, Action<LibUsbDotNetV3UsbHidServiceBuilder<TServiceKey>, LibUsbDotNetV3Options> configure) {}
  }

  public static class LibUsbDotNetV3UsbHidServiceBuilderExtensions {
    public static LibUsbDotNetV3UsbHidServiceBuilder<TServiceKey> AddResiliencePipelineForOpenEndPoint<TServiceKey>(this LibUsbDotNetV3UsbHidServiceBuilder<TServiceKey> builder, Action<ResiliencePipelineBuilder, AddResiliencePipelineContext<LibUsbDotNetV3ResiliencePipelineKeyPair<TServiceKey>>> configure) {}
    public static LibUsbDotNetV3UsbHidServiceBuilder<TServiceKey> AddResiliencePipelineForOpenEndPoint<TServiceKey>(this LibUsbDotNetV3UsbHidServiceBuilder<TServiceKey> builder, RetryStrategyOptions retryOptions) {}
  }

  public sealed class LibUsbDotNetV3UsbHidServiceBuilder<TServiceKey> : UsbHidServiceBuilder<TServiceKey> {
    public override IUsbHidService Build(IServiceProvider serviceProvider) {}
  }

  public static class LibUsbDotNetV3UsbHidServiceProviderExtensions {
    public static ResiliencePipelineProvider<string>? GetResiliencePipelineProviderForLibUsbDotNetV3UsbHidService(this IServiceProvider serviceProvider, object? serviceKey) {}
  }

  public readonly record struct LibUsbDotNetV3ResiliencePipelineKeyPair<TServiceKey> {
    public LibUsbDotNetV3ResiliencePipelineKeyPair(TServiceKey serviceKey, string pipelineKey) {}

    public string PipelineKey { get; }
    public TServiceKey ServiceKey { get; }

    public override string ToString() {}
  }
}
// API list generated by Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks v1.8.2.0.
// Smdn.Reflection.ReverseGenerating.ListApi.Core v1.6.2.0 (https://github.com/smdn/Smdn.Reflection.ReverseGenerating)
