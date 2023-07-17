//Copyright by Sandjar Berdiev

using System;
using System.Collections.Generic;

namespace Berdiev.Storage.Xml
{
    /// <summary>
    ///     A record represents a single storage item of a repository.
    ///     This can contain inner records that enables storage compositions.
    /// </summary>
    public interface IRecord
    {
        /// <summary>
        ///     Name of the record. This name is represented as tag name in the xml file.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     This represents the value of the record.
        ///     The value can be null.
        /// </summary>
        string Value { get; }

        /// <summary>
        ///     Attributes of the record. This is used to precise or describe a record.
        /// </summary>
        IReadOnlyList<IAttribute> Attributes { get; }

        /// <summary>
        ///     Child records of the current record.
        /// </summary>
        IReadOnlyList<IRecord> InnerRecords { get; }
    }
}
