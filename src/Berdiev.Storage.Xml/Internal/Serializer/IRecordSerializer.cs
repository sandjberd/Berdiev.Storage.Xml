//Copyright by Sandjar Berdiev

using System;
using System.Xml;

namespace Berdiev.Storage.Xml.Internal.Serializer
{
    internal interface IRecordSerializer
    {
        bool CanSerialize(Type type);

        XmlElement Serialize(object objectToSerialize, string elementName, XmlDocument xmlDoc);
    }
}