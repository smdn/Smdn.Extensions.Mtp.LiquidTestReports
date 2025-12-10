// SPDX-FileCopyrightText: 2025 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
// cspell:ignore liquidtr
using System.IO;
using System.Threading.Tasks;

using Microsoft.Testing.Extensions;
using Microsoft.Testing.Platform.Builder;

using NUnit.Framework;

namespace Smdn.Extensions.Mtp.LiquidTestReports;

[TestFixture]
public class LiquidTestReportsGeneratorCommandLineTests {
  private static async Task<ITestApplication> CreateTestApplicationAsync(params string[] arguments)
  {
    var applicationBuilder = await TestApplication.CreateBuilderAsync(
      args: arguments
    ).ConfigureAwait(false);

    applicationBuilder.RegisterTestFramework(
      capabilitiesFactory: static _ => new PseudoTestFrameworkCapabilities(),
      frameworkFactory: static (capabilities, serviceProvider) => new PseudoTestFramework(
        capabilities,
        serviceProvider,
        onRunTestExecutionAsync: null
      )
    );

    applicationBuilder.AddTrxReportProvider();
    applicationBuilder.AddLiquidTestReportsProvider();

    return await applicationBuilder.BuildAsync();
  }

  [Test]
  public async Task LiquidTestReportsTemplateFilePathOption_FilePathNotSpecified()
  {
    var application = await CreateTestApplicationAsync(
      "--liquidtr-template"
    ).ConfigureAwait(false);

    Assert.That(
      await application.RunAsync(),
      Is.EqualTo(MtpTestApplicationExitCodes.InvalidCommandLine)
    );
  }

  [Test]
  public async Task LiquidTestReportsTemplateFilePathOption_FilePathMustExist()
  {
    const string PathToTemplateFile = "nonexistent-template-file.md";

    var application = await CreateTestApplicationAsync(
      "--liquidtr-template", PathToTemplateFile
    ).ConfigureAwait(false);

    Assert.That(
      await application.RunAsync(),
      Is.EqualTo(MtpTestApplicationExitCodes.InvalidCommandLine)
    );
  }

  [Test]
  public async Task LiquidTestReportsTemplateFilePathOption_FilePathMustBeExactlyOne()
  {
    var pathToTemplateFile1 = Path.Join(
      TestContext.CurrentContext.TestDirectory,
      $"{nameof(LiquidTestReportsTemplateFilePathOption_FilePathMustBeExactlyOne)}-template1.md"
    );
    var pathToTemplateFile2 = Path.Join(
      TestContext.CurrentContext.TestDirectory,
      $"{nameof(LiquidTestReportsTemplateFilePathOption_FilePathMustBeExactlyOne)}-template2.md"
    );

    try {
      File.WriteAllText(path: pathToTemplateFile1, contents: "template1");
      File.WriteAllText(path: pathToTemplateFile2, contents: "template2");

      var application = await CreateTestApplicationAsync(
        "--liquidtr-template", pathToTemplateFile1, "--liquidtr-template", pathToTemplateFile2
      ).ConfigureAwait(false);

      Assert.That(
        await application.RunAsync(),
        Is.EqualTo(MtpTestApplicationExitCodes.InvalidCommandLine)
      );
    }
    finally {
      File.Delete(pathToTemplateFile1);
      File.Delete(pathToTemplateFile2);
    }
  }

  [Test]
  public async Task LiquidTestReportsParameterOption_ParameterNotSpecified()
  {
    var application = await CreateTestApplicationAsync(
      "--liquidtr-parameter"
    ).ConfigureAwait(false);

    Assert.That(
      await application.RunAsync(),
      Is.EqualTo(MtpTestApplicationExitCodes.ZeroTests) // no validation error, runs zero tests
    );
  }

  [Test]
  public async Task LiquidTestReportsParameterOption_MultipleParameters()
  {
    var application = await CreateTestApplicationAsync(
      "--liquidtr-parameter", "A=1",
      "--liquidtr-parameter", "B=2",
      "--liquidtr-parameter", "C=3",
      "--liquidtr-parameter", "C=4" // overrides parameter C
    ).ConfigureAwait(false);

    Assert.That(
      await application.RunAsync(),
      Is.EqualTo(MtpTestApplicationExitCodes.ZeroTests) // no validation error, runs zero tests
    );
  }

  [TestCase("X")]
  [TestCase("=X")]
  public async Task LiquidTestReportsParameterOption_ParameterInvalidFormat(string parameter)
  {
    var application = await CreateTestApplicationAsync(
      "--liquidtr-parameter", parameter
    ).ConfigureAwait(false);

    Assert.That(
      await application.RunAsync(),
      Is.EqualTo(MtpTestApplicationExitCodes.InvalidCommandLine)
    );
  }

  [TestCase("X=")]
  [TestCase("X==")]
  public async Task LiquidTestReportsParameterOption_ParameterValid(string parameter)
  {
    var application = await CreateTestApplicationAsync(
      "--liquidtr-parameter", parameter
    ).ConfigureAwait(false);

    Assert.That(
      await application.RunAsync(),
      Is.EqualTo(MtpTestApplicationExitCodes.ZeroTests) // no validation error, runs zero tests
    );
  }
}
