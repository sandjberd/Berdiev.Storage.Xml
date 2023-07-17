//Copyright by Sandjar Berdiev

using System;
using System.Globalization;
using System.Xml;

namespace Berdiev.Storage.Xml.Internal.Serializer
{
    internal class DateTimeSerializer : RecordSerializerBase
    {
        public DateTimeSerializer(SerializerService serializerService) : base(typeof(DateTime), serializerService)
        {
        }

        public override XmlElement Serialize(object objectToSerialize, string elementName, XmlDocument xmlDoc)
        {
            VerifyType(objectToSerialize);

            var dateTime = (DateTime) objectToSerialize;

            var xmlElement = xmlDoc.CreateElement(elementName);

            xmlElement.InnerText = dateTime.ToFileTimeUtc().ToString(CultureInfo.InvariantCulture);

            return xmlElement;
        }
    }
}
