// Smdn.IO.UsbHid.Abstractions.dll (Smdn.IO.UsbHid.Abstractions-1.0.0-preview4)
//   Name: Smdn.IO.UsbHid.Abstractions
//   AssemblyVersion: 1.0.0.0
//   InformationalVersion: 1.0.0-preview4+b7044df27092d278564bc08759155314a915b3c7
//   TargetFramework: .NETStandard,Version=v2.1
//   Configuration: Release
//   Metadata: RepositoryUrl=https://github.com/smdn/Smdn.IO.UsbHid
//   Metadata: RepositoryBranch=main
//   Metadata: RepositoryCommit=b7044df27092d278564bc08759155314a915b3c7
//   Referenced assemblies:
//     Microsoft.Extensions.DependencyInjection.Abstractions, Version=2.1.0.0, Culture=neutral, PublicKeyToken=adb9793829ddae60
//     netstandard, Version=2.1.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51
#nullable enable annotations

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Smdn.IO.UsbHid;
using Smdn.IO.UsbHid.Abstractions;

namespace Smdn.IO.UsbHid {
  public interface IUsbHidDevice :
    IAsyncDisposable,
    IDisposable
  {
    int ProductId { get; }
    int VendorId { get; }

    IUsbHidEndPoint OpenEndPoint(bool openOutEndPoint, bool openInEndPoint, bool shouldDisposeDevice, CancellationToken cancellationToken);
    ValueTask<IUsbHidEndPoint> OpenEndPointAsync(bool openOutEndPoint, bool openInEndPoint, bool shouldDisposeDevice, CancellationToken cancellationToken);
    bool TryGetDeviceIdentifier([NotNullWhen(true)] out string? deviceIdentifier);
    bool TryGetManufacturer([NotNullWhen(true)] out string? manufacturer);
    bool TryGetProductName([NotNullWhen(true)] out string? productName);
    bool TryGetSerialNumber([NotNullWhen(true)] out string? serialNumber);
  }

  public interface IUsbHidDevice<TUnderlyingDevice> : IUsbHidDevice where TUnderlyingDevice : notnull {
    TUnderlyingDevice UnderlyingDevice { get; }
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

  public interface IUsbHidEndPoint<TUnderlyingReadEndPoint, TUnderlyingWriteEndPoint> : IUsbHidEndPoint {
    TUnderlyingReadEndPoint ReadEndPoint { get; }
    TUnderlyingWriteEndPoint WriteEndPoint { get; }
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
    public static IReadOnlyList<IUsbHidDevice> FindAllDevices(this IUsbHidService usbHidService, int? vendorId, int? productId, Predicate<IUsbHidDevice>? predicate = null, CancellationToken cancellationToken = default) {}
    public static IReadOnlyList<IUsbHidDevice> FindAllDevices<TUnderlyingDevice>(this IUsbHidService usbHidService, int? vendorId, int? productId, Predicate<TUnderlyingDevice> predicate, CancellationToken cancellationToken = default) where TUnderlyingDevice : notnull {}
    public static IUsbHidDevice? FindDevice(this IUsbHidService usbHidService, int? vendorId, int? productId, Predicate<IUsbHidDevice>? predicate = null, CancellationToken cancellationToken = default) {}
    public static IUsbHidDevice? FindDevice<TUnderlyingDevice>(this IUsbHidService usbHidService, int? vendorId, int? productId, Predicate<TUnderlyingDevice> predicate, CancellationToken cancellationToken = default) where TUnderlyingDevice : notnull {}
    public static IReadOnlyList<IUsbHidDevice> GetDevices(this IUsbHidService usbHidService, int? vendorId = null, int? productId = null, CancellationToken cancellationToken = default) {}
  }

  public class UsbHidException : InvalidOperationException {
    public UsbHidException() {}
    public UsbHidException(string? message) {}
    public UsbHidException(string? message, Exception? innerException) {}
  }
}

namespace Smdn.IO.UsbHid.Abstractions {
  public sealed class NullUsbHidDevice : IUsbHidDevice<NullUsbHidUnderlyingDevice> {
    public static NullUsbHidDevice Instance { get; } // = "Smdn.IO.UsbHid.Abstractions.NullUsbHidDevice"

    public int ProductId { get; }
    public NullUsbHidUnderlyingDevice UnderlyingDevice { get; }
    public int VendorId { get; }

    public void Dispose() {}
    public ValueTask DisposeAsync() {}
    public IUsbHidEndPoint OpenEndPoint(bool openOutEndPoint, bool openInEndPoint, bool shouldDisposeDevice, CancellationToken cancellationToken) {}
    public ValueTask<IUsbHidEndPoint> OpenEndPointAsync(bool openOutEndPoint, bool openInEndPoint, bool shouldDisposeDevice, CancellationToken cancellationToken) {}
    public bool TryGetDeviceIdentifier([NotNullWhen(true)] out string? deviceIdentifier) {}
    public bool TryGetManufacturer([NotNullWhen(true)] out string? manufacturer) {}
    public bool TryGetProductName([NotNullWhen(true)] out string? productName) {}
    public bool TryGetSerialNumber([NotNullWhen(true)] out string? serialNumber) {}
  }

  public sealed class NullUsbHidEndPoint : IUsbHidEndPoint<NullUsbHidUnderlyingReadEndPoint, NullUsbHidUnderlyingWriteEndPoint> {
    public static NullUsbHidEndPoint Instance { get; } // = "Smdn.IO.UsbHid.Abstractions.NullUsbHidEndPoint"

    public bool CanRead { get; }
    public bool CanWrite { get; }
    public IUsbHidDevice Device { get; }
    public NullUsbHidUnderlyingReadEndPoint ReadEndPoint { get; }
    public NullUsbHidUnderlyingWriteEndPoint WriteEndPoint { get; }

    public void Dispose() {}
    public ValueTask DisposeAsync() {}
    public int Read(Span<byte> buffer, CancellationToken cancellationToken = default) {}
    public ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) {}
    public void Write(ReadOnlySpan<byte> buffer, CancellationToken cancellationToken = default) {}
    public ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default) {}
  }

  public sealed class NullUsbHidUnderlyingDevice {
    public static NullUsbHidUnderlyingDevice Instance { get; } // = "Smdn.IO.UsbHid.Abstractions.NullUsbHidUnderlyingDevice"
  }

  public sealed class NullUsbHidUnderlyingReadEndPoint {
    public static NullUsbHidUnderlyingReadEndPoint Instance { get; } // = "Smdn.IO.UsbHid.Abstractions.NullUsbHidUnderlyingReadEndPoint"
  }

  public sealed class NullUsbHidUnderlyingWriteEndPoint {
    public static NullUsbHidUnderlyingWriteEndPoint Instance { get; } // = "Smdn.IO.UsbHid.Abstractions.NullUsbHidUnderlyingWriteEndPoint"
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
