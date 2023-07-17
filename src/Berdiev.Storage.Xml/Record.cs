//Copyright by Sandjar Berdiev

using Berdiev.Storage.Xml.Internal;
using System;
using System.Collections.Generic;

namespace Berdiev.Storage.Xml
{
    /// <inheritdoc />
    public class Record : IRecord
    {
        private Record(string name, string value, IReadOnlyList<IAttribute> attributes, IReadOnlyList<IRecord> records)
        {
            Name = name;
            Value = value;
            Attributes = attributes;
            InnerRecords = records;
        }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public IReadOnlyList<IAttribute> Attributes { get; }

        /// <inheritdoc />
        public IReadOnlyList<IRecord> InnerRecords { get; }

        /// <inheritdoc />
        public string Value { get; }

        /// <summary>
        ///     Creates a simple record with value and optional attributes.
        /// </summary>
        /// <param name="name">Name of the record.</param>
        /// <param name="value">Value of the record.</param>
        /// <param name="attributes">Optional attributes.</param>
        /// <returns>New record.</returns>
        public static Record Create(string name, string value, params IAttribute[] attributes)
        {
            return new Record(name, value, attributes, new List<IRecord>());
        }

        /// <summary>
        ///     Creates a record with one inner record as child.
        /// </summary>
        /// <param name="name">Name of the record.</param>
        /// <param name="record">Inner record that represents the child record.</param>
        /// <param name="attributes">Optional attributes.</param>
        /// <returns>New record.</returns>
        public static Record CreateWithChild(string name, IRecord record, params IAttribute[] attributes)
        {
            return new Record(name, string.Empty, attributes, new List<IRecord> { record });
        }

        /// <summary>
        ///     Creates a record with many inner records as children.
        /// </summary>
        /// <param name="name">Name of the record.</param>
        /// <param name="records">Inner records that represents the children record.</param>
        /// <param name="attributes">Optional attributes.</param>
        /// <returns>New record.</returns>
        public static Record CreateWithChildren(string name, IRecord[] records, params IAttribute[] attributes)
        {
            return new Record(name, string.Empty, attributes, records);
        }

        // internals

        internal void Accept<T>(IVisitor visitor, T handOver)
        {
            visitor.Visit(this, handOver);
        }

    }
}
