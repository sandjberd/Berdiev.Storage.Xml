//Copyright by Sandjar Berdiev

using System;

namespace Berdiev.Storage.Xml.Internal.Deserializer
{
    internal class GuidDeserializer : RecordDeserializerBase
    {
        public GuidDeserializer(RecordDeserializerService deserializerService) : base(typeof(Guid), deserializerService)
        {
        }

        public override object Deserialize(Type type, IRecord record)
        {
            VerifyType(type);

            return Guid.Parse(record.Value);
        }
    }
}
