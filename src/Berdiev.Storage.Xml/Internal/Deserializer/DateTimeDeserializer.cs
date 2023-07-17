//Copyright by Sandjar Berdiev

using System;
using System.Globalization;

namespace Berdiev.Storage.Xml.Internal.Deserializer
{
    internal class DateTimeDeserializer : RecordDeserializerBase
    {
        public DateTimeDeserializer(RecordDeserializerService deserializerService) : base(typeof(DateTime), deserializerService)
        {
        }

        public override object Deserialize(Type type, IRecord record)
        {
            VerifyType(type);

            return DateTime.FromFileTimeUtc(long.Parse(record.Value, CultureInfo.CurrentCulture));
        }
    }
}
