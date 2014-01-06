using System;
using System.Collections.Generic;
using System.Xml;
using FixiePlugin.Elements;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;

namespace FixiePlugin
{
    [SolutionComponent]
    public class TestElementSerializer : IUnitTestElementSerializer
    {
        private static readonly IDictionary<string, Func<XmlElement, IUnitTestElement, ISolution, UnitTestElementFactory, IUnitTestElement>> DeserialiseMap = new Dictionary<string, Func<XmlElement, IUnitTestElement, ISolution, UnitTestElementFactory, IUnitTestElement>>
                                                                                          {
                                                                                              {typeof (TestClassElement).Name, TestClassElement.ReadFromXml},
                                                                                              {typeof (TestMethodElement).Name, TestMethodElement.ReadFromXml},
                                                                                              {typeof (TestCaseElement).Name, TestCaseElement.ReadFromXml},
                                                                                          };

        private readonly TestProvider provider;
        private readonly UnitTestElementFactory unitTestElementFactory;
        private readonly ISolution solution;

        public TestElementSerializer(TestProvider provider, UnitTestElementFactory unitTestElementFactory, ISolution solution)
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

            Func<XmlElement, IUnitTestElement, ISolution, UnitTestElementFactory, IUnitTestElement> func;
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
