//Copyright by Sandjar Berdiev

using Berdiev.Storage.Xml.Internal;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Berdiev.Storage.Xml
{
    /// <summary>
    ///     This represents a factory that creates a storage repository instance with capability of CRUD operations.
    /// </summary>
    public class RepositoryFactory
    {
        /// <summary>
        ///     Creates a repository instance by using the given specification.
        /// </summary>
        /// <param name="specification">This is used to create a repository.</param>
        /// <returns>New repository instance.</returns>
        public static Task<IRepository> CreateRepositoryAsync(RepositorySpecification specification)
        {
            var xmlDoc = _CreateXmlDoc(specification);

            var repository = new Repository(xmlDoc, specification);

            return Task.FromResult<IRepository>(repository);
        }

        private static XmlDocument _CreateXmlDoc(RepositorySpecification spec)
        {
            var xmlDoc = new XmlDocument();
            var declaration = xmlDoc.CreateXmlDeclaration("1.0", Encoding.UTF8.WebName, string.Empty);
            xmlDoc.AppendChild(declaration);

            var rootNode = xmlDoc.CreateElement(spec.Name);
            rootNode.InnerXml = string.Empty;

            xmlDoc.AppendChild(rootNode);

            xmlDoc.Save(spec.StoragePath);

            return xmlDoc;
        }

    }
}
