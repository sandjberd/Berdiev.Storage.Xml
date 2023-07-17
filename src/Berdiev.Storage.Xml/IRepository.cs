//Copyright by Sandjar Berdiev

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Berdiev.Storage.Xml
{
    /// <summary>
    ///     This represents the xml repository that provides access to xml storage.
    ///     The repository is capable of CRUD operations of the xml storage. 
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        ///     The specification of the current repository.
        /// </summary>
        RepositorySpecification Specification { get; }

        /// <summary>
        ///     Loads records from current repository.
        /// </summary>
        /// <returns>A collection of loaded records.</returns>
        Task<IReadOnlyList<IRecord>> GetRecordsAsync();

        /// <summary>
        ///     Loads records from current repository corresponding to the given name.
        /// </summary>
        /// <param name="recordName">Record name to search for in the repository</param>
        /// <returns>A collection of loaded records. The collection can be empty if no records were found.</returns>
        Task<IReadOnlyList<IRecord>> GetRecordsByNameAsync(String recordName);

        /// <summary>
        ///     Loads serialized data from given record.
        /// </summary>
        /// <param name="records">Records that contain the serialized data.</param>
        /// <param name="name">Record name that is used as index of serialized data.</param>
        /// <typeparam name="T">Serialized data type</typeparam>
        /// <returns>Deserialized data.</returns>
        Task<T> LoadDataAsync<T>(IReadOnlyList<IRecord> records, string name);

        /// <summary>
        ///     Appends record into the xml storage. 
        /// </summary>
        /// <param name="record">Record that should be appended.</param>
        /// <returns>The future of the operation.</returns>
        Task AppendRecordAsync(IRecord record);

        /// <summary>
        ///     This operation replaces a specific record with another one.
        /// </summary>
        /// <param name="oldRecord">Record to replace.</param>
        /// <param name="newRecord">Record to insert.</param>
        /// <returns>The future of the operation.</returns>
        Task UpdateRecordAsync(IRecord oldRecord, IRecord newRecord);

        /// <summary>
        ///     Deletes the specific record.
        /// </summary>
        /// <param name="record">Record to delete.</param>
        /// <returns>The future of the operation.</returns>
        Task DeleteAsync(IRecord record);
    }
}
