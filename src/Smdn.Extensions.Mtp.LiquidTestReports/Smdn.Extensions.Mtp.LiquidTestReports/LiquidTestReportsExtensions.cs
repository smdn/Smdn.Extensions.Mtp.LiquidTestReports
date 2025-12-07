// SPDX-FileCopyrightText: 2025 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Microsoft.Testing.Platform.Builder;
using Microsoft.Testing.Platform.Extensions;
using Microsoft.Testing.Platform.Services;

namespace Smdn.Extensions.Mtp.LiquidTestReports;

/// <summary>
/// Provides extension methods for adding TRX report generation to a test application.
/// </summary>
public static class LiquidTestReportsExtensions {
  /// <summary>
  /// Adds LiquidTestReports file generation to a test application.
  /// </summary>
  /// <param name="builder">The test application builder.</param>
  [CLSCompliant(false)]
  public static void AddLiquidTestReportsProvider(this ITestApplicationBuilder builder)
  {
    if (builder is null)
      throw new ArgumentNullException(nameof(builder));

    builder.CommandLine.AddProvider(
      static () => new LiquidTestReportsGeneratorCommandLine()
    );

    var compositeTestSessionLiquidTestReportsConverterService =
      new CompositeExtensionFactory<LiquidTestReportsConverter>(
        static serviceProvider => new LiquidTestReportsConverter(
          commandLineOptionsService: serviceProvider.GetCommandLineOptions(),
          messageBus: serviceProvider.GetMessageBus(),
          outputDevice: serviceProvider.GetOutputDevice()
        )
      );

    builder.TestHost.AddDataConsumer(compositeTestSessionLiquidTestReportsConverterService);
    builder.TestHost.AddTestSessionLifetimeHandle(compositeTestSessionLiquidTestReportsConverterService);
  }
}
