//Copyright by Sandjar Berdiev

using System;

namespace Berdiev.Storage.Xml.Internal.Deserializer
{
    internal interface IRecordDeserializer
    {
        bool CanDeserialize(Type type);

        object Deserialize(Type type, IRecord record);
    }
}