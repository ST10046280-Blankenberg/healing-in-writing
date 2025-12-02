# Contributing Guide

<p align="center">
  <img src="https://img.shields.io/badge/Branches-feature%7Cfix%7Cchore%7Cdocs-3A7CA5?style=flat-square" alt="Branch naming">
  <img src="https://img.shields.io/badge/Commits-type%3A%20summary-444444?style=flat-square" alt="Commit style">
  <img src="https://img.shields.io/badge/Tests-dotnet%20test-critical?style=flat-square" alt="dotnet test required">
</p>

Keep changes small, explain your intent, and respect the service-first architecture.

---

## Branch Naming
Use prefixes that signal the work type:

| Prefix | Usage |
| --- | --- |
| `feature/<short-topic>` | New functionality or large enhancements. |
| `fix/<short-topic>` | Bug fixes or regressions. |
| `chore/<short-topic>` | Maintenance, tooling, refactors. |
| `docs/<short-topic>` | Documentation-only updates. |

## Commit Style
Format each message as `type: short summary` using lowercase types such as feature, fix, chore, or docs. Aim for clear, action-led summaries.

## Pull Requests
- Follow the checklist in [the PR template](.github/pull_request_template.md).
- Ensure `dotnet test` passes locally before requesting a review.
- Summarise architectural impacts so reviewers can validate controller-service boundaries quickly.

> Need help? Start a draft PR early and flag open questions in the description.
