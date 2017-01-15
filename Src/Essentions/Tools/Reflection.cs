using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace Essentions.Tools
{
    /// <summary> Reflection-related helper methods. </summary>
    public static class Reflection
    {
        /// <summary>Gets the value of the specified field for the given instance.</summary>
        /// <param name="obj">Instance which field value will be retrieved.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>Field value.</returns>
        /// <exception cref="ArgumentException">
        ///     The field was not found. - or - The field had no get accessor.
        /// </exception>
        /// <exception cref="InvalidCastException">Value type differs from field type.</exception>
        public static TValue GetField<T, TValue>([NotNull] T obj, [NotNull] string fieldName)
        {
            Check.NotNull(obj)
                 .NotNullOrEmpty(fieldName);

            var field = GetFieldInfo<T>(fieldName);
            return (TValue)field.GetValue(obj);
        }

        /// <summary>Sets the value of the specified field for the given instance.</summary>
        /// <param name="obj">Instance which field will be changed.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="value">Value to set for field.</param>
        /// <exception cref="ArgumentException">
        ///     field was not found. - or -
        ///     Value type differs from field type. - or - The field had no set accessor.
        /// </exception>
        public static void SetField<T, TVal>([NotNull] T obj,
                                             [NotNull] string fieldName,
                                             [CanBeNull] TVal value)
        {
            Check.NotNull(obj)
                 .NotNullOrEmpty(fieldName);

            var field = GetFieldInfo<T>(fieldName);
            field.SetValue(obj, value);
        }

        /// <summary>Gets the value of the indexed property matching the key type for the given instance.</summary>
        /// <param name="obj">Instance which indexed property value will be retrieved.</param>
        /// <param name="key">Index value.</param>
        /// <returns>Property value.</returns>
        /// <exception cref="ArgumentException">
        ///     Indexed property with index of type <typeparamref name="TIndex" />
        ///     was not found. - or - The indexed property had no get accessor.
        /// </exception>
        /// <exception cref="InvalidCastException">Value type differs from property type.</exception>
        public static TValue GetIndexedProperty<T, TIndex, TValue>([NotNull] T obj, [CanBeNull] TIndex key)
        {
            Check.NotNull(obj);

            var indexedProperty = GetIndexedPropertyInfo<T>(typeof(TIndex));
            return (TValue)indexedProperty.GetValue(obj, new object[] { key });
        }

        /// <summary>Gets the value of the specified property for the given instance.</summary>
        /// <param name="obj">Instance which property value will be retrieved.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>Property value.</returns>
        /// <exception cref="ArgumentException">
        ///     Property was not found. - or - The property had no get accessor.
        /// </exception>
        /// <exception cref="InvalidCastException">Value type differs from property type.</exception>
        public static TValue GetProperty<T, TValue>([NotNull] T obj, [NotNull] string propertyName)
        {
            Check.NotNull(obj)
                 .NotNullOrEmpty(propertyName);

            var property = GetPropertyInfo<T>(propertyName);
            return (TValue)property.GetValue(obj, null);
        }

        /// <summary>Sets the value of the indexed property matching the key type for the given instance.</summary>
        /// <param name="obj">Instance which indexed property value will be set.</param>
        /// <param name="key">Index value.</param>
        /// <param name="value">Value to set for property.</param>
        /// <exception cref="ArgumentException">
        ///     Indexed property with index of type <typeparamref name="TIndex" />
        ///     was not found. - or - The indexed property had no set accessor.
        /// </exception>
        public static void SetIndexedProperty<T, TIndex, TVal>([NotNull] T obj,
                                                               [CanBeNull] TIndex key,
                                                               [CanBeNull] TVal value)
        {
            Check.NotNull(obj);

            var indexedProperty = GetIndexedPropertyInfo<T>(typeof(TIndex));
            indexedProperty.SetValue(obj, value, new object[] { key });
        }


        /// <summary>Sets the value of the specified property for the given instance.</summary>
        /// <param name="obj">Instance which property will be changed.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">Value to set for property.</param>
        /// <exception cref="ArgumentException">
        ///     Property was not found. - or -
        ///     Value type differs from property type. - or - The property had no set accessor.
        /// </exception>
        public static void SetProperty<T, TVal>([NotNull] T obj,
                                                [NotNull] string propertyName,
                                                [CanBeNull] TVal value)
        {
            Check.NotNull(obj)
                 .NotNullOrEmpty(propertyName);

            var property = GetPropertyInfo<T>(propertyName);
            property.SetValue(obj, value, null);
        }

        /// <summary>Gets the static field value for the given type.</summary>
        /// <typeparam name="T">Type of the field value.</typeparam>
        /// <param name="type">The type.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>Value of the found field.</returns>
        /// <exception cref="ArgumentException">Field was not found.</exception>
        /// <exception cref="InvalidCastException">Value type differs from property type.</exception>
        [CanBeNull]
        public static T GetStaticField<T>([NotNull] Type type, [NotNull] string fieldName)
        {
            Check.NotNull(type)
                 .NotNull(fieldName);

            var field = GetFieldInfo(type, fieldName);
            return (T)field.GetValue(null);
        }

        /// <summary>Sets the static field value for the given type.</summary>
        /// <typeparam name="T">Type of the field value.</typeparam>
        /// <param name="type">The type to get field for.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="value">Value to set.</param>
        /// <exception cref="ArgumentException">Field was not found.</exception>
        public static void SetStaticField<T>([NotNull] Type type,
                                             [NotNull] string fieldName,
                                             [CanBeNull] T value)
        {
            Check.NotNull(type)
                 .NotNull(fieldName);

            var field = GetFieldInfo(type, fieldName);
            field.SetValue(null, value);
        }

        /// <summary>Gets the static property value for the given type.</summary>
        /// <typeparam name="T">Type of the property value.</typeparam>
        /// <param name="type">The type to get property for.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>Value of the found property.</returns>
        /// <exception cref="ArgumentException">Property was not found.</exception>
        /// <exception cref="InvalidCastException">Value type differs from property type.</exception>
        [CanBeNull]
        public static T GetStaticProperty<T>([NotNull] Type type, [NotNull] string propertyName)
        {
            Check.NotNull(type)
                 .NotNull(propertyName);

            var property = GetPropertyInfo(type, propertyName);
            return (T)property.GetValue(null);
        }

        /// <summary>Sets the static property value for the given type.</summary>
        /// <typeparam name="T">Type of the property value.</typeparam>
        /// <param name="type">The type to get property for.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">Value to set.</param>
        /// <exception cref="ArgumentException">Property was not found.</exception>
        public static void SetStaticProperty<T>([NotNull] Type type,
                                                [NotNull] string propertyName,
                                                [CanBeNull] T value)
        {
            Check.NotNull(type)
                 .NotNull(propertyName);

            var property = GetPropertyInfo(type, propertyName);
            property.SetValue(null, value);
        }

        /// <summary>
        ///     Finds type with the specified assembly-qualified name or a full name. If the type name
        ///     is assembly-qualified, searches loaded type's assembly. If the type name is a full name,
        ///     searches given <paramref name="searchedAssemblies" /> .
        /// </summary>
        /// <param name="typeName">
        ///     Either assembly qualified name of the type or a full name,
        ///     but then <paramref name="searchedAssemblies" /> should be supplied.
        /// </param>
        /// <param name="searchedAssemblies">
        ///     Assemblies to search for the given type, like
        ///     <c>AppDomain.CurrentDomain.GetAssemblies();</c>.
        ///     If null and given type name is not assembly qualified, function will return null.
        /// </param>
        /// <returns>Found type or null.</returns>
        [CanBeNull]
        public static Type FindType(
            [NotNull] string typeName,
            [CanBeNull] IEnumerable<Assembly> searchedAssemblies = null)
        {
            Check.NotNull(typeName);

            // If given type name is assembly-qualified, extract assembly name and class name
            string className, assemblyName;
            if (!ParseTypeFullName(typeName, out assemblyName, out className)) className = typeName;
            // This happens if type name is not assembly-qualified, and there are no assemblies to search
            if (string.IsNullOrEmpty(assemblyName) && (searchedAssemblies == null)) return null;
            // Either load assembly from assembly-qualified name or search all given assemblies
            Type foundType = null;
            if (searchedAssemblies == null) {
                var loadedAssembly = Assembly.Load(new AssemblyName(assemblyName));
                foundType = loadedAssembly?.GetType(className);
            } else {
                // Find target type assembly among the given assemblies
                foreach (var searchedAssembly in searchedAssemblies) {
                    foundType = searchedAssembly?.GetType(className);
                    if (foundType != null) break;
                }
            }

            return foundType;
        }


        /// <summary>Determines whether the specified type is derived from the given concrete type.</summary>
        /// <param name="type">Type to test.</param>
        /// <param name="concrete">Concrete type.</param>
        /// <returns><c>true</c> if type is derived from given concrete type; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentException">Passed concrete type actually is an interface type.</exception>
        public static bool IsTypeDerivedFromConcreteType([NotNull] Type type, [NotNull] Type concrete)
        {
            Check.NotNull(type)
                 .NotNull(concrete);

            var concreteTypeInfo = concrete.GetTypeInfo();
            if (concreteTypeInfo.IsInterface)
                throw new ArgumentException("Passed concrete type is an interface", nameof(concrete));

            var derived = false;
            if (!concreteTypeInfo.IsGenericType) {
                derived = type.GetTypeInfo().IsSubclassOf(concrete);
            } else {
                // Deal with concrete generic types with support for non-constructed types like List<>
                var spinningType = type;
                while ((spinningType != null) && (spinningType != typeof(object))) {
                    // Check current level of class hierarchy.
                    var spinningTypeInfo = spinningType.GetTypeInfo();
                    var currentHierarchyType = spinningTypeInfo.IsGenericType
                        ? spinningType.GetGenericTypeDefinition()
                        : spinningType;
                    if (concrete == currentHierarchyType) {
                        derived = true;
                        break;
                    }

                    spinningType = spinningTypeInfo.BaseType; // Advance to the next level of class hierarchy.
                }
            }

            return derived;
        }


        /// <summary>Determines whether the specified type is derived from the given interface type.</summary>
        /// <param name="type">Type to test.</param>
        /// <param name="iface">Interface type.</param>
        /// <returns><c>true</c> if type is derived from given interface type; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentException">Passed interface type actually is a concrete type.</exception>
        public static bool IsTypeDerivedFromInterface([NotNull] Type type, [NotNull] Type iface)
        {
            Check.NotNull(type)
                 .NotNull(iface);

            var targetTypeInfo = type.GetTypeInfo();
            var ifaceTypeInfo = iface.GetTypeInfo();
            if (!ifaceTypeInfo.IsInterface)
                throw new ArgumentException("Passed interface type is a concrete type", nameof(iface));

            var targetInterfaces = targetTypeInfo.ImplementedInterfaces;
            return !ifaceTypeInfo.IsGenericType
                ? targetInterfaces.Contains(iface)
                : IsDerivedFromInterface(type, iface, targetInterfaces);
        }

        [NotNull]
        private static FieldInfo GetFieldInfo<T>([NotNull] string fieldName)
        {
            return GetFieldInfo(typeof(T), fieldName);
        }

        private static FieldInfo GetFieldInfo([NotNull] Type type, [NotNull] string fieldName)
        {
            var field = type.GetTypeInfo().GetDeclaredField(fieldName);
            if (field == null)
                throw new ArgumentException($"Field '{fieldName}' was not found.", nameof(fieldName));

            return field;
        }


        [NotNull]
        private static PropertyInfo GetIndexedPropertyInfo<T>([NotNull] params Type[] index)
        {
            foreach (var property in typeof(T).GetTypeInfo().DeclaredProperties) {
                var indexParameters = property.GetIndexParameters().ToArray();
                if (indexParameters.Length == index.Length) {
                    var mismatch =
                        index.Where((indexType, i) => indexParameters[i].ParameterType != indexType).Any();

                    if (!mismatch) return property;
                }
            }

            throw new ArgumentException("Indexed property with index of type '" +
                                        $"{string.Join(", ", index.Select(type => type.Name))}'" +
                                        " was not found.", nameof(index));
        }

        [NotNull]
        private static PropertyInfo GetPropertyInfo<T>([NotNull] string propertyName)
        {
            return GetPropertyInfo(typeof(T), propertyName);
        }

        [NotNull]
        private static PropertyInfo GetPropertyInfo([NotNull] Type type, [NotNull] string propertyName)
        {
            var property = type.GetTypeInfo().GetDeclaredProperty(propertyName);
            if (property == null)
                throw new ArgumentException($"Property '{propertyName}' was not found.", nameof(propertyName));

            return property;
        }

        private static bool IsDerivedFromInterface([NotNull] Type target, [NotNull] Type iface,
                                                   [NotNull] IEnumerable<Type> targetInterfaces)
        {
            var targetTypeInfo = target.GetTypeInfo();
            var ifaceTypeInfo = iface.GetTypeInfo();
            var derived = false;

            if (!ifaceTypeInfo.IsGenericTypeDefinition) derived = ifaceTypeInfo.IsAssignableFrom(targetTypeInfo);
            else
                foreach (var targetInterface in targetInterfaces)
                    if (targetInterface.GetTypeInfo().IsGenericType &&
                        (targetInterface.GetGenericTypeDefinition() == iface)) {
                        derived = true;
                        break;
                    }

            return derived;
        }

        private static bool ParseTypeFullName(string typeFullName, out string assemblyName, out string className)
        {
            assemblyName = className = string.Empty;
            var commaIndex = typeFullName.IndexOf(",", StringComparison.Ordinal);
            if (commaIndex > 0) {
                className = typeFullName.Substring(0, commaIndex).Trim();
                assemblyName = typeFullName.Substring(commaIndex + 1).Trim();
                var asUri = assemblyName.ToUri();
                if (asUri != null) assemblyName = asUri.Segments.LastOrDefault();

                return true;
            }

            return false;
        }
    }
}