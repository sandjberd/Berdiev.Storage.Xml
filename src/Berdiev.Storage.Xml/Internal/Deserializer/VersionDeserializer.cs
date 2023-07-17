//Copyright by Sandjar Berdiev

using System;

namespace Berdiev.Storage.Xml.Internal.Deserializer
{
    internal class VersionDeserializer : RecordDeserializerBase
    {
        public VersionDeserializer(RecordDeserializerService deserializerService) : base(typeof(Version), deserializerService)
        {
        }

        public override object Deserialize(Type type, IRecord record)
        {
            VerifyType(type);

            return Version.Parse(record.Value);
        }
    }
}
