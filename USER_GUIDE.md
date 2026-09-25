# TTG Unity Template — Complete User Guide

Prepared: 25 September 2026

This guide describes the template currently saved at `/Users/pavanreddy/TTG-Unity-Template`. It describes the actual files and code, not everything proposed in the original implementation plan.

### Update: Firebase is a separate local installation

The Git-ready template now excludes Firebase binaries. Existing local copies retain their SDK, but a clone/source ZIP does not contain it. Follow the [step-by-step Firebase guide](Packages/com.tripletapgames.foundation/Documentation~/FirebaseInstallation.md): clone → restore package dependencies → import Analytics and Crashlytics 13.13.0 → **Refresh Firebase Adapter** → add this game's Firebase files → enable and validate.

Keep the trusted installers in ignored `LocalSDKInstallers/` and reuse them across games. During import, deselect Assets/ExternalDependencyManager because the template already provides it through UPM. Never mix Firebase asset and UPM installation methods. Setup now distinguishes required/detected Firebase versions and refuses unsupported versions rather than relying only on DLL existence.

Firebase's adapter is generated under ignored Assets/TTGGenerated/FirebaseAdapter; it does not use a committed PlayerSettings compiler flag. **Apply Configuration** alone does not generate it: use **Refresh Firebase Adapter**, including after a foundation update. Before SDK removal, disable the service, click **Disable Firebase Adapter Before SDK Removal**, and wait for compilation. The button only removes reproducible adapter code, not the SDK or credentials.

Missing Firebase is non-blocking when disabled; enabled Firebase requires both SDK products, a current adapter and the active platform's config file. This preparation does not establish redistribution permission for other imported SDKs or publish anything to GitHub.

## 1. What this template is

Think of this project as a reusable starting folder for a new game. The SDK files and package dependencies are already selected, and a shared TTG Foundation package provides APIs for initialization, analytics, advertising and purchases.

You still need to supply each game's settings, connect startup, implement consent UI and gameplay, and test real devices. Installing SDKs does not create vendor accounts, configure dashboards, or activate services.

The original Pop Sort game has not been copied into this template. Its gameplay and existing integration remain separate.

### Current status

| Item | What is available |
| --- | --- |
| Unity project | Empty starting scene, using Unity 2022.3.62f3 |
| SDK dependencies | Installed asset SDKs and pinned package dependencies |
| TTG Foundation | Embedded package, version 0.1.0 |
| Configuration | Blank local project and advertising assets |
| Initialization | `TTGInitializer` and an importable bootstrap example |
| `TTManager.cs` | Not included |
| Automatic initialization in Main | Not connected |
| Editor checks | Compilation passed and 10 existing EditMode tests passed in the staging copy used to create this template |
| Release readiness | Not established; native builds and live vendor behavior need testing |
| Unity 6 | Compatibility has not been verified |

The test result is a starting check, not proof that the entire original foundation plan has been completed.

## 2. Installed SDKs

| SDK | Installed version/source | Purpose |
| --- | --- | --- |
| UniTask | 2.5.11, pinned Git tag | Asynchronous startup and operations |
| Firebase Analytics/Crashlytics | 13.13.0, separate ignored local asset import | Analytics and crash reporting |
| AppLovin MAX | 8.6.6, UPM | Advertising |
| GameAnalytics | 8.2.0, OpenUPM | Game analytics |
| Singular | 5.10.1, official Git tag | Attribution |
| Unity IAP | 4.14.0, Unity registry | Store purchases |
| Facebook | Existing 17.x asset import | Facebook integration |
| DOTween | Existing Demigiant asset import; exact version not independently verified | Animation/tweening utility |
| External Dependency Manager | 1.2.189, OpenUPM | Native dependency resolution |

Facebook has not been upgraded to the originally proposed 18.1.0. Do not assume DOTween matches the proposed 1.3.030 pin.

MAX mediation-network adapters are not included. Add only the networks selected for each game. Foundation runtime code does not depend on DOTween.

## 3. Open the template for the first time

1. Open Unity Hub.
2. Choose **Add project from disk** and select `/Users/pavanreddy/TTG-Unity-Template`.
3. Open it with **Unity 2022.3.62f3**.
4. Wait for package downloads, imports and compilation to finish. A fresh computer needs internet access and Git for Git-based dependencies.
5. Open `Assets/Scenes/Main.unity`.
6. Check the Console for red errors before making changes.

The scene contains a camera and directional light. Pressing Play does not currently call the TTG initializer. That is expected, not a failure.

Firebase is intentionally absent from a fresh clone: use the guided pinned import above if needed. Do not copy SDK folders from another game just because no service starts. Duplicate SDK installations can cause compilation and native build conflicts.

## 4. Start a new game from the template

Keep one clean master copy. Develop each game in its own copy.

1. Close the template in Unity.
2. Copy the folder and give the copy the new game's name.
3. Keep `Assets`, `Packages`, `ProjectSettings`, `.gitignore` and the documentation. Preserve Unity `.meta` files.
4. You do not need to copy `Library`, `Temp`, `Logs`, `obj` or `UserSettings`; Unity recreates these.
5. Add the copied project to Unity Hub and open it.
6. In **Edit > Project Settings > Player**, change Company Name, Product Name, and the Android/iOS application identifiers.
7. Set the game's icons, orientation and other player settings.
8. Create vendor dashboard entries and store products for this game, then enter its own configuration.

Do not leave the placeholder identifier `com.tripletapgames.template` on a release game. Do not copy another game's populated config or Firebase files.

## 5. Understand the folders

| Location | Purpose |
| --- | --- |
| `Assets/Scenes/Main.unity` | Empty starting scene |
| `Assets/Firebase` | Firebase SDK files, not the game's Firebase configuration |
| `Assets/FacebookSDK` | Facebook SDK files |
| `Assets/Plugins/Demigiant` | DOTween files |
| `Assets/TTGGenerated/Resources/TTG` | Local TTG configuration assets; ignored by Git |
| `Assets/Editor/TTGTemplateSetup.cs` | Helper used to create the starter scene/settings; not a runtime manager |
| `Packages/com.tripletapgames.foundation` | Editable embedded foundation package |
| `Packages/manifest.json` | Dependency declarations and registries |
| `Packages/packages-lock.json` | Resolved package versions |
| `ProjectSettings` | Unity project settings |

Do not manually run `TTGTemplateSetup.Create` on a developed game: it resets template identifiers, recreates Main, changes build scenes and writes scripting defines.

## 6. Use the Project Setup window

Open **Tools > Triple Tap Games > Project Setup**.

| Button | What it does |
| --- | --- |
| Install Missing Packages | Starts installation of missing supported package dependencies |
| Create / Locate TTG Project Config | Creates or selects the main settings asset |
| Create / Locate TTG Ads Config | Creates or selects the advertising settings asset |
| Import Values From Environment | Reads the supported process environment variables listed later |
| Apply Configuration | Applies the available vendor-setting edits and adapter compilation defines for the active target |
| Validate Project | Displays findings for the active build target |
| Open Documentation | Currently opens the Triple Tap Games GitHub page, not this guide |

Important: the dependency display uses catalog versions and installation detection. A checkmark is not proof that an asset-imported SDK matches that displayed version. Also, **Apply Configuration is not a complete vendor setup wizard**: some edits only work if the vendor's settings asset already exists at the expected location.

The template already contains its dependency setup. Do not repeatedly import asset SDKs or use installation buttons to force an upgrade.

## 7. Configure services

Select `TTGProjectConfig.asset` from the setup window. Its Inspector contains the service sections.

### Enabled and Required

- **Enabled off:** the initializer skips that service.
- **Enabled on, Required off:** it attempts startup; an optional failure is reported as a warning.
- **Enabled on, Required on:** a service failure makes the overall initialization report fail.

All integrations start disabled and optional. Enable one at a time so problems are easier to identify. Making a service optional does not impose a timeout on a stalled SDK call.

### Environment and logging

The asset has Development, Staging and Production selections. This is not an automatic switch between three credential sets. Populate or supply the correct configuration for the chosen environment yourself.

Keep debug logging limited during development and review vendor logging separately. Never print complete configuration assets or credential values.

### Per-service setup

| Service | TTG fields | Additional work |
| --- | --- | --- |
| Facebook | App ID, client token | Create/review Facebook's own settings; configure the game's platform entries |
| Firebase | Analytics and Crashlytics toggles | Add this game's Android/iOS Firebase files and review native dependency resolution |
| GameAnalytics | Separate Android/iOS game and secret keys | Configure the vendor's settings too: the current adapter checks TTG keys but does not copy them into GameAnalytics settings |
| AppLovin | SDK key, privacy policy URL | Review MAX Integration Manager settings, ad units and chosen mediation networks |
| Singular | API key and API secret | Review attribution/platform setup; not all additional TTG Singular fields are wired into the runtime adapter |
| IAP | Product IDs and product types | Configure matching products in the stores and implement game entitlement handling |

Use `Assets/google-services.json` for Android Firebase configuration and `Assets/GoogleService-Info.plist` for iOS. These must belong to the new game and match its platform identifiers. Do not mix Firebase asset-import and UPM installation methods.

The default privacy-policy URL in TTG config is a placeholder for your project to review. Its presence does not configure consent UI or satisfy a release review.

## 8. Connect initialization to the scene

There is no `TTManager` component in this template. The equivalent central startup API is the static class `TTGInitializer`.

For a basic development bootstrap:

1. Open Unity's Package Manager and select **TTG Foundation**.
2. Import its **Bootstrap Example** sample.
3. In Main, create an empty GameObject named `TTG Bootstrap`.
4. Add the imported `TTGBootstrapExample` component to it.
5. Save the scene.
6. Press Play and inspect the Console.

The sample calls:

```csharp
var report = await TTGInitializer.InitializeAsync(destroyCancellationToken);
```

It loads the local Resources configs, checks enabled services, and returns an initialization report. Its destruction token is tied to the component, so keep the bootstrap scene/object alive while startup is in progress. The sample is deliberately minimal: it is not a persistent manager, loading screen, consent UI or complete error-recovery flow.

For your own startup code, import `TripleTapGames.Foundation`. The initializer exposes `IsInitializing`, `IsInitialized` and `OnInitialized`. Use the returned per-service results, not just one Boolean, when deciding whether a feature is ready. Warnings and deferred services can coexist with an overall non-failure result.

Successful initialization is reused, and concurrent callers share the stored initialization task. Scene reload, shutdown/retry and disabled-domain-reload workflows still need testing; do not treat these as a complete production lifecycle system.

You can also supply explicit assets:

```csharp
var report = await TTGInitializer.InitializeAsync(
    projectConfig, adsConfig, cancellationToken);
```

Here `projectConfig` is a `TTGProjectConfig`, `adsConfig` is a `TTAdsConfig`, and `cancellationToken` is supplied by your calling code.

## 9. Handle privacy and consent

The game or its consent-management platform must display the UI and obtain the player's actual choices. TTG stores normalized states; it does not supply the UI or request Apple's tracking permission for you.

The normalized fields are Analytics, Advertising, AdPersonalization, AgeRestricted and Tracking. Do not guess an age-state mapping or force Granted/NotApplicable just to make an SDK start.

The intended flow is:

1. Start foundation initialization and inspect its report.
2. Show your consent UI if choices are unresolved.
3. Map the actual choices and platform tracking result into a `TTGConsentState`.
4. Call `await TTGPrivacy.SetConsentAsync(actualConsentState)`.
5. Inspect the returned results for newly attempted services.

States are persisted through PlayerPrefs. In the current code, “resolved” only checks that Analytics and Advertising are not Unknown. It does not mean that all privacy questions are answered or that consent was granted. The function applies adapter flags and attempts eligible deferred services; deny, revoke and age-restricted paths need device-level verification before release.

Do not assume a successful initializer report proves consent compliance or that no vendor auto-initialization occurred. Audit each SDK's own startup and collection settings.

## 10. Send analytics events

After configuration, consent handling and provider initialization, gameplay can use these APIs:

```csharp
using TripleTapGames.Foundation;

// Inside your gameplay methods:
TTGAnalytics.LevelStarted(1);
TTGAnalytics.LevelCompleted(1);
TTGAnalytics.LevelFailed(1);
TTGAnalytics.LogEvent("tutorial_completed");
```

Use a consistent level identifier. Do not send the same event through both TTG and a direct SDK call unless duplication is intentional.

Events go to registered, initialized analytics providers. The current facade does not queue events sent before a provider is ready. An installed attribution SDK is not automatically a general analytics destination. Verify delivery in each intended vendor dashboard; a quiet Console is not proof of delivery.

## 11. Configure and show ads

Select `TTAdsConfig.asset` and configure:

- Ads Enabled, plus AppLovin Enabled in the project config.
- Separate Android and iOS ad-unit IDs.
- Required ad formats: interstitial, rewarded, banner and/or MREC.
- Minimum session time, interstitial level threshold and cooldown.

The AppOpenId, TestMode and other exposed settings must not be assumed to activate corresponding vendor behavior. There is no public App Open operation in the current facade. Set up test advertising through verified vendor controls before testing; do not click live ads.

Examples below belong inside your gameplay methods:

```csharp
TTGAds.NotifyLevelCompleted();
TTGAds.ShowInterstitial(TTGAdPlacement.LevelComplete, result =>
{
    // Continue gameplay regardless of whether an ad was available.
});

TTGAds.ShowRewarded(TTGAdPlacement.ExtraCoins,
    onRewarded: () =>
    {
        // Grant the player's reward here, not in onClosed.
    },
    onClosed: result =>
    {
        // Restore the normal game UI here.
    });

TTGAds.ShowBanner(TTGBannerPosition.Bottom);
TTGAds.HideBanner();
TTGAds.ShowMrec();
TTGAds.HideMrec();
```

Check `IsInterstitialReady` or `IsRewardedReady` for UI availability, but still handle the operation's result because readiness can change.

Important current limitation: `LevelInterval` is implemented as a minimum completed-level threshold. It is not reset after each ad, so it does **not** currently mean “show every N levels.” Add and test recurring-level logic before relying on that behavior. The session timer begins when the ads provider is configured.

Handle no-fill, offline use, failure and cancellation without blocking gameplay. Callback guards exist, but provider exceptions and callback ordering still require real-device testing.

## 12. Configure purchases

In the project config, enable IAP and add each exact store product ID and type:

- Consumable: an item that can be bought repeatedly.
- NonConsumable: a lasting entitlement.
- Subscription: a subscription product requiring additional entitlement lifecycle handling.

A minimal call is:

```csharp
TTGIAP.Purchase("your_product_id", result =>
{
    if (result.Status == TTGPurchaseStatus.Succeeded)
    {
        // Fulfil and persist the entitlement safely.
    }
    else
    {
        // Handle cancellation, unavailable product, or failure.
    }
});
```

Disable repeated taps while a purchase is pending. The current adapter stores callbacks by product ID; overlapping purchases of the same ID are not a supported robust flow.

Success is reported from Unity IAP's purchase callback, not immediately from the button press. Backend receipt validation is not implemented. Purchase analytics are sent by the TTG facade on reported success; do not duplicate them unintentionally.

`TTGIAP.RestorePurchases(callback)` exists, but is not a complete entitlement restoration system. Its Boolean does not identify restored products, and on non-Apple targets the current adapter simply returns true. Restored/replayed purchases without a pending request are not delivered to your gameplay through a general entitlement event. Resolve these gaps before shipping non-consumables or subscriptions.

## 13. Environment import and automation

The Project Setup window can import the following process environment variables:

| Variables | Destination |
| --- | --- |
| `TTG_FACEBOOK_APP_ID`, `TTG_FACEBOOK_CLIENT_TOKEN` | Facebook fields |
| `TTG_APPLOVIN_SDK_KEY` | MAX key |
| `TTG_SINGULAR_API_KEY`, `TTG_SINGULAR_API_SECRET` | Singular fields |
| `TTG_GA_ANDROID_GAME_KEY`, `TTG_GA_ANDROID_SECRET` | Android GameAnalytics fields |
| `TTG_GA_IOS_GAME_KEY`, `TTG_GA_IOS_SECRET` | iOS GameAnalytics fields |
| `TTG_FIREBASE_ANDROID_CONFIG_PATH` | File copied to `Assets/google-services.json` |
| `TTG_FIREBASE_IOS_CONFIG_PATH` | File copied to `Assets/GoogleService-Info.plist` |

Unity must inherit these variables when it starts. A running Unity/Hub process does not automatically receive variables later set in another terminal.

Missing or empty credential variables keep existing field values. Existing Firebase destination files are overwritten when a valid source is supplied. Review the game/environment before importing.

This importer does not populate ad IDs, product lists, service enable flags or the Environment selection. There is no complete public CI generation command supplied by this template. CI needs its own reviewed entry point and secure variable/file injection.

## 14. Validate and build

1. Install the needed Android/iOS build modules for the selected Unity editor through Unity Hub.
2. Select the intended platform in Build Settings.
3. Recheck identifiers and platform-specific configs.
4. Run Apply Configuration and Validate Project for that active target.
5. Fix relevant errors and investigate warnings.
6. Run EditMode tests using Unity's Test Runner.
7. Export/build for a real device and inspect native dependency resolution.
8. Test startup, denial/revocation of consent, offline use, ads and store sandbox flows.

The Android processor edits generated manifests to set `android:debuggable="false"` and `android:allowBackup="false"`. It does not replace your custom manifest. Inspect the final merged application manifest because later native merging/build steps can affect the outcome.

The iOS processor currently supplies contributor infrastructure, not a complete built-in collection of platform settings. Do not assume it adds all frameworks, URL schemes, tracking descriptions or SKAdNetwork data. Review vendor processors and the generated Xcode project.

Validation is a useful check, not a release certification. The template has not yet passed the original plan's complete Android/iOS/Unity 6 matrix.

## 15. Keep configuration out of source control

The supplied `.gitignore` excludes generated TTG settings and Firebase game configuration. Commit reusable code, scenes, package manifests, lockfiles and appropriate Unity project settings.

Before every commit, review changed files. Vendor tools can generate settings outside the ignored folder; do not assume all vendor-generated assets are safe. An ignore rule does not untrack a file that is already committed.

On another machine, recreate the ignored TTG assets from Project Setup and provision that game's configuration. A clone without ignored assets will not contain the populated config.

Runtime SDK identifiers and keys embedded in a client build can be extracted; they are not server secrets. Keep privileged backend credentials out of Unity assets, code, logs and builds.

## 16. Common problems

| Symptom | What to check |
| --- | --- |
| Nothing initializes in Play mode | Import and attach the bootstrap sample; Main has no startup component by default |
| Config not found | Create both configs and check the exact `Resources/TTG` paths/names |
| Services are skipped | Check each service's Enabled flag |
| Services remain deferred | Review actual consent state and the results from SetConsentAsync |
| Ads never become ready | Check platform IDs, service/format switches, vendor setup, network availability and consent |
| Interstitial is blocked | Check session duration, cooldown and completed-level threshold |
| Analytics events are absent | Verify provider initialization and vendor settings; early events are not queued |
| Purchase says NotInitialized | Wait for IAP startup and verify its products/store setup |
| Restore reports success but nothing unlocks | Implement and test an entitlement restoration path; the current Boolean is insufficient |
| Duplicate types/plugins | Check for mixed asset/UPM installs, especially GameAnalytics and Firebase |
| Package installation fails offline | Retry with network access and inspect Package Manager errors; do not import duplicate SDKs |
| Config disappears after cloning | It is intentionally ignored; recreate and provision it |
| Setup says a version is installed but it differs | Verify actual imported SDK metadata; catalog labels are not strict version verification |

Do not delete SDK folders or overwrite package settings as a first troubleshooting step. Record the specific error and isolate the integration responsible.

## 17. Upgrade safely

Keep the template's dependency versions and lockfile together. For an upgrade, use a separate copy or branch, update the dependency catalog and manifest/import consistently, and rerun compilation, tests and mobile builds. Package-only upgrades do not upgrade asset-imported Firebase, Facebook or DOTween.

Do not upgrade the master template in the middle of a game's release without validating the resulting project. Record the known-good Unity and SDK versions for each game.

## 18. New-game and release checklists

### Before gameplay integration

- [ ] Work in a copy, not the clean master.
- [ ] Use Unity 2022.3.62f3 and resolve compilation errors.
- [ ] Set this game's name and platform identifiers.
- [ ] Create/provision its own configs and vendor entries.
- [ ] Connect one deliberate bootstrap path.
- [ ] Implement consent UI and inspect per-service startup results.
- [ ] Enable and test services one at a time.

### Before release

- [ ] Review the known limitations in initialization, consent, ads and IAP above.
- [ ] Validate every supported target and inspect generated native settings.
- [ ] Verify real-device analytics and crash reporting.
- [ ] Test ad no-fill, failure, rewards and transitions.
- [ ] Test purchase cancellation, confirmation, duplicate prevention, replay and restore.
- [ ] Verify denied/revoked consent and platform tracking behavior.
- [ ] Audit credentials, logs and source-control changes.
- [ ] Validate release identifiers, signing and vendor configurations.
- [ ] Do not claim Unity 6 or untested platform compatibility until tested.

The next practical step is to copy the template for a small test game, connect the bootstrap example, and configure one service end-to-end before adding the others.
