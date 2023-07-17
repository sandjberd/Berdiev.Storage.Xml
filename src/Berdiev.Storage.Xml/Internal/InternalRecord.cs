//Copyright by Sandjar Berdiev

using System;
using System.Collections.Generic;

namespace Berdiev.Storage.Xml.Internal
{
    internal class InternalRecord : IRecord
    {
        private readonly List<IAttribute> _attributes;
        private readonly List<IRecord> _innerRecords;

        public InternalRecord()
        {
            _attributes = new List<IAttribute>();
            _innerRecords = new List<IRecord>();
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public IReadOnlyList<IAttribute> Attributes => _attributes;

        public IReadOnlyList<IRecord> InnerRecords => _innerRecords;

        public void AddInnerRecord(InternalRecord internalRecord)
        {
            _innerRecords.Add(internalRecord);
        }

        public void AddAttribute(IAttribute attribute)
        {
            _attributes.Add(attribute);
        }

    }
}
