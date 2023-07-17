//Copyright by Sandjar Berdiev

using System;

namespace Berdiev.Storage.Xml
{
    /// <summary>
    ///     Represents xml storage errors.
    /// </summary>
    public class StorageXmlException : Exception
    {
        /// <inheritdoc />
        public StorageXmlException()
        {
        }

        /// <inheritdoc />
        public StorageXmlException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public StorageXmlException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
