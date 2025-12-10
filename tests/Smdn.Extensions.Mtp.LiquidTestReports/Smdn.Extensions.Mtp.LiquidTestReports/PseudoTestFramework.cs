// SPDX-FileCopyrightText: 2025 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Testing.Platform.Capabilities.TestFramework;
using Microsoft.Testing.Platform.Extensions.TestFramework;
using Microsoft.Testing.Platform.Requests;

namespace Smdn.Extensions.Mtp.LiquidTestReports;

internal class PseudoTestFramework : ITestFramework {
  public string Uid => nameof(PseudoTestFramework);
  public string Version => "1.0.0";
  public string DisplayName => nameof(PseudoTestFramework);
  public string Description => nameof(PseudoTestFramework);

  public IServiceProvider ServiceProvider { get; }

  private readonly Func<PseudoTestFramework, CancellationToken, Task>? onRunTestExecutionAsync;

#pragma warning disable IDE0060
  public PseudoTestFramework(
    ITestFrameworkCapabilities capabilities,
    IServiceProvider serviceProvider,
    Func<PseudoTestFramework, CancellationToken, Task>? onRunTestExecutionAsync
  )
#pragma warning restore IDE0060
  {
    ServiceProvider = serviceProvider;
    this.onRunTestExecutionAsync = onRunTestExecutionAsync;
  }

  public Task<bool> IsEnabledAsync()
    => Task.FromResult(true);

  public Task<CreateTestSessionResult> CreateTestSessionAsync(CreateTestSessionContext context)
    => Task.FromResult(new CreateTestSessionResult() { IsSuccess = true });

  public Task<CloseTestSessionResult> CloseTestSessionAsync(CloseTestSessionContext context)
    => Task.FromResult(new CloseTestSessionResult() { IsSuccess = true });

  public async Task ExecuteRequestAsync(ExecuteRequestContext context)
  {
    switch (context.Request) {
      case RunTestExecutionRequest:
        if (onRunTestExecutionAsync is not null)
          await onRunTestExecutionAsync(this, context.CancellationToken).ConfigureAwait(false);

        context.Complete();

        break;

      case DiscoverTestExecutionRequest:
        context.Complete();
        break;

      default:
        throw new NotImplementedException($"Request type {context.Request.GetType()} is not supported.");
    }
  }
}
