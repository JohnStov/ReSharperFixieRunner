using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;

using ReSharperFixieRunner.UnitTestProvider.Elements;

namespace ReSharperFixieRunner.UnitTestProvider
{
    using ReadFromXmlFunc = Func<XmlElement, IUnitTestElement, ISolution, UnitTestElementFactory, IUnitTestElement>;

    [SolutionComponent]
    public class FixieTestElementSerializer : IUnitTestElementSerializer
    {
        private static readonly IDictionary<string, ReadFromXmlFunc> DeserialiseMap = new Dictionary<string, ReadFromXmlFunc>
                                                                                          {
                                                                                              {typeof (FixieTestClassElement).Name, FixieTestClassElement.ReadFromXml},
                                                                                              {typeof (FixieTestMethodElement).Name, FixieTestMethodElement.ReadFromXml},
                                                                                          };

        private readonly FixieTestProvider provider;
        private readonly UnitTestElementFactory unitTestElementFactory;
        private readonly ISolution solution;

        public FixieTestElementSerializer(FixieTestProvider provider, UnitTestElementFactory unitTestElementFactory, ISolution solution)
        {
            this.provider = provider;
            this.unitTestElementFactory = unitTestElementFactory;
            this.solution = solution;
        }

        public void SerializeElement(XmlElement parent, IUnitTestElement element)
        {
            parent.SetAttribute("type", element.GetType().Name);

            // Make sure that the element is actually ours before trying to serialise it
            // This can happen if there are two providers with the same "Fixie" id installed
            var writableUnitTestElement = element as ISerializableUnitTestElement;
            if (writableUnitTestElement != null)
                writableUnitTestElement.WriteToXml(parent);
        }

        public IUnitTestElement DeserializeElement(XmlElement parent, IUnitTestElement parentElement)
        {
            if (!parent.HasAttribute("type"))
                throw new ArgumentException("Element is not Fixie");

            ReadFromXmlFunc func;
            if (DeserialiseMap.TryGetValue(parent.GetAttribute("type"), out func))
                return func(parent, parentElement, solution, unitTestElementFactory);

            throw new ArgumentException("Element is not Fixie");
        }

        public IUnitTestProvider Provider
        {
            get { return provider; }
        }
    }
}
