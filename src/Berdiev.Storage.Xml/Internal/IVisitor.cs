//Copyright by Sandjar Berdiev

namespace Berdiev.Storage.Xml.Internal
{
    internal interface IVisitor
    {
        void Visit<T>(IRecord record, T handOver);
    }
}
