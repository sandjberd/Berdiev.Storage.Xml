//Copyright by Sandjar Berdiev

namespace Berdiev.Storage.Xml.Internal
{
    internal class RecordAttribute : IAttribute
    {
        private RecordAttribute(string attributeName, string attributeValue)
        {
            AttributeName = attributeName;
            AttributeValue = attributeValue;
        }

        public string AttributeName { get; }

        public string AttributeValue { get; }

        public static IAttribute From(string name, string value)
        {
            return new RecordAttribute(name, value);
        }

    }
}
