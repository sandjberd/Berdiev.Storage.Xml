//Copyright by Sandjar Berdiev

namespace Berdiev.Storage.Xml
{
    /// <summary>
    ///     This represents the xml attribute that is used in xml storage file.
    /// </summary>
    public interface IAttribute
    {
        /// <summary>
        ///     Represents the attribute name.
        /// </summary>
        string AttributeName { get; }

        /// <summary>
        ///     Represents the attribute value.
        /// </summary>
        string AttributeValue { get; }
    }
}
