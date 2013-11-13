using System.Xml;

namespace ReSharperFixieRunner.UnitTestProvider.Elements
{
    public interface ISerializableUnitTestElement
    {
        void WriteToXml(XmlElement element);
    }
}