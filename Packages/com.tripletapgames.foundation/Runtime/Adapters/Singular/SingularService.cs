using System.Threading;
using Cysharp.Threading.Tasks;
using Singular;
using UnityEngine;

namespace TripleTapGames.Foundation.Adapters.Singular
{
    internal sealed class SingularService : ITTGService, ITTGConsentAdapter
    {
        private GameObject sdkObject;
        public string ServiceName => "Singular";
        public int InitializationOrder => 450;
        public bool IsInitialized { get; private set; }
        public bool RequiresConsent => true;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Register() => TTGServiceRegistry.Global.Register(new SingularService());

        public bool IsEnabled(TTGProjectConfig config) => config.Singular.Enabled;
        public bool IsRequired(TTGProjectConfig config) => config.Singular.Required;

        public UniTask<TTGInitializationResult> InitializeAsync(TTGServiceContext context, CancellationToken cancellationToken)
        {
            if (IsInitialized) return UniTask.FromResult(TTGInitializationResult.Successful(ServiceName));
            if (string.IsNullOrWhiteSpace(context.ProjectConfig.Singular.ApiKey) || string.IsNullOrWhiteSpace(context.ProjectConfig.Singular.ApiSecret))
                return UniTask.FromResult(new TTGInitializationResult(ServiceName, TTGInitializationStatus.Failure, "Singular SDK credentials are missing."));

            sdkObject = GameObject.Find("SingularSDKObject");
            if (sdkObject == null)
            {
                sdkObject = new GameObject("SingularSDKObject");
                sdkObject.SetActive(false);
                var sdk = sdkObject.AddComponent<SingularSDK>();
                sdk.InitializeOnAwake = false;
                sdk.SingularAPIKey = context.ProjectConfig.Singular.ApiKey;
                sdk.SingularAPISecret = context.ProjectConfig.Singular.ApiSecret;
                sdk.enableLogging = false; // Vendor debug logging includes credential values.
                sdkObject.SetActive(true);
                Object.DontDestroyOnLoad(sdkObject);
            }
            SingularSDK.InitializeSingularSDK();
            IsInitialized = true;
            return UniTask.FromResult(TTGInitializationResult.Successful(ServiceName));
        }

        public UniTask ApplyConsentAsync(TTGConsentState state, CancellationToken cancellationToken)
        {
            if (state.Analytics == TTGConsentStatus.Denied) SingularSDK.StopAllTracking();
            else if (state.Analytics == TTGConsentStatus.Granted) SingularSDK.ResumeAllTracking();
            return UniTask.CompletedTask;
        }

        public void Shutdown() => IsInitialized = false;
    }
}
