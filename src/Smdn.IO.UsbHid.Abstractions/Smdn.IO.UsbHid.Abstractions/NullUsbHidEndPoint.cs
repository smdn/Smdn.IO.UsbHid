// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Smdn.IO.UsbHid.Abstractions;

/// <summary>
/// Represents a null object implementation of the <see cref="IUsbHidEndPoint"/>
/// interface that performs no operations.
/// </summary>
/// <remarks>
/// This class implements the Null Object pattern. All methods are designed to
/// be "no-op" (no operation), returning default values or indicating failure
/// without throwing exceptions.
/// </remarks>
public sealed class NullUsbHidEndPoint : IUsbHidEndPoint<NullUsbHidUnderlyingReadEndPoint, NullUsbHidUnderlyingWriteEndPoint> {
  public static NullUsbHidEndPoint Instance { get; } = new();

  /// <inheritdoc/>
  /// <value>Always returns <see cref="NullUsbHidDevice.Instance"/>.</value>
  public IUsbHidDevice Device => NullUsbHidDevice.Instance;

  /// <inheritdoc/>
  /// <value>Always returns <see cref="NullUsbHidUnderlyingReadEndPoint.Instance"/>.</value>
  public NullUsbHidUnderlyingReadEndPoint ReadEndPoint => NullUsbHidUnderlyingReadEndPoint.Instance;

  /// <inheritdoc/>
  /// <value>Always returns <see cref="NullUsbHidUnderlyingWriteEndPoint.Instance"/>.</value>
  public NullUsbHidUnderlyingWriteEndPoint WriteEndPoint => NullUsbHidUnderlyingWriteEndPoint.Instance;

  /// <inheritdoc/>
  /// <value>Always <see langword="true"/>.</value>
  public bool CanRead => true;

  /// <inheritdoc/>
  /// <value>Always <see langword="true"/>.</value>
  public bool CanWrite => true;

  private NullUsbHidEndPoint()
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
  /// This implementation performs no operation and returns <c>0</c>.
  /// </remarks>
  public int Read(Span<byte> buffer, CancellationToken cancellationToken = default)
    => 0; // do nothing

  /// <inheritdoc/>
  /// <remarks>
  /// This implementation performs no operation and returns <c>0</c>.
  /// </remarks>
  public ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    => new(0); // do nothing

  /// <inheritdoc/>
  /// <remarks>
  /// This implementation performs no operation.
  /// </remarks>
  public void Write(ReadOnlySpan<byte> buffer, CancellationToken cancellationToken = default)
  {
    // do nothing
  }

  /// <inheritdoc/>
  /// <remarks>
  /// This implementation performs no operation.
  /// </remarks>
  public ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    => default; // do nothing
}
