//Copyright by Sandjar Berdiev

using System;
using System.Collections.Generic;

namespace Berdiev.Storage.Xml
{
    /// <inheritdoc />
    public class Record<T> : IRecord<T> where T : IRecordSerializable
    {
        private Record(Guid id, string name, T value, IReadOnlyList<IAttribute> attributes)
        {
            Name = name;
            Attributes = attributes;
            Id = id;
            TypedValue = value;
        }

        /// <inheritdoc />
        public T TypedValue { get; }

        /// <inheritdoc />
        public Guid Id { get; }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public IReadOnlyList<IAttribute> Attributes { get; }

        /// <inheritdoc />
        public IReadOnlyList<IRecord> InnerRecords => new List<IRecord>();

        /// <summary>
        ///     Since the value is a serialized object, this value can't be used and is therefore null.
        /// </summary>
        public string Value => String.Empty;

        /// <summary>
        ///     Creates a record with serialized object.
        /// </summary>
        /// <param name="name">Name of the record.</param>
        /// <param name="value">Value of the record that will be serialized before it gets persisted.</param>
        /// <param name="attributes">Optional attributes.</param>
        /// <typeparam name="T">Type of the value that gets serialized.</typeparam>
        /// <returns>New record.</returns>
        public static Record<T> Create<T>(string name, T value, params IAttribute[] attributes) where T : IRecordSerializable
        {
            return new Record<T>(Guid.NewGuid(), name, value, attributes);
        }

    }
}
