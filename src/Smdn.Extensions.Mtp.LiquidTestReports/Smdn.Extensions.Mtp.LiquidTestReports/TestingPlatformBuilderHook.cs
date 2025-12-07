// SPDX-FileCopyrightText: 2025 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
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
  /// <param name="_">The command line arguments.</param>
  public static void AddExtensions(ITestApplicationBuilder testApplicationBuilder, string[] _)
    => testApplicationBuilder.AddLiquidTestReportsProvider();
}
