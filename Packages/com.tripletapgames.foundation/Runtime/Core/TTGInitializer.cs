using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace TripleTapGames.Foundation
{
    public static class TTGInitializer
    {
        private static UniTask<TTGInitializationReport> initializationTask;
        private static bool hasTask;
        private static TTGInitializationReport lastReport;
        private static TTGServiceContext lastContext;
        private static TTGServiceRegistry lastRegistry;

        public static bool IsInitialized { get; private set; }
        public static bool IsInitializing { get; private set; }
        public static event Action OnInitialized;

        public static UniTask<TTGInitializationReport> InitializeAsync(CancellationToken cancellationToken = default)
        {
            return InitializeAsync(TTGConfigProvider.LoadProjectConfig(), TTGConfigProvider.LoadAdsConfig(), TTGServiceRegistry.Global, cancellationToken);
        }

        public static UniTask<TTGInitializationReport> InitializeAsync(
            TTGProjectConfig projectConfig,
            TTAdsConfig adsConfig,
            CancellationToken cancellationToken = default)
        {
            return InitializeAsync(projectConfig, adsConfig, TTGServiceRegistry.Global, cancellationToken);
        }

        public static UniTask<TTGInitializationReport> InitializeAsync(
            TTGProjectConfig projectConfig,
            TTAdsConfig adsConfig,
            TTGServiceRegistry registry,
            CancellationToken cancellationToken = default)
        {
            if (IsInitialized && lastReport != null) return UniTask.FromResult(lastReport);
            if (hasTask) return initializationTask;

            initializationTask = InitializeInternalAsync(projectConfig, adsConfig, registry, cancellationToken).Preserve();
            hasTask = true;
            return initializationTask;
        }

        private static async UniTask<TTGInitializationReport> InitializeInternalAsync(
            TTGProjectConfig projectConfig,
            TTAdsConfig adsConfig,
            TTGServiceRegistry registry,
            CancellationToken cancellationToken)
        {
            IsInitializing = true;
            var results = new List<TTGInitializationResult>();
            var overall = TTGInitializationStatus.Success;

            try
            {
                if (projectConfig == null)
                {
                    overall = TTGInitializationStatus.Failure;
                    results.Add(new TTGInitializationResult("Configuration", overall, "TTGProjectConfig was not found."));
                    return lastReport = new TTGInitializationReport(results, overall);
                }

                TTGLogger.DebugLoggingEnabled = projectConfig.EnableDebugLogging;
                TTGPrivacy.LoadPersistedState();
                var context = new TTGServiceContext(projectConfig, adsConfig);
                lastContext = context;
                lastRegistry = registry;

                foreach (var service in registry.Services)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (!service.IsEnabled(projectConfig))
                    {
                        results.Add(new TTGInitializationResult(service.ServiceName, TTGInitializationStatus.Skipped, "Disabled."));
                        continue;
                    }

                    if (service.RequiresConsent && !TTGPrivacy.HasResolvedConsent)
                    {
                        results.Add(new TTGInitializationResult(service.ServiceName, TTGInitializationStatus.Deferred, "Waiting for host-provided consent."));
                        overall = TTGInitializationStatus.Warning;
                        continue;
                    }

                    TTGInitializationResult result;
                    try
                    {
                        result = await service.InitializeAsync(context, cancellationToken);
                    }
                    catch (Exception exception)
                    {
                        result = new TTGInitializationResult(service.ServiceName, TTGInitializationStatus.Failure, "Initialization threw an exception.", exception);
                    }

                    results.Add(result);
                    if (result.Status == TTGInitializationStatus.Failure)
                    {
                        if (service.IsRequired(projectConfig)) overall = TTGInitializationStatus.Failure;
                        else if (overall != TTGInitializationStatus.Failure) overall = TTGInitializationStatus.Warning;
                    }
                    else if (result.Status == TTGInitializationStatus.Warning && overall == TTGInitializationStatus.Success)
                    {
                        overall = TTGInitializationStatus.Warning;
                    }
                }

                lastReport = new TTGInitializationReport(results, overall);
                IsInitialized = overall != TTGInitializationStatus.Failure;
                if (IsInitialized) OnInitialized?.Invoke();
                return lastReport;
            }
            finally
            {
                IsInitializing = false;
                if (!IsInitialized) hasTask = false;
            }
        }

        public static void Shutdown()
        {
            TTGServiceRegistry.Global.ShutdownAll();
            ResetState();
        }

        internal static async UniTask<IReadOnlyList<TTGInitializationResult>> InitializeDeferredServicesAsync(
            CancellationToken cancellationToken)
        {
            var initialized = new List<TTGInitializationResult>();
            if (lastContext == null || lastRegistry == null || !TTGPrivacy.HasResolvedConsent) return initialized;

            foreach (var service in lastRegistry.Services)
            {
                if (!service.RequiresConsent || service.IsInitialized || !service.IsEnabled(lastContext.ProjectConfig)) continue;
                try
                {
                    initialized.Add(await service.InitializeAsync(lastContext, cancellationToken));
                }
                catch (Exception exception)
                {
                    initialized.Add(new TTGInitializationResult(service.ServiceName, TTGInitializationStatus.Failure,
                        "Deferred initialization threw an exception.", exception));
                }
            }
            return initialized;
        }

        internal static void ResetState()
        {
            IsInitialized = false;
            IsInitializing = false;
            hasTask = false;
            lastReport = null;
            lastContext = null;
            lastRegistry = null;
            initializationTask = default;
        }
    }
}
