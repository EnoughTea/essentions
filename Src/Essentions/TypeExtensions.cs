using System;
using System.Numerics;
using System.Reflection;
using JetBrains.Annotations;

namespace Essentions
{
    /// <summary> Extension methods for the <see cref="Type" />. </summary>
    public static class TypeExtensions
    {
        /// <summary> Retrieves the default value for a given type, like <c>default(T)</c>, only at runtime. </summary>
        /// <param name="type">The type to get the default value for.</param>
        /// <returns>The default value for the given type.</returns>
        /// <remarks>
        ///     Returns null for nullables, references types and <see cref="System.Void" />. For a value type
        ///     returns its instance, so method fails on non-public or open generic types.
        /// </remarks>
        /// <exception cref="ArgumentException">
        ///     Value type is non-public -or- value type is an open generic types -or-
        ///     <see cref="Activator.CreateInstance(Type)" /> threw an exception.
        /// </exception>
        public static T GetDefaultValue<T>([NotNull] this Type type)
        {
            var typeInfo = type.GetTypeInfo();
            // Deal with reference types and nullables first, also lets return null for void.
            if (!typeInfo.IsValueType || (Nullable.GetUnderlyingType(type) != null) || (type == typeof(void)))
                return default(T);

            // Default value cannot be determined for generic value types.
            if (typeInfo.ContainsGenericParameters)
                throw new ArgumentException($"No default for value type '{type}', since it has generic parameters");
            // Avoid non-public value types.
            if (typeInfo.IsNotPublic && !typeInfo.IsPrimitive)
                throw new ArgumentException($"No default for value type '{type}, it is not a public type.");

            // Finally, only 'normal' value types left.
            try {
                return (T)Activator.CreateInstance(type);
            }
            catch (Exception e) {
                throw new ArgumentException($"Cannot get default for value type '{type}': \"{e.Message}\")", e);
            }
        }

        /// <summary> Determines whether this type contains the specified type in its inheritance chain.
        /// Works with any types, be they interfaces, abstract classes or generics with parameters. </summary>
        /// <remarks>
        ///     Non-constructed generics such as List&lt;&gt; are considered part of a constructed generic type.
        ///     So given type <c>MyList&lt;T&gt; : List&lt;T&gt;</c>, code
        ///     <c>typeof(MyList&lt;&gt;)DerivedFrom(List&lt;&gt;)</c> will return true.
        /// </remarks>
        /// <param name="type">The type to check.</param>
        /// <param name="subtype">Type to search for.</param>
        public static bool DerivedFrom([NotNull] this Type type, [NotNull] Type subtype)
        {
            Check.NotNull(type)
                 .NotNull(subtype);

            if (type == subtype) return false;

            return subtype.GetTypeInfo().IsInterface
                ? Tools.Reflection.IsTypeDerivedFromInterface(type, subtype)
                : Tools.Reflection.IsTypeDerivedFromConcreteType(type, subtype);
        }

        /// <summary> Creates object of this type using specified constructor arguments. </summary>
        /// <param name="type">Type of object to create.</param>
        /// <param name="ctorArgs">The constructor arguments.</param>
        /// <returns>Instantiated object of given type or null.</returns>
        [CanBeNull]
        public static object Instantiate([NotNull] this Type type, [CanBeNull] object[] ctorArgs = null)
        {
            Check.NotNull(type);

            object result = null;
            var typeInfo = type.GetTypeInfo();

            if (typeInfo.ContainsGenericParameters)
                throw new ArgumentException($"Cannot instantiate type '{type}', since it has generic parameters",
                                            nameof(type));

            if (type == typeof(void)) {
                return null;
            }

            var nullableType = Nullable.GetUnderlyingType(type);
            if (nullableType != null) {
                return (ctorArgs?.Length > 0) ? ctorArgs[0] : nullableType.Instantiate();
            }

            if (type.IsArray) {
                result = InstantiateArrayType(type, ctorArgs);
            }
            else if (!typeInfo.IsAbstract && !typeInfo.IsInterface) {
                result = Activator.CreateInstance(type, ctorArgs);
            }

            return result;
        }

        /// <summary> Determines whether the specified type is integer type. </summary>
        /// <param name="type">The type to check.</param>
        /// <returns><c>true</c> if the specified type is integer type; otherwise, <c>false</c>.</returns>
        public static bool IsIntegerType([NotNull] this Type type)
        {
            Check.NotNull(type);

            if (!type.GetTypeInfo().IsValueType) return false;

            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
            return (underlyingType == typeof(byte)) ||
                   (underlyingType == typeof(sbyte)) ||
                   (underlyingType == typeof(short)) ||
                   (underlyingType == typeof(ushort)) ||
                   (underlyingType == typeof(int)) ||
                   (underlyingType == typeof(uint)) ||
                   (underlyingType == typeof(long)) ||
                   (underlyingType == typeof(ulong)) ||
                   (underlyingType == typeof(BigInteger));
        }

        /// <summary> Determines whether the specified type is integer or real number type. </summary>
        /// <param name="type">The type to check.</param>
        /// <returns><c>true</c> if the specified type is integer or real number type; otherwise, <c>false</c>.</returns>
        public static bool IsNumericType([NotNull] this Type type)
        {
            Check.NotNull(type);
            if (!type.GetTypeInfo().IsValueType) return false;

            return type.IsIntegerType() || type.IsRealNumberType();
        }

        /// <summary> Determines whether the specified type is real number type. </summary>
        /// <param name="type">The type to check.</param>
        /// <returns><c>true</c> if the specified type is real number type; otherwise, <c>false</c>.</returns>
        public static bool IsRealNumberType([NotNull] this Type type)
        {
            Check.NotNull(type);
            if (!type.GetTypeInfo().IsValueType) return false;

            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
            return (underlyingType == typeof(float)) ||
                   (underlyingType == typeof(double)) ||
                   (underlyingType == typeof(decimal)) ||
                   (underlyingType == typeof(Complex));
        }

        private static object InstantiateArrayType([NotNull] Type type, [CanBeNull] object[] ctorArgs)
        {
            Check.True(type.IsArray);

            object result = null;
            var arrayElementType = type.GetElementType();
            if (arrayElementType != null) {
                var arrayLength = 0;
                if (ctorArgs != null)
                    try {
                        arrayLength = Convert.ToInt32(ctorArgs[0]);
                    }
                    catch (InvalidCastException) {
                    }

                result = Array.CreateInstance(arrayElementType, arrayLength);
            }

            return result;
        }
    }
}