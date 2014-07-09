using System;
using System.Linq;
using NEventStore;
using NUnit.Framework;
using OrigoDB.Core;
using OrigoDB.Core.Journaling;
using OrigoDB.Core.Proxy;
using OrigoDb.Modules.NEventStore;

namespace OrigoDB.Modules.NEventStore.Test
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void SmokeTest()
        {
            var wireup = Wireup.Init()
                .LogToOutputWindow()
                .UsingInMemoryPersistence()
                //.UsingSqlPersistence("NEventStore") // Connection string is in app.config
                //.WithDialect(new MsSqlDialect())
                //.EnlistInAmbientTransaction() // two-phase commit
                .InitializeStorageEngine()
                //.TrackPerformanceInstance("example")
                .UsingJsonSerialization();
            var target = new NEventStoreCommandStore(new EngineConfiguration(), wireup);

            var appender = target.CreateAppender(0);
            appender.AppendModelCreated(typeof(Model));
            appender.Append(new ProxyCommand<Model>("a", null){Timestamp = DateTime.Now});

            var entries = target.GetJournalEntriesFrom(0).ToArray();
            CollectionAssert.AllItemsAreInstancesOfType(entries, typeof(JournalEntry));
            Assert.IsInstanceOf<JournalEntry<ModelCreated>>(entries[0]);
            CollectionAssert.AreEqual(entries.Select(e => e.Id), Enumerable.Range(0,2));
        }
    }
}
