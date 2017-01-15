using System;
using System.IO;
using System.Runtime.Serialization;
using Essentions.Collections;
using NUnit.Framework;

namespace Essentions.Tests.Collections
{
    [TestFixture]
    public class WeakValueDictionaryTests
    {
        const string Xml = @"<weakDict z:Id=""i1"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:z=""http://schemas.microsoft.com/2003/10/Serialization/""><data xmlns:a=""http://schemas.microsoft.com/2003/10/Serialization/Arrays""><a:KeyValueOfintWeakReferenceU6ho3Bhd><a:Key>0</a:Key><a:Value xmlns:b=""http://schemas.datacontract.org/2004/07/System""><TrackedObject z:Id=""i2"" i:type=""null""/><TrackResurrection i:type=""c:boolean"" xmlns:c=""http://www.w3.org/2001/XMLSchema"">false</TrackResurrection></a:Value></a:KeyValueOfintWeakReferenceU6ho3Bhd><a:KeyValueOfintWeakReferenceU6ho3Bhd><a:Key>1</a:Key><a:Value xmlns:b=""http://schemas.datacontract.org/2004/07/System""><TrackedObject z:Id=""i3"" i:type=""item""><v>1</v></TrackedObject><TrackResurrection i:type=""c:boolean"" xmlns:c=""http://www.w3.org/2001/XMLSchema"">false</TrackResurrection></a:Value></a:KeyValueOfintWeakReferenceU6ho3Bhd></data><dead xmlns:a=""http://schemas.datacontract.org/2004/07/System.Collections.Generic""><a:_array/><a:_head>0</a:_head><a:_size>0</a:_size><a:_tail>0</a:_tail><a:_version>0</a:_version></dead></weakDict>";

        [Test]
        public void When_WeakDictionary_is_created_Then_it_is_empty()
        {
            var dict = new WeakValueDictionary<int, Item>();

            Assert.That(dict.Count, Is.Zero);
        }

        [Test]
        public void When_dictionary_is_cleared_Then_it_is_empty()
        {
            var dict = new WeakValueDictionary<int, Item> {
                [0] = null,
                [1] = new Item()
            };

            dict.Clear();

            Assert.That(dict.Count, Is.Zero);
        }

        [Test]
        public void When_null_item_is_added_Then_its_key_exists()
        {
            var dict = new WeakValueDictionary<int, Item>();

            dict[1] = null;
            Item item;
            bool exists = dict.TryGetValue(1, out item);

            Assert.That(dict.Count, Is.EqualTo(1));
            Assert.That(exists, Is.True);
            Assert.That(item, Is.Null);
        }

        [Test]
        public void When_item_is_added_Then_it_can_be_retrieved()
        {
            var dict = new WeakValueDictionary<int, Item>();
            Item item = new Item();

            dict[1] = item;
            bool exists = dict.TryGetValue(1, out item);

            Assert.That(dict.Count, Is.EqualTo(1));
            Assert.That(exists, Is.True);
            Assert.That(item, Is.EqualTo(item));
        }

        [Test]
        public void When_element_is_set_twice_with_for_the_same_key_Then_key_value_is_changed()
        {
            var dict = new WeakValueDictionary<int, Item>();
            var item = new Item();
            var item2 = new Item();

            dict[0] = item;
            dict[0] = item2;

            Assert.That(dict.Count, Is.EqualTo(1));
            Assert.That(dict[0], Is.EqualTo(item2));
        }

        [Test]
        public void When_key_is_removed_Then_count_is_decreased()
        {
            var dict = new WeakValueDictionary<int, Item> {
                [0] = new Item(),
                [1] = new Item()
            };

            bool removed = dict.Remove(0);

            Assert.That(removed, Is.True);
            Assert.That(dict.Count, Is.EqualTo(1));
        }

        [Test]
        public void When_non_existing_key_is_removed_Then_false_is_returned()
        {
            var dict = new WeakValueDictionary<int, Item> {
                [0] = new Item(),
                [1] = new Item()
            };

            bool removed = dict.Remove(3);

            Assert.That(removed, Is.False);
            Assert.That(dict.Count, Is.EqualTo(2));
        }


        [Test]
        public void When_getting_keys_collection_Then_actual_dictionary_keys_are_returned()
        {
            var dict = new WeakValueDictionary<int, Item> {
                [0] = new Item(),
                [1] = new Item(),
                [2] = new Item()
            };

            var keys = dict.Keys;

            Assert.That(keys.Count, Is.EqualTo(3));
            Assert.That(keys, Is.EqualTo(new[] {0, 1, 2}));
        }

        [Test]
        public void When_getting_values_collection_Then_actual_dictionary_values_without_nulls_are_returned()
        {
            var item = new Item();
            var item2 = new Item();
            var dict = new WeakValueDictionary<int, Item> {
                [0] = item,
                [1] = null,
                [2] = item2
            };

            var values = dict.Values;

            Assert.That(values.Count, Is.EqualTo(2));
            Assert.That(values, Is.EqualTo(new[] { item, item2 }));
        }

        [Test]
        public void When_item_is_added_Then_it_can_be_garbage_collected()
        {
            var dict = new WeakValueDictionary<int, Item> { [1] = new Item() };

            GC.Collect();
            GC.WaitForPendingFinalizers();

            AssertEnumeration(dict, 1);
            Assert.That(dict.ContainsKey(1), Is.False);
            Assert.That(dict.Count, Is.EqualTo(0));
        }

        private void AssertEnumeration(WeakValueDictionary<int, Item> dict, int key)
        {
            foreach (var kvp in dict) {
                Assert.That(kvp.Key, Is.Not.EqualTo(key));
            }
        }

        [Test]
        public void When_element_is_added_twice_with_the_same_key_Then_exception_is_thrown()
        {
            var dict = new WeakValueDictionary<int, Item> { { 0, new Item() } };

            Assert.Throws<ArgumentException>(() => dict.Add(0, new Item()));
        }

        [Test]
        public void When_dictionary_is_serialized_to_xml_Then_valid_xml_is_returned()
        {
            var dict = new WeakValueDictionary<int, Item>();
            dict[0] = null;
            dict[1] = new Item { Value = 1 };
            string resultingXml;
            var serializer = new DataContractSerializer(typeof(WeakValueDictionary<int, Item>), new[] { typeof(Item) });

            using (var memStream = new MemoryStream())
            using (var reader = new StreamReader(memStream)) {
                serializer.WriteObject(memStream, dict);
                memStream.Seek(0, SeekOrigin.Begin);
                resultingXml = reader.ReadToEnd();
            }

            Assert.That(resultingXml, Is.EqualTo(Xml));
        }

        [Test]
        public void When_dictionary_xml_is_deserialized_Then_valid_dictionary_is_returned()
        {
            WeakValueDictionary<int, Item> dict;
            var serializer = new DataContractSerializer(typeof(WeakValueDictionary<int, Item>), new[] { typeof(Item) });

            using (var memStream = Xml.ToMemoryStream()) {
                dict = (WeakValueDictionary<int, Item>)serializer.ReadObject(memStream);
            }

            Assert.That(dict, Is.Not.Null);
            Assert.That(dict.Count, Is.EqualTo(2));
            Assert.That(dict[0], Is.Null);
            Assert.That(dict[1].Value, Is.EqualTo(1));
        }

        [DataContract(Name = "item", IsReference = true, Namespace = "")]
        public class Item
        {
            [DataMember(Name = "v")]
            public int Value { get; set; }
        }
    }
}