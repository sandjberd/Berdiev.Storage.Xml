//Copyright by Sandjar Berdiev


using System;
using System.Collections.Generic;

namespace Berdiev.Storage.Xml.Tests
{
    public class DummySubSerialization : IRecordSerializable
    {
        public Dictionary<Guid, String> MyDictionary { get; set; }

        public Dictionary<Version, DateTime> NuDict { get; set; }

        public List<int> Numbers { get; set; }
    }
}
