//Copyright by Sandjar Berdiev

using System;
using System.Collections;
using System.Xml;

namespace Berdiev.Storage.Xml.Internal.Serializer
{
    internal class DictionarySerializer : RecordSerializerBase
    {
        public DictionarySerializer(SerializerService serializerService) : base(typeof(IDictionary), serializerService)
        {
        }

        public override bool CanSerialize(Type type)
        {
            return typeof(IDictionary).IsAssignableFrom(type);
        }

        public override XmlElement Serialize(object objectToSerialize, string elementName, XmlDocument xmlDoc)
        {
            var dictionary = (IDictionary) objectToSerialize;
            var xmlElement = xmlDoc.CreateElement(elementName);

            if (!dictionary.GetEnumerator().MoveNext())
                return xmlElement;

            var keys = new object[dictionary.Count];
            var values = new object[dictionary.Count];

            dictionary.Keys.CopyTo(keys, 0);
            dictionary.Values.CopyTo(values, 0);

            for (var i = 0; i < dictionary.Count; i++)
            {
                var keyValueElement = xmlDoc.CreateElement("KeyValues");

                var key = SerializerService.Serialize(keys[i], "Key", xmlDoc);
                var value = SerializerService.Serialize(values[i], "Value", xmlDoc);

                keyValueElement.AppendChild(key);
                keyValueElement.AppendChild(value);

                xmlElement.AppendChild(keyValueElement);
            }

            return xmlElement;

        }
    }
}
