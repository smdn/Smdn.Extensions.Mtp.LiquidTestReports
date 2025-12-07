// SPDX-FileCopyrightText: 2025 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.IO;

using Smdn.OperatingSystem;

using NUnit.Framework;

namespace Smdn.Extensions.Mtp.LiquidTestReports;

[TestFixture]
public class Tests {
  [Test]
  public void VerifyGeneratedReportContent_NUnitTestProject()
    => VerifyGeneratedReportContent(
      projectPath: Path.Join(TestProjectInfo.RootDirectory, "NUnitTestProject", "NUnitTestProject.csproj")
    );

  [Test]
  public void VerifyGeneratedReportContent_MSTestTestProject()
    => VerifyGeneratedReportContent(
      projectPath: Path.Join(TestProjectInfo.RootDirectory, "MSTestTestProject", "MSTestTestProject.csproj")
    );

  private static void VerifyGeneratedReportContent(
    string projectPath
  )
  {
    int exitCode;
    string stdout, stderr;

    static IReadOnlyDictionary<string, string> GetEnvironmentVariablesForDotnetCommand(
      IReadOnlyDictionary<string, string>? additionalEnvironmentVariables
    )
    {
      var environmentVariables = new Dictionary<string, string>() {
        ["MSBUILDTERMINALLOGGER"] = "off", // make sure to disable terminal logger
        ["NO_COLOR"] = "NO_COLOR", // disable emitting ANSI color escape codes
        ["DOTNET_CLI_UI_LANGUAGE"] = "en", // force to use English for stdout/stderr messages to avoid character corruption
      };

      if (additionalEnvironmentVariables is not null) {
        foreach (var pair in additionalEnvironmentVariables) {
          environmentVariables[pair.Key] = pair.Value;
        }
      }

      return environmentVariables;
    }

    /*
     * execute 'dotnet clean'
     */
    exitCode = Shell.Execute(
      command: "dotnet",
      arguments: [
        "clean", // dotnet clean
        projectPath,
      ],
      environmentVariables: GetEnvironmentVariablesForDotnetCommand(null),
      out stdout,
      out stderr
    );

    if (exitCode != 0)
      throw new InvalidOperationException($"clean failed: (stdout: {stdout}, stderr: {stderr})");

    /*
     * execute 'dotnet build'
     */
    exitCode = Shell.Execute(
      command: "dotnet",
      arguments: [
        "build", // dotnet build
        projectPath,
      ],
      environmentVariables: GetEnvironmentVariablesForDotnetCommand(null),
      out stdout,
      out stderr
    );

    if (exitCode != 0)
      throw new InvalidOperationException($"build failed: (stdout: {stdout}, stderr: {stderr})");

    /*
     * execute 'dotnet msbuild /t:VerifyGeneratedReportContent' (no build)
     */
    exitCode = Shell.Execute(
      command: "dotnet",
      arguments: [
        "msbuild", // dotnet msbuild
        "/t:VerifyGeneratedReportContent",
        projectPath,
      ],
      environmentVariables: GetEnvironmentVariablesForDotnetCommand(null),
      out stdout,
      out stderr
    );

    if (exitCode != 0)
      throw new InvalidOperationException($"run failed: (stdout: '{stdout}', stderr: '{stderr}')");
  }
}
