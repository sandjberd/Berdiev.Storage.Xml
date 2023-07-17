//Copyright by Sandjar Berdiev

using System;
using System.Globalization;

namespace Berdiev.Storage.Xml.Internal.Deserializer
{
    internal class TimeSpanDeserializer : RecordDeserializerBase
    {
        public TimeSpanDeserializer(RecordDeserializerService deserializerService) : base(typeof(TimeSpan), deserializerService)
        {
        }

        public override object Deserialize(Type type, IRecord record)
        {
            VerifyType(type);

            return TimeSpan.FromTicks(long.Parse(record.Value, CultureInfo.CurrentCulture));
        }
    }
}
