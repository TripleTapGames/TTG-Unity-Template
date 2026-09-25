using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameAnalyticsSDK;
using UnityEngine;

namespace TripleTapGames.Foundation.Adapters.GameAnalytics
{
    internal sealed class GameAnalyticsService : ITTGService, ITTGAnalyticsProvider, ITTGConsentAdapter
    {
        public string ServiceName => "GameAnalytics";
        public string ProviderName => "GameAnalytics";
        public int InitializationOrder => 400;
        public bool IsInitialized { get; private set; }
        public bool RequiresConsent => true;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Register() => TTGServiceRegistry.Global.Register(new GameAnalyticsService());

        public bool IsEnabled(TTGProjectConfig config) => config.GameAnalytics.Enabled;
        public bool IsRequired(TTGProjectConfig config) => config.GameAnalytics.Required;

        public UniTask<TTGInitializationResult> InitializeAsync(TTGServiceContext context, CancellationToken cancellationToken)
        {
            if (IsInitialized) return UniTask.FromResult(TTGInitializationResult.Successful(ServiceName));
            var platform = context.ProjectConfig.GameAnalytics;
            var key = Application.platform == RuntimePlatform.IPhonePlayer ? platform.IosGameKey : platform.AndroidGameKey;
            var secret = Application.platform == RuntimePlatform.IPhonePlayer ? platform.IosSecretKey : platform.AndroidSecretKey;
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(secret))
                return UniTask.FromResult(new TTGInitializationResult(ServiceName, TTGInitializationStatus.Failure, "Platform credentials are missing."));
            GameAnalyticsSDK.GameAnalytics.Initialize();
            IsInitialized = true;
            TTGAnalytics.RegisterProvider(this);
            return UniTask.FromResult(TTGInitializationResult.Successful(ServiceName));
        }

        public void LogEvent(string eventName, IReadOnlyDictionary<string, object> parameters = null)
        {
            if (parameters == null) GameAnalyticsSDK.GameAnalytics.NewDesignEvent(eventName);
            else GameAnalyticsSDK.GameAnalytics.NewDesignEvent(eventName, new Dictionary<string, object>(parameters));
        }

        public UniTask ApplyConsentAsync(TTGConsentState state, CancellationToken cancellationToken)
        {
            GameAnalyticsSDK.GameAnalytics.SetEnabledEventSubmission(state.Analytics == TTGConsentStatus.Granted);
            return UniTask.CompletedTask;
        }

        public void Shutdown()
        {
            TTGAnalytics.UnregisterProvider(this);
            IsInitialized = false;
        }
    }
}
