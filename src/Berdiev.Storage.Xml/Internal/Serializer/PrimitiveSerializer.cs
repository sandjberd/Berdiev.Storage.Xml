//Copyright by Sandjar Berdiev

using System;
using System.Xml;

namespace Berdiev.Storage.Xml.Internal.Serializer
{
    internal class PrimitiveSerializer : RecordSerializerBase
    {
        public PrimitiveSerializer(SerializerService serializerService) : base(typeof(object), serializerService)
        {
        }

        public override bool CanSerialize(Type type)
        {
            return type.IsPrimitive || typeof(String) == type;
        }

        public override XmlElement Serialize(object objectToSerialize, string elementName, XmlDocument xmlDoc)
        {
            var xmlElement = xmlDoc.CreateElement(elementName);

            xmlElement.InnerText = objectToSerialize.ToString();

            return xmlElement;
        }
    }
}
