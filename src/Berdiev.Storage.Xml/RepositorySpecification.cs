//Copyright by Sandjar Berdiev

using System;

namespace Berdiev.Storage.Xml
{
    /// <summary>
    ///     This represents a blueprint to create a repository instance. It contains information how and where the repository must be created and managed. 
    /// </summary>
    public class RepositorySpecification
    {
        /// <summary>
        ///     Instantiates new specification.
        /// </summary>
        /// <param name="name">Name of the repository.</param>
        /// <param name="storagePath">Path of the storage</param>
        /// <param name="version">Version of the repository</param>
        public RepositorySpecification(string name, string storagePath, Version version)
        {
            Name = name;
            StoragePath = storagePath;
            Version = version;
        }

        /// <summary>
        ///     Instantiates new specification with version 1.0.0.
        /// </summary>
        /// <param name="name">Name of the repository.</param>
        /// <param name="storagePath">Path of the storage</param>
        public RepositorySpecification(string name, string storagePath)
        {
            Name = name;
            StoragePath = storagePath;
            Version = new Version(1, 0, 0);
        }

        /// <summary>
        ///     Version of the repository.
        /// </summary>
        public Version Version { get; }

        /// <summary>
        ///     Name of the repository.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Path of the storage.
        /// </summary>
        public string StoragePath { get; }
    }
}
