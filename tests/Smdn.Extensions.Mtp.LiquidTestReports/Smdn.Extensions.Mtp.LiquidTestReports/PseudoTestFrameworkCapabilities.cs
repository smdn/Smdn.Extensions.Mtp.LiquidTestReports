// SPDX-FileCopyrightText: 2025 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System.Collections.Generic;

using Microsoft.Testing.Extensions.TrxReport.Abstractions;
using Microsoft.Testing.Platform.Capabilities.TestFramework;

namespace Smdn.Extensions.Mtp.LiquidTestReports;

internal sealed class PseudoTestFrameworkCapabilities : ITestFrameworkCapabilities {
  private readonly TrxCapability trx = new();

  public IReadOnlyCollection<ITestFrameworkCapability> Capabilities => [trx];

  private sealed class TrxCapability : ITrxReportCapability {
    public bool IsTrxEnabled { get; set; }
    public bool IsSupported => true;
    public void Enable() => IsTrxEnabled = true;
  }
}
