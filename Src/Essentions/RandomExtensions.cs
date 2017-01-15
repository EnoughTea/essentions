// Few methods were taken from Amaranth library (https://github.com/munificent/amaranth/),
// distributed under MIT license:
#region License
/*
Copyright (c) 2009-2016 Robert Nystrom

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
associated documentation files (the "Software"), to deal in the Software without restriction,
including without limitation the rights to use, copy, modify, merge, publish, distribute,
sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or
substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO
THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO
EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace Essentions
{
    /// <summary> Extension methods for <see cref="Random" />. </summary>
    public static class RandomExtensions
    {
        /// <summary>Generates string of the random symbols.</summary>
        /// <param name="rand">The random number generator.</param>
        /// <param name="length">Generated string length.</param>
        /// <param name="letters">Letters to use. ASCII by default</param>
        /// <param name="specialSymbols">Special symbols to use. ASCII by default.</param>
        /// <param name="specialCharChance">Chance to generate a special symbol instead of a letter.</param>
        /// <returns>New random string consisting of specified symbols.</returns>
        public static string NextString([NotNull] this Random rand, int length,
                                        string letters =
                                            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789",
                                        string specialSymbols =
                                            @"!""#$%&'()*+,-./:;<=>?@[\]^_`{|}~¡¢£¤¥¦§¨©ª«»¬­®¯°±²³´–—‘’‚“”„†‡•…‰" +
                                            "€™µ¶·¹÷×º¼½¾¿ ",
                                        double specialCharChance = 0.18)
        {
            Check.NotNull(rand)
                 .Greater(length, 0, "Length of the generated string should be >= 0", nameof(length));

            var generated = new StringBuilder(length);

            for (var i = 0; i < generated.Length; i++)
                if (rand.NextDouble() < specialCharChance)
                    generated.Append(specialSymbols[rand.Next(specialSymbols.Length)]);
                else generated.Append(letters[rand.Next(letters.Length)]);

            return generated.ToString();
        }

        /// <summary>Equally likely to return true or false. Uses <see cref="Random.Next()" />.</summary>
        /// <returns></returns>
        public static bool NextBoolean(this Random rand)
        {
            return rand.Next(2) > 0;
        }

        /// <summary> Returns a random byte in the range from zero to byte.MaxValue, inclusive. </summary>
        /// <param name="rand">The random number generator.</param>
        /// <returns> A random byte in the range from zero to byte.MaxValue, inclusive. </returns>
        public static byte NextByte([NotNull] this Random rand)
        {
            Check.NotNull(rand);

            return (byte)rand.Next(0, byte.MaxValue + 1);
        }

        /// <summary>Picks a character from the specified string.</summary>
        /// <param name="rand">The random number generator.</param>
        /// <param name="symbolsToPickFrom">The string to pick character from.</param>
        /// <returns>Random character form the specified string.</returns>
        public static char NextChar([NotNull] this Random rand, [NotNull] string symbolsToPickFrom)
        {
            Check.NotNull(rand)
                 .NotNull(symbolsToPickFrom);

            if (symbolsToPickFrom.Length == 0) return default(char);

            return symbolsToPickFrom[rand.Next(symbolsToPickFrom.Length)];
        }

        /// <summary> Returns DateTime in the range [min, max). </summary>
        /// <param name="rand">The random number generator.</param>
        /// <param name="min">The minimum limit for date range.</param>
        /// <param name="max">The maximum limit for date range.</param>
        /// <returns>DateTime in the range [min, max).</returns>
        public static DateTime NextDate([NotNull] this Random rand, DateTime min, DateTime max)
        {
            Check.NotNull(rand)
                 .Less(min, max);

            var minTicks = min.Ticks;
            var maxTicks = max.Ticks;
            var rn = (maxTicks - minTicks) * rand.NextDouble() + minTicks;
            return new DateTime((long)rn);
        }

        /// <summary> Returns a random date in the specified range. </summary>
        /// <param name="rand">The random number generator.</param>
        /// <param name="minYear">The min year.</param>
        /// <param name="maxYear">The max year.</param>
        /// <returns>A random date between given bounds.</returns>
        public static DateTime NextDate([NotNull] this Random rand, int minYear, int maxYear)
        {
            Check.NotNull(rand)
                 .InRange(minYear, 1, 9999)
                 .InRange(maxYear, minYear, 9999);

            var days = rand.NextDouble() * 356 * (maxYear - minYear);
            return new DateTime(minYear, 1, 1).AddDays(days);
        }

        /// <summary> Picks random <see cref="Enum" />. </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="rand">The random number generator.</param>
        /// <returns>Random Enum value.</returns>
        public static TEnum NextEnum<TEnum>([NotNull] this Random rand) where TEnum : struct
        {
            Check.NotNull(rand);

            var enumItems = Enum.GetValues(typeof(TEnum)) as TEnum[];
            Debug.Assert(enumItems != null);
            return enumItems.Length > 0 ? rand.NextItem(enumItems) : default(TEnum);
        }

        /// <summary> Returns an exponentially distributed, positive, random deviate of unit mean. </summary>
        /// <param name="rand">The random number generator.</param>
        /// <returns>An exponentially distributed, positive, random deviate of unit mean.</returns>
        public static double NextExponential([NotNull] this Random rand)
        {
            Check.NotNull(rand);

            var dum = 0d;
            while (dum.EqualsZero()) dum = rand.NextDouble();

            return -1d * Math.Log(dum, Math.E);
        }

        /// <summary> Gets the next sample value from the gaussian distribution with the Box-Muller algorithm. </summary>
        /// <param name="rand">The random number generator.</param>
        /// <param name="mean">The mean.</param>
        /// <param name="deviation">The deviation.</param>
        /// <returns>
        ///     The next pseudorandom, Gaussian ("normally") distributed double value with specified mean
        ///     and standard deviation.
        /// </returns>
        public static double NextNormal([NotNull] this Random rand, double mean, double deviation)
        {
            Check.NotNull(rand);

            return mean + rand.NextNormal() * deviation;
        }

        /// <summary>
        ///     Gets the next sample value from the gaussian distribution with the Box-Muller algorithm.
        /// </summary>
        /// <remarks> In case of GOTTA GO FAST read about Ziggurat gaussian sampler.</remarks>
        /// <param name="rand">The random number generator.</param>
        /// <returns>
        ///     The next pseudorandom, Gaussian ("normally") distributed double value with mean 0.0
        ///     and standard deviation 1.0.
        /// </returns>
        public static double NextNormal([NotNull] this Random rand)
        {
            Check.NotNull(rand);

            // Generate two new gaussian values.
            double x, y, sqr;

            // We need a non-zero random point inside the unit circle.
            do {
                x = 2.0 * rand.NextDouble() - 1.0;
                y = 2.0 * rand.NextDouble() - 1.0;
                sqr = x * x + y * y;
            } while ((sqr >= 1.0) || (sqr == 0));

            // Make the Box-Muller transformation.
            var fac = Math.Sqrt(-2.0 * Math.Log(sqr) / sqr);
            return x * fac;
        }

        /// <summary> Picks random item from the provided sequence. </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="rand">The random number generator.</param>
        /// <param name="items">The sequence to select value from.</param>
        /// <returns>Random item from the sequence.</returns>
        public static TItem NextItem<TItem>([NotNull] this Random rand, [NotNull] IEnumerable<TItem> items)
        {
            Check.NotNull(rand);

            #region IList
            var list = items as IList<TItem>;
            if (list != null) {
                if(list.Count == 0) {
                    return default(TItem);
                }

                return list[rand.Next(list.Count)];
            }
            #endregion

            #region ICollection
            var collection = items as ICollection<TItem>;
            if (collection != null) {
                int length = collection.Count;
                if (length == 0) {
                    return default(TItem);
                }

                return items.ElementAt(rand.Next(length));
            }
            #endregion

            var copy = items.ToArray();
            if (copy.Length == 0) {
                return default(TItem);
            }

            return copy[rand.Next(copy.Length)];
        }

        /// <summary>Picks item from the provided collection using the specified probabilities.</summary>
        /// <typeparam name="TItem">Type of the item.</typeparam>
        /// <param name="rand">The random number generator.</param>
        /// <param name="items">The collection to select value from.</param>
        /// <param name="probabilityProvider">Provides probability to get the given item.</param>
        /// <returns>Random item from the collection.</returns>
        public static TItem NextItem<TItem>(
            [NotNull] this Random rand,
            [NotNull] ICollection<TItem> items,
            [NotNull] Func<TItem, double> probabilityProvider)
        {
            Check.NotNull(rand)
                 .NotNull(probabilityProvider)
                 .Greater(items.Count, 0);

            double probabilitySum = items.Sum(probabilityProvider);
            double itemProbability = rand.NextDouble() * probabilitySum;
            double probabilityAccumulator = 0;

            foreach (var item in items) {
                probabilityAccumulator += probabilityProvider(item);
                if (probabilityAccumulator >= itemProbability) {
                    return item;
                }
            }

            return default(TItem);
        }

        /// <summary> Gets a random subset from the array. </summary>
        /// <typeparam name="TItem">Element type.</typeparam>
        /// <param name="rand">The random number generator.</param>
        /// <param name="items">Elements to chose from.</param>
        /// <param name="count">Number of elements to return.</param>
        /// <returns>Array that contains <paramref name="count" /> items from the original array</returns>
        public static TItem[] NextItems<TItem>([NotNull] this Random rand, [NotNull] TItem[] items, int count)
        {
            Check.NotNull(rand)
                 .Greater(count, 0)
                 .LessOrEqual(count, items.Length);

            var indexes = Enumerable.Range(0, items.Length).ToList();
            var result = new TItem[count];

            for (var i = 0; i < count; i++) {
                var next = rand.Next(indexes.Count);
                var index = indexes[next];
                indexes.RemoveAt(next);
                result[i] = items[index];
            }

            return result;
        }

        /// <summary>
        ///     Returns a random short in the range from <paramref name="min" /> to <paramref name="max" />,
        ///     exclusive.
        /// </summary>
        /// <param name="rand">The random number generator.</param>
        /// <param name="min">Minimum bound.</param>
        /// <param name="max">Maximum bound, exclusive.</param>
        /// <returns>
        ///     A random short in the range from <paramref name="min" /> to <paramref name="max" />, exclusive.
        /// </returns>
        public static short NextShort([NotNull] this Random rand, short min = 0, int max = short.MaxValue + 1)
        {
            Check.NotNull(rand)
                 .Less(min, max)
                 .LessOrEqual(max, short.MaxValue + 1, "Maximum short should be <= short.MaxValue + 1");

            return (short)rand.Next(min, max);
        }

        /// <summary> Generates a random floating-point number in the range [0, 1). </summary>
        /// <param name="rand">The random number generator to be used.</param>
        /// <returns>The generated number.</returns>
        public static float NextSingle([NotNull] this Random rand)
        {
            Check.NotNull(rand);

            return (float)rand.NextDouble();
        }

        /// <summary> Generates a random floating-point number between zero and a given bound. </summary>
        /// <param name="rand">The random number generator to be used.</param>
        /// <param name="exclusiveMax">The exclusive upper bound.</param>
        /// <returns>The generated number.</returns>
        public static float NextSingle([NotNull] this Random rand, float exclusiveMax)
        {
            Check.NotNull(rand);

            return rand.NextSingle() * exclusiveMax;
        }

        /// <summary> Returns TimeSpan in the range [min, max). </summary>
        /// <param name="rand">The random number generator.</param>
        /// <param name="min">The minimum limit for time span range.</param>
        /// <param name="max">The maximum limit for time span range.</param>
        /// <returns>TimeSpan in the range [min, max).</returns>
        public static TimeSpan NextTimeSpan([NotNull] this Random rand, TimeSpan min, TimeSpan max)
        {
            Check.NotNull(rand)
                 .Less(min, max);


            var minTicks = min.Ticks;
            var maxTicks = max.Ticks;
            var rn = (maxTicks - minTicks) * rand.NextDouble() + minTicks;
            return new TimeSpan((long)rn);
        }

        /// <summary>Generates values from a triangular distribution.</summary>
        /// <remarks>
        ///     See http://en.wikipedia.org/wiki/Triangular_distribution.
        /// </remarks>
        /// <param name="rand"></param>
        /// <param name="a">Minimum</param>
        /// <param name="b">Maximum</param>
        /// <param name="c">Mode (most frequent value)</param>
        /// <returns></returns>
        public static double NextTriangular([NotNull] this Random rand, double a, double b, double c)
        {
            Check.NotNull(rand);

            var u = rand.NextDouble();

            return u < (c - a) / (b - a)
                ? a + Math.Sqrt(u * (b - a) * (c - a))
                : b - Math.Sqrt((1 - u) * (b - a) * (b - c));
        }

        /// <summary>
        ///     Gets a random number centered around "center" with range "range" (inclusive)
        ///     using a triangular distribution. For example <see cref="NextTriangularInt" />(8, 4) will return values
        ///     between 4 and 12 (inclusive) which are leaning towards 8.
        /// </summary>
        /// <remarks>
        ///     Taken from https://github.com/munificent/amaranth/blob/master/Amaranth.Engine/Classes/Rng.cs
        ///     (MIT license).
        ///     This means output values will range from (center - range) to (center + range)
        ///     inclusive, with most values near the center, but not with a normal distribution.
        ///     Think of it as a poor man's bell curve.
        ///     The algorithm works essentially by choosing a random point inside the triangle,
        ///     and then calculating the x coordinate of that point. It works like this:
        ///     Consider Center 4, Range 3:
        ///     *
        ///     * | *
        ///     * | | | *
        ///     * | | | | | *
        ///     --+-----+-----+--
        ///     0 1 2 3 4 5 6 7 8
        ///     -r     c     r
        ///     Now flip the left half of the triangle (from 1 to 3) vertically and move it
        ///     over to the right so that we have a square.
        ///     +-------+
        ///     |       V
        ///     |
        ///     |   R L L L
        ///     | . R R L L
        ///     . . R R R L
        ///     . . . R R R R
        ///     --+-----+-----+--
        ///     0 1 2 3 4 5 6 7 8
        ///     Choose a point in that square. Figure out which half of the triangle the
        ///     point is in, and then remap the point back out to the original triangle.
        ///     The result is the x coordinate of the point in the original triangle.
        /// </remarks>
        /// <param name="rand">The random number generator.</param>
        /// <param name="center">Center of the triangle distribution.</param>
        /// <param name="range">Range of the triangle distribution in both directions from  specified center.</param>
        /// <returns>Random value on the triangle distribution.</returns>
        public static int NextTriangularInt([NotNull] this Random rand, int center, int range)
        {
            Check.NotNull(rand)
                 .GreaterOrEqual(range, 0);

            // pick a point in the square
            var x = rand.Next(range + 1);
            var y = rand.Next(range + 1);

            // figure out which triangle we are in
            if (x <= y) return center + x;

            // smaller triangle
            return center - range - 1 + x;
        }

        /// <summary> Returns true with odds of 1 to 'chance'. </summary>
        public static bool OneIn([NotNull] this Random rand, int chance)
        {
            Check.NotNull(rand)
                 .Greater(chance, 0);

            return rand.Next(chance) == 0;
        }

        /// <summary> Dice emulation. To emulate "4d6" call Dice(4, 6). </summary>
        /// <param name="rand">The random number generator.</param>
        /// <param name="numberOfDices">Number of the dices to roll.</param>
        /// <param name="numberOfSides">Sides count on the single dice.</param>
        /// <returns>Total roll score.</returns>
        public static int Roll([NotNull] this Random rand, int numberOfDices, int numberOfSides)
        {
            Check.NotNull(rand)
                 .Greater(numberOfDices, 0)
                 .Greater(numberOfSides, 0);

            var total = 0;
            for (var i = 0; i < numberOfDices; i++) total += rand.Next(numberOfSides) + 1;

            return total;
        }

        /// <summary> Randomly shuffles items within a list using Fisher-Yates shuffle. </summary>
        /// <param name="rand">The random number generator.</param>
        /// <param name="items">The list to shuffle.</param>
        public static void Shuffle<T>([NotNull] this Random rand, [NotNull] IList<T> items)
        {
            Check.NotNull(rand)
                 .NotNull(items);

            // Implementation of http://en.wikipedia.org/wiki/Fisher–Yates_shuffle :
            for (var i = items.Count - 1; i > 0; i--) {
                var swapIndex = rand.Next(i + 1);
                items.Swap(swapIndex, i);
            }
        }

        /// <summary>
        ///     Repeatedly increments a given starting value as long as random numbers continue to be chosen from
        ///     within a given range. Yields numbers whose probability gradually tapers off from the starting value.
        /// </summary>
        /// <remarks>
        ///     Taken from https://github.com/munificent/amaranth/blob/master/Amaranth.Engine/Classes/Rng.cs
        ///     (MIT license).
        /// </remarks>
        /// <param name="rand">The random number generator.</param>
        /// <param name="start">Starting value.</param>
        /// <param name="increment">Amount to modify starting value every successful iteration./// </param>
        /// <param name="chance">The odds of an iteration being successful.</param>
        /// <param name="outOf">The range to choose from to see if the iteration is successful.</param>
        /// <returns>The resulting value.</returns>
        public static int Taper([NotNull] this Random rand, int start, int increment, int chance, int outOf)
        {
            Check.NotNull(rand)
                 .NonZero(increment)
                 .Greater(chance, 0)
                 .Greater(outOf, 0)
                 .Less(chance, outOf);

            var value = start;
            while (rand.Next(outOf) < chance) value += increment;

            return value;
        }

        /// <summary>
        ///     Randomly changes the given starting value repeatedly up or down by one with the given
        ///     probabilities. Will only walk in one direction.
        /// </summary>
        /// <remarks>
        ///     Taken from https://github.com/munificent/amaranth/blob/master/Amaranth.Engine/Classes/Rng.cs
        ///     (MIT license).
        /// </remarks>
        /// <param name="rand">The random number generator.</param>
        /// <param name="start">Value to start at.</param>
        /// <param name="oneInToDec">The chance of decreasing, odds are one to <paramref name="oneInToDec" />.</param>
        /// <param name="oneInToInc">The chance of increasing, odds are one to <paramref name="oneInToInc" />.</param>
        /// <returns>Value after random walking.</returns>
        public static int Walk([NotNull] this Random rand, int start, int oneInToDec, int oneInToInc)
        {
            Check.NotNull(rand)
                 .GreaterOrEqual(oneInToDec, 0)
                 .NotEqual(oneInToDec, 1)
                 .GreaterOrEqual(oneInToInc, 0)
                 .NotEqual(oneInToInc, 1);

            var sanity = 10000;

            // decide if walking up or down
            var direction = rand.Next(oneInToDec + oneInToInc);
            if (direction < oneInToDec) while (rand.OneIn(oneInToDec) && (sanity-- > 0)) start--;
            else if (direction < oneInToDec + oneInToInc) while (rand.OneIn(oneInToInc) && (sanity-- > 0)) start++;

            return start;
        }
    }
}