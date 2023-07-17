//Copyright by Sandjar Berdiev

using System;
using System.Xml;

namespace Berdiev.Storage.Xml.Internal.Serializer
{
    internal class VersionSerializer : RecordSerializerBase
    {
        public VersionSerializer(SerializerService serializerService) : base(typeof(Version), serializerService)
        {
        }

        public override XmlElement Serialize(object objectToSerialize, string elementName, XmlDocument xmlDoc)
        {
            VerifyType(objectToSerialize);

            var version = (Version) objectToSerialize;

            var xmlElement = xmlDoc.CreateElement(elementName);

            xmlElement.InnerText = version.ToString();

            return xmlElement;
        }
    }
}
