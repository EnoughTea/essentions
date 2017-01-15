using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Essentions
{
    /// <summary> Extension methods for functions. </summary>
    public static class FunctionExtensions
    {
        /// <summary>
        ///     Transforms a function that takes n arguments into a function that takes only one argument and
        ///     returns a curried function of n-1 arguments.
        /// </summary>
        /// <typeparam name="TA">The first argument type.</typeparam>
        /// <typeparam name="TB">The second argument type.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="f">The function to curry.</param>
        /// <returns>The curried function.</returns>
        /// <remarks>
        ///     See Wes Dyers article Currying and Partial Function Application
        ///     (http://blogs.msdn.com/wesdyer/archive/2007/01/29/currying-and-partial-function-application.aspx)
        /// </remarks>
        [NotNull]
        public static Func<TA, Func<TB, TResult>> Curry<TA, TB, TResult>(
            [NotNull] this Func<TA, TB, TResult> f)
        {
            Check.NotNull(f);

            return a => Partial(f, a);
        }

        /// <summary>Memoizes the specified binary function.</summary>
        /// <typeparam name="TA">Type of the argument.</typeparam>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="f">Function to memoize.</param>
        /// <returns>Memoized function.</returns>
        [NotNull]
        public static Func<TA, TResult> Memoize<TA, TResult>([NotNull] this Func<TA, TResult> f)
        {
            Check.NotNull(f);

            var cache = new Dictionary<TA, TResult>();
            return wrapped => {
                TResult result;
                if (!cache.TryGetValue(wrapped, out result)) {
                    result = f(wrapped);
                    cache.Add(wrapped, result);
                }

                return result;
            };
        }

        /// <summary>Memoizes the specified binary function.</summary>
        /// <typeparam name="TA">Type of the first argument.</typeparam>
        /// <typeparam name="TB">Type of the second argument.</typeparam>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="f">Function to memoize.</param>
        /// <returns>Memoized function.</returns>
        [NotNull]
        public static Func<TA, TB, TResult> Memoize<TA, TB, TResult>(
            [NotNull] this Func<TA, TB, TResult> f)
        {
            Check.NotNull(f);

            return f.Tuplify().Memoize().Untuplify();
        }

        /// <summary> Partial takes a function and an argument as the first argument of the function. </summary>
        /// <typeparam name="TA">The first argument to the function.</typeparam>
        /// <typeparam name="TB">The second argument to the function.</typeparam>
        /// <typeparam name="TResult">The result of the function.</typeparam>
        /// <param name="f">The function.</param>
        /// <param name="a">The first argument of the function.</param>
        /// <returns> A function which takes the remaining arguments, and then applies all the arguments to the original function. </returns>
        /// <remarks>
        ///     See Wes Dyers article Currying and Partial Function Application
        ///     (http://blogs.msdn.com/wesdyer/archive/2007/01/29/currying-and-partial-function-application.aspx)
        /// </remarks>
        [NotNull]
        public static Func<TB, TResult> Partial<TA, TB, TResult>([NotNull] this Func<TA, TB, TResult> f, TA a)
        {
            Check.NotNull(f);

            return b => f(a, b);
        }

        /// <summary> Convers function with two arguments into a function with a single argument.</summary>
        /// <typeparam name="TA">Type of the first argument.</typeparam>
        /// <typeparam name="TB">Type of the second argument.</typeparam>
        /// <typeparam name="TResult">Type of the function result.</typeparam>
        /// <param name="f">The function to tuplify.</param>
        /// <returns>Function with a single argument.</returns>
        [NotNull]
        public static Func<Tuple<TA, TB>, TResult> Tuplify<TA, TB, TResult>([NotNull] this Func<TA, TB, TResult> f)
        {
            Check.NotNull(f);

            return t => f(t.Item1, t.Item2);
        }

        /// <summary> Convers function with three arguments into a function with a single argument.</summary>
        /// <typeparam name="TA">Type of the first argument.</typeparam>
        /// <typeparam name="TB">Type of the second argument.</typeparam>
        /// <typeparam name="TC">Type of the third argument.</typeparam>
        /// <typeparam name="TResult">Type of the function result.</typeparam>
        /// <param name="f">The function to tuplify.</param>
        /// <returns>Function with a single argument.</returns>
        [NotNull]
        public static Func<Tuple<TA, TB, TC>, TResult> Tuplify<TA, TB, TC, TResult>(
            [NotNull] this Func<TA, TB, TC, TResult> f)
        {
            Check.NotNull(f);

            return t => f(t.Item1, t.Item2, t.Item3);
        }

        /// <summary> Convers function with single tuple argument into a function with a multiple arguments.</summary>
        /// <typeparam name="TA">Type of the first tuple element.</typeparam>
        /// <typeparam name="TB">Type of the second tuple element.</typeparam>
        /// <typeparam name="TResult">Type of the function result.</typeparam>
        /// <param name="f">The function to untuplify.</param>
        /// <returns>Function with a multiple arguments.</returns>
        [NotNull]
        public static Func<TA, TB, TResult> Untuplify<TA, TB, TResult>([NotNull] this Func<Tuple<TA, TB>, TResult> f)
        {
            Check.NotNull(f);

            return (a, b) => f(Tuple.Create(a, b));
        }

        /// <summary> Convers function with single tuple argument into a function with a multiple arguments.</summary>
        /// <typeparam name="TA">Type of the first tuple element.</typeparam>
        /// <typeparam name="TB">Type of the second tuple element.</typeparam>
        /// <typeparam name="TC">Type of the third tuple element.</typeparam>
        /// <typeparam name="TResult">Type of the function result.</typeparam>
        /// <param name="f">The function to untuplify.</param>
        /// <returns>Function with a multiple arguments.</returns>
        [NotNull]
        public static Func<TA, TB, TC, TResult> Untuplify<TA, TB, TC, TResult>(
            [NotNull] this Func<Tuple<TA, TB, TC>, TResult> f)
        {
            Check.NotNull(f);

            return (a, b, c) => f(Tuple.Create(a, b, c));
        }
    }
}