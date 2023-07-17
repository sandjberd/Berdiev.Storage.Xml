//Copyright by Sandjar Berdiev

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Berdiev.Storage.Xml.Internal.Deserializer;
using Berdiev.Storage.Xml.Internal.Serializer;

namespace Berdiev.Storage.Xml.Internal
{
    internal class Repository : IRepository
    {
        private readonly XmlDocument _xmlDoc;
        private readonly object _lockObject;
        private readonly SerializerService _serializerService;
        private readonly RecordDeserializerService _deserialzerService;

        public Repository(XmlDocument xmlDoc, RepositorySpecification specification)
        {
            _lockObject = new object();

            lock (_lockObject)
            {
                _xmlDoc = xmlDoc;
            }

            _serializerService = SerializerService.Create();
            _serializerService.Register();
            _deserialzerService = RecordDeserializerService.Create();
            _deserialzerService.Register();

            Specification = specification;
        }

        public async Task<IReadOnlyList<IRecord>> GetRecordsByNameAsync(string recordName)
        {
            if (string.IsNullOrEmpty(recordName))
                throw new ArgumentNullException($"'{recordName}' must not be null or empty!");

            var allRecords = await GetRecordsAsync().ConfigureAwait(false);

            var recordsByName = new List<IRecord>();

            foreach (var record in allRecords)
            {
                var foundRecords = record.WhereRecord(r => r.Name.Equals(recordName, StringComparison.Ordinal));

                recordsByName.AddRange(foundRecords);
            }

            return recordsByName;
        }

        public Task<T> LoadDataAsync<T>(IReadOnlyList<IRecord> records, string name)
        {
            return Task.FromResult(_deserialzerService.Deserialize<T>(records, name));
        }

        public Task AppendRecordAsync(IRecord record)
        { 
            if (!record.InnerRecords.Any())
            {
                var xmlElement = _CreateXmlSingleRecord(record);
                _xmlDoc.ChildNodes[1].AppendChild(xmlElement);
            }
            else
            {
                var xmlElement = _CreateXmlStructuredRecord(record);
                _xmlDoc.ChildNodes[1].AppendChild(xmlElement);
            }

            _SaveStorage();

            return Task.CompletedTask;
        }

        public Task DeleteAsync(IRecord record)
        {
            var xmlNodes = _GetXmlElementsByRecord(record, _xmlDoc.ChildNodes);

            foreach (var xmlNode in xmlNodes)
            {
                xmlNode.ParentNode?.RemoveChild(xmlNode);
            }

            _SaveStorage();

            return Task.CompletedTask;
        }

        public RepositorySpecification Specification { get; }

        public Task<IReadOnlyList<IRecord>> GetRecordsAsync()
        {
            var elems = _xmlDoc.ChildNodes[1];

            var records = new List<IRecord>();

            foreach (XmlElement xmlElement in elems)
            {
                var record = new InternalRecord
                {
                    Name = xmlElement.Name,
                };

                _UnpackXmlIntoRecord(xmlElement, record);

                var attributes = _GetAttributes(xmlElement);

                foreach (var attribute in attributes)
                {
                    record.AddAttribute(attribute);
                }

                records.Add(record);
            }

            return Task.FromResult<IReadOnlyList<IRecord>>(records);
        }

        public Task UpdateRecordAsync(IRecord oldRecord, IRecord newRecord)
        {
            var xmlNodes = _GetXmlElementsByRecord(oldRecord, _xmlDoc.ChildNodes);

            foreach (var xmlNode in xmlNodes)
            {
                if (!newRecord.InnerRecords.Any())
                {
                    var xmlElementSingle = _CreateXmlSingleRecord(newRecord);
                    xmlNode.ParentNode?.ReplaceChild(xmlElementSingle, xmlNode);
                }
                else
                {
                    var xmlElementStructured = _CreateXmlStructuredRecord(newRecord);
                    xmlNode.ParentNode?.ReplaceChild(xmlElementStructured, xmlNode);
                }
                
            }

            _SaveStorage();

            return Task.CompletedTask;
        }

        private XmlElement _CreateXmlStructuredRecord(IRecord record)
        {
            var innerRecord = record as Record;

            var xmlElement = _CreateStructuredElement(innerRecord);

            return xmlElement;
        }

        private XmlElement _CreateStructuredElement(Record record)
        {
            var xmlElement = _xmlDoc.CreateElement(record.Name);

            var visitor = new StructuredRecordWriterVisitor(_xmlDoc);

            foreach (var innerRecord in record.InnerRecords)
            {
                var innerInternalRecord = innerRecord as Record;

                innerInternalRecord.Accept(visitor, xmlElement);
            }

            _AddAttributes(record, xmlElement);

            return xmlElement;
        }

        private XmlElement _CreateXmlSingleRecord(IRecord record)
        {
            var xmlElement = _CreateSingleElement(record);

            _AddAttributes(record, xmlElement);

            return xmlElement;

        }

        private XmlElement _CreateSingleElement(IRecord record)
        {
            var xmlElement = _xmlDoc.CreateElement(record.Name);

            if (record is Record untypedRecord)
            {
                xmlElement.InnerText = untypedRecord.Value;
            }
            else
            {
                var serializedElement = _serializerService.Serialize(((IRecord<IRecordSerializable>)record).TypedValue, record.Name, _xmlDoc);
                xmlElement = serializedElement;
            }

            return xmlElement;
        }

        private void _AddAttributes(IRecord record, XmlElement xmlElement)
        {
            var attributes = record.Attributes;

            foreach (var attribute in attributes)
            {
                var attr = _xmlDoc.CreateAttribute(attribute.AttributeName);
                attr.Value = attribute.AttributeValue;
                xmlElement.SetAttributeNode(attr);
            }
        }


        private void _UnpackXmlIntoRecord(XmlElement xmlElement, InternalRecord record)
        {
            foreach (var xmlElementChildNode in xmlElement.ChildNodes)
            {
                if (!(xmlElementChildNode is XmlElement childElement))
                {
                    record.Value = xmlElement.InnerText;
                    continue;
                }

                var innerRecord = new InternalRecord
                {
                    Name = childElement.Name,
                };

                _UnpackXmlIntoRecord(childElement, innerRecord);

                record.AddInnerRecord(innerRecord);
                var attributesStructured = _GetAttributes(childElement);
                foreach (var attribute in attributesStructured)
                {
                    innerRecord.AddAttribute(attribute);
                }
            }
        }


        private XmlElement _SearchElementById(XmlElement element)
        {
            XmlElement matchedElement = null;

            foreach (var xmlElement in element.ChildNodes)
            {
                if (!(xmlElement is XmlElement xmlTypedElement))
                    continue;

                //if (xmlTypedElement.Name.Equals(element.Name, StringComparison.Ordinal) && xmlTypedElement.InnerText)

                matchedElement = _SearchElementById(xmlTypedElement);
            }

            return matchedElement;
        }

        private void _SaveStorage()
        {
            lock (_lockObject)
            {
                _xmlDoc.Save(Specification.StoragePath);
            }

        }

        private IEnumerable<XmlNode> _GetXmlElementsByRecord(IRecord record, XmlNodeList nodes)
        {
            var xmlElements = new List<XmlNode>();

            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes.Item(i);

                if (node == null)
                    continue;

                if (_IsRecordEqualNode(record, node))
                {
                    xmlElements.Add(node);
                }
                

                if (node.HasChildNodes)
                {
                    var subElements = _GetXmlElementsByRecord(record, node.ChildNodes);
                    xmlElements.AddRange(subElements);
                }
            }

            return xmlElements;
        }

        private bool _IsRecordEqualNode(IRecord record, XmlNode node)
        {
            var innerXmlElements = new List<XmlElement>();

            if (!(node is XmlElement xmlElement))
                return false;

            if (!record.Name.Equals(xmlElement.Name, StringComparison.Ordinal))
                return false;

            if (!record.Attributes.Count.Equals(xmlElement.Attributes.Count))
                return false;

            for (int i = 0; i < xmlElement.ChildNodes.Count; i++)
            {
                var innerNode = xmlElement.ChildNodes.Item(i);

                if (!(innerNode is XmlElement innerXmlElement))
                    continue;
                
                innerXmlElements.Add(innerXmlElement);
            }

            if (!record.InnerRecords.Count.Equals(innerXmlElements.Count))
                return false;

            if (xmlElement.Value == null && string.IsNullOrEmpty(record.Value))
                return true;

            if (!xmlElement.InnerText.Equals(record.Value, StringComparison.Ordinal))
                return false;

            return true;
        }

        private IReadOnlyList<IAttribute> _GetAttributes(XmlElement element)
        {
            var attributes = new List<IAttribute>();

            foreach (XmlAttribute elementAttribute in element.Attributes)
            {
                if (elementAttribute.Name.Equals("id-special", StringComparison.Ordinal))
                    continue;

                attributes.Add(RecordAttribute.From(elementAttribute.Name, elementAttribute.Value));
            }

            return attributes;
        }


    }
}
