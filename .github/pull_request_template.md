## Goal
Explain the problem this PR solves and confirm it follows our architecture. One sentence scope: what changed and why.

## Summary
- Change 1
- Change 2
- Change 3

## Screenshots (if UI changed)
- Before:
- After:

## Testing
- [ ] Not run (reason)
- [ ] `dotnet test` passed
- [ ] Manual QA (steps and result)
Notes: include any edge cases you tried

## Architecture Checks
- [ ] Controllers only orchestrate service calls
- [ ] Domain and services own business logic
- [ ] Repositories handle persistence only
- [ ] Mapping helpers updated if models or view models changed

## Data and Config
- [ ] No EF Core migrations
- [ ] EF Core migration included and reviewed
- [ ] No config changes
- [ ] Config updated (keys, appsettings) and documented

## Auth and Roles
- [ ] No changes to auth
- [ ] Changes affect roles or access (Guest, Volunteer, Admin). Details:

## Privacy and Security
- [ ] No new personal data handled
- [ ] Personal data touched. POPIA considerations noted
- [ ] Input validation and error handling confirmed

## Accessibility and Performance
- [ ] Basic a11y check on new views
- [ ] No noticeable perf impact

## Rollback Plan
If we need to revert, how do we do it safely?

## Follow-up
List TODOs, known gaps, or next tasks to track.
