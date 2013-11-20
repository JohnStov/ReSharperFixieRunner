using System.Xml;

namespace ReSharperFixieTestProvider.UnitTestProvider.Elements
{
    public interface ISerializableUnitTestElement
    {
        void WriteToXml(XmlElement element);
    }
}