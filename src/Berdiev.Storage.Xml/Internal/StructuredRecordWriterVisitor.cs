//Copyright by Sandjar Berdiev

using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Berdiev.Storage.Xml.Internal
{
    internal class StructuredRecordWriterVisitor : IVisitor
    {
        private readonly XmlDocument _xmlDocument;

        public StructuredRecordWriterVisitor(XmlDocument xmlDocument)
        {
            _xmlDocument = xmlDocument;
        }

        public void Visit<T>(IRecord record, T handOver)
        {
            if (!(handOver is XmlElement xmlElement))
                return;

            if (!record.InnerRecords.Any())
            {
                _AddValueRecord(record, xmlElement);
            }
            else
            {
                _AddStructuredMethod<T>(record, xmlElement);
            }
        }

        private void _AddStructuredMethod<T>(IRecord record, XmlElement xmlElement)
        {
            var innerRecord = _xmlDocument.CreateElement(record.Name);

            xmlElement.AppendChild(_AddAttributes(record.Attributes, innerRecord));

            foreach (var recordInnerRecord in record.InnerRecords)
            {
                var internalInnerRecord = recordInnerRecord as Record;

                internalInnerRecord.Accept<XmlElement>(this, innerRecord);
            }
        }

        private void _AddValueRecord(IRecord record, XmlElement xmlElement)
        {
            var value = _xmlDocument.CreateElement(record.Name);
            value.InnerText = record.Value;

            xmlElement.AppendChild(_AddAttributes(record.Attributes, value));
        }

        private XmlElement _AddAttributes(IReadOnlyList<IAttribute> attributes, XmlElement xmlElement)
        {
            foreach (var attribute in attributes)
            {
                xmlElement.SetAttribute(attribute.AttributeName, attribute.AttributeValue);
            }

            return xmlElement;
        }

    }
}
