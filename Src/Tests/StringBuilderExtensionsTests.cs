using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Essentions.Tests
{
    [TestFixture]
    public class StringBuilderExtensionsTests
    {
        [Test]
        public void When_ToEnumerable_is_used_on_non_empty_sb_Its_contents_are_returned()
        {
            string contents = "Alpha";
            StringBuilder sb = new StringBuilder(contents);
            Assert.That(string.Join("", sb.ToEnumerable()), Is.EqualTo(contents));
        }

        [Test]
        public void When_ToEnumerable_is_used_on_empty_sb_Then_empty_enumerable_is_returned()
        {
            StringBuilder sb = new StringBuilder();
            Assert.That(sb.ToEnumerable(), Is.EqualTo(Enumerable.Empty<char>()));
        }
    }
}