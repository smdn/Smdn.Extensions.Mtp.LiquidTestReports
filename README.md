[![GitHub license](https://img.shields.io/github/license/smdn/Smdn.Extensions.Mtp.LiquidTestReports)](https://github.com/smdn/Smdn.Extensions.Mtp.LiquidTestReports/blob/main/LICENSE.txt)
[![tests/main](https://img.shields.io/github/actions/workflow/status/smdn/Smdn.Extensions.Mtp.LiquidTestReports/test.yml?branch=main&label=tests%2Fmain)](https://github.com/smdn/Smdn.Extensions.Mtp.LiquidTestReports/actions/workflows/test.yml)
[![CodeQL](https://github.com/smdn/Smdn.Extensions.Mtp.LiquidTestReports/actions/workflows/codeql-analysis.yml/badge.svg?branch=main)](https://github.com/smdn/Smdn.Extensions.Mtp.LiquidTestReports/actions/workflows/codeql-analysis.yml)

# Smdn.Extensions.Mtp.LiquidTestReports
[![NuGet](https://img.shields.io/nuget/v/Smdn.Extensions.Mtp.LiquidTestReports.svg)](https://www.nuget.org/packages/Smdn.Extensions.Mtp.LiquidTestReports/)

The [Microsoft.Testing.Platform extension](https://learn.microsoft.com/dotnet/core/testing/microsoft-testing-platform-extensions) to convert TRX files generated during the test session into the preferred format using [LiquidTestReports](https://github.com/kurtmkurtm/LiquidTestReports), the [Liquid template language](https://shopify.github.io/liquid/) processor.

> [!NOTE]
> `Smdn.Extensions.Mtp.LiquidTestReports` is currently only available as a pre-release version. This is due to the reason that the final release version of [LiquidTestReports.Core](https://www.nuget.org/packages/LiquidTestReports.Core/) v2, one of its dependency packages, has not yet been released.
> The versions of the `Smdn.Extensions.Mtp.LiquidTestReports` package currently available that have the suffix `-rc` can be used as release versions.

# For contributors
Contributions are appreciated!

If there's a feature you would like to add or a bug you would like to fix, please read [Contribution guidelines](./CONTRIBUTING.md) and create an Issue or Pull Request.

IssueやPull Requestを送る際は、[Contribution guidelines](./CONTRIBUTING.md)をご覧頂ください。　可能なら英語が望ましいですが、日本語で構いません。

# Notice
## License
This project is licensed under the terms of the [MIT License](./LICENSE.txt).

## Credit
This project incorporates implementations partially ported from the following projects. See also [ThirdPartyNotices.md](./ThirdPartyNotices.md) for detail.

- [DotLiquid](https://github.com/dotliquid/dotliquid)
- [LiquidTestReports](https://github.com/kurtmkurtm/LiquidTestReports)
- [Microsoft.Testing.Platform, Microsoft.Testing.Extensions.TrxReport](https://github.com/microsoft/testfx)
