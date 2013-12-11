using System.Xml;

namespace FixiePlugin.Elements
{
    public interface ISerializableUnitTestElement
    {
        void WriteToXml(XmlElement element);
    }
}