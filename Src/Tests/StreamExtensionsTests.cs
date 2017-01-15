using System;
using System.IO;
using System.Linq;
using Essentions.Tests.Tools;
using Essentions.Tools;
using NUnit.Framework;

namespace Essentions.Tests
{
    [TestFixture]
    public class StreamExtensionsTests
    {
        private const string ExampleText =
            @"Erik Naggum was the first person I killfiled in GNUS.
His style was sometimes shockingly blunt and aggressive. After a while, though, I realized I was missing out, and I came to treasure the information and insight in his messages.

I learned yesterday that Erik died. I'm sorry to hear it; I occasionally contacted him to clarify or expand on some technical matter he wrote about in the past, and he was always helpful. I thought I would just be able to do that whenever I wanted, but now it's too late.

His death has, not surprisingly, led some people to go through the same initial experience I had, seeing some blunt and shocking language and wondering why anyone would care about its author. Here are some links that I hope show a small part of Erik's contributions to knowledge.";


        private static readonly string _ExampleStrange = Environment.NewLine + Environment.NewLine +
                                                        @"ﬗﬖﬆﭚתּﻕﻧﻠﺍﹽᶞᶮᵁᵓᵅᵡ\t\r\nҶҲҊѹɁɨɤȔCÄ¬¨K¯«©¦\t";

        [Test]
        public void TestReadLines()
        {
            string readText;
            using (var reader = new StreamReader(ExampleText.ToMemoryStream())) {
                readText = reader.ReadLines().Aggregate((acc, line) => acc + Environment.NewLine + line);
            }

            Assert.AreEqual(ExampleText, readText);

            string readStrange;
            using (var reader = new StreamReader(_ExampleStrange.ToMemoryStream())) {
                readStrange = reader.ReadLines().Aggregate((acc, line) => acc + Environment.NewLine + line);
            }

            Assert.AreEqual(_ExampleStrange, readStrange);
        }

        [Test]
        public void TestMeasure()
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream)) {
                Assert.That(stream.MeasurePositionDifference(s => writer.Write(new byte[0])), Is.EqualTo(0));
                Assert.That(stream.MeasurePositionDifference(s => writer.Write(new byte[4])), Is.EqualTo(4));
                Assert.That(stream.MeasurePositionDifference(s => writer.Write(new byte[40])), Is.EqualTo(40));
            }
        }

        [Test]
        public void TestReadStruct()
        {
            TestStaticBlittable staticBlittable = new TestStaticBlittable {
                Value = 1024,
                Value2 = 0.1d,
                Value3 = "Vi"
            };

            using (var stream = Change.StructToBytes(staticBlittable).ToMemoryStream())
            {
                TestStaticBlittable read = stream.ReadStruct<TestStaticBlittable>();
                Assert.That(read.Value, Is.EqualTo(staticBlittable.Value));
                Assert.That(read.Value2, Is.EqualTo(staticBlittable.Value2));
                Assert.That(read.Value3, Is.EqualTo(staticBlittable.Value3));
            }
        }
    }
}