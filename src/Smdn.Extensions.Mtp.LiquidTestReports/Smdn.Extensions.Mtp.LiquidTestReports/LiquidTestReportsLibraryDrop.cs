// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using LiquidTestReports.Core.Drops;

namespace Smdn.Extensions.Mtp.LiquidTestReports;

internal sealed class LiquidTestReportsLibraryDrop : LibraryDrop {
  public override string Text => $"{ExtensionInfo.DisplayName} {ExtensionInfo.SemVer}";
  public override string Link => global::ExtensionInfo.RepositoryUrl;
}
