using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;

namespace TripleTapGames.Foundation.Tests
{
    public sealed class TTGInitializationTests
    {
        private TTGProjectConfig config;

        [SetUp]
        public void SetUp()
        {
            TTGInitializer.ResetState();
            TTGPrivacy.ResetForTests();
            config = ScriptableObject.CreateInstance<TTGProjectConfig>();
        }

        [TearDown]
        public void TearDown()
        {
            TTGInitializer.ResetState();
            Object.DestroyImmediate(config);
        }

        [Test]
        public void RepeatedInitializationUsesCompletedResult()
        {
            var registry = new TTGServiceRegistry();
            var service = new MockService(TTGInitializationStatus.Success, required: true);
            registry.Register(service);
            var first = TTGInitializer.InitializeAsync(config, null, registry).GetAwaiter().GetResult();
            var second = TTGInitializer.InitializeAsync(config, null, registry).GetAwaiter().GetResult();
            Assert.That(first, Is.SameAs(second));
            Assert.That(service.InitializationCount, Is.EqualTo(1));
        }

        [Test]
        public void OptionalFailureProducesWarningAndContinues()
        {
            var registry = new TTGServiceRegistry();
            registry.Register(new MockService(TTGInitializationStatus.Failure, required: false, order: 1));
            var later = new MockService(TTGInitializationStatus.Success, required: false, order: 2);
            registry.Register(later);
            var report = TTGInitializer.InitializeAsync(config, null, registry).GetAwaiter().GetResult();
            Assert.That(report.OverallStatus, Is.EqualTo(TTGInitializationStatus.Warning));
            Assert.That(later.InitializationCount, Is.EqualTo(1));
        }

        [Test]
        public void RequiredFailureFailsReport()
        {
            var registry = new TTGServiceRegistry();
            registry.Register(new MockService(TTGInitializationStatus.Failure, required: true, order: 1));
            registry.Register(new MockService(TTGInitializationStatus.Failure, required: false, order: 2));
            var report = TTGInitializer.InitializeAsync(config, null, registry).GetAwaiter().GetResult();
            Assert.That(report.OverallStatus, Is.EqualTo(TTGInitializationStatus.Failure));
        }

        private sealed class MockService : ITTGService
        {
            private readonly TTGInitializationStatus status;
            private readonly bool required;
            public string ServiceName => "Mock" + InitializationOrder;
            public int InitializationOrder { get; }
            public bool IsInitialized { get; private set; }
            public bool RequiresConsent => false;
            public int InitializationCount { get; private set; }

            public MockService(TTGInitializationStatus status, bool required, int order = 0)
            {
                this.status = status;
                this.required = required;
                InitializationOrder = order;
            }

            public bool IsEnabled(TTGProjectConfig projectConfig) => true;
            public bool IsRequired(TTGProjectConfig projectConfig) => required;
            public UniTask<TTGInitializationResult> InitializeAsync(TTGServiceContext context, CancellationToken cancellationToken)
            {
                InitializationCount++;
                IsInitialized = status == TTGInitializationStatus.Success;
                return UniTask.FromResult(new TTGInitializationResult(ServiceName, status));
            }
            public void Shutdown() => IsInitialized = false;
        }
    }
}
