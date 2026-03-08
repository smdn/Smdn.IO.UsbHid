// Smdn.IO.UsbHid.Abstractions.dll (Smdn.IO.UsbHid.Abstractions-1.0.0-preview3)
//   Name: Smdn.IO.UsbHid.Abstractions
//   AssemblyVersion: 1.0.0.0
//   InformationalVersion: 1.0.0-preview3+e6609dc8e8a36618e863c7b9c3888e6a77da5601
//   TargetFramework: .NETFramework,Version=v4.6.2
//   Configuration: Release
//   Metadata: RepositoryUrl=https://github.com/smdn/Smdn.IO.UsbHid
//   Metadata: RepositoryBranch=main
//   Metadata: RepositoryCommit=e6609dc8e8a36618e863c7b9c3888e6a77da5601
//   Referenced assemblies:
//     Microsoft.Bcl.AsyncInterfaces, Version=8.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51
//     Microsoft.Extensions.DependencyInjection.Abstractions, Version=2.1.0.0, Culture=neutral, PublicKeyToken=adb9793829ddae60
//     System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
//     System.Memory, Version=4.0.1.1, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51
//     System.Threading.Tasks.Extensions, Version=4.2.0.1, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51
//     mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
#nullable enable annotations

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Smdn.IO.UsbHid;

namespace Smdn.IO.UsbHid {
  public interface IUsbHidDevice :
    IAsyncDisposable,
    IDisposable
  {
    int ProductId { get; }
    int VendorId { get; }

    IUsbHidEndPoint OpenEndPoint(bool openOutEndPoint, bool openInEndPoint, bool shouldDisposeDevice, CancellationToken cancellationToken);
    ValueTask<IUsbHidEndPoint> OpenEndPointAsync(bool openOutEndPoint, bool openInEndPoint, bool shouldDisposeDevice, CancellationToken cancellationToken);
    bool TryGetDeviceIdentifier(out string? deviceIdentifier);
    bool TryGetManufacturer(out string? manufacturer);
    bool TryGetProductName(out string? productName);
    bool TryGetSerialNumber(out string? serialNumber);
  }

  public interface IUsbHidDevice<TDevice> : IUsbHidDevice where TDevice : notnull {
    TDevice UnderlyingDevice { get; }
  }

  public interface IUsbHidEndPoint :
    IAsyncDisposable,
    IDisposable
  {
    bool CanRead { get; }
    bool CanWrite { get; }
    IUsbHidDevice Device { get; }

    int Read(Span<byte> buffer, CancellationToken cancellationToken);
    ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken);
    void Write(ReadOnlySpan<byte> buffer, CancellationToken cancellationToken);
    ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken);
  }

  public interface IUsbHidEndPoint<TReadEndPoint, TWriteEndPoint> : IUsbHidEndPoint {
    TReadEndPoint ReadEndPoint { get; }
    TWriteEndPoint WriteEndPoint { get; }
  }

  public interface IUsbHidService :
    IAsyncDisposable,
    IDisposable
  {
    IReadOnlyList<IUsbHidDevice> GetDevices(CancellationToken cancellationToken);
  }

  public static class IUsbHidDeviceExtensions {
    public static IUsbHidEndPoint OpenEndPoint(this IUsbHidDevice device, CancellationToken cancellationToken = default) {}
    public static IUsbHidEndPoint OpenEndPoint(this IUsbHidDevice device, bool shouldDisposeDevice) {}
    public static IUsbHidEndPoint OpenEndPoint(this IUsbHidDevice device, bool shouldDisposeDevice, CancellationToken cancellationToken = default) {}
    public static ValueTask<IUsbHidEndPoint> OpenEndPointAsync(this IUsbHidDevice device, CancellationToken cancellationToken = default) {}
    public static ValueTask<IUsbHidEndPoint> OpenEndPointAsync(this IUsbHidDevice device, bool shouldDisposeDevice) {}
    public static ValueTask<IUsbHidEndPoint> OpenEndPointAsync(this IUsbHidDevice device, bool shouldDisposeDevice, CancellationToken cancellationToken = default) {}
    public static string ToIdentificationString(this IUsbHidDevice device) {}
  }

  public static class IUsbHidServiceExtensions {
    public static IUsbHidDevice? FindDevice(this IUsbHidService usbHidService, int? vendorId, int? productId, Predicate<IUsbHidDevice>? predicate = null, CancellationToken cancellationToken = default) {}
    public static IUsbHidDevice? FindDevice<TDevice>(this IUsbHidService usbHidService, int? vendorId, int? productId, Predicate<TDevice> predicate, CancellationToken cancellationToken = default) where TDevice : notnull {}
    public static IReadOnlyList<IUsbHidDevice> GetDevices(this IUsbHidService usbHidService, int? vendorId = null, int? productId = null, CancellationToken cancellationToken = default) {}
  }

  public class UsbHidException : InvalidOperationException {
    public UsbHidException() {}
    public UsbHidException(string? message) {}
    public UsbHidException(string? message, Exception? innerException) {}
  }
}

namespace Smdn.IO.UsbHid.DependencyInjection {
  public abstract class UsbHidServiceBuilder<TServiceKey> {
    protected UsbHidServiceBuilder(IServiceCollection services, TServiceKey serviceKey, Func<TServiceKey, string?>? selectOptionsNameForServiceKey) {}

    public TServiceKey ServiceKey { get; }
    public IServiceCollection Services { get; }

    public abstract IUsbHidService Build(IServiceProvider serviceProvider);
    public virtual string? GetOptionsName() {}
  }
}
// API list generated by Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks v1.8.2.0.
// Smdn.Reflection.ReverseGenerating.ListApi.Core v1.6.2.0 (https://github.com/smdn/Smdn.Reflection.ReverseGenerating)
