// Smdn.IO.UsbHid.Providers.LibUsbDotNet.dll (Smdn.IO.UsbHid.Providers.LibUsbDotNet-1.0.0-preview3)
//   Name: Smdn.IO.UsbHid.Providers.LibUsbDotNet
//   AssemblyVersion: 1.0.0.0
//   InformationalVersion: 1.0.0-preview3+f1cc8729011e4f92a8ec7f2d2964048385e37942
//   TargetFramework: .NETCoreApp,Version=v10.0
//   Configuration: Release
//   Metadata: IsTrimmable=True
//   Metadata: RepositoryUrl=https://github.com/smdn/Smdn.IO.UsbHid
//   Metadata: RepositoryBranch=main
//   Metadata: RepositoryCommit=f1cc8729011e4f92a8ec7f2d2964048385e37942
//   Referenced assemblies:
//     LibUsbDotNet.LibUsbDotNet, Version=2.2.0.0, Culture=neutral, PublicKeyToken=c677239abe1e02a9
//     Microsoft.Extensions.DependencyInjection.Abstractions, Version=8.0.0.0, Culture=neutral, PublicKeyToken=adb9793829ddae60
//     Microsoft.Extensions.Logging.Abstractions, Version=6.0.0.0, Culture=neutral, PublicKeyToken=adb9793829ddae60
//     Microsoft.Extensions.Options, Version=6.0.0.0, Culture=neutral, PublicKeyToken=adb9793829ddae60
//     Polly.Core, Version=8.0.0.0, Culture=neutral, PublicKeyToken=c8a3ffc3f8f825cc
//     Polly.Extensions, Version=8.0.0.0, Culture=neutral, PublicKeyToken=c8a3ffc3f8f825cc
//     Smdn.Extensions.Polly.KeyedRegistry, Version=1.2.0.0, Culture=neutral
//     Smdn.IO.UsbHid.Abstractions, Version=1.0.0.0, Culture=neutral
//     System.Collections, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
//     System.ComponentModel, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
//     System.Linq, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
//     System.Memory, Version=10.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51
//     System.Runtime, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
//     System.Runtime.InteropServices, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
#nullable enable annotations

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using LibUsbDotNet;
using Polly;
using Polly.DependencyInjection;
using Polly.Registry;
using Polly.Retry;
using Smdn.IO.UsbHid;
using Smdn.IO.UsbHid.DependencyInjection;

namespace Smdn.IO.UsbHid {
  public interface ILibUsbSession : IDisposable {
    void Initialize(LibUsbDotNetOptions options);
  }

  public sealed class LibUsbDotNetOptions {
    public LibUsbDotNetOptions() {}

    public LogLevel DebugLevel { get; set; }
    public DllImportResolver? LibUsbDllImportResolver { get; set; }
    public string? LibUsbLibraryPath { get; set; }
    public int ReadEndPointBufferSize { get; set; }
    public TimeSpan ReadEndPointTimeout { get; set; }
    public TimeSpan WriteEndPointTimeout { get; set; }

    public LibUsbDotNetOptions Configure(LibUsbDotNetOptions baseOptions) {}
  }

  public sealed class LibUsbDotNetUsbHidDevice : IUsbHidDevice<UsbDevice> {
    public static string ResiliencePipelineKeyForOpenEndPoint { get; } = "LibUsbDotNetUsbHidDevice.resiliencePipelineOpenEndPoint";

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

  public sealed class LibUsbDotNetUsbHidEndPoint : IUsbHidEndPoint<UsbEndpointReader, UsbEndpointWriter> {
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
  public static class LibUsbDotNetServiceCollectionExtensions {
    public static IServiceCollection AddLibUsbDotNetUsbHid(this IServiceCollection services) {}
    public static IServiceCollection AddLibUsbDotNetUsbHid(this IServiceCollection services, Action<LibUsbDotNetUsbHidServiceBuilder<object?>, LibUsbDotNetOptions> configure) {}
    public static IServiceCollection AddLibUsbDotNetUsbHid(this IServiceCollection services, string serviceKey) {}
    public static IServiceCollection AddLibUsbDotNetUsbHid(this IServiceCollection services, string serviceKey, Action<LibUsbDotNetUsbHidServiceBuilder<string>, LibUsbDotNetOptions> configure) {}
    public static IServiceCollection AddLibUsbDotNetUsbHid<TServiceKey>(this IServiceCollection services, TServiceKey serviceKey, Func<TServiceKey, string?> selectOptionsNameForServiceKey) {}
    public static IServiceCollection AddLibUsbDotNetUsbHid<TServiceKey>(this IServiceCollection services, TServiceKey serviceKey, Func<TServiceKey, string?> selectOptionsNameForServiceKey, Action<LibUsbDotNetUsbHidServiceBuilder<TServiceKey>, LibUsbDotNetOptions> configure) {}
  }

  public static class LibUsbDotNetUsbHidServiceBuilderExtensions {
    public static LibUsbDotNetUsbHidServiceBuilder<TServiceKey> AddResiliencePipelineForOpenEndPoint<TServiceKey>(this LibUsbDotNetUsbHidServiceBuilder<TServiceKey> builder, Action<ResiliencePipelineBuilder, AddResiliencePipelineContext<LibUsbDotNetResiliencePipelineKeyPair<TServiceKey>>> configure) {}
    public static LibUsbDotNetUsbHidServiceBuilder<TServiceKey> AddResiliencePipelineForOpenEndPoint<TServiceKey>(this LibUsbDotNetUsbHidServiceBuilder<TServiceKey> builder, RetryStrategyOptions retryOptions) {}
  }

  public sealed class LibUsbDotNetUsbHidServiceBuilder<TServiceKey> : UsbHidServiceBuilder<TServiceKey> {
    public override IUsbHidService Build(IServiceProvider serviceProvider) {}
  }

  public static class LibUsbDotNetUsbHidServiceProviderExtensions {
    public static ResiliencePipelineProvider<string>? GetResiliencePipelineProviderForLibUsbDotNetUsbHidService(this IServiceProvider serviceProvider, object? serviceKey) {}
  }

  public readonly record struct LibUsbDotNetResiliencePipelineKeyPair<TServiceKey> {
    public LibUsbDotNetResiliencePipelineKeyPair(TServiceKey serviceKey, string pipelineKey) {}

    public string PipelineKey { get; }
    public TServiceKey ServiceKey { get; }

    public override string ToString() {}
  }
}
// API list generated by Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks v1.8.2.0.
// Smdn.Reflection.ReverseGenerating.ListApi.Core v1.6.2.0 (https://github.com/smdn/Smdn.Reflection.ReverseGenerating)
