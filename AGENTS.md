# Repository Guidelines

## Project Structure
The repository is a Unity project hosting the `com.bazyleu.unistate` UPM package. The shipped package lives in `Assets/UniState/` — `package.json` plus `Runtime/`, with the core state machine in `Runtime/Core/` and the optional DI integrations (VContainer, Zenject, Reflex) in `Runtime/Integrations/`, each compiled under its `UNISTATE_*_SUPPORT` define. Everything else is not shipped to consumers: `Assets/UniStateTests/` (EditMode and PlayMode tests), `Assets/Benchmarks/`, `Assets/Examples/`, `Packages/` and `ProjectSettings/` of the host project, and the GitHub workflows.

## Changelog
`CHANGELOG.md` is the consumer-facing record of what changed, kept in loosely following style: a `## [Unreleased]` section on top, then one section per released version, newest first, each grouped into `### Added`, `### Changed`, `### Fixed` and `### Removed` (only the groups that apply, in that order). Sections carry no dates — the Git tag is the release record.

Update `[Unreleased]` in the same commit that makes the change, never as a follow-up:
- Write entries for what a consumer of the UPM package observes — public API, state machine and state lifecycle behavior, error handling, DI integrations, supported Unity version, required dependencies. Name public types and members in backticks.
- Prefix anything that can break a consumer's build or change behavior they rely on with `**BREAKING**:`. When the README gains an upgrade guide section for the change, point to it from the entry.
- Leave out internal-only churn (tests, benchmarks, examples, CI wiring, host project packages and settings, editor settings, formatting, README-only wording). Mention it only when it changes what ships — a raised minimum Unity version or a new integration does, a test Unity version bump does not.
- Use a sub-bullet list under an entry when one change has several facets worth spelling out; otherwise keep it to a single line.

Changes from Unreleased to specific release version would be done manually, do not edit it yourself. Do not edit any historical data in the CHANGELOG.md file.
