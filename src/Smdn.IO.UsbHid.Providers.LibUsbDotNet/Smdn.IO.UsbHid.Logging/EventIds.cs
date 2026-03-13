// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
namespace Smdn.IO.UsbHid.Logging;

/// <summary>
/// Defines the event IDs commonly used for logging with
/// <see cref="Microsoft.Extensions.Logging.ILogger"/> in USB HID device operations.
/// </summary>
internal static class EventIds {
  public const int OpenEndPointAttemptToOpen = 10;
  public const int OpenEndPointAttemptToSetConfiguration = 11;
  public const int OpenEndPointSetConfigurationExpectedException = 12;
  public const int OpenEndPointSetConfigurationFailed = 13;
  public const int OpenEndPointClaimInterfaceFailed = 14;
}
