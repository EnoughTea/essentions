using System;
using System.Numerics;
using Essentions.Tools;
using NUnit.Framework;

namespace Essentions.Tests.Tools
{
    [TestFixture]
    public class ReflectionTests
    {
        #region Fields

        [Test]
        public void When_field_value_is_got_Then_valid_value_is_returned()
        {
            var test = new TestClass();
            Assert.That(Reflection.GetField<TestClass, int>(test, nameof(TestClass._getSetIndexerValue)),
                        Is.EqualTo(2));
        }

        [Test]
        public void When_GetFieldValue_used_for_property_value_Then_exception_is_thrown()
        {
            var test = new TestClass();
            Assert.Throws<ArgumentException>(
                () => Reflection.GetField<TestClass, int>(test, nameof(TestClass.GetSetValue)));
        }

        [Test]
        public void When_GetFieldValue_used_for_empty_string_Then_exception_is_thrown()
        {
            var test = new TestClass();
            Assert.Throws<ArgumentException>(() => Reflection.GetField<TestClass, int>(test, ""));
        }

        [Test]
        public void When_field_value_is_set_Then_fields_has_valid_value()
        {
            var test = new TestClass();
            Reflection.SetField(test, nameof(TestClass._getSetIndexerValue), 50);
            Assert.That(test._getSetIndexerValue, Is.EqualTo(50));
        }

        [Test]
        public void When_SetFieldValue_used_for_property_value_Then_exception_is_thrown()
        {
            var test = new TestClass();
            Assert.Throws<ArgumentException>(
                () => Reflection.SetField(test, nameof(TestClass.GetSetValue), -1));
        }

        [Test]
        public void When_SetFieldValue_used_for_empty_string_Then_exception_is_thrown()
        {
            var test = new TestClass();
            Assert.Throws<ArgumentException>(() => Reflection.SetField(test, "", -1));
        }

        #endregion

        #region Static fields

        [Test]
        public void When_static_field_value_is_got_Then_valid_value_is_returned()
        {
            Assert.That(
                Reflection.GetStaticField<int>(typeof(TestClass), nameof(TestClass._staticSetOnlyValue)),
                Is.EqualTo(6));
        }

        [Test]
        public void When_GetStaticFieldValue_used_for_static_property_value_Then_exception_is_thrown()
        {
            Assert.Throws<ArgumentException>(
                () => Reflection.GetStaticField<int>(typeof(TestClass), nameof(TestClass.StaticGetSetValue)));
        }

        [Test]
        public void When_GetStaticFieldValue_used_for_empty_string_Then_exception_is_thrown()
        {
            Assert.Throws<ArgumentException>(
                () => Reflection.GetStaticField<int>(typeof(TestClass), ""));
        }

        [Test]
        public void When_static_field_value_is_set_Then_fields_has_valid_value()
        {
            Reflection.SetStaticField(typeof(TestClass), nameof(TestClass._staticSetOnlyValue), 50);
            Assert.That(TestClass._staticSetOnlyValue, Is.EqualTo(50));
        }

        [Test]
        public void When_SetStaticFieldValue_used_for_property_value_Then_exception_is_thrown()
        {
            Assert.Throws<ArgumentException>(
                () => Reflection.SetStaticField(typeof(TestClass), nameof(TestClass.StaticGetSetValue), -1));
        }

        [Test]
        public void When_SetStaticFieldValue_used_for_empty_string_Then_exception_is_thrown()
        {
            Assert.Throws<ArgumentException>(
                () => Reflection.SetStaticField(typeof(TestClass), "", -1));
        }

        #endregion

        #region Indexed properties

        [Test]
        public void When_get_only_indexed_property_value_is_got_Then_valid_value_is_returned()
        {
            var test = new TestClass();
            Assert.That(Reflection.GetIndexedProperty<TestClass, int, int>(test, 100),
                        Is.EqualTo(1));
            Assert.That(Reflection.GetIndexedProperty<TestClass, int, int>(test, 10),
                        Is.EqualTo(-1));
        }

        [Test]
        public void When_get_set_indexed_property_value_is_got_Then_valid_value_is_returned()
        {
            var test = new TestClass();
            Assert.That(Reflection.GetIndexedProperty<TestClass, string, int>(test, "any"),
                        Is.EqualTo(2));
        }

        [Test]
        public void When_GetIndexedPropertyValue_is_used_with_non_existing_index_type_Then_exception_is_thrown()
        {
            var test = new TestClass();
            Assert.Throws<ArgumentException>(
                () => Reflection.GetIndexedProperty<TestClass, object, int>(test, "any"));
        }

        [Test]
        public void When_set_only_indexed_property_value_is_set_Then_property_has_valid_value()
        {
            var test = new TestClass();
            Reflection.SetIndexedProperty(test, TimeSpan.Zero, 100);
            Assert.That(test._setOnlyIndexerValue, Is.EqualTo(100));
        }

        [Test]
        public void When_get_set_indexed_property_value_is_set_Then_property_has_valid_value()
        {
            var test = new TestClass();
            Reflection.SetIndexedProperty(test, "", 100);
            Assert.That(test._getSetIndexerValue, Is.EqualTo(100));
        }

        [Test]
        public void When_SetIndexedPropertyValue_is_used_with_non_existing_index_type_Then_exception_is_thrown()
        {
            var test = new TestClass();
            Assert.Throws<ArgumentException>(
                () => Reflection.SetIndexedProperty(test, new object(), 100));
        }

        #endregion

        #region Non-indexed properties

        [Test]
        public void When_get_set_property_value_is_got_Then_valid_value_is_returned()
        {
            var test = new TestClass();
            Assert.That(Reflection.GetProperty<TestClass, int>(test, nameof(TestClass.GetSetValue)),
                        Is.EqualTo(4));
        }

        [Test]
        public void When_get_only_property_value_is_got_Then_valid_value_is_returned()
        {
            var test = new TestClass();
            Assert.That(Reflection.GetProperty<TestClass, int>(test, nameof(TestClass.GetOnlyValue)),
                        Is.EqualTo(3));
        }

        [Test]
        public void When_set_only_property_value_is_got_Then_exception_is_thrown()
        {
            var test = new TestClass();
            Assert.Throws<ArgumentException>(
                () => Reflection.GetProperty<TestClass, int>(test, nameof(TestClass.SetOnlyValue)));
        }

        [Test]
        public void When_GetPropertyValue_used_for_field_value_Then_exception_is_thrown()
        {
            var test = new TestClass();
            Assert.Throws<ArgumentException>(
                () => Reflection.GetProperty<TestClass, int>(test, nameof(TestClass._setOnlyValue)));
        }

        [Test]
        public void When_GetPropertyValue_used_for_empty_string_Then_exception_is_thrown()
        {
            var test = new TestClass();
            Assert.Throws<ArgumentException>(
                () => Reflection.GetProperty<TestClass, int>(test, ""));
        }

        [Test]
        public void When_set_only_property_value_is_set_Then_property_has_valid_value()
        {
            var test = new TestClass();
            Reflection.SetProperty(test, nameof(TestClass.SetOnlyValue), 100);
            Assert.That(test._setOnlyValue, Is.EqualTo(100));
        }

        [Test]
        public void When_get_set_property_value_is_set_Then_property_has_valid_value()
        {
            var test = new TestClass();
            Reflection.SetProperty(test, nameof(TestClass.GetSetValue), 100);
            Assert.That(test.GetSetValue, Is.EqualTo(100));
        }

        [Test]
        public void When_get_only_property_value_is_set_Then_exception_is_thrown()
        {
            var test = new TestClass();

            Assert.Throws<ArgumentException>(
                () => Reflection.SetProperty(test, nameof(TestClass.GetOnlyValue), 100));
        }

        [Test]
        public void When_SetPropertyValue_used_for_field_value_Then_exception_is_thrown()
        {
            var test = new TestClass();
            Assert.Throws<ArgumentException>(
                () => Reflection.SetProperty(test, nameof(TestClass._setOnlyValue), -1));
        }

        [Test]
        public void When_SetPropertyValue_used_for_empty_string_Then_exception_is_thrown()
        {
            var test = new TestClass();
            Assert.Throws<ArgumentException>(() => Reflection.SetProperty(test, "", -1));
        }

        #endregion

        #region Static non-indexed properties

        [Test]
        public void When_static_get_set_property_value_is_got_Then_valid_value_is_returned()
        {
            Assert.That(
                Reflection.GetStaticProperty<int>(typeof(TestClass), nameof(TestClass.StaticGetSetValue)),
                Is.EqualTo(8));
        }

        [Test]
        public void When_static_get_only_property_value_is_got_Then_valid_value_is_returned()
        {
            Assert.That(
                Reflection.GetStaticProperty<int>(typeof(TestClass), nameof(TestClass.StaticGetOnlyValue)),
                Is.EqualTo(7));
        }

        [Test]
        public void When_static_set_only_property_value_is_got_Then_exception_is_thrown()
        {
            Assert.Throws<ArgumentException>(
                () => Reflection.GetStaticProperty<int>(typeof(TestClass), nameof(TestClass.StaticSetOnlyValue)));
        }

        [Test]
        public void When_GetStaticPropertyValue_used_for_field_value_Then_exception_is_thrown()
        {
            Assert.Throws<ArgumentException>(
                () => Reflection.GetStaticProperty<int>(typeof(TestClass), nameof(TestClass._staticSetOnlyValue)));
        }

        [Test]
        public void When_GetStaticPropertyValue_used_for_empty_string_Then_exception_is_thrown()
        {
            Assert.Throws<ArgumentException>(
                () => Reflection.GetStaticProperty<int>(typeof(TestClass), ""));
        }

        [Test]
        public void When_static_set_only_property_value_is_set_Then_property_has_valid_value()
        {
            Reflection.SetStaticProperty(typeof(TestClass), nameof(TestClass.StaticSetOnlyValue), 100);
            Assert.That(TestClass._staticSetOnlyValue, Is.EqualTo(100));
        }

        [Test]
        public void When_static_get_set_property_value_is_set_Then_property_has_valid_value()
        {
            Reflection.SetStaticProperty(typeof(TestClass), nameof(TestClass.StaticGetSetValue), 100);
            Assert.That(TestClass.StaticGetSetValue, Is.EqualTo(100));
        }

        [Test]
        public void When_static_get_only_property_value_is_set_Then_exception_is_thrown()
        {
            Assert.Throws<ArgumentException>(
                () => Reflection.SetStaticProperty(typeof(TestClass), nameof(TestClass.StaticGetOnlyValue), 100));
        }

        [Test]
        public void When_SetStaticPropertyValue_used_for_field_value_Then_exception_is_thrown()
        {
            Assert.Throws<ArgumentException>(
                () => Reflection.SetStaticProperty(typeof(TestClass), nameof(TestClass._staticSetOnlyValue), -1));
        }

        [Test]
        public void When_SetStaticPropertyValue_used_for_empty_string_Then_exception_is_thrown()
        {
            Assert.Throws<ArgumentException>(() => Reflection.SetStaticProperty(typeof(TestClass), "", -1));
        }

        #endregion

        [Test]
        public void When_finding_type_by_its_short_name_without_given_assemblies_Then_null_is_returned()
        {
            Assert.That(Reflection.FindType(typeof(BigInteger).Name), Is.Null);
        }

        [Test]
        public void When_finding_type_by_its_fill_name_without_given_assemblies_Then_null_is_returned()
        {
            Assert.That(Reflection.FindType(typeof(BigInteger).FullName), Is.Null);
        }

        [Test]
        public void When_finding_type_by_its_assembly_qualified_name_without_given_assemblies_Then_type_is_found()
        {
            Assert.That(Reflection.FindType(typeof(BigInteger).AssemblyQualifiedName),
                        Is.EqualTo(typeof(BigInteger)));
        }

        [Test]
        public void When_finding_type_by_its_short_name_with_given_assemblies_Then_null_is_returned()
        {
            var currentAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            Assert.That(Reflection.FindType(typeof(BigInteger).Name, currentAssemblies), Is.Null);
        }

        [Test]
        public void When_finding_type_by_its_full_name_with_given_assemblies_Then_type_is_found()
        {
            var currentAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            Assert.That(Reflection.FindType(typeof(BigInteger).FullName, currentAssemblies),
                        Is.EqualTo(typeof(BigInteger)));
        }

        [Test]
        public void When_finding_type_by_its_assembly_qualified_name_with_given_assemblies_Then_type_is_found()
        {
            var currentAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            Assert.That(Reflection.FindType(typeof(BigInteger).AssemblyQualifiedName, currentAssemblies),
                        Is.EqualTo(typeof(BigInteger)));
        }

        [Test]
        public void When_finding_non_existing_type_with_given_assemblies_Then_null_is_returned()
        {
            var currentAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            Assert.That(Reflection.FindType("NOT A REAL TYPE", currentAssemblies), Is.Null);
        }

        private class TestClass
        {
            internal int _setOnlyValue;
            internal int _getSetIndexerValue = 2;
            internal int _setOnlyIndexerValue = 5;

            internal static int _staticSetOnlyValue = 6;

            public static int StaticGetOnlyValue { get; } = 7;

            internal static int StaticSetOnlyValue
            {
                set { _staticSetOnlyValue = value; }
            }

            public static int StaticGetSetValue { get; set; } = 8;

            public int this[int index]
            {
                get {
                    if (index == 100) {
                        return 1;
                    }

                    return -1;
                }
            }

            public int this[string index]
            {
                get { return _getSetIndexerValue; }
                set { _getSetIndexerValue = value; }
            }

            public int this[TimeSpan index]
            {
                set { _setOnlyIndexerValue = value; }
            }

            public int GetOnlyValue { get; } = 3;

            internal int SetOnlyValue
            {
                set { _setOnlyValue = value; }
            }

            public int GetSetValue { get; set; } = 4;
        }
    }
}