//Copyright by Sandjar Berdiev

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Berdiev.Storage.Xml.Internal.Serializer
{
    internal class SerializerService
    {
        private List<IRecordSerializer> _serializers;
        private static SerializerService _instance;

        private SerializerService()
        {
            
        }

        public static SerializerService Create()
        {
            if (_instance == null)
                _instance = new SerializerService();

            return _instance;
        }

        public void Register()
        {
            if (_serializers != null && _serializers.Any())
                return;

            _serializers = new List<IRecordSerializer>
            {
                new PrimitiveSerializer(this),
                new DateTimeSerializer(this),
                new GuidSerializer(this),
                new TimeSpanSerializer(this),
                new VersionSerializer(this),
                new RecordSerializableSerializer(this),
                new EnumerableSerializer(this),
                new DictionarySerializer(this)
            };
        }

        public bool CanSerialize(Type typeToSerialize)
        {
            foreach (var recordSerializer in _serializers)
            {
                if (recordSerializer.CanSerialize(typeToSerialize))
                    return true;
            }

            return false;
        }

        public XmlElement Serialize(object objectToSerialize, string name, XmlDocument xmlDoc)
        {
            var serializer = _FindSerializer(objectToSerialize.GetType());

            var xmlElement = serializer.Serialize(objectToSerialize, name, xmlDoc);

            return xmlElement;
        }

        private IRecordSerializer _FindSerializer(Type typeToSerialize)
        {
            var serializer = _serializers.FirstOrDefault(recordSerializer => recordSerializer.CanSerialize(typeToSerialize));

            if (serializer == null)
                throw new StorageXmlException($"Can not serialize {typeToSerialize} !");

            return serializer;
        }
    }
}
