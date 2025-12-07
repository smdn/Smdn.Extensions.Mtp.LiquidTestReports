// SPDX-FileCopyrightText: 2025 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Microsoft.Testing.Platform.Builder;

namespace Smdn.Extensions.Mtp.LiquidTestReports;

/// <summary>
/// This class is used by Microsoft.Testing.Platform.MSBuild to hook into the
/// Testing Platform Builder to add LiquidTestReports support.
/// </summary>
public static class TestingPlatformBuilderHook {
  /// <summary>
  /// Adds TrxReport support to the Testing Platform Builder.
  /// </summary>
  /// <param name="testApplicationBuilder">The test application builder.</param>
  /// <param name="arguments">The command line arguments.</param>
#pragma warning disable IDE0060
  [CLSCompliant(false)]
  public static void AddExtensions(ITestApplicationBuilder testApplicationBuilder, string[] arguments)
    => testApplicationBuilder.AddLiquidTestReportsProvider();
#pragma warning restore IDE0060
}
