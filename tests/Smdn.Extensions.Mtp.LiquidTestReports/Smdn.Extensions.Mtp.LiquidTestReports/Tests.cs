// SPDX-FileCopyrightText: 2025 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Smdn.OperatingSystem;

using NUnit.Framework;

namespace Smdn.Extensions.Mtp.LiquidTestReports;

[TestFixture]
public class Tests {
  private static IReadOnlyDictionary<string, string> GetEnvironmentVariablesForDotnetCommand(
    IEnumerable<KeyValuePair<string, string>>? additionalEnvironmentVariables
  )
  {
    var environmentVariables = new Dictionary<string, string>() {
      ["MSBUILDTERMINALLOGGER"] = "off", // make sure to disable terminal logger
      ["NO_COLOR"] = "NO_COLOR", // disable emitting ANSI color escape codes
      ["DOTNET_CLI_UI_LANGUAGE"] = "en", // force to use English for stdout/stderr messages to avoid character corruption
    };

    if (Environment.GetEnvironmentVariable("DOTNET_ROOT") is string envDotnetRoot)
      environmentVariables["DOTNET_ROOT"] = envDotnetRoot; // inherit the DOTNET_ROOT from current process

    if (additionalEnvironmentVariables is not null) {
      foreach (var pair in additionalEnvironmentVariables) {
        environmentVariables[pair.Key] = pair.Value;
      }
    }

    return environmentVariables;
  }

  private static IEnumerable<string> YieldTestProjectPaths()
  {
    yield return Path.Join(TestProjectInfo.RootDirectory, "NUnitTestProject", "NUnitTestProject.csproj");
    yield return Path.Join(TestProjectInfo.RootDirectory, "MSTestTestProject", "MSTestTestProject.csproj");
  }

  [TestCaseSource(nameof(YieldTestProjectPaths))]
  public void VerifyGeneratedReportContent(string testProjectPath)
    => InvokeTargetVerifyGeneratedReportContent(
      projectPath: testProjectPath
    );

  private static void InvokeTargetVerifyGeneratedReportContent(
    string projectPath
  )
  {
    int exitCode;
    string stdout, stderr;

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
      throw new InvalidOperationException($"clean failed: (exitCode: {exitCode}, stdout: {stdout}, stderr: {stderr})");

    /*
     * execute 'dotnet build'
     */
    exitCode = Shell.Execute(
      command: "dotnet",
      arguments: [
        "build", // dotnet build
        $"/p:TestReleasedPackage={(TestProjectInfo.TestReleasedPackage ? "true" : "false")}",
        projectPath,
      ],
      environmentVariables: GetEnvironmentVariablesForDotnetCommand(null),
      out stdout,
      out stderr
    );

    if (exitCode != 0)
      throw new InvalidOperationException($"build failed: (exitCode: {exitCode}, stdout: {stdout}, stderr: {stderr})");

    /*
     * execute 'dotnet msbuild /t:VerifyGeneratedReportContent' (no build)
     */
    exitCode = Shell.Execute(
      command: "dotnet",
      arguments: [
        "msbuild", // dotnet msbuild
        $"/p:TestReleasedPackage={(TestProjectInfo.TestReleasedPackage ? "true" : "false")}",
        "/t:VerifyGeneratedReportContent",
        projectPath,
      ],
      environmentVariables: GetEnvironmentVariablesForDotnetCommand(null),
      out stdout,
      out stderr
    );

    if (exitCode != 0)
      throw new InvalidOperationException($"run failed: (exitCode: {exitCode}, stdout: '{stdout}', stderr: '{stderr}')");
  }

  private static void InvokeDotnetTest(
    string projectPath,
    IEnumerable<string>? additionalDotnetTestCommandLineArguments,
    IEnumerable<KeyValuePair<string, string>>? additionalEnvironmentVariables
  )
  {
    int exitCode;
    string stdout, stderr;

    /*
     * execute 'dotnet clean'
     */
    exitCode = Shell.Execute(
      command: "dotnet",
      arguments: [
        "clean", // dotnet clean
        projectPath,
      ],
      environmentVariables: GetEnvironmentVariablesForDotnetCommand(additionalEnvironmentVariables),
      out stdout,
      out stderr
    );

    if (exitCode != 0)
      throw new InvalidOperationException($"clean failed: (exitCode: {exitCode}, stdout: {stdout}, stderr: {stderr})");

    /*
     * execute 'dotnet test'
     */
    exitCode = Shell.Execute(
      command: "dotnet",
      arguments: [
        "test", // dotnet test
        .. additionalDotnetTestCommandLineArguments ?? [],
        $"/p:TestReleasedPackage={(TestProjectInfo.TestReleasedPackage ? "true" : "false")}",
        "--project",
        projectPath,
      ],
      environmentVariables: GetEnvironmentVariablesForDotnetCommand(additionalEnvironmentVariables),
      out stdout,
      out stderr
    );

    if (exitCode is not (0 or 2 or 134))
      throw new InvalidOperationException($"run failed: (exitCode: {exitCode}, stdout: '{stdout}', stderr: '{stderr}')");
  }

  [TestCaseSource(nameof(YieldTestProjectPaths))]
  public void AppendGitHubStepSummary(string testProjectPath)
  {
    const string GITHUB_STEP_SUMMARY = nameof(GITHUB_STEP_SUMMARY);

    var githubStepSummaryFilePath = Path.Join(
      TestContext.CurrentContext.TestDirectory,
      $"{nameof(AppendGitHubStepSummary)}.{Path.GetFileName(testProjectPath)}.step-summary.tmp"
    );

    if (File.Exists(githubStepSummaryFilePath))
      File.Delete(githubStepSummaryFilePath);

    InvokeDotnetTest(
      projectPath: testProjectPath,
      additionalDotnetTestCommandLineArguments: [
        "--liquidtr-github-step-summary",
      ],
      additionalEnvironmentVariables: [
        KeyValuePair.Create(GITHUB_STEP_SUMMARY, githubStepSummaryFilePath),
      ]
    );

    Assert.That(githubStepSummaryFilePath, Does.Exist.IgnoreDirectories);

    var lines = File.ReadAllLines(githubStepSummaryFilePath);

    Assert.That(
      lines
        .Where(static line => line.StartsWith("|TargetFramework|", StringComparison.Ordinal))
        .Select(static line => line.Substring("|TargetFramework|".Length).TrimEnd('|')),
      Is.EquivalentTo([".NET 10.0", ".NET 8.0"])
    );
    Assert.That(
      lines
        .Where(static line => line.StartsWith("|SessionUid|", StringComparison.Ordinal))
        .Select(static line => line.Substring("|SessionUid|".Length).TrimEnd('|'))
        .Distinct(StringComparer.Ordinal)
        .Count(),
      Is.EqualTo(2)
    );
  }
}
