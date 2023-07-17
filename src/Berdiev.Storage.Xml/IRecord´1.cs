//Copyright by Sandjar Berdiev


namespace Berdiev.Storage.Xml
{
    /// <inheritdoc />
    public interface IRecord<out T> : IRecord
    {
        /// <summary>
        ///     Complex typed value that can be serialized and stored in the xml repository.
        /// </summary>
        T TypedValue { get; }
    }
}
