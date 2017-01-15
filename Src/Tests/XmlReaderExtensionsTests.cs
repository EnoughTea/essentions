using System.Linq;
using System.Xml;
using NUnit.Framework;

namespace Essentions.Tests
{
    [TestFixture]
    public class XmlReaderExtensionsTests
    {
        private const string XmlTest = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<root>
    <numbers type=""array"">
        <value>3</value>
        <value>2</value>
        <value>1</value>
    </numbers>
    <letters>
        <value>a</value>
    </letters>
</root>";

        [Test]
        public void When_GetAttributeOrDefault_is_used_on_element_with_attribute_Then_its_value_is_returned()
        {
            using (var stream = XmlTest.ToMemoryStream()) {
                var reader = new XmlTextReader(stream);
                reader.MoveToContent();
                reader.Read();
                reader.Read();
                Assert.That(reader.GetAttributeOrDefault("type", "whatever"), Is.EqualTo("array"));
            }
        }

        [Test]
        public void When_GetAttributeOrDefault_is_used_on_element_without_attribute_Then_default_value_is_returned()
        {
            using (var stream = XmlTest.ToMemoryStream()) {
                var reader = new XmlTextReader(stream);
                reader.MoveToContent();
                Assert.That(reader.GetAttributeOrDefault("class", "none"), Is.EqualTo("none"));
            }
        }

        [Test]
        public void When_ToXElement_is_used_with_existing_node_name_Then_this_node_is_returned()
        {
            using (var stream = XmlTest.ToMemoryStream()) {
                var reader = new XmlTextReader(stream);
                var numbers = reader.ToXElement("numbers").FirstOrDefault();
                Assert.That(numbers, Is.Not.Null);
                Assert.That(numbers.Element("value"), Is.Not.Null);
            }
        }

        [Test]
        public void When_ToXElement_is_used_with_non_existing_node_name_Then_empty_sequence_is_returned()
        {
            using (var stream = XmlTest.ToMemoryStream()) {
                var reader = new XmlTextReader(stream);
                var numbers = reader.ToXElement("anything").FirstOrDefault();
                Assert.That(numbers, Is.Null);
            }
        }
    }
}