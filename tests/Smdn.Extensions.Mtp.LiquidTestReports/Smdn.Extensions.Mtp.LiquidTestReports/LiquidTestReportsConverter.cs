// SPDX-FileCopyrightText: 2025 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
// cspell:ignore liquidtr
using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;

using Microsoft.Testing.Extensions;
using Microsoft.Testing.Platform.Builder;
using Microsoft.Testing.Platform.Services;

namespace Smdn.Extensions.Mtp.LiquidTestReports;

[TestFixture]
public class LiquidTestReportsConverterTests {
  private const string DisableTrxProducerCheckingOption = $"--liquidtr-disable-trx-producer-checking";

  // cspell:ignore nunittestproject
  private const string PseudoTrxFileContents = """
<?xml version="1.0" encoding="utf-8"?>
<TestRun id="19731cbc-4719-400b-801a-7d07b0198bbe" name="@pseudo 2025-12-08 13:45:20.9323136" xmlns="http://microsoft.com/schemas/VisualStudio/TeamTest/2010">
  <Times creation="2025-12-08T13:45:19.5317846Z" queuing="2025-12-08T13:45:19.5317846Z" start="2025-12-08T13:45:19.5317846Z" finish="2025-12-08T13:45:20.9332957Z" />
  <TestSettings name="default" id="7c530dc7-ed1a-48c6-a71e-347da8d4403b"/>
  <Results>
    <UnitTestResult executionId="5cc081d7-ae13-494f-952d-1cf4f6b6abd2" testId="0938b1eb-771c-01a8-7b08-3ce1e2491cb3" testName="TestCaseFail" computerName="pseudo" duration="00:00:00.0930400" startTime="2025-12-08T13:45:20.7065048+00:00" endTime="2025-12-08T13:45:20.7994321+00:00" testType="13CDC9D9-DDB5-4fa4-A97D-D965CCFC6D4B" outcome="Failed" testListId="8C84FA94-04C1-424b-9868-57A2D4851A1D" relativeResultsDirectory="5cc081d7-ae13-494f-952d-1cf4f6b6abd2">
      <Output>
        <ErrorInfo>
          <Message>  Assert.That(n, Is.Zero)
  Expected: 0
  But was:  1
</Message>
          <StackTrace>   at NUnitTestProject.Tests.TestCaseFail() in /opt/pseudo/NUnitTestProject/Tests.cs:line 22

1)    at NUnitTestProject.Tests.TestCaseFail() in /opt/pseudo/NUnitTestProject/Tests.cs:line 22

</StackTrace>
        </ErrorInfo>
      </Output>
    </UnitTestResult>
    <UnitTestResult executionId="36ae3e30-add5-45f2-97f5-94d3126dc5aa" testId="97c2bdae-a7fe-921f-f76e-8166a41944c7" testName="TestCasePass" computerName="sana" duration="00:00:00.0003959" startTime="2025-12-08T13:45:20.8014349+00:00" endTime="2025-12-08T13:45:20.8018314+00:00" testType="13CDC9D9-DDB5-4fa4-A97D-D965CCFC6D4B" outcome="Passed" testListId="8C84FA94-04C1-424b-9868-57A2D4851A1D" relativeResultsDirectory="36ae3e30-add5-45f2-97f5-94d3126dc5aa" />
    <UnitTestResult executionId="12a2d8b0-1de8-45e4-8f60-b05e9e933419" testId="b69687af-325c-0d31-2ffc-c947806f139e" testName="TestCaseSkipped" computerName="sana" duration="00:00:00.0007360" startTime="2025-12-08T13:45:20.8018668+00:00" endTime="2025-12-08T13:45:20.8026030+00:00" testType="13CDC9D9-DDB5-4fa4-A97D-D965CCFC6D4B" outcome="NotExecuted" testListId="8C84FA94-04C1-424b-9868-57A2D4851A1D" relativeResultsDirectory="12a2d8b0-1de8-45e4-8f60-b05e9e933419">
      <Output>
        <StdOut></StdOut>
      </Output>
    </UnitTestResult>
  </Results>
  <TestDefinitions>
    <UnitTest name="TestCaseFail" storage="/opt/pseudo/nunittestproject/bin/debug/net10.0/nunittestproject.dll" id="0938b1eb-771c-01a8-7b08-3ce1e2491cb3">
      <Execution id="5cc081d7-ae13-494f-952d-1cf4f6b6abd2" />
      <TestMethod codeBase="/opt/pseudo/NUnitTestProject/bin/Debug/net10.0/NUnitTestProject.dll" adapterTypeName="executor://NUnitExtension/1.0.0+d6494ae0f5a93e7c23bbe410d3349cda3ceabd59" className="NUnitTestProject.Tests" name="TestCaseFail" />
    </UnitTest>
    <UnitTest name="TestCasePass" storage="/opt/pseudo/nunittestproject/bin/debug/net10.0/nunittestproject.dll" id="97c2bdae-a7fe-921f-f76e-8166a41944c7">
      <Execution id="36ae3e30-add5-45f2-97f5-94d3126dc5aa" />
      <TestMethod codeBase="/opt/pseudo/NUnitTestProject/bin/Debug/net10.0/NUnitTestProject.dll" adapterTypeName="executor://NUnitExtension/1.0.0+d6494ae0f5a93e7c23bbe410d3349cda3ceabd59" className="NUnitTestProject.Tests" name="TestCasePass" />
    </UnitTest>
    <UnitTest name="TestCaseSkipped" storage="/opt/pseudo/nunittestproject/bin/debug/net10.0/nunittestproject.dll" id="b69687af-325c-0d31-2ffc-c947806f139e">
      <Execution id="12a2d8b0-1de8-45e4-8f60-b05e9e933419" />
      <TestMethod codeBase="/opt/pseudo/NUnitTestProject/bin/Debug/net10.0/NUnitTestProject.dll" adapterTypeName="executor://NUnitExtension/1.0.0+d6494ae0f5a93e7c23bbe410d3349cda3ceabd59" className="NUnitTestProject.Tests" name="TestCaseSkipped" />
    </UnitTest>
  </TestDefinitions>
  <TestEntries>
    <TestEntry testId="0938b1eb-771c-01a8-7b08-3ce1e2491cb3" executionId="5cc081d7-ae13-494f-952d-1cf4f6b6abd2" testListId="8C84FA94-04C1-424b-9868-57A2D4851A1D" />
    <TestEntry testId="97c2bdae-a7fe-921f-f76e-8166a41944c7" executionId="36ae3e30-add5-45f2-97f5-94d3126dc5aa" testListId="8C84FA94-04C1-424b-9868-57A2D4851A1D" />
    <TestEntry testId="b69687af-325c-0d31-2ffc-c947806f139e" executionId="12a2d8b0-1de8-45e4-8f60-b05e9e933419" testListId="8C84FA94-04C1-424b-9868-57A2D4851A1D" />
  </TestEntries>
  <TestLists>
    <TestList name="Results Not in a List" id="8C84FA94-04C1-424b-9868-57A2D4851A1D" />
    <TestList name="All Loaded Results" id="19431567-8539-422a-85d7-44ee4e166bda" />
  </TestLists>
  <ResultSummary outcome="Completed">
    <Counters total="3" executed="2" passed="1" failed="1" error="0" timeout="0" aborted="0" inconclusive="0" passedButRunAborted="0" notRunnable="0" notExecuted="1" disconnected="0" warning="0" completed="0" inProgress="0" pending="0" />
  </ResultSummary>
</TestRun>
""";
  private const string TestCaseAndOutcomeTemplateFileContent = """
{%- for set in run.result_sets -%}
{%-   for result in set.results -%}
- {{ result.test_case.display_name | escape }}={{ result.outcome | escape }}
{%-   endfor -%}
{%- endfor -%}
""";

  private static async Task<ITestApplication> CreateTestApplicationAsync(
    Func<PseudoTestFramework, CancellationToken, Task> onRunTestExecutionAsync,
    params string[] arguments
  )
  {
    var applicationBuilder = await TestApplication.CreateBuilderAsync(
      args: arguments
    ).ConfigureAwait(false);

    applicationBuilder.TestHost.AddTestSessionLifetimeHandle(
      static serviceProvider => new PseudoTrxGenerator(
        messageBus: serviceProvider.GetMessageBus()
      )
    );

    applicationBuilder.RegisterTestFramework(
      capabilitiesFactory: static _ => new PseudoTestFrameworkCapabilities(),
      frameworkFactory: (capabilities, serviceProvider) => new PseudoTestFramework(
        capabilities,
        serviceProvider,
        onRunTestExecutionAsync: onRunTestExecutionAsync
      )
    );

    applicationBuilder.AddTrxReportProvider();
    applicationBuilder.AddLiquidTestReportsProvider();

    return await applicationBuilder.BuildAsync();
  }

  private static string GetFilePathForTestCase(
    string fileName,
    string extension,
    [CallerMemberName] string testCaseMethodName = ""
  )
    => Path.Join(TestContext.CurrentContext.TestDirectory, $"{fileName}-{testCaseMethodName}.{extension.TrimStart('.')}");

  private async Task RunTestApplicationWithDefaultConfigurations(
    string templateFileContents,
    string templateFileExtension,
    Func<FileInfo, string[]> generateArguments,
    Func<ITestApplication, FileInfo, ValueTask> assertAsync,
    [CallerMemberName] string testCaseMethodName = ""
  )
  {
    var templateFile = new FileInfo(GetFilePathForTestCase("Template", templateFileExtension, testCaseMethodName));
    var trxFile = new FileInfo(GetFilePathForTestCase("TestResult", "trx", testCaseMethodName));

    await File.WriteAllTextAsync(
      path: templateFile.FullName,
      contents: templateFileContents
    ).ConfigureAwait(false);

    var application = await CreateTestApplicationAsync(
      onRunTestExecutionAsync: async (testFramework, cancellationToken) => {
        var generator = testFramework.ServiceProvider.GetRequiredService<PseudoTrxGenerator>();

        await generator.GenerateTrxAsync(
          filePath: trxFile.FullName,
          trxFileContents: PseudoTrxFileContents
        ).ConfigureAwait(false);
      },
      arguments: generateArguments(templateFile)
    ).ConfigureAwait(false);

    await assertAsync(application, trxFile).ConfigureAwait(false);
  }

  [Test]
  public async Task ExcludeTrxGeneratedOtherThanFromMicrosoftTestingExtensionsTrxReportByDefault()
  {
    const string TemplateFileExtension = ".md";

    await RunTestApplicationWithDefaultConfigurations(
      templateFileContents: TestCaseAndOutcomeTemplateFileContent,
      templateFileExtension: TemplateFileExtension,
      generateArguments: static templateFile => ["--liquidtr-template", templateFile.FullName],
      assertAsync: async (application, trxFile) => {
        Assert.That(
          await application.RunAsync(),
          Is.EqualTo(MtpTestApplicationExitCodes.ZeroTests),
          "Application must exit without any errors."
        );
        Assert.That(
          trxFile.FullName,
          Does.Exist.IgnoreDirectories,
          "TRX file must be generated."
        );
        Assert.That(
          Path.ChangeExtension(trxFile.FullName, TemplateFileExtension),
          Does.Not.Exist,
          "Converted report must not be generated."
        );
      }
    ).ConfigureAwait(false);
  }

  [Test]
  public async Task DisableTrxProducerChecking()
  {
    const string TemplateFileExtension = ".md";

    await RunTestApplicationWithDefaultConfigurations(
      templateFileContents: TestCaseAndOutcomeTemplateFileContent,
      templateFileExtension: TemplateFileExtension,
      generateArguments: static templateFile => ["--liquidtr-template", templateFile.FullName, DisableTrxProducerCheckingOption],
      assertAsync: async (application, trxFile) => {
        Assert.That(
          await application.RunAsync(),
          Is.EqualTo(MtpTestApplicationExitCodes.ZeroTests),
          "Application must exit without any errors."
        );
        Assert.That(
          trxFile.FullName,
          Does.Exist.IgnoreDirectories,
          "TRX file must be generated."
        );

        var generatedFilePath = Path.ChangeExtension(trxFile.FullName, TemplateFileExtension);

        Assert.That(
          generatedFilePath,
          Does.Exist.IgnoreDirectories,
          "Converted report must be generated."
        );

        var generatedContentLines = await File.ReadAllLinesAsync(generatedFilePath).ConfigureAwait(false);

        Assert.That(generatedContentLines, Does.Contain("- TestCaseFail=Failed"));
        Assert.That(generatedContentLines, Does.Contain("- TestCasePass=Passed"));
        Assert.That(generatedContentLines, Does.Contain("- TestCaseSkipped=Skipped"));
      }
    ).ConfigureAwait(false);
  }

  [Test]
  public async Task DisableExtension()
  {
    const string TemplateFileExtension = ".md";

    await RunTestApplicationWithDefaultConfigurations(
      templateFileContents: TestCaseAndOutcomeTemplateFileContent,
      templateFileExtension: TemplateFileExtension,
      generateArguments: static _ => [DisableTrxProducerCheckingOption], // no '--liquidtr-template' arguments
      assertAsync: async (application, trxFile) => {
        Assert.That(
          await application.RunAsync(),
          Is.EqualTo(MtpTestApplicationExitCodes.ZeroTests),
          "Application must exit without any errors."
        );
        Assert.That(
          trxFile.FullName,
          Does.Exist.IgnoreDirectories,
          "TRX file must be generated."
        );
        Assert.That(
          Path.ChangeExtension(trxFile.FullName, TemplateFileExtension),
          Does.Not.Exist,
          "Converted report must not be generated."
        );
      }
    ).ConfigureAwait(false);
  }

  [TestCase(".md")]
  [TestCase(".txt")]
  [TestCase(".xml")]
  public async Task DefaultOutputExtension(string templateFileExtension)
  {
    await RunTestApplicationWithDefaultConfigurations(
      templateFileContents: TestCaseAndOutcomeTemplateFileContent,
      templateFileExtension: templateFileExtension,
      generateArguments: static templateFile => [
        "--liquidtr-template", templateFile.FullName,
        DisableTrxProducerCheckingOption
      ],
      assertAsync: async (application, trxFile) => {
        Assert.That(
          await application.RunAsync(),
          Is.EqualTo(MtpTestApplicationExitCodes.ZeroTests),
          "Application must exit without any errors."
        );
        Assert.That(
          trxFile.FullName,
          Does.Exist.IgnoreDirectories,
          "TRX file must be generated."
        );
        Assert.That(
          Path.ChangeExtension(trxFile.FullName, templateFileExtension),
          Does.Exist.IgnoreDirectories,
          $"Converted report must be generated with the file extension '{templateFileExtension}'."
        );
      }
    ).ConfigureAwait(false);
  }

  [TestCase(".txt", ".txt")]
  [TestCase("txt", ".txt")]
  [TestCase(".xml", ".xml")]
  [TestCase("xml", ".xml")]
  public async Task OutputExtensionOption(string outputExtensionOptionValue, string expectedFileExtension)
  {
    const string TemplateFileExtension = ".md";

    await RunTestApplicationWithDefaultConfigurations(
      templateFileContents: TestCaseAndOutcomeTemplateFileContent,
      templateFileExtension: TemplateFileExtension,
      generateArguments: templateFile => [
        "--liquidtr-template", templateFile.FullName,
        "--liquidtr-output-extension", outputExtensionOptionValue,
        DisableTrxProducerCheckingOption
      ],
      assertAsync: async (application, trxFile) => {
        Assert.That(
          await application.RunAsync(),
          Is.EqualTo(MtpTestApplicationExitCodes.ZeroTests),
          "Application must exit without any errors."
        );
        Assert.That(
          trxFile.FullName,
          Does.Exist.IgnoreDirectories,
          "TRX file must be generated."
        );
        Assert.That(
          Path.ChangeExtension(trxFile.FullName, expectedFileExtension),
          Does.Exist.IgnoreDirectories,
          $"Converted report must be generated with the file extension '{expectedFileExtension}'."
        );
      }
    ).ConfigureAwait(false);
  }

  private static System.Collections.IEnumerable YieldTestCases_ParameterOption()
  {
    (string, string[], Action<string[]>)[] testCases = [
      (
        """
        parameters.A = '{{ parameters.A }}'
        parameters.B = '{{ parameters.B }}'
        parameters.C = '{{ parameters.C }}'
        """,
        [
          "--liquidtr-parameter", "A=0",
          "--liquidtr-parameter", "B=true",
          "--liquidtr-parameter", "C=Bye!", // will be overwritten
          "--liquidtr-parameter", "C=Hello!",
        ],
        static generatedContentLines => {
          Assert.That(generatedContentLines, Does.Contain("parameters.A = '0'"));
          Assert.That(generatedContentLines, Does.Contain("parameters.B = 'true'"));
          Assert.That(generatedContentLines, Does.Contain("parameters.C = 'Hello!'"));
          Assert.That(generatedContentLines, Does.Not.Contain("parameters.C = 'Bye!'"));
        }
      ),
      (
        """
        parameters.X = '{{ parameters.X }}'
        """,
        [
          "--liquidtr-parameter", "X=",
        ],
        static generatedContentLines => {
          Assert.That(generatedContentLines, Does.Contain("parameters.X = ''"));
        }
      ),
      (
        """
        parameters.X = '{{ parameters.X }}'
        parameters.x = '{{ parameters.x }}'
        """,
        [
          "--liquidtr-parameter", "X=0",
          "--liquidtr-parameter", "x=1",
        ],
        static generatedContentLines => {
          Assert.That(generatedContentLines, Does.Contain("parameters.X = '1'"));
          Assert.That(generatedContentLines, Does.Contain("parameters.x = '1'"));
          Assert.That(generatedContentLines, Does.Not.Contain("parameters.X = '0'"));
          Assert.That(generatedContentLines, Does.Not.Contain("parameters.x = '0'"));
        }
      ),
      // built-in parameters
      (
        """
        parameters.SessionUid = '{{ parameters.SessionUid }}'
        parameters.TestTargetName = '{{ parameters.TestTargetName }}'
        parameters.TargetFramework = '{{ parameters.TargetFramework }}'
        parameters.RuntimeIdentifier = '{{ parameters.RuntimeIdentifier }}'
        """,
        [],
        static generatedContentLines => {
          Assert.That(
            generatedContentLines.FirstOrDefault(static l => l.StartsWith("parameters.SessionUid =", StringComparison.Ordinal)),
            Is.Not.Null.And.Not.EqualTo("parameters.SessionUid = ''")
          );
          Assert.That(
            generatedContentLines.FirstOrDefault(static l => l.StartsWith("parameters.TestTargetName =", StringComparison.Ordinal)),
            Is.Not.Null.And.Not.EqualTo("parameters.TestTargetName = ''")
          );
          Assert.That(
            generatedContentLines.FirstOrDefault(static l => l.StartsWith("parameters.TargetFramework =", StringComparison.Ordinal)),
            Is.Not.Null.And.Not.EqualTo("parameters.TargetFramework = ''")
          );
          Assert.That(
            generatedContentLines.FirstOrDefault(static l => l.StartsWith("parameters.RuntimeIdentifier =", StringComparison.Ordinal)),
            Is.Not.Null.And.Not.EqualTo("parameters.RuntimeIdentifier = ''")
          );
        }
      ),
      // overwrite built-in parameters
      (
        """
        parameters.SessionUid = '{{ parameters.SessionUid }}'
        parameters.TestTargetName = '{{ parameters.TestTargetName }}'
        parameters.TargetFramework = '{{ parameters.TargetFramework }}'
        parameters.RuntimeIdentifier = '{{ parameters.RuntimeIdentifier }}'
        """,
        [
          "--liquidtr-parameter", "SessionUid=SessionUid",
          "--liquidtr-parameter", "TestTargetName=TestTargetName",
          "--liquidtr-parameter", "TargetFramework=TargetFramework",
          "--liquidtr-parameter", "runtimeidentifier=null", // will be overwritten, cspell:ignore runtimeidentifier
          "--liquidtr-parameter", "RuntimeIdentifier=RuntimeIdentifier",
        ],
        static generatedContentLines => {
          Assert.That(generatedContentLines, Does.Contain("parameters.SessionUid = 'SessionUid'"));
          Assert.That(generatedContentLines, Does.Contain("parameters.TestTargetName = 'TestTargetName'"));
          Assert.That(generatedContentLines, Does.Contain("parameters.TargetFramework = 'TargetFramework'"));
          Assert.That(generatedContentLines, Does.Contain("parameters.RuntimeIdentifier = 'RuntimeIdentifier'"));
        }
      ),
    ];

    foreach (var (templateFileContents, parameterArguments, assertGeneratedContentLines) in testCases) {
      yield return new object?[] { templateFileContents, parameterArguments, assertGeneratedContentLines };
    }
  }

  [TestCaseSource(nameof(YieldTestCases_ParameterOption))]
  public async Task ParameterOption(
    string templateFileContents,
    string[] parameterArguments,
    Action<string[]> assertGeneratedContentLines
  )
  {
    const string TemplateFileExtension = ".md";

    await RunTestApplicationWithDefaultConfigurations(
      templateFileContents: templateFileContents,
      templateFileExtension: TemplateFileExtension,
      generateArguments: templateFile => [
        "--liquidtr-template", templateFile.FullName,
        .. parameterArguments,
        DisableTrxProducerCheckingOption
      ],
      assertAsync: async (application, trxFile) => {
        Assert.That(
          await application.RunAsync(),
          Is.EqualTo(MtpTestApplicationExitCodes.ZeroTests),
          "Application must exit without any errors."
        );
        Assert.That(
          trxFile.FullName,
          Does.Exist.IgnoreDirectories,
          "TRX file must be generated."
        );

        var generatedFilePath = Path.ChangeExtension(trxFile.FullName, TemplateFileExtension);

        Assert.That(
          generatedFilePath,
          Does.Exist.IgnoreDirectories,
          "Converted report must be generated."
        );

        assertGeneratedContentLines(
          await File.ReadAllLinesAsync(generatedFilePath).ConfigureAwait(false)
        );
      }
    ).ConfigureAwait(false);
  }

  [Test]
  public async Task GitHubStepSummaryOption(
    [Values] bool enableOption,
    [Values] bool isEnvVarSet
  )
  {
    const string TemplateFileExtension = ".md";
    const string GITHUB_STEP_SUMMARY = nameof(GITHUB_STEP_SUMMARY);
    const string InitialStepSummaryFileContents =
      """
      GitHub Actions step summary line #1
      GitHub Actions step summary line #2
      GitHub Actions step summary line #3

      """;

    var stepSummaryFile = isEnvVarSet
      ? new FileInfo(GetFilePathForTestCase($"{nameof(GITHUB_STEP_SUMMARY)}-{enableOption}-{isEnvVarSet}", TemplateFileExtension))
      : null;
    var initialValueOfEnvVar = Environment.GetEnvironmentVariable(GITHUB_STEP_SUMMARY);

    try {
      if (isEnvVarSet) {
        Environment.SetEnvironmentVariable(GITHUB_STEP_SUMMARY, stepSummaryFile!.FullName);

        await File.WriteAllTextAsync(
          path: stepSummaryFile.FullName,
          contents: InitialStepSummaryFileContents
        ).ConfigureAwait(false);
      }
      else {
        Environment.SetEnvironmentVariable(GITHUB_STEP_SUMMARY, null);
      }

      await RunTestApplicationWithDefaultConfigurations(
        templateFileContents: TestCaseAndOutcomeTemplateFileContent,
        templateFileExtension: TemplateFileExtension,
        generateArguments: templateFile => [
          "--liquidtr-template", templateFile.FullName,
          .. enableOption ? new[] { "--liquidtr-github-step-summary" } : [],
          DisableTrxProducerCheckingOption
        ],
        assertAsync: async (application, trxFile) => {
          Assert.That(
            await application.RunAsync(),
            Is.EqualTo(MtpTestApplicationExitCodes.ZeroTests),
            "Application must exit without any errors."
          );
          Assert.That(
            trxFile.FullName,
            Does.Exist.IgnoreDirectories,
            "TRX file must be generated."
          );

          Assert.That(
            Path.ChangeExtension(trxFile.FullName, TemplateFileExtension),
            Does.Exist.IgnoreDirectories,
            "Converted report must be generated."
          );

          if (enableOption && isEnvVarSet) {
            var generatedFilePath = Path.ChangeExtension(trxFile.FullName, TemplateFileExtension);

            Assert.That(
              (await File.ReadAllTextAsync(stepSummaryFile!.FullName).ConfigureAwait(false)).TrimEnd(),
              Is.EqualTo(
                InitialStepSummaryFileContents + (await File.ReadAllTextAsync(generatedFilePath).ConfigureAwait(false)).TrimEnd()
              ),
              "Step summary must be appended."
            );
          }
          else if (stepSummaryFile is not null) {
            Assert.That(
              await File.ReadAllTextAsync(stepSummaryFile!.FullName).ConfigureAwait(false),
              Is.EqualTo(InitialStepSummaryFileContents),
              "Step summary must not be appended."
            );
          }
        }
      ).ConfigureAwait(false);
    }
    finally {
      Environment.SetEnvironmentVariable(GITHUB_STEP_SUMMARY, initialValueOfEnvVar);
    }
  }
}
