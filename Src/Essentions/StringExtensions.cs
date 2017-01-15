using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Essentions
{
    /// <summary> Various string extension methods. </summary>
    public static class StringExtensions
    {
        private static readonly Regex _HexNumber = new Regex(@"^\s*(0[xX]|&[hH])?[0-9a-fA-F]+\s*$",
                                                             RegexOptions.CultureInvariant);

        /// <summary>
        ///     Returns the part of a string that precedes the first occurrence, if any, of a given character.
        /// </summary>
        /// <param name="text">The initial string.</param>
        /// <param name="character">The character to be found.</param>
        /// <returns>
        ///     The part of the string preceding that character, or the initial string if the character was not
        ///     found.
        /// </returns>
        [NotNull]
        public static string BeforeFirst([NotNull] this string text, char character)
        {
            Check.NotNull(text);

            var characterIndex = text.IndexOf(character);
            if (characterIndex == -1) return text;

            if (characterIndex == 0) return string.Empty;

            return text.Substring(0, characterIndex);
        }

        /// <summary>
        ///     Returns the part of a string that precedes the last occurrence, if any, of a given character.
        /// </summary>
        /// <param name="text">The initial string.</param>
        /// <param name="character">The character to be found.</param>
        /// <returns>
        ///     The part of the string preceding the last occurrence that character,
        ///     or the initial string if the character was not found.
        /// </returns>
        [NotNull]
        public static string BeforeLast([NotNull] this string text, char character)
        {
            Check.NotNull(text);

            var characterIndex = text.LastIndexOf(character);
            if (characterIndex == -1) return text;

            if (characterIndex == 0) return string.Empty;

            return text.Substring(0, characterIndex);
        }

        /// <summary>
        ///     Uses <see cref="Convert.ChangeType(object, Type, IFormatProvider)" />
        ///     to convert the string representation of a primitive type or an enum to the actual type.
        /// </summary>
        /// <typeparam name="T">Type to convert to.</typeparam>
        /// <param name="text">The string to convert.</param>
        /// <param name="defaultValue">The default value if string cannot be converted.</param>
        /// <param name="formatProvider">
        ///     Format provider used for type change. Uses
        ///     <see cref="CultureInfo.InvariantCulture" /> by default.
        /// </param>
        /// <returns>Value resulting from a casted string.</returns>
        [CanBeNull]
        public static T Convert<T>([NotNull] this string text,
                                   [CanBeNull] T defaultValue = default(T),
                                   [CanBeNull] IFormatProvider formatProvider = null)
        {
            Check.NotNull(text);

            var type = typeof(T);
            var result = defaultValue;

            try {
                if (type.GetTypeInfo().IsEnum) {
                    result = (T)Enum.Parse(type, text, true);
                }
                else if (IsConvertableType(type)) {
                    result = (T)System.Convert.ChangeType(text, type, formatProvider ?? CultureInfo.InvariantCulture);
                }
                else {
                    var underlyingType = Nullable.GetUnderlyingType(type);
                    if (IsConvertableType(underlyingType))
                        result = (T)System.Convert.ChangeType(text, underlyingType,
                                                              formatProvider ?? CultureInfo.InvariantCulture);
                }
            }
            catch (InvalidCastException) {
            }
            catch (FormatException) {
            }
            catch (OverflowException) {
            }
            catch (ArgumentException) {
            }

            return result;
        }

        /// <summary> Checks whether the specified string's end checks against a given predicate. </summary>
        /// <param name="text">The string to check.</param>
        /// <param name="lastCharTest">Predicate for the last character in the given string.</param>
        /// <returns>true if the given string's end checks against a given predicate; false otherwise.</returns>
        public static bool EndsWith([NotNull] this string text, [NotNull] Predicate<char> lastCharTest)
        {
            Check.NotNull(text)
                 .NotNull(lastCharTest);

            return (text.Length > 0) && lastCharTest(text[text.Length - 1]);
        }

        /// <summary> Converts hex representation of the string into actual string. </summary>
        /// <param name="text">The self value.</param>
        /// <param name="separator">The byte separator.</param>
        /// <returns>String converted from hex.</returns>
        [NotNull]
        public static string FromHex([NotNull] this string text, [NotNull] string separator = "%")
        {
            Check.NotNull(text)
                 .NotNullOrEmpty(separator);

            var result = new StringBuilder();
            var byteStrings = text.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var byteString in byteStrings)
                result.Append(char.ConvertFromUtf32(System.Convert.ToInt32(byteString, 16)));

            return result.ToString();
        }

        /// <summary> Determines whether the string is lowercase. </summary>
        /// <param name="text">The target string.</param>
        /// <returns>
        ///     <c>true</c> if the string is lowercase; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsLower([NotNull] this string text)
        {
            Check.NotNull(text);

            // Consider string to be lowercase if it has no uppercase letters.
            var isLower = true;
            foreach (var letter in text)
                if (char.IsUpper(letter)) {
                    isLower = false;
                    break;
                }

            return isLower;
        }

        /// <summary>
        ///     Determines whether the specified string is a string representation of a number.
        ///     By default looks for all number styles, including hexadecimal numbers.
        /// </summary>
        /// <param name="text">The string to test.</param>
        /// <param name="allowedStyles">
        ///     A bitwise combination of <see cref="NumberStyles" /> values that indicates
        ///     the permitted format of the string. A typical value to specify is <see cref="NumberStyles.Float" /> combined
        ///     with <see cref="NumberStyles.AllowThousands" />.
        ///     Default is <see cref="NumberStyles.Any" /> combined with <see cref="NumberStyles.AllowHexSpecifier" />.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the specified string can be converted to a number; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNumeric([NotNull] this string text,
                                     NumberStyles allowedStyles = NumberStyles.Any | NumberStyles.AllowHexSpecifier)
        {
            Check.NotNull(text);

            // Use regex to search for hexes, since int.TryParse does not like 0x prefix.
            if ((allowedStyles & NumberStyles.AllowHexSpecifier) != 0) {
                if (_HexNumber.IsMatch(text)) return true;
                // We will need to continue searching, and AllowHexSpecifier does not work with other number styles.
                allowedStyles &= ~NumberStyles.AllowHexSpecifier;
            }

            double result;
            bool local, invariant;
            invariant = local = double.TryParse(text, allowedStyles, CultureInfo.CurrentCulture, out result);
            if (!local) invariant = double.TryParse(text, allowedStyles, CultureInfo.InvariantCulture, out result);

            return local || invariant;
        }

        /// <summary> Determines whether the string is uppercase. </summary>
        /// <param name="text">The target string.</param>
        /// <returns>
        ///     <c>true</c> if the string is uppercase; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsUpper([NotNull] this string text)
        {
            Check.NotNull(text);

            // Consider string to be uppercase if it has no lowercase letters.)
            var isUpper = true;
            foreach (var letter in text)
                if (char.IsLower(letter)) {
                    isUpper = false;
                    break;
                }

            return isUpper;
        }

        /// <summary> Parses the specified string into the <typeparamref name="TEnum" />, ignoring the case. </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="text">The self value.</param>
        /// <returns>Parsed enum</returns>
        public static TEnum ParseEnum<TEnum>([NotNull] this string text) where TEnum : struct
        {
            Check.NotNull(text);

            return (TEnum)Enum.Parse(typeof(TEnum), text, true);
        }

        /// <summary> Removes a substring from a string. </summary>
        /// <param name="text">The original string.</param>
        /// <param name="value">The substring to be removed.</param>
        /// <returns>The resulting string.</returns>
        public static string Remove([NotNull] this string text, [NotNull] string value)
        {
            Check.NotNull(text)
                 .NotNullOrEmpty(value);

            return text.Replace(value, string.Empty);
        }

        /// <summary> Removes specified characters from a string. </summary>
        /// <remarks>
        ///     Uses iterator-based approach, so in case of GOTTA GO FAST use
        ///     <see cref="Regex.Replace(string, string)" /> instead.
        /// </remarks>
        /// <param name="text">The original string.</param>
        /// <param name="charsToRemove">String with chars to remove.</param>
        /// <returns>String with given characters removed.</returns>
        public static string RemoveChars([NotNull] this string text, [NotNull] string charsToRemove)
        {
            Check.NotNull(text)
                 .NotNullOrEmpty(charsToRemove);

            var cleaned = text;
            foreach (var charToRemove in charsToRemove)
                cleaned = cleaned.Replace(charToRemove.ToString(), string.Empty);

            return cleaned;
        }

        /// <summary> Repeats given string for the specified amount of times. </summary>
        /// <param name="text">String to repeat.</param>
        /// <param name="count">Number of times given string must be repeated.</param>
        /// <returns>The resulting string.</returns>
        [NotNull]
        public static string Repeat([NotNull] this string text, int count)
        {
            Check.NotNull(text)
                 .GreaterOrEqual(count, 0);

            return new StringBuilder().Insert(0, text, count).ToString();
        }

        /// <summary>
        ///     Returns a string array that contains the substrings in this string that are
        ///     delimited by the specified separator. A parameter specifies whether to return empty array elements.
        /// </summary>
        /// <param name="value">String to split using soecified separator.</param>
        /// <param name="separator">String that delimit the substrings in this string, an empty string or null.</param>
        /// <param name="removeEmptyLines">
        ///     set to <c>true</c> to omit empty array elements from the array returned;
        ///     or <c>false</c> to include empty array elements in the array returned.
        /// </param>
        /// <returns>
        ///     An array whose elements contain the substrings in this string that are delimited by given
        ///     separator.
        /// </returns>
        public static string[] Split([NotNull] this string value,
                                     [CanBeNull] string separator,
                                     bool removeEmptyLines = true)
        {
            Check.NotNull(value);

            var options = removeEmptyLines ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None;
            return value.Split(new[] { separator }, options);
        }

        /// <summary> Checks whether the specified string's start checks against a given predicate. </summary>
        /// <param name="text">The string to check.</param>
        /// <param name="firstCharTest">Predicate for the first character in the given string.</param>
        /// <returns>true if the given string's start checks against a given predicate; false otherwise.</returns>
        public static bool StartsWith([NotNull] this string text, [NotNull] Predicate<char> firstCharTest)
        {
            Check.NotNull(text)
                 .NotNull(firstCharTest);

            return (text.Length > 0) && firstCharTest(text[0]);
        }

        /// <summary>
        ///     Retrieves a substring from this instance. The substring starts from the beginning of this instance
        ///     and continues until first occurrence of the target string.
        /// </summary>
        /// <example>
        ///     <code>
        /// string test = "This is test string, repeat, test string";
        /// Assert.That(test.Substring("test"), Is.EqualTo("This is "));
        /// </code>
        /// </example>
        /// <param name="text">Instance to retrieve substring from.</param>
        /// <param name="target">The string to search for.</param>
        /// <returns>
        ///     A string that is that begins from the beginning of this instance and ends at the target string position,
        ///     or empty string instance is empty.
        /// </returns>
        public static string Substring([NotNull] this string text, [NotNull] string target)
        {
            return Substring(text, 0, target);
        }

        /// <summary>
        ///     Retrieves a substring from this instance. The substring starts at a specified
        ///     character position and continues until first occurrence of the target string.
        /// </summary>
        /// <example>
        ///     <code>
        /// string test = "This is test string, repeat, test string";
        /// Assert.That(test.Substring(0, "test"), Is.EqualTo("This is "));
        /// Assert.That(test.Substring(8, "test"), Is.EqualTo(""));
        /// Assert.That(test.Substring(9, "test"), Is.EqualTo("est string, repeat, "));
        /// </code>
        /// </example>
        /// <param name="text">Instance to retrieve substring from.</param>
        /// <param name="startIndex">Search for a target string starts at this index. </param>
        /// <param name="target">The string to search for.</param>
        /// <returns>
        ///     A string that is that begins at startIndex in this instance and ends at the target string position,
        ///     or empty string if <paramref name="startIndex" /> is equal to the length of this instance or
        ///     instance is empty.
        /// </returns>
        [NotNull]
        public static string Substring([NotNull] this string text, int startIndex, [NotNull] string target)
        {
            Check.NotNull(text)
                 .NotNullOrEmpty(target)
                 .GreaterOrEqual(startIndex, 0);

            var result = string.Empty;
            if (text != string.Empty) {
                var targetIndex = text.IndexOf(target, startIndex, StringComparison.OrdinalIgnoreCase);
                result = targetIndex >= 0
                    ? text.Substring(startIndex, targetIndex - startIndex)
                    : text.Substring(startIndex);
            }

            return result;
        }

        /// <summary> Converts UTF8 string to its Base64 representation. </summary>
        /// <param name="text">The self.</param>
        /// <returns>Base64 representation of the UTF8 string.</returns>
        [NotNull]
        public static string ToBase64([NotNull] this string text)
        {
            Check.NotNull(text);

            return System.Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
        }

        /// <summary> Converts string to its hex representation. </summary>
        /// <param name="text">The self value.</param>
        /// <param name="separator">The byte separator.</param>
        /// <returns>Hex representation of the string.</returns>
        [NotNull]
        public static string ToHex([NotNull] this string text, [NotNull] string separator = "%")
        {
            Check.NotNull(text)
                 .NotNullOrEmpty(separator);

            var result = new StringBuilder();
            foreach (var letter in text) {
                var value = System.Convert.ToInt32(letter);
                result.Append(separator);
                result.Append(value.ToString("x", CultureInfo.InvariantCulture));
            }

            return result.ToString();
        }

        /// <summary> Creates a memory stream and writes this string to it. Uses UT8 encoding by default. </summary>
        /// <param name="text">String to write to stream.</param>
        /// <param name="encoding">Encoding to use. UTF8 by default.</param>
        /// <returns>Memory stream with string written to it.</returns>
        [NotNull]
        public static MemoryStream ToMemoryStream([NotNull] this string text, [CanBeNull] Encoding encoding = null)
        {
            Check.NotNull(text);

            return new MemoryStream((encoding ?? Encoding.UTF8).GetBytes(text));
        }

        /// <summary>Returns URI corresponding to this string.</summary>
        /// <param name="text">The target string.</param>
        /// <returns>URI corresponding to the specified string.</returns>
        [CanBeNull]
        public static Uri ToUri([NotNull] this string text)
        {
            Check.NotNull(text);

            try {
                return new Uri(text);
            }
            catch {
                // ignored
            }

            return null;
        }

        /// <summary>Truncates string to given length if needed. </summary>
        /// <param name="text">The target string.</param>
        /// <param name="length">Desired maximum length.</param>
        /// <returns>Truncated string.</returns>
        public static string Truncate([NotNull] this string text, int length)
        {
            Check.NotNull(text)
                 .GreaterOrEqual(length, 0);

            return text.Length > length ? text.Substring(0, length) : text;
        }

        internal static bool IsConvertableType([CanBeNull] Type type)
        {
            if (type == null) {
                return false;
            }

            return (type == typeof(string)) ||
                   (type == typeof(decimal)) ||
                   (type == typeof(DateTime)) ||
                   (type == typeof(float)) ||
                   (type == typeof(double)) ||
                   (type == typeof(long)) ||
                   (type == typeof(ulong)) ||
                   (type == typeof(int)) ||
                   (type == typeof(uint)) ||
                   (type == typeof(short)) ||
                   (type == typeof(ushort)) ||
                   (type == typeof(byte)) ||
                   (type == typeof(sbyte)) ||
                   (type == typeof(char)) ||
                   (type == typeof(bool)) ||
                   (type == typeof(object)) ||
                   // 2 types below are inaccessible to us, so let's check by their full name and
                   // hope there will never be someone insane enough to cause a name clash with standard namespace.
                   type.GetTypeInfo().FullName.Equals("System.DBNull", StringComparison.OrdinalIgnoreCase) ||
                   type.GetTypeInfo().ImplementedInterfaces
                       .Any(iface => iface.FullName.Equals("System.IConvertible", StringComparison.OrdinalIgnoreCase));
        }
    }
}