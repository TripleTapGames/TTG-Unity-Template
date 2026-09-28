# TTG Unity Template

A separate, empty Unity project for starting new games. Open with Unity **2022.3.62f3**. This project does not contain Pop Sort gameplay, scenes, or game credentials.

## Start here

1. In Unity Hub, choose Add project from disk and select this folder.
2. Let Unity finish importing and downloading packages. Internet access and Git are needed on a fresh machine.
3. In Player Settings, change the company name, product name and Android/iOS application identifiers.
4. Open **Tools > Triple Tap Games > Project Setup**. On a clone, install Firebase separately if needed, following the [Firebase installation guide](Packages/com.tripletapgames.foundation/Documentation~/FirebaseInstallation.md). Import Analytics and Crashlytics 13.13.0, then click **Refresh Firebase Adapter** and wait for compilation. Locate/create blank configuration assets, enter that game's settings, enable the services you need, and run validation.
5. Click **Create / Update Local Loading Scene**. Loading is generated locally because it can contain project credentials and is intentionally excluded from Git.
6. Click **Create / Update Base Game Flow**. The committed Core scene contains the reusable flow, and the action repairs its Win/Lose UI, button EventSystem, Build Settings order and default level-sequence asset.
7. Assign ordered level prefabs and stable IDs in `Assets/Game/Config/DefaultLevelSequence.asset`. Gameplay reports outcomes through `TTGGameFlow.Instance.WinLevel()` or `LoseLevel()`.
8. Follow each vendor's setup requirements, including Firebase configuration files, store products, advertising consent, and platform dependency resolution. Test on real devices before release.

SDK installation does not activate services. Foundation integrations default to disabled. The generated Loading scene owns consent-aware initialization and progress UI, then enters Core. The base game flow provides prefab sequencing, Win/Lose UI and TTG level analytics; GameAnalytics receives native progression events. See the [complete user guide](USER_GUIDE.md).

## Installed SDKs

| SDK | Version/source |
| --- | --- |
| UniTask | 2.5.11, pinned Git tag |
| Firebase Analytics and Crashlytics | 13.13.0, separate local asset import; excluded from Git |
| AppLovin MAX | 8.6.6, UPM |
| GameAnalytics | 8.2.0, OpenUPM |
| Singular | 5.10.1, pinned official Git tag |
| Unity IAP | 4.14.0, Unity registry |
| Facebook | Existing 17.x asset SDK, without Pop Sort settings; not upgraded to 18.1.0 |
| DOTween | Existing Demigiant asset import; version not independently verified |
| External Dependency Manager | 1.2.189, OpenUPM |

Package resolution is recorded in `Packages/packages-lock.json`. Do not install another asset copy of GameAnalytics or mix Firebase asset-import and UPM installation methods. MAX mediation-network adapters are not included; select the networks needed by each game.

## Configuration and safety

Blank local configs are under `Assets/TTGGenerated/Resources/TTG`, which is ignored by Git. After cloning without this ignored folder, create configs from the Project Setup window. Never commit populated vendor settings or Firebase project files. Runtime SDK keys included in client builds are not server secrets; privileged credentials belong on your backend.

The foundation package is included as an embedded package. An Editor compilation and its existing EditMode tests do not prove live vendor behavior. Device ads, purchases, attribution, consent behavior, native Android/iOS builds, and Unity 6 compatibility require separate validation before shipping. This template is not acceptance of the entire foundation implementation plan.

The original Pop Sort project is separate and has not been migrated into this project.

## Lightweight Git workflow

Clone → let packages resolve → import pinned Firebase locally → Refresh Firebase Adapter → configure the game → validate. Firebase files, generated native dependencies, local adapters and credentials are ignored; they are not included in clones or GitHub source ZIPs. Existing local Firebase files are preserved. Keep reusable installers in ignored `LocalSDKInstallers/`, outside Assets. The template opens without Firebase; enable the integration only after setup.

Before uninstalling Firebase, disable the service and use **Disable Firebase Adapter Before SDK Removal**, then wait for compilation. No shared compiler flag is needed. See the linked guide for recovery and CI instructions. Remaining imported SDKs still require their own redistribution and upload review.
