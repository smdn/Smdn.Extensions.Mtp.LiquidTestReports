// SPDX-FileCopyrightText: 2025 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#if SYSTEM_IO_FILE_READLINESASYNC
#define SYSTEM_IO_FILE_READALLTEXTASYNC
#define SYSTEM_IO_FILE_WRITEALLTEXTASYNC
#endif

using System;
using System.Collections.Generic;
#if SYSTEM_DIAGNOSTICS_CODEANALYSIS_MEMBERNOTNULLWHENATTRIBUTE
using System.Diagnostics.CodeAnalysis;
#endif
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
#if SYSTEM_RUNTIME_INTEROPSERVICES_RUNTIMEINFORMATION_RUNTIMEIDENTIFIER
using System.Runtime.InteropServices;
#endif
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

using DotLiquid;

using LiquidTestReports.Core;
using LiquidTestReports.Core.Drops;
using LiquidTestReports.Core.Models;

using Microsoft.Testing.Extensions;
using Microsoft.Testing.Platform.CommandLine;
using Microsoft.Testing.Platform.Extensions;
using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.Extensions.OutputDevice;
using Microsoft.Testing.Platform.Extensions.TestHost;
using Microsoft.Testing.Platform.Messages;
using Microsoft.Testing.Platform.OutputDevice;
using Microsoft.Testing.Platform.Services;
using Microsoft.Testing.Platform.TestHost;

using Schemas.VisualStudio.TeamTest;

namespace Smdn.Extensions.Mtp.LiquidTestReports;

#pragma warning disable IDE0055
internal sealed class LiquidTestReportsConverter :
  IDataConsumer,
  IDataProducer,
  ITestSessionLifetimeHandler,
  IOutputDeviceDataProducer
{
#pragma warning restore IDE0055
  private readonly IMessageBus messageBus;
  private readonly IOutputDevice outputDevice;
  private readonly Task<bool> isEnabledTask;
  private readonly FileInfo? templateFile;
  private readonly string? outputFileExtension;
  private readonly bool appendGitHubStepSummary;
  private readonly Dictionary<string, string> templateParameters = new(Template.NamingConvention.StringComparer);

  private SessionUid? sessionUid;

#if SYSTEM_DIAGNOSTICS_CODEANALYSIS_MEMBERNOTNULLWHENATTRIBUTE
#pragma warning disable CS3016
  [MemberNotNullWhen(true, nameof(templateFile), nameof(outputFileExtension))]
#pragma warning restore CS3016
#endif
  private bool IsEnabled { get; }

  public LiquidTestReportsConverter(
    ICommandLineOptions commandLineOptionsService,
    IMessageBus messageBus,
    IOutputDevice outputDevice
  )
  {
    this.messageBus = messageBus;
    this.outputDevice = outputDevice;

    IsEnabled = false;
    templateFile = null;
    outputFileExtension = null;
    appendGitHubStepSummary = false;

    if (
      commandLineOptionsService.TryGetOptionArgumentList(
        LiquidTestReportsGeneratorCommandLine.LiquidTestReportsTemplateFilePathOptionName,
        out var argsTemplateFilePath
      )
    ) {
      IsEnabled = true;
      templateFile = new(argsTemplateFilePath[0]);
      outputFileExtension = templateFile.Extension; // use the extension of template file by default

      if (
        commandLineOptionsService.TryGetOptionArgumentList(
          LiquidTestReportsGeneratorCommandLine.LiquidTestReportsOutputFileExtensionOptionName,
          out var argsOutputFileExtension
        )
      ) {
        outputFileExtension = argsOutputFileExtension[0];

#pragma warning disable SA1003
        if (
#if SYSTEM_STRING_STARTSWITH_CHAR
          !outputFileExtension.StartsWith('.')
#else
          !outputFileExtension.StartsWith(".", StringComparison.Ordinal)
#endif
        ) {
          outputFileExtension = "." + outputFileExtension;
        }
#pragma warning restore SA1003
      }

      appendGitHubStepSummary = commandLineOptionsService.IsOptionSet(
        LiquidTestReportsGeneratorCommandLine.LiquidTestReportsGitHubStepSummaryOptionName
      );

      if (
        commandLineOptionsService.TryGetOptionArgumentList(
          LiquidTestReportsGeneratorCommandLine.LiquidTestReportsParameterOptionName,
          out var argsParameter
        )
      ) {
        foreach (var parameterNameAndValuePair in argsParameter) {
          var indexOfSeparator =
#if SYSTEM_STRING_INDEXOF_CHAR_STRINGCOMPARISON
            parameterNameAndValuePair.IndexOf('=', StringComparison.Ordinal);
#else
            parameterNameAndValuePair.IndexOf('=');
#endif

          if (indexOfSeparator < 1)
            continue; // ignore invalid formats and zero-length names

          var name = parameterNameAndValuePair.Substring(0, indexOfSeparator);
          var value = indexOfSeparator == parameterNameAndValuePair.Length - 1
              ? string.Empty
              : parameterNameAndValuePair.Substring(indexOfSeparator + 1);

          templateParameters[name] = value; // overwrite if duplicates are found
        }
      }
    }

    isEnabledTask = Task.FromResult(IsEnabled);
  }

  public Type[] DataTypesConsumed { get; } = [
    typeof(SessionFileArtifact) // consumes TRX file produced by Microsoft.Testing.Extensions.TrxReport
  ];

  public Type[] DataTypesProduced { get; } = [
    typeof(SessionFileArtifact) // produces LiquidTestReports output
  ];

  /// <inheritdoc />
  public string Uid => nameof(LiquidTestReportsConverter);

  /// <inheritdoc />
  public string Version => global::ExtensionInfo.SemVer;

  /// <inheritdoc />
  public string DisplayName { get; } = global::ExtensionInfo.DisplayName;

  /// <inheritdoc />
  public string Description { get; } = global::ExtensionInfo.Description;

  /// <inheritdoc />
  public Task<bool> IsEnabledAsync() => isEnabledTask;

  /// <inheritdoc />
  public async Task OnTestSessionStartingAsync(ITestSessionContext testSessionContext)
  {
    if (!IsEnabled)
      return;
    if (testSessionContext.CancellationToken.IsCancellationRequested)
      return;

    sessionUid = testSessionContext.SessionUid;
  }

  /// <inheritdoc />
  public Task OnTestSessionFinishingAsync(ITestSessionContext testSessionContext)
    => Task.CompletedTask;

  /// <summary>
  /// Consumes the TRX files output from Microsoft.Testing.Extensions.TrxReport and converts them using LiquidTestReports.
  /// Also publishes the converted results via <see cref="IMessageBus.PublishAsync"/>.
  /// </summary>
  /// <inheritdoc />
  public async Task ConsumeAsync(IDataProducer dataProducer, IData value, CancellationToken cancellationToken)
  {
    if (!IsEnabled || cancellationToken.IsCancellationRequested)
      return;

    // expects the SessionFileArtifact produced by the assembly 'Microsoft.Testing.Extensions.TrxReport'
    if (dataProducer.GetType().Assembly != typeof(TrxReportExtensions).Assembly)
      return;

    // expects the SessionFileArtifact that represents a generated TRX file
    if (value is not SessionFileArtifact fileArtifact)
      return;

    // expects the TRX file
    if (!string.Equals(fileArtifact.FileInfo.Extension, ".trx", StringComparison.OrdinalIgnoreCase))
      return;

    // expects the TRX file generated within the same test session
    if (!string.Equals(fileArtifact.SessionUid.Value, sessionUid?.Value, StringComparison.OrdinalIgnoreCase))
      return;

    var outputFile = new FileInfo(Path.ChangeExtension(fileArtifact.FileInfo.FullName, outputFileExtension));

    try {
#if !SYSTEM_DIAGNOSTICS_CODEANALYSIS_MEMBERNOTNULLWHENATTRIBUTE
#pragma warning disable CS8604
#endif
      // converts the TRX file using LiquidTestReports
      var generatedContent = await ConvertAsync(
        trxFile: fileArtifact.FileInfo,
        templateFile: templateFile,
        outputFile: outputFile,
        cancellationToken: cancellationToken
      ).ConfigureAwait(false);
#if !SYSTEM_DIAGNOSTICS_CODEANALYSIS_MEMBERNOTNULLWHENATTRIBUTE
#pragma warning restore CS8604
#endif

      if (string.IsNullOrEmpty(generatedContent) || !outputFile.Exists) {
        await outputDevice.DisplayAsync(
          this,
          new ErrorMessageOutputDeviceData(message: "LiquidTestReports generated no content"),
          cancellationToken
        ).ConfigureAwait(false);

        return;
      }

      await outputDevice.DisplayAsync(
        this,
        new FormattedTextOutputDeviceData(
          string.Format(CultureInfo.InvariantCulture, "LiquidTestReports file output: {0}", outputFile)
        ) {
          ForegroundColor = new SystemConsoleColor { ConsoleColor = ConsoleColor.DarkGreen },
        },
        cancellationToken
      ).ConfigureAwait(false);

      if (appendGitHubStepSummary) {
        await GitHubActions.AppendStepSummaryAsync(
          contents: generatedContent!,
          displayOutputDeviceAsync: DisplayOutputDeviceAsync,
          cancellationToken: cancellationToken
        ).ConfigureAwait(false);
      }

      await messageBus.PublishAsync(
        this,
        new SessionFileArtifact(
          sessionUid: fileArtifact.SessionUid,
          fileInfo: outputFile,
          displayName: "LiquidTestReports output",
          description: "LiquidTestReports output for the test session"
        )
      ).ConfigureAwait(false);
    }
    catch (OperationCanceledException ex) when (ex.CancellationToken == cancellationToken) {
      return; // cancellation requested, just exit
    }
  }

  private Task DisplayOutputDeviceAsync(
    IOutputDeviceData data,
    CancellationToken cancellationToken
  )
    => outputDevice.DisplayAsync(
      producer: this,
      data: data,
      cancellationToken: cancellationToken
    );

  private async Task<string?> ConvertAsync(
    FileInfo trxFile,
    FileInfo templateFile,
    FileInfo outputFile,
    CancellationToken cancellationToken
  )
  {
    // define built-in parameters that can be used in templates
    // these parameters can be overridden by values specified on the command line
    IEnumerable<KeyValuePair<string, string>> builtInParameters = [
      KeyValuePair.Create("SessionUid", sessionUid?.Value ?? string.Empty),
      KeyValuePair.Create("TestTargetName", Assembly.GetEntryAssembly()?.GetName().Name ?? string.Empty),
      KeyValuePair.Create("TargetFramework", Assembly.GetEntryAssembly()?.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkDisplayName ?? string.Empty),
      KeyValuePair.Create(
        "RuntimeIdentifier",
#if SYSTEM_RUNTIME_INTEROPSERVICES_RUNTIMEINFORMATION_RUNTIMEIDENTIFIER
        RuntimeInformation.RuntimeIdentifier
#else
        string.Empty
#endif
      )
    ];
    var generatedFile = new FileInfo(Path.ChangeExtension(trxFile.FullName, outputFileExtension));
    var (generatedReportContent, generatorErrors) = await GenerateReportAsync(
      reportInput: new LiquidTestReportsTrxInput(trxFile),
      templateFile: templateFile,
      libraryDrop: new LibraryDrop() {
        Parameters =
#if SYSTEM_COLLECTIONS_OBJECTMODEL_READONLYDICTIONARY_EMPTY
          System.Collections.ObjectModel.ReadOnlyDictionary<string, object>.Empty,
#else
          new Dictionary<string, object>(Template.NamingConvention.StringComparer) { },
#endif
      },
#pragma warning disable SA1114
      parameters: templateParameters
        .Concat(builtInParameters.Where(pair => !templateParameters.ContainsKey(pair.Key)))
        .ToDictionary(
#if !NET8_0_OR_GREATER
          keySelector: static pair => pair.Key,
          elementSelector: static pair => pair.Value,
#endif
          comparer: templateParameters.Comparer
        ),
#pragma warning restore SA1114
      cancellationToken: cancellationToken
    ).ConfigureAwait(false);

    if (string.IsNullOrEmpty(generatedReportContent))
      return null; // error: generated no content

    // ensure the directory for the output file exists
    if (generatedFile.Directory is DirectoryInfo directory)
      directory.Create();

#if SYSTEM_IO_FILE_WRITEALLTEXTASYNC
    await File.WriteAllTextAsync(
      generatedFile.FullName,
      generatedReportContent,
      cancellationToken
    ).ConfigureAwait(false);
#else
    File.WriteAllText(
      generatedFile.FullName,
      generatedReportContent
    );
#endif

    foreach (var error in generatorErrors) {
      await outputDevice.DisplayAsync(
        this,
        new WarningMessageOutputDeviceData(
          message: $"LiquidTestReports error: {error.Message}"
        ),
        cancellationToken
      ).ConfigureAwait(false);
    }

    return generatedReportContent;
  }

  private static async Task<(string ReportContent, IList<Exception> GeneratorErrors)> GenerateReportAsync(
    LiquidTestReportsTrxInput /* IReportInput */ reportInput,
    FileInfo templateFile,
    LibraryDrop libraryDrop,
    IReadOnlyDictionary<string, string> parameters,
    CancellationToken cancellationToken
  )
  {
    var testRunDrop = new TestRunDrop() {
      ResultSets = new TestResultSetDropCollection(),
      TestRunStatistics = new TestRunStatisticsDrop(),
    };

    if (reportInput.Format != InputFormatType.Trx)
      throw new NotSupportedException($"input format '{reportInput.Format}' is not supported");

    foreach (var file in reportInput.Files) {
      var results = await LoadTrxAsync(file, cancellationToken).ConfigureAwait(false);

      // TrxMapper exists in the 'LiquidTestReports.Cli'' namespace and yet
      // is implemented in the assembly LiquidTestReports.Core.
      global::LiquidTestReports.Cli.adapters.TrxMapper.Map(results, testRunDrop, reportInput);
    }

    var reportGenerator = new ReportGeneratorWrapper(
      new LibraryTestRun {
        Run = testRunDrop,
        Library = libraryDrop,
        Parameters = parameters,
      }
    );

    var templateContent =
#if SYSTEM_IO_FILE_READALLTEXTASYNC
      await File.ReadAllTextAsync(
        templateFile.FullName,
        cancellationToken
      ).ConfigureAwait(false);
#else
      File.ReadAllText(templateFile.FullName);
#endif

    var report = reportGenerator.GenerateReport(
      templateContent,
      out var generatorErrors
    );

    return (report, generatorErrors);
  }

  private static Task<TestRunType> LoadTrxAsync(
    FileInfo trxFile,
    CancellationToken cancellationToken
  )
  {
    if (cancellationToken.IsCancellationRequested)
      return Task.FromCanceled<TestRunType>(cancellationToken);

    using var reader = XmlReader.Create(
      inputUri: trxFile.FullName,
      settings: new XmlReaderSettings {
        Async = true,
        CloseInput = true,
      }
    );

    var serializer = new XmlSerializer(typeof(TestRunType));

    if (serializer.Deserialize(reader) is not TestRunType testRun)
      throw new InvalidDataException("failed to deserialize TRX file");

    return Task.FromResult(testRun);
  }
}
