//Copyright by Sandjar Berdiev

using System;
using System.Xml;

namespace Berdiev.Storage.Xml.Internal.Serializer
{
    internal abstract class RecordSerializerBase : IRecordSerializer
    {
        protected SerializerService SerializerService { get; }
        private readonly Type _typeCapability;

        protected RecordSerializerBase(Type typeCapability, SerializerService serializerService)
        {
            SerializerService = serializerService;
            _typeCapability = typeCapability;
        }


        public virtual bool CanSerialize(Type type)
        {
            return type == _typeCapability;
        }

        public abstract XmlElement Serialize(object objectToSerialize, string elementName, XmlDocument xmlDoc);

        protected void VerifyType(object objectToSerialize)
        {
            if (objectToSerialize.GetType() != _typeCapability)
                throw new StorageXmlException($"{objectToSerialize.GetType()} can't be serialized!");
        }
    }
}
