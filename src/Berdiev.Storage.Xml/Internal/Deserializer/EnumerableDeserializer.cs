//Copyright by Sandjar Berdiev

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Berdiev.Storage.Xml.Internal.Deserializer
{
    internal class EnumerableDeserializer : RecordDeserializerBase
    {
        public EnumerableDeserializer(RecordDeserializerService deserializerService) : base(typeof(IEnumerable), deserializerService)
        {
        }

        public override bool CanDeserialize(Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type) &&
                   !typeof(IDictionary).IsAssignableFrom(type) &&
                   !typeof(String).IsAssignableFrom(type);
        }

        public override object Deserialize(Type type, IRecord record)
        {
            var listGenericType = typeof(List<>);

            var itemType = type.GetGenericArguments().FirstOrDefault();

            var genericListType = listGenericType.MakeGenericType(itemType);

            var list = Activator.CreateInstance(genericListType) as IList;

            try
            {
                foreach (var innerRecord in record.InnerRecords)
                {
                    var obj = DeserializerService.Deserialize(itemType, innerRecord);
                    list.Add(obj);
                }
            }
            catch (Exception e)
            {
                throw new StorageXmlException($"Can not deserialize '{type}' ! {Environment.NewLine} {e.Message}");
            }

            return list;
        }
    }
}
