# OpsRelay Git Workflow

This document defines the branch model, commit conventions and release process
used by the OpsRelay repository.

## Branch Model

| Branch | Purpose | Lifetime |
|--------|---------|----------|
| `main` | Production-ready releases only. Every commit is tagged. | Permanent |
| `develop` | Integration branch for completed feature work. | Permanent |
| `feature/T{NNN}-{slug}` | One short-lived branch per planned task. | Deleted after merge |
| `migration/net6-to-net8` | Long-lived branch for the Phase 2 modernization. | Deleted after merge |
| `release/v{major}.{minor}.{patch}` | Release stabilization and changelog finalization. | Deleted after merge |

Direct commits to `main` are prohibited. All changes arrive through a
`--no-ff` merge so the branch topology remains visible in history.

## Branch Lifecycle

1. Start from the integration branch and ensure it is current.
   ```bash
   git checkout develop
   git pull origin develop
   ```
2. Create a task branch.
   ```bash
   git checkout -b feature/T001-repo-init
   ```
3. Implement the task, keeping commits small and meaningful.
4. Verify the build and tests locally.
   ```bash
   dotnet build OpsRelay.sln
   dotnet test
   ```
5. Merge back with an explicit merge commit.
   ```bash
   git checkout develop
   git merge --no-ff feature/T001-repo-init
   git branch -d feature/T001-repo-init
   ```
6. Tag releases on `develop` and promote to `main` through a release branch.

## Conventional Commits

Commit messages use the format `type(scope): description`.

| Type | Use for |
|------|---------|
| `feat` | New functionality |
| `fix` | Bug fixes |
| `refactor` | Behavior-preserving code changes |
| `test` | Test additions or corrections |
| `docs` | Documentation-only changes |
| `chore` | Build, tooling and repository maintenance |
| `build` | Build-system or dependency changes |
| `ci` | CI/CD configuration |

Scopes follow the layer being changed: `domain`, `application`,
`infrastructure`, `api`, `frontend`, `tests`, `docker`, `ci`.

Example:

```text
feat(domain): add Incident aggregate with SLA deadline

Introduce the Incident aggregate root, value objects and domain events.
```

A breaking change is recorded in the footer:

```text
BREAKING CHANGE: target framework upgraded from net6.0 to net8.0
```

## Versioning and Tags

The project follows Semantic Versioning. Annotated tags mark releases:

| Tag | Milestone |
|-----|-----------|
| `v0.1.0` | .NET 6 Clean Architecture baseline (Phase 1) |
| `v0.2.0` | .NET 8 / C# 12 modernization (Phase 2) |
| `v0.3.0` | SLA engine and domain test coverage (Phase 3) |
| `v1.0.0` | Production release with frontend, Docker and CI |

Create a tag after merging the release task into `develop`:

```bash
git tag -a v0.1.0 -m "Release v0.1.0: .NET 6 baseline"
git push origin develop --tags
```

## Phase 2 Migration Path

The .NET 8 modernization is developed on `migration/net6-to-net8`, branched from
`develop` at tag `v0.1.0`. That branch must remain buildable throughout the
migration so each commit can be reviewed independently. When the migration is
approved, it is merged into `develop` with `--no-ff`, tagged `v0.2.0`, and the
migration branch is deleted. The full before/after rationale is captured in
`docs/adr/ADR-001-net8-migration.md`.

## Pull Request Checklist

- [ ] Branch follows the naming convention.
- [ ] Commit messages follow Conventional Commits.
- [ ] `dotnet build OpsRelay.sln` passes with zero warnings.
- [ ] `dotnet test` passes.
- [ ] Acceptance criteria from the task plan are covered.
- [ ] No secrets, credentials or local configuration are committed.
- [ ] `CHANGELOG.md` and `build-state.md` reflect the completed work.
