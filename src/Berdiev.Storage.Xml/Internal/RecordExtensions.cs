//Copyright by Sandjar Berdiev

using System;
using System.Collections.Generic;
using System.Linq;

namespace Berdiev.Storage.Xml.Internal
{
    /// <summary>
    ///     This represents extension methods for records.
    /// </summary>
    public static class RecordExtensions
    {
        /// <summary>
        ///     Indicates if the given record has child records.
        /// </summary>
        public static bool HasChildren(this IRecord record)
        {
            if (record == null)
                return false;

            return record.InnerRecords.Any();
        }

        /// <summary>
        ///     Filters a record based on the given predicate.
        /// </summary>
        public static IEnumerable<IRecord> WhereRecord(this IRecord record, Predicate<IRecord> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException($"'{nameof(predicate)}' must not be null");

            var foundRecords = new List<IRecord>();

            if (predicate(record))
                foundRecords.Add(record);

            foreach (var recordInnerRecord in record.InnerRecords)
            {
                _Dig(recordInnerRecord, r =>
                {
                    if (predicate(r))
                        foundRecords.Add(r);
                });
            }

            return foundRecords;
        }

        /// <summary>
        ///     Filters a sequence based on the given predicate.
        /// </summary>
        public static IEnumerable<IRecord> WhereRecord(this IEnumerable<IRecord> records, Predicate<IRecord> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException($"'{nameof(predicate)}' must not be null");

            if (records == null)
                throw new ArgumentNullException($"'{nameof(records)}' must not be null");

            var foundRecords = new List<IRecord>();

            foreach (var recordInnerRecord in records)
            {
                _Dig(recordInnerRecord, r =>
                {
                    if (predicate(r))
                        foundRecords.Add(r);
                });
            }

            return foundRecords;
        }

        private static void _Dig(IRecord record, Action<IRecord> onDiggingRecord)
        {
            onDiggingRecord(record);

            if (record.HasChildren())
            {
                foreach (var recordInnerRecord in record.InnerRecords)
                {
                    _Dig(recordInnerRecord, onDiggingRecord);
                }
            }
        }
    }
}
