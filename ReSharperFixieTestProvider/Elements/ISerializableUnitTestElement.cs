using System.Xml;

namespace ReSharperFixieTestProvider.Elements
{
    public interface ISerializableUnitTestElement
    {
        void WriteToXml(XmlElement element);
    }
}