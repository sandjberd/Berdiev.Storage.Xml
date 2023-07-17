//Copyright by Sandjar Berdiev

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Berdiev.Storage.Xml.Tests
{
    public class DummySerialization : IRecordSerializable
    {
        public String Name { get; set; }

        public bool IsCool { get; set; }

        public Version Version { get; set; }

        public DateTime Date { get; set; }

        public TimeSpan TimeSpan { get; set; }

        public Guid Id { get; set; }

        public Guid SubId { get; set; }

        public IReadOnlyList<DummySubSerialization> Children { get; set; }

        public IReadOnlyList<List<int>> Inception { get; set; }
    }
}
