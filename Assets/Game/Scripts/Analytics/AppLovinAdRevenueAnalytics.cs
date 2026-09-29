using System.Collections.Concurrent;
using TripleTapGames.Foundation;
using UnityEngine;

namespace TripleTapGames.Game.Analytics
{
    /// <summary>
    /// Game-owned bridge from AppLovin MAX paid-impression callbacks to TTG analytics.
    /// MAX invokes revenue callbacks on a background thread, so impressions are queued
    /// and forwarded to analytics providers from Unity's main thread.
    /// </summary>
    [DefaultExecutionOrder(-10000)]
    public sealed class AppLovinAdRevenueAnalytics : MonoBehaviour
    {
        private static AppLovinAdRevenueAnalytics instance;
        private readonly ConcurrentQueue<TTGAdImpression> pendingImpressions =
            new ConcurrentQueue<TTGAdImpression>();
        private bool subscribed;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void EnsureInstance()
        {
            if (instance != null) return;
            var existing = FindObjectOfType<AppLovinAdRevenueAnalytics>(true);
            if (existing != null)
            {
                instance = existing;
                return;
            }

            var bridge = new GameObject("[TTG] AppLovin Ad Revenue Analytics");
            instance = bridge.AddComponent<AppLovinAdRevenueAnalytics>();
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            if (subscribed) return;
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnAdRevenuePaid;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnAdRevenuePaid;
            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnAdRevenuePaid;
            MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent += OnAdRevenuePaid;
            MaxSdkCallbacks.AppOpen.OnAdRevenuePaidEvent += OnAdRevenuePaid;
            subscribed = true;
        }

        private void OnDisable()
        {
            if (!subscribed) return;
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent -= OnAdRevenuePaid;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent -= OnAdRevenuePaid;
            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent -= OnAdRevenuePaid;
            MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent -= OnAdRevenuePaid;
            MaxSdkCallbacks.AppOpen.OnAdRevenuePaidEvent -= OnAdRevenuePaid;
            subscribed = false;
        }

        private void Update()
        {
            while (pendingImpressions.TryDequeue(out var impression))
                TTGAnalytics.AdImpression(impression);
        }

        private void OnAdRevenuePaid(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (adInfo == null) return;
            pendingImpressions.Enqueue(new TTGAdImpression
            {
                AdSource = "AppLovin",
                NetworkName = adInfo.NetworkName,
                AdFormat = adInfo.AdFormat,
                Placement = adInfo.Placement,
                AdUnitId = string.IsNullOrWhiteSpace(adUnitId) ? adInfo.AdUnitIdentifier : adUnitId,
                Currency = "USD",
                Revenue = adInfo.Revenue
            });
        }
    }
}
