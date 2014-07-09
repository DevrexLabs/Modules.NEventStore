using System;
using NEventStore;
using OrigoDB.Core;

namespace OrigoDb.Modules.NEventStore
{
    public class NEventStoreJournalWriter : IJournalWriter
    {
        private IEventStream _stream;


        public NEventStoreJournalWriter(IEventStream eventStream)
        {
            _stream = eventStream;
        }

        public void Close()
        {
            _stream.Dispose();
        }

        public void Write(JournalEntry item)
        {
            var @event = new EventMessage()
            {
                Body = item
            };
            _stream.Add(@event);
            _stream.CommitChanges(Guid.NewGuid());
        }

        public void Dispose()
        {
            _stream.Dispose();
        }
    }
}