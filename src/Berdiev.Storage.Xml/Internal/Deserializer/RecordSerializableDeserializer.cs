//Copyright by Sandjar Berdiev

using System;
using System.Linq;
using System.Reflection;

namespace Berdiev.Storage.Xml.Internal.Deserializer
{
    internal class RecordSerializableDeserializer : RecordDeserializerBase
    {
        public RecordSerializableDeserializer(RecordDeserializerService deserializerService) : base(typeof(IRecordSerializable), deserializerService)
        {
        }

        public override bool CanDeserialize(Type type)
        {
            return typeof(IRecordSerializable).IsAssignableFrom(type);
        }

        public override object Deserialize(Type type, IRecord record)
        {
            var sameProperties = _CheckIfContainsProperties(record, type);

            if (!sameProperties)
                throw new StorageXmlException($"{type} can't be deserialized! Some properties are missing.");

            var obj = Activator.CreateInstance(type);

            foreach (var innerRecord in record.InnerRecords)
            {
                var property = obj.GetType().GetProperty(innerRecord.Name, BindingFlags.Public | BindingFlags.Instance);

                var propertyObject = DeserializerService.Deserialize(property.PropertyType, innerRecord);

                if (property.SetMethod != null)
                    property.SetValue(obj, propertyObject);
            }

            return obj;
        }

        private bool _CheckIfContainsProperties(IRecord record, Type type)
        {
            var propertyInfos = type.GetProperties();

            var propertyNames = propertyInfos.Select(x => x.Name);

            foreach (var name in propertyNames)
            {
                var containsName = record.InnerRecords.Select(x => x.Name).Contains(name);

                if (!containsName)
                    return false;
            }

            return true;
        }
    }
}
