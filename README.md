[![GitHub license](https://img.shields.io/github/license/smdn/Smdn.Template.Library)](https://github.com/smdn/Smdn.Template.Library/blob/main/LICENSE.txt)
[![tests/main](https://img.shields.io/github/actions/workflow/status/smdn/Smdn.Template.Library/test.yml?branch=main&label=tests%2Fmain)](https://github.com/smdn/Smdn.Template.Library/actions/workflows/test.yml)
[![CodeQL](https://github.com/smdn/Smdn.Template.Library/actions/workflows/codeql-analysis.yml/badge.svg?branch=main)](https://github.com/smdn/Smdn.Template.Library/actions/workflows/codeql-analysis.yml)
[![NuGet](https://img.shields.io/nuget/v/Smdn.svg)](https://www.nuget.org/packages/Smdn/)

# Smdn.Template.Library
Smdn.Template.Library is a .NET library template.

# Initial configurations to do
- [ ] Configure [Repository settings](/../../settings)
  - General
    - Releases
      - [ ] ✅ [Enable release immutability](/../../settings#release_immutability)
    - Pull Requests
      - [ ] ✅ [Allow merge commits](/../../settings#merge_types_merge_commit)
      - [ ] ✅ [Allow squash merging](/../../settings#merge_types_squash)
      - [ ] ⛔ [Allow rebase merging](/../../settings#merge_types_rebase)
      - [ ] ✅ [Always suggest updating pull request branches](/../../settings#allow_update_branch)
      - [ ] ✅ [Automatically delete head branches](/../../settings#merge_types_delete_branch)
  - [ ] Configure [Rules](/../../settings/rules)
    - Add **New branch ruleset** for `main` branch
      - Ruleset Name: `main branch`
      - Enforcement status: Active
      - Bypass list:
        - Repository admin: Always allow
      - Target branches: Include default branch
      - Branch rules
        - ✅ Restrict deletions
        - ✅ Require a pull request before merging
          - ✅ Dismiss stale pull request approvals when new commits are pushed
          - ✅ Require review from Code Owners
          - ✅ Require approvals of the most recent reviewable push
          - Allowed merge methods:
            - Merge
            - Squash
        - ✅ Block force pushes
    - Add **New tag ruleset** for release tags
      - Ruleset Name: `release tags`
      - Enforcement status: Active
      - Bypass list:
        - Repository admin
      - Tag targeting criteria:
        - `releases/*`
        - `new-release/*`
      - Tag rules
        - ✅ Restrict creations
        - ✅ Restrict updates
        - ✅ Restrict deletions
        - ✅ Block force pushes
  - [ ] Configure [Code security and analysis](/../../settings/security_analysis)
    - Enable **Private vulnerability reporting**
    - Enable **Dependabot alerts**
  - [ ] [Generate PAT](https://github.com/settings/tokens) and [set Actions secrets](/../../settings/secrets/actions) for [workflows/generate-release-target](/.github/workflows/generate-release-target.yml) and [workflows/publish-release-target.yml](/.github/workflows/publish-release-target.yml)
  - [ ] Configure the cron schedule of [CodeQL workflow](/.github/workflows/codeql-analysis.yml)
  - [ ] [Create a issue label](/../../labels) for the release targets.
    - Label name: `release-target` (Must be the same as that specified in the files of release-target workflows.)
    - Description: `Describing a new release`
    - Color: `#006B75`
- [ ] Change `PackageProjectUrl` and `RepositoryUrl` in [Directory.Build.props](src/Directory.Build.props)
- [ ] Rename project directories and project files.
- [ ] Change links of badges.
- [ ] Add [Contribution guidelines](./CONTRIBUTING.md).

# For contributors
Contributions are appreciated!

If there's a feature you would like to add or a bug you would like to fix, please read [Contribution guidelines](./CONTRIBUTING.md) and create an Issue or Pull Request.

IssueやPull Requestを送る際は、[Contribution guidelines](./CONTRIBUTING.md)をご覧頂ください。　可能なら英語が望ましいですが、日本語で構いません。
