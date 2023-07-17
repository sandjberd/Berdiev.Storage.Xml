//Copyright by Sandjar Berdiev

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Berdiev.Storage.Xml.Internal.Deserializer
{
    internal class RecordDeserializerService
    {
        private List<IRecordDeserializer> _deserializers;
        private static RecordDeserializerService _instance;

        private RecordDeserializerService()
        {
            
        }


        public static RecordDeserializerService Create()
        {
            if (_instance != null)
                return _instance;

            _instance = new RecordDeserializerService();

            return _instance;
        }

        public void Register()
        {
            if (_deserializers != null && _deserializers.Any())
                return;

            _deserializers = new List<IRecordDeserializer>
            {
                new PrimitiveDeserializer(this),
                new DateTimeDeserializer(this),
                new GuidDeserializer(this),
                new TimeSpanDeserializer(this),
                new VersionDeserializer(this),
                new EnumerableDeserializer(this),
                new DictionaryDeserializer(this),
                new RecordSerializableDeserializer(this),
            };
        }

        public T Deserialize<T>(IReadOnlyList<IRecord> records, string recordName)
        {
            IRecord desiredRecord = null;

            foreach (var internalRecord in records)
            {
                var objToSerialize = _HasObjectInRecord(internalRecord, typeof(T), recordName);

                if (objToSerialize.Item1)
                    desiredRecord = objToSerialize.Item2;
            }

            if (desiredRecord == null)
                throw new StorageXmlException($"Can't find record {recordName} of type {typeof(T)}");

            return (T)Deserialize(typeof(T), desiredRecord);
        }

        internal object Deserialize(Type type, IRecord record)
        {
            var deserializer = _FindDeserializer(type);

            var deserializedObject = deserializer.Deserialize(type, record);

            return deserializedObject;
        }

        private IRecordDeserializer _FindDeserializer(Type type)
        {
            var deserializer = _deserializers.FirstOrDefault(x => x.CanDeserialize(type));

            if (deserializer == null)
                throw new StorageXmlException("Can't find defined type in record!");

            return deserializer;
        }

        private static (bool, IRecord) _HasObjectInRecord(IRecord record, Type type, string recordName)
        {
            var hasSameTypeName = record.Name.Equals(recordName, StringComparison.Ordinal);

            if (!hasSameTypeName)
            {
                foreach (var innerRecord in record.InnerRecords)
                {
                    var check = (_HasObjectInRecord(innerRecord, type, recordName));

                    if (check.Item1)
                        return check;
                }
            }
            else
            {
                return (true, record);
            }

            return (false, null);
        }
    }
}
