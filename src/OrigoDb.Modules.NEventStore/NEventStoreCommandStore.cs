using System;
using System.Collections.Generic;
using System.IO;
using NEventStore;
using OrigoDB.Core;
using OrigoDB.Core.Storage;

namespace OrigoDb.Modules.NEventStore
{
    public class NEventStoreCommandStore : CommandStore
    {
        private const string StreamId = "j";
        private const string BucketId = "b";
        readonly IStoreEvents _eventStore;

        public NEventStoreCommandStore(EngineConfiguration config, Wireup wireup) : base(config)
        {
            _eventStore = wireup.Build();
        }

        protected override IJournalWriter CreateStoreSpecificJournalWriter()
        {
            return new NEventStoreJournalWriter(_eventStore.OpenStream(BucketId,StreamId,0,0));

        }

        public override IEnumerable<JournalEntry> GetJournalEntriesFrom(ulong entryId)
        {
            if (entryId > Int32.MaxValue) throw new ArgumentOutOfRangeException("entryId", "NEventStore storage uses Int32");
            

            foreach (var commit in _eventStore.Advanced.GetFrom(BucketId, StreamId, (int) entryId, Int32.MaxValue))
                foreach (var @event in commit.Events)
                    yield return (JournalEntry) @event.Body;
            
        }

        public override IEnumerable<JournalEntry> GetJournalEntriesBeforeOrAt(DateTime pointInTime)
        {
            foreach (ICommit commit in _eventStore.Advanced.GetFrom(BucketId, pointInTime))
            {
                if (commit.StreamId != StreamId)
                    throw new InvalidDataException("Unexpected StreamId: " + commit.StreamId);

                foreach (var @event in commit.Events)
                    yield return (JournalEntry) @event.Body;
            }
        }

        public override Stream CreateJournalWriterStream(ulong firstEntryId = 1)
        {
            throw new NotImplementedException();
        }
    }
}
