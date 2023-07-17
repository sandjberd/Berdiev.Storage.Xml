//Copyright by Sandjar Berdiev

using System;
using System.Collections;
using System.Linq;

namespace Berdiev.Storage.Xml.Internal.Deserializer
{
    internal class DictionaryDeserializer : RecordDeserializerBase
    {
        public DictionaryDeserializer(RecordDeserializerService deserializerService) : base(typeof(IDictionary), deserializerService)
        {
        }

        public override bool CanDeserialize(Type type)
        {
            return typeof(IDictionary).IsAssignableFrom(type);
        }

        public override object Deserialize(Type type, IRecord record)
        {
            var dictObject = (IDictionary)Activator.CreateInstance(type);
            var itemTypes = dictObject.GetType().GetGenericArguments();

            foreach (var keyValueRecord in record.InnerRecords)
            {
                var keyRecord = keyValueRecord.InnerRecords.First();
                var valueRecord = keyValueRecord.InnerRecords.Skip(1).First();

                var keyObject = _CreateKeyValueObject(keyRecord, itemTypes.First());
                var valueObject = _CreateKeyValueObject(valueRecord, itemTypes[1]);

                dictObject.Add(keyObject, valueObject);
            }

            return dictObject;

        }

        private object _CreateKeyValueObject(IRecord record, Type objectType)
        {
            return DeserializerService.Deserialize(objectType, record);
        }
    }
}
