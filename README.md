# TTG Unity Template

A separate, empty Unity project for starting new games. Open with Unity **2022.3.62f3**. This project does not contain Pop Sort gameplay, scenes, or game credentials.

## Start here

1. In Unity Hub, choose Add project from disk and select this folder.
2. Let Unity finish importing and downloading packages. Internet access and Git are needed on a fresh machine.
3. In Player Settings, change the company name, product name and Android/iOS application identifiers.
4. Open **Tools > Triple Tap Games > Project Setup**. On a clone, install Firebase separately if needed, following the [Firebase installation guide](https://github.com/TripleTapGames/TTG-Unity-Foundation/blob/v0.2.1/Documentation~/FirebaseInstallation.md). Import Analytics and Crashlytics 13.13.0, then click **Refresh Firebase Adapter** and wait for compilation. Locate/create blank configuration assets, enter that game's settings, enable the services you need, and run validation.
5. Click **Create / Update Local Loading Scene**. Loading is generated locally because it can contain project credentials and is intentionally excluded from Git.
6. Build the game's own level configuration, progression, Win/Lose UI, Next, and Retry flow. Foundation does not impose a gameplay structure.
7. Connect the game's accepted level start, completion, and failure points to the explicit TTG analytics and ad-gating APIs in the [analytics events guide](ANALYTICS_EVENTS_GUIDE.md).
8. Follow each vendor's setup requirements, including Firebase configuration files, store products, advertising consent, and platform dependency resolution. Test on real devices before release.

SDK installation does not activate services. Foundation integrations default to disabled. The generated Loading scene owns consent-aware initialization and progress UI, then enters the configured game scene. The game owns all gameplay and level progression. `Assets/Game/Scripts/Analytics/AppLovinAdRevenueAnalytics.cs` forwards MAX paid impressions to TTG analytics on the Unity main thread. See the [complete user guide](USER_GUIDE.md) and the [analytics events guide](ANALYTICS_EVENTS_GUIDE.md).

## Installed SDKs

| SDK | Version/source |
| --- | --- |
| UniTask | 2.5.11, pinned Git tag |
| Firebase Analytics and Crashlytics | 13.13.0, separate local asset import; excluded from Git |
| AppLovin MAX | 8.6.6, UPM |
| GameAnalytics | 8.2.0, OpenUPM |
| Singular | 5.10.1, pinned official Git tag |
| Unity IAP | 4.14.0, Unity registry |
| Facebook | 17.0.0 local asset import |
| DOTween | Existing Demigiant asset import; version not independently verified |
| External Dependency Manager | 1.2.189, OpenUPM |

Package resolution is recorded in `Packages/packages-lock.json`. Do not install another asset copy of GameAnalytics or mix Firebase asset-import and UPM installation methods. MAX mediation-network adapters are not included; select the networks needed by each game.

## Configuration and safety

Blank local configs are under `Assets/TTGGenerated/Resources/TTG`, which is ignored by Git. After cloning without this ignored folder, create configs from the Project Setup window. Never commit populated vendor settings or Firebase project files. Runtime SDK keys included in client builds are not server secrets; privileged credentials belong on your backend.

The foundation package is downloaded from the immutable Git tag `v0.2.1` in the public [TTG-Unity-Foundation repository](https://github.com/TripleTapGames/TTG-Unity-Foundation). The [complete Foundation user guide](https://github.com/TripleTapGames/TTG-Unity-Foundation/blob/v0.2.1/Documentation~/UserGuide.md) covers installation and runtime APIs. To upgrade, change only its tag in `Packages/manifest.json`, let Unity update `packages-lock.json`, and validate the project. An Editor compilation and its existing EditMode tests do not prove live vendor behavior. Device ads, purchases, attribution, consent behavior, native Android/iOS builds, and Unity 6 compatibility require separate validation before shipping.

The original Pop Sort project is separate and has not been migrated into this project.

## Lightweight Git workflow

Clone → let packages resolve → import pinned Firebase locally → Refresh Firebase Adapter → configure the game → validate. Firebase files, generated native dependencies, local adapters and credentials are ignored; they are not included in clones or GitHub source ZIPs. Existing local Firebase files are preserved. Keep reusable installers in ignored `LocalSDKInstallers/`, outside Assets. The template opens without Firebase; enable the integration only after setup.

Before uninstalling Firebase, disable the service and use **Disable Firebase Adapter Before SDK Removal**, then wait for compilation. No shared compiler flag is needed. See the linked guide for recovery and CI instructions. Remaining imported SDKs still require their own redistribution and upload review.
