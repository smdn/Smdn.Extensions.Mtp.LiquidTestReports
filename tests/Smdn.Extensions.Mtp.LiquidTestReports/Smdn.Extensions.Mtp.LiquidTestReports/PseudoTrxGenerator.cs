// SPDX-FileCopyrightText: 2025 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.Extensions.TestHost;
using Microsoft.Testing.Platform.Messages;
using Microsoft.Testing.Platform.Services;
using Microsoft.Testing.Platform.TestHost;

namespace Smdn.Extensions.Mtp.LiquidTestReports;

internal sealed class PseudoTrxGenerator : IDataProducer, ITestSessionLifetimeHandler {
  private readonly IMessageBus messageBus;

  private SessionUid? sessionUid;

  public PseudoTrxGenerator(
    IMessageBus messageBus
  )
  {
    this.messageBus = messageBus;
  }

  public Type[] DataTypesProduced { get; } = [
    typeof(SessionFileArtifact) // produces TRX file
  ];

  public string Uid => nameof(PseudoTrxGenerator);
  public string Version => "1.0.0";
  public string DisplayName { get; } = nameof(PseudoTrxGenerator);
  public string Description { get; } = nameof(PseudoTrxGenerator);

  public Task<bool> IsEnabledAsync()
    => Task.FromResult(true);

  public Task OnTestSessionStartingAsync(ITestSessionContext testSessionContext)
  {
    sessionUid = testSessionContext.SessionUid;

    return Task.CompletedTask;
  }

  public Task OnTestSessionFinishingAsync(ITestSessionContext testSessionContext)
    => Task.CompletedTask;

  public async Task GenerateTrxAsync(string filePath, string trxFileContents)
  {
    var fileInfo = new FileInfo(filePath);

    await File.WriteAllTextAsync(fileInfo.FullName, contents: trxFileContents);

    await messageBus.PublishAsync(
      this,
      new SessionFileArtifact(
        sessionUid: sessionUid ?? throw new InvalidOperationException("session not started"),
        fileInfo: fileInfo,
        displayName: "TRX output",
        description: "TRX output for the test session"
      )
    ).ConfigureAwait(false);
  }
}
