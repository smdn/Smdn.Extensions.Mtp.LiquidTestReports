// SPDX-FileCopyrightText: 2025 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
namespace Smdn.Extensions.Mtp.LiquidTestReports;

/// <seealso href="https://learn.microsoft.com/dotnet/core/testing/microsoft-testing-platform-exit-codes">
/// Microsoft.Testing.Platform exit codes
/// </seealso>
/// <seealso href="https://github.com/microsoft/testfx/blob/main/src/Platform/Microsoft.Testing.Platform/Helpers/ExitCodes.cs">
/// Microsoft.Testing.Platform.Helpers.ExitCodes
/// </seealso>
public static class MtpTestApplicationExitCodes {
  public const int Success = 0;
  public const int GenericFailure = 1;
  public const int AtLeastOneTestFailed = 2;
  public const int TestSessionAborted = 3;
  public const int InvalidPlatformSetup = 4;
  public const int InvalidCommandLine = 5;
  // public const int FeatureNotImplemented = 6;
  public const int TestHostProcessExitedNonGracefully = 7;
  public const int ZeroTests = 8;
  public const int MinimumExpectedTestsPolicyViolation = 9;
  public const int TestAdapterTestSessionFailure = 10;
  public const int DependentProcessExited = 11;
  public const int IncompatibleProtocolVersion = 12;
  public const int TestExecutionStoppedForMaxFailedTests = 13;
}
