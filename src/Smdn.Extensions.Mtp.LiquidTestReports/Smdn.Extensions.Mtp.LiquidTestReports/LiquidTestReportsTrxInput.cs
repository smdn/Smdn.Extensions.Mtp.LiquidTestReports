// SPDX-FileCopyrightText: 2025 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.IO;

using LiquidTestReports.Core.Models;

namespace Smdn.Extensions.Mtp.LiquidTestReports;

internal class LiquidTestReportsTrxInput : IReportInput {
  public IEnumerable<FileInfo> Files { get; }
  public InputFormatType Format => InputFormatType.Trx;
  public string? GroupTitle { get; }
  public string? TestSuffix { get; }
  public IReadOnlyDictionary<string, string>? Parameters { get; }

  public LiquidTestReportsTrxInput(
    FileInfo trxFile
  )
    : this(
      trxFile: trxFile,
      groupTitle: null,
      testSuffix: null,
      parameters: null
    )
  {
  }

  public LiquidTestReportsTrxInput(
    FileInfo trxFile,
    string? groupTitle,
    string? testSuffix,
    IReadOnlyDictionary<string, string>? parameters
  )
  {
    Files = [trxFile ?? throw new ArgumentNullException(nameof(trxFile))];
    GroupTitle = groupTitle;
    TestSuffix = testSuffix;
    Parameters = parameters;
  }
}
