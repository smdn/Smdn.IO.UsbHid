// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
using System.Diagnostics.CodeAnalysis;
#endif
using System.Threading;
using System.Threading.Tasks;

namespace Smdn.IO.UsbHid.Abstractions;

/// <summary>
/// Represents a null object implementation of the <see cref="IUsbHidDevice"/>
/// interface that performs no operations.
/// </summary>
/// <remarks>
/// This class implements the Null Object pattern. All methods are designed to
/// be "no-op" (no operation), returning default values or indicating failure
/// without throwing exceptions.
/// </remarks>
public sealed class NullUsbHidDevice : IUsbHidDevice<NullUsbHidUnderlyingDevice> {
  public static NullUsbHidDevice Instance { get; } = new();

  /// <inheritdoc/>
  /// <value>Always returns <see cref="NullUsbHidUnderlyingDevice.Instance"/>.</value>
  public NullUsbHidUnderlyingDevice UnderlyingDevice => NullUsbHidUnderlyingDevice.Instance;

  /// <inheritdoc/>
  /// <value>Always <see langword="default"/>.</value>
  public int VendorId { get; } = default;

  /// <inheritdoc/>
  /// <value>Always <see langword="default"/>.</value>
  public int ProductId { get; } = default;

  private NullUsbHidDevice()
  {
    // prohibit instance creation
  }

  /// <inheritdoc/>
  /// <remarks>
  /// This implementation performs no operation.
  /// </remarks>
  public void Dispose()
  {
    // do nothing
  }

  /// <inheritdoc/>
  /// <remarks>
  /// This implementation performs no operation.
  /// </remarks>
  public ValueTask DisposeAsync()
    => default; // do nothing

  /// <inheritdoc/>
  /// <remarks>
  /// This implementation always returns <see langword="false"/> without retrieving any value.
  /// </remarks>
  public bool TryGetProductName(
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
    [NotNullWhen(true)]
#endif
    out string? productName
  )
  {
    productName = default;

    return false;
  }

  /// <inheritdoc/>
  /// <remarks>
  /// This implementation always returns <see langword="false"/> without retrieving any value.
  /// </remarks>
  /// <returns>Always <see langword="false"/>.</returns>
  public bool TryGetManufacturer(
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
    [NotNullWhen(true)]
#endif
    out string? manufacturer
  )
  {
    manufacturer = default;

    return false;
  }

  /// <inheritdoc/>
  /// <remarks>
  /// This implementation always returns <see langword="false"/> without retrieving any value.
  /// </remarks>
  /// <returns>Always <see langword="false"/>.</returns>
  public bool TryGetSerialNumber(
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
    [NotNullWhen(true)]
#endif
    out string? serialNumber
  )
  {
    serialNumber = default;

    return false;
  }

  /// <inheritdoc/>
  /// <remarks>
  /// This implementation always returns <see langword="false"/> without retrieving any value.
  /// </remarks>
  /// <returns>Always <see langword="false"/>.</returns>
  public bool TryGetDeviceIdentifier(
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
    [NotNullWhen(true)]
#endif
    out string? deviceIdentifier
  )
  {
    deviceIdentifier = default;

    return false;
  }

  /// <inheritdoc/>
  /// <remarks>
  /// This implementation performs no operation and returns a <see cref="NullUsbHidEndPoint"/>.
  /// </remarks>
  public IUsbHidEndPoint OpenEndPoint(
    bool openOutEndPoint,
    bool openInEndPoint,
    bool shouldDisposeDevice,
    CancellationToken cancellationToken
  )
    => NullUsbHidEndPoint.Instance;

  /// <inheritdoc/>
  /// <remarks>
  /// This implementation performs no operation and returns a <see cref="NullUsbHidEndPoint"/>.
  /// </remarks>
  public ValueTask<IUsbHidEndPoint> OpenEndPointAsync(
    bool openOutEndPoint,
    bool openInEndPoint,
    bool shouldDisposeDevice,
    CancellationToken cancellationToken
  )
    => new(NullUsbHidEndPoint.Instance);
}
