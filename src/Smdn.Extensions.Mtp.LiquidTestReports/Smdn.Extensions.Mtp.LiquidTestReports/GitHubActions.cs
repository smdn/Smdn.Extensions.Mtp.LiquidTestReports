// SPDX-FileCopyrightText: 2025 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Testing.Platform.OutputDevice;

internal static class GitHubActions {
  public const string GITHUB_STEP_SUMMARY = nameof(GITHUB_STEP_SUMMARY);

  private static readonly SemaphoreSlim FileAppendSemaphore = new(1, 1);

  internal static async Task AppendStepSummaryAsync(
    string contents,
    Func<IOutputDeviceData, CancellationToken, Task> displayOutputDeviceAsync,
    CancellationToken cancellationToken
  )
  {
    if (
      Environment.GetEnvironmentVariable(GITHUB_STEP_SUMMARY) is not string stepSummaryFilePath ||
      string.IsNullOrEmpty(stepSummaryFilePath)
    ) {
      await displayOutputDeviceAsync(
        new WarningMessageOutputDeviceData(
          message: $"The environment variable {GITHUB_STEP_SUMMARY} is not set."
        ),
        cancellationToken
      ).ConfigureAwait(false);

      return;
    }

    try {
#pragma warning disable SA1114
      await TryIOAsync(
        async ct => {
          try {
            await FileAppendSemaphore.WaitAsync(ct).ConfigureAwait(false);

            await AppendTextAsync(
              path: stepSummaryFilePath,
              contents: contents,
              cancellationToken: ct
            ).ConfigureAwait(false);
          }
          finally {
            FileAppendSemaphore.Release();
          }
        },
        cancellationToken: cancellationToken
      ).ConfigureAwait(false);
#pragma warning restore SA1114
    }
    catch (Exception ex) when (ex is UnauthorizedAccessException or IOException) {
      await displayOutputDeviceAsync(
        new ExceptionOutputDeviceData(exception: ex),
        cancellationToken
      ).ConfigureAwait(false);
    }

    static async Task AppendTextAsync(
      string path,
      string contents,
      CancellationToken cancellationToken
    )
    {
      using var stream = File.Open(path, FileMode.Append, FileAccess.Write, FileShare.Read);
      using var writer = new StreamWriter(stream);

#if SYSTEM_IO_TEXTWRITER_WRITELINEASYNC_CANCELLATIONTOKEN
      await writer.WriteLineAsync(contents.AsMemory(), cancellationToken).ConfigureAwait(false);
#else
      await writer.WriteLineAsync(contents).ConfigureAwait(false);
#endif

      // append blank lines for subsequent appending
#if SYSTEM_IO_TEXTWRITER_WRITELINEASYNC_CANCELLATIONTOKEN
      await writer.WriteLineAsync(ReadOnlyMemory<char>.Empty, cancellationToken).ConfigureAwait(false);
#else
      await writer.WriteLineAsync().ConfigureAwait(false);
#endif

#if SYSTEM_IO_TEXTWRITER_FLUSHASYNC_CANCELLATIONTOKEN
      await writer.FlushAsync(cancellationToken).ConfigureAwait(false);
#else
      await writer.FlushAsync().ConfigureAwait(false);
#endif

      writer.Close();
    }

    static async Task TryIOAsync(
      Func<CancellationToken, Task> actionAsync,
      CancellationToken cancellationToken
    )
    {
      const int MaxRetry = 10;
      const int Interval = 200;

      Exception? caughtException = null;

      for (var retry = MaxRetry; retry != 0; retry--) {
        try {
          await actionAsync(cancellationToken).ConfigureAwait(false);

          return; // completed without exception
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException or IOException) {
          // hold the exception and try again
          caughtException = ex;
        }

        await Task.Delay(Interval, cancellationToken).ConfigureAwait(false);
      }

      if (caughtException is not null)
        ExceptionDispatchInfo.Capture(caughtException).Throw();
    }
  }
}
