using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace TestXmlSchema
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        [DeploymentItem("OfficeApp1.xml")]
        [DeploymentItem("offappmanifest.xsd")]
        public void TestMethod1()
        {
            XmlSchema schema = null;
            using(StreamReader sr = new StreamReader("offappmanifest.xsd"))
            {
                 schema = XmlSchema.Read(sr, null);                
            }

            string rawXml = File.ReadAllText("OfficeApp1.xml");

            XmlSchemaSet xmlSchemaSet = new XmlSchemaSet();
            xmlSchemaSet.Add(schema);
            XmlReaderSettings settings = new XmlReaderSettings()
            {
                Schemas = xmlSchemaSet,
                ValidationType = ValidationType.Schema
            };
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema ;
            settings.ValidationEventHandler += (ValidationEventHandler)((o, e) =>
            {
                switch (e.Severity)
                {
                    case XmlSeverityType.Error:
                        Console.Write("Line: {0} Column: {1} Message: {2}", e.Exception.LineNumber, e.Exception.LinePosition,e.Message);
                        Assert.Fail();
                        break;
                    case XmlSeverityType.Warning:
                        Assert.Fail();
                        break;
                }
            });

            using (StringReader stringReader = new StringReader(rawXml))
            {
                using (XmlReader reader = XmlReader.Create((TextReader)stringReader, settings))
                    XDocument.Load(reader);
            }
        }
    }
}
