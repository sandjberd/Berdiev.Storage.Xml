//Copyright by Sandjar Berdiev

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Berdiev.Storage.Xml.Internal.Deserializer
{
    internal abstract class RecordDeserializerBase : IRecordDeserializer
    {
        protected RecordDeserializerService DeserializerService { get; }
        private readonly Type _typeCapability;

        protected RecordDeserializerBase(Type typeCapability, RecordDeserializerService deserializerService)
        {
            DeserializerService = deserializerService;
            _typeCapability = typeCapability;
        }

        public virtual bool CanDeserialize(Type type)
        {
            return type == _typeCapability;
        }

        public abstract object Deserialize(Type type, IRecord record);

        protected bool HasChildRecords(IRecord record)
        {
            return record.InnerRecords.Any() && record.Value == null;
        }

        protected void VerifyType(Type type)
        {
            if (type != _typeCapability)
                throw new StorageXmlException($"{type} can't be deserialized!");
        }
    }
}
