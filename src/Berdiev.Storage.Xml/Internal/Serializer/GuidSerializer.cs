//Copyright by Sandjar Berdiev

using System;
using System.Xml;

namespace Berdiev.Storage.Xml.Internal.Serializer
{
    internal class GuidSerializer : RecordSerializerBase
    {
        public GuidSerializer(SerializerService serializerService) : base(typeof(Guid), serializerService)
        {
        }

        public override XmlElement Serialize(object objectToSerialize, string elementName, XmlDocument xmlDoc)
        {
            VerifyType(objectToSerialize);

            var guid = (Guid) objectToSerialize;

            var xmlElement = xmlDoc.CreateElement(elementName);

            xmlElement.InnerText = guid.ToString();

            return xmlElement;
        }
    }
}
