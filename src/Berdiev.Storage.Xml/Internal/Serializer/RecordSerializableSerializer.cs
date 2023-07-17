//Copyright by Sandjar Berdiev

using System;
using System.Reflection;
using System.Xml;

namespace Berdiev.Storage.Xml.Internal.Serializer
{
    internal class RecordSerializableSerializer : RecordSerializerBase
    {
        public RecordSerializableSerializer(SerializerService serializerService) : base(typeof(IRecordSerializable), serializerService)
        {
        }

        public override bool CanSerialize(Type type)
        {
            return typeof(IRecordSerializable).IsAssignableFrom(type);
        }

        public override XmlElement Serialize(object objectToSerialize, string elementName, XmlDocument xmlDoc)
        {
            var type = objectToSerialize.GetType();

            var rootElement = xmlDoc.CreateElement(elementName);

            var properties = type.GetProperties();

            foreach (var propertyInfo in properties)
            {
                var xmlElement = _CreateXmlElementOfProperty(propertyInfo, xmlDoc, objectToSerialize);

                rootElement.AppendChild(xmlElement);
            }

            return rootElement;
        }

        private XmlElement _CreateXmlElementOfProperty(PropertyInfo propertyInfo, XmlDocument xmlDoc, object rootObject)
        {
            var propertyObject = propertyInfo.GetValue(rootObject);

            var canSerialize = SerializerService.CanSerialize(propertyInfo.PropertyType);

            if (!canSerialize)
                throw new StorageXmlException($"Can not serialize property '{propertyInfo.Name}' of type '{propertyInfo.PropertyType}' !");

            if (propertyObject == null)
            {
                var xmlElement = xmlDoc.CreateElement(propertyInfo.Name);
                return xmlElement;
            }

            return SerializerService.Serialize(propertyObject, propertyInfo.Name, xmlDoc);
        }
    }
}
