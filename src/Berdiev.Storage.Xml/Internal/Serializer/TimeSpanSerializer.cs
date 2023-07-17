//Copyright by Sandjar Berdiev

using System;
using System.Globalization;
using System.Xml;

namespace Berdiev.Storage.Xml.Internal.Serializer
{
    internal class TimeSpanSerializer : RecordSerializerBase
    {
        public TimeSpanSerializer(SerializerService serializerService) : base(typeof(TimeSpan), serializerService)
        {
        }

        public override XmlElement Serialize(object objectToSerialize, string elementName, XmlDocument xmlDoc)
        {
            VerifyType(objectToSerialize);

            var timeSpan = (TimeSpan) objectToSerialize;

            var xmlElement = xmlDoc.CreateElement(elementName);

            xmlElement.InnerText = timeSpan.Ticks.ToString(CultureInfo.InvariantCulture);

            return xmlElement;
        }
    }
}
