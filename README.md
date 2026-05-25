[![GitHub license](https://img.shields.io/github/license/smdn/Smdn.Extensions.Mtp.LiquidTestReports)](https://github.com/smdn/Smdn.Extensions.Mtp.LiquidTestReports/blob/main/LICENSE.txt)
[![tests/main](https://img.shields.io/github/actions/workflow/status/smdn/Smdn.Extensions.Mtp.LiquidTestReports/test.yml?branch=main&label=tests%2Fmain)](https://github.com/smdn/Smdn.Extensions.Mtp.LiquidTestReports/actions/workflows/test.yml)
[![CodeQL](https://github.com/smdn/Smdn.Extensions.Mtp.LiquidTestReports/actions/workflows/codeql-analysis.yml/badge.svg?branch=main)](https://github.com/smdn/Smdn.Extensions.Mtp.LiquidTestReports/actions/workflows/codeql-analysis.yml)

# Smdn.Extensions.Mtp.LiquidTestReports
[![NuGet](https://img.shields.io/nuget/v/Smdn.Extensions.Mtp.LiquidTestReports.svg)](https://www.nuget.org/packages/Smdn.Extensions.Mtp.LiquidTestReports/)

The [Microsoft.Testing.Platform extension](https://learn.microsoft.com/dotnet/core/testing/microsoft-testing-platform-extensions) to convert TRX files generated during the test session into the preferred format using [LiquidTestReports](https://github.com/kurtmkurtm/LiquidTestReports), the [Liquid template language](https://shopify.github.io/liquid/) processor.

To generate and convert a test report, specify at least the following three required options as arguments to `dotnet test`.

```
dotnet test --report-trx --report-trx-filename TestResult.trx --liquidtr-template TestResult.template.md
```

For details on each option, refer to the output of `dotnet test --help`.

For instructions on configuring test projects in NUnit and MSTest, refer to the [examples](./examples/Smdn.Extensions.Mtp.LiquidTestReports/) directory.

# For contributors
Contributions are appreciated!

If there's a feature you would like to add or a bug you would like to fix, please read [Contribution guidelines](./CONTRIBUTING.md) and create an Issue or Pull Request.

IssueやPull Requestを送る際は、[Contribution guidelines](./CONTRIBUTING.md)をご覧頂ください。　可能なら英語が望ましいですが、日本語で構いません。

# Notice
## License
This project is licensed under the terms of the [MIT License](./LICENSE.txt).

However, the final published NuGet package incorporates a compiled third-party component (`LiquidTestReports.Core`) under the **BSD 2-Clause License**.
- A comprehensive license file combining both terms is available in [LICENSE_BUNDLE.txt](./src/Smdn.Extensions.Mtp.LiquidTestReports/LICENSE_BUNDLE.txt) (packaged as `LICENSE_BUNDLE.txt` inside the NuGet artifact).
- Detailed license distributions and copyright statements are also maintained in [ThirdPartyNotices.md](./ThirdPartyNotices.md).

## Credits
This project incorporates implementations ported and vendored from the following project:

- [LiquidTestReports](https://github.com/kurtmkurtm/LiquidTestReports)

  The source code of `src/LiquidTestReports.Core` is integrated, locally modified, and compiled internally within this repository. Detailed metadata regarding the baseline upstream commit, rationales for modification, and file exclusions are documented in [src/external/README.md](./src/external/README.md).

This project uses the following components as external package dependencies:

- [DotLiquid](https://github.com/dotliquid/dotliquid)
- [Microsoft.Testing.Platform, Microsoft.Testing.Extensions.TrxReport](https://github.com/microsoft/testfx)

See also [ThirdPartyNotices.md](./ThirdPartyNotices.md) for detail.
