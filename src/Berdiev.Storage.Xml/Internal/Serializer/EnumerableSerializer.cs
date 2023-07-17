//Copyright by Sandjar Berdiev

using System;
using System.Collections;
using System.Globalization;
using System.Xml;

namespace Berdiev.Storage.Xml.Internal.Serializer
{
    internal class EnumerableSerializer : RecordSerializerBase
    {
        public EnumerableSerializer(SerializerService serializerService) : base(typeof(IEnumerable), serializerService)
        {
        }

        public override bool CanSerialize(Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type) &&
                   !typeof(IDictionary).IsAssignableFrom(type) &&
                   type != typeof(string);
        }

        public override XmlElement Serialize(object objectToSerialize, string elementName, XmlDocument xmlDoc)
        {
            var enumerable = (IEnumerable) objectToSerialize;

            if (enumerable == null || !enumerable.GetEnumerator().MoveNext())
            {
                var emptyList = xmlDoc.CreateElement(elementName);
                return emptyList;
            }

            var list = xmlDoc.CreateElement(elementName);

            var counter = 0;
            foreach (var item in enumerable)
            {
                var xmlElement = SerializerService.Serialize(item, counter.ToString(CultureInfo.CurrentCulture), xmlDoc);

                list.AppendChild(xmlElement);

                counter++;
            }
            return list;

        }
    }
}
