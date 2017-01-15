using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace Essentions
{
    /// <summary> Extenson methods for <see cref="XmlReader" />. </summary>
    public static class XmlReaderExtensions
    {
        /// <summary> Gets the specified xml attribute of the current element or a default value. </summary>
        /// <typeparam name="T">Type of the attribute value.</typeparam>
        /// <param name="reader">The xml reader.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="formatProvider">
        ///     The format provider for the type conversion.
        ///     Uses <see cref="CultureInfo.InvariantCulture" /> by default.
        /// </param>
        /// <returns> Read value or default value. </returns>
        [CanBeNull]
        public static T GetAttributeOrDefault<T>(
            [NotNull] this XmlReader reader,
            [NotNull] string attributeName,
            [CanBeNull] T defaultValue = default(T),
            [CanBeNull] IFormatProvider formatProvider = null)
        {
            Check.NotNull(reader)
                 .NotNullOrWhiteSpace(attributeName);

            var result = reader.GetAttribute(attributeName);
            return result != null
                ? result.Convert(defaultValue, formatProvider ?? CultureInfo.InvariantCulture)
                : defaultValue;
        }

        /// <summary>
        ///     Combine XmlReader and LINQ to XML by creating an <see cref="XElement" /> from an
        ///     <see cref="XmlReader" /> for the given element name.
        /// </summary>
        /// <remarks>
        ///     Thanks to Jon Skeet: http://stackoverflow.com/questions/2441673/reading-xml-with-xmlreader-in-c-sharp.
        /// </remarks>
        /// <param name="reader">XML reader.</param>
        /// <param name="elementName">Name of the element which will be turned into <see cref="XElement" />.</param>
        /// <returns><see cref="XElement" /> for each of document nodes named <paramref name="elementName" />.</returns>
        public static IEnumerable<XElement> ToXElement([NotNull] this XmlReader reader, [NotNull] string elementName)
        {
            Check.NotNull(reader)
                 .NotNullOrWhiteSpace(elementName);

            while (reader.Read())
                if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == elementName)) {
                    var element = XNode.ReadFrom(reader) as XElement;
                    if (element != null) yield return element;
                }
        }
    }
}