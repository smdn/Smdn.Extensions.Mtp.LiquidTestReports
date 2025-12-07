// SPDX-FileCopyrightText: 2025 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#if NET5_0_OR_GREATER
#define SYSTEM_REFLECTION_CREATEDELEGATE_OF_T
#endif

using System;
using System.Collections.Generic;
#if SYSTEM_DIAGNOSTICS_CODEANALYSIS_REQUIRESUNREFERENCEDCODEATTRIBUTE
using System.Diagnostics.CodeAnalysis;
#endif
using System.Reflection;

using LiquidTestReports.Core.Drops;

namespace LiquidTestReports.Core;

/// <summary>
/// A wrapper class for calling <see href="https://github.com/kurtmkurtm/LiquidTestReports/blob/master/src/LiquidTestReports.Core/ReportGenerator.cs">LiquidTestReports.Core.ReportGenerator</see>,
/// which is not currently exposed as a public API, using reflection.
/// </summary>
internal class ReportGeneratorWrapper {
  private const string TargetTypeFullName = "LiquidTestReports.Core.ReportGenerator";

  private delegate string GenerateReportDelegate(
    string templateString,
    out IList<Exception> renderingErrors
  );

  private readonly GenerateReportDelegate generateReport;

#if SYSTEM_DIAGNOSTICS_CODEANALYSIS_REQUIRESUNREFERENCEDCODEATTRIBUTE
  [RequiresUnreferencedCode($"{nameof(ReportGeneratorWrapper)} is incompatible with trimming.")]
#endif
  public ReportGeneratorWrapper(LibraryTestRun libraryTestRun)
  {
    var typeOfReportGenerator = typeof(LibraryTestRun)
      .Assembly
      .GetType(TargetTypeFullName)
      ?? throw new InvalidOperationException($"Failed to get type info of '{TargetTypeFullName}'.");

    var reportGenerator = Activator.CreateInstance(
      type: typeOfReportGenerator,
      bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance,
      binder: null,
      args: [libraryTestRun],
      culture: null
    );

    var methodInfoOfGenerateReport = typeOfReportGenerator
      .GetMethod(
        name: nameof(GenerateReport),
        bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance,
        binder: null,
        types: [typeof(string), typeof(IList<Exception>).MakeByRefType()],
        modifiers: null
      )
      ?? throw new InvalidOperationException($"Failed to get '{nameof(GenerateReport)}' method info from '{typeOfReportGenerator.FullName}'");

#if SYSTEM_REFLECTION_CREATEDELEGATE_OF_T
    generateReport = methodInfoOfGenerateReport.CreateDelegate<GenerateReportDelegate>(
      target: reportGenerator
    );
#else
    generateReport = (GenerateReportDelegate)methodInfoOfGenerateReport.CreateDelegate(
      delegateType: typeof(GenerateReportDelegate),
      target: reportGenerator
    );
#endif
  }

  public string GenerateReport(
    string templateString,
    out IList<Exception> renderingErrors
  )
    => generateReport(
      templateString,
      out renderingErrors
    );
}
