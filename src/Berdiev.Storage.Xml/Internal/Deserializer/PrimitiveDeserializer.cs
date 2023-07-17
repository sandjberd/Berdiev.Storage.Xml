//Copyright by Sandjar Berdiev

using System;
using System.Globalization;

namespace Berdiev.Storage.Xml.Internal.Deserializer
{
    internal class PrimitiveDeserializer : RecordDeserializerBase
    {
        public PrimitiveDeserializer(RecordDeserializerService deserializerService) : base(typeof(object), deserializerService)
        {
        }

        public override bool CanDeserialize(Type type)
        {
            if (type.IsPrimitive || type == typeof(String))
                return true;

            return false;
        }

        public override object Deserialize(Type type, IRecord record)
        {
            try
            {
                if (type == typeof(String))
                    return record.Value;

                if (type == typeof(int))
                    return int.Parse(record.Value, CultureInfo.CurrentCulture);

                if (type == typeof(uint))
                    return uint.Parse(record.Value, CultureInfo.CurrentCulture);

                if (type == typeof(long))
                    return long.Parse(record.Value, CultureInfo.CurrentCulture);

                if (type == typeof(ulong))
                    return ulong.Parse(record.Value, CultureInfo.CurrentCulture);

                if (type == typeof(float))
                    return float.Parse(record.Value, CultureInfo.CurrentCulture);

                if (type == typeof(decimal))
                    return decimal.Parse(record.Value, CultureInfo.CurrentCulture);

                if (type == typeof(double))
                    return double.Parse(record.Value, CultureInfo.CurrentCulture);

                if (type == typeof(byte))
                    return byte.Parse(record.Value, CultureInfo.CurrentCulture);

                if (type == typeof(sbyte))
                    return sbyte.Parse(record.Value, CultureInfo.CurrentCulture);

                if (type == typeof(char))
                    return char.Parse(record.Value);

                if (type == typeof(bool))
                    return bool.Parse(record.Value);
            }
            catch (Exception)
            {
                throw new StorageXmlException($"Could not convert record '{record.Name}' to desired type '{type}'");
            }

            throw new StorageXmlException($"Type '{type}' not supported");
        }
    }
}
