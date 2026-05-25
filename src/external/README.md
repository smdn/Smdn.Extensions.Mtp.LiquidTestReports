# External Component: LiquidTestReports.Core

This directory contains an external software component integrated into this repository for internal implementation purposes.

## Component Metadata

- **Original Project:** [kurtmkurtm/LiquidTestReports](https://github.com/kurtmkurtm/LiquidTestReports/)
- **Upstream Source:** `src/LiquidTestReports.Core`
- **Baseline Branch:** `master`
- **Baseline Commit:** `1f39c20cec7aa06f50f88843ebe99f58f7ead3c5`

## Status and Modification Rationale

The upstream repository has been inactive for an extended period, with no responses to issues or new commits for over a year. Consequently, directly integrating necessary bug fixes and maintenance adjustments via upstream contributions is currently unfeasible.

To address these maintenance requirements, the source code of `src/LiquidTestReports.Core` has been copied into this directory. Local modifications have been applied to this copied source code to ensure proper functionality and integration.

## Usage and Build Configuration

This component is compiled as a separate assembly and is referenced exclusively by the host project via a `ProjectReference`.
This library is not published independently as a standalone NuGet package. Instead, the compiled binary is bundled internally within the host project's final NuGet package (`Smdn.Extensions.Mtp.LiquidTestReports`).

## Licensing

The code within this directory is licensed under the **BSD 2-Clause License**, which is distinct from the primary license (MIT License) applied to the rest of this repository.

- The original copyright notice, conditions, and disclaimers are preserved in [LiquidTestReports.Core.LICENSE](./LiquidTestReports.Core.LICENSE).
- Machine-readable Software Bill of Materials (SBOM) metadata is provided in [LiquidTestReports.Core.spdx.json](./LiquidTestReports.Core.spdx.json).

Any local modifications applied within this directory remain subject to the terms of the original BSD 2-Clause License.
