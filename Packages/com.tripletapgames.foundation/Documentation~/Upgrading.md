# Upgrading SDKs

1. Update the pinned version once in `TTGDependencyCatalog` and the matching adapter assembly version constraint.
2. Read the vendor migration notes and update adapter calls without exposing vendor types in public TTG APIs.
3. Compile a clean Unity 2022.3 project with only UniTask, then the complete dependency set.
4. Run EditMode tests and export Android and iOS projects.
5. Verify Android manifest output, iOS project idempotence, initialization, consent, ads, purchase cancellation, and restore behavior.
6. Record the approved upgrade in `CHANGELOG.md`.
