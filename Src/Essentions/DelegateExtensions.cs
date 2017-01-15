using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading;
using JetBrains.Annotations;

namespace Essentions
{
    /// <summary> Extension methods for delegates. </summary>
    public static class DelegateExtensions
    {
        /// <summary>
        ///     Performs invocation of the predicate delegate making sure that it exists first.
        ///     Can be called on null.
        /// </summary>
        /// <param name="handler">The predicate to be invoked.</param>
        /// <param name="arg">The argument associated with the predicate.</param>
        /// <param name="defaultResult">Result to return if delegate is null.</param>
        public static bool Call<T>(
            [CanBeNull] this Predicate<T> handler,
            [CanBeNull] T arg,
            bool defaultResult = false)
        {
            var currentHandler = Volatile.Read(ref handler);
            return currentHandler?.Invoke(arg) ?? defaultResult;
        }

        /// <summary>
        ///     Performs invocation of the comparison delegate making sure that it exists first.
        ///     Can be called on null.
        /// </summary>
        /// <param name="handler">The comparison to be invoked.</param>
        /// <param name="arg">The first instance to compare.</param>
        /// <param name="arg2">The second instance to compare.</param>
        /// <param name="defaultResult">Result to return if delegate is null.</param>
        public static int Call<T>(
            [CanBeNull] this Comparison<T> handler,
            [CanBeNull] T arg, [CanBeNull] T arg2,
            int defaultResult = 0)
        {
            var currentHandler = Volatile.Read(ref handler);
            return currentHandler?.Invoke(arg, arg2) ?? defaultResult;
        }

        /// <summary>
        ///     Performs invocation of the delegate making sure that it exists first.
        ///     Can be called on null.
        /// </summary>
        /// <param name="handler">The event handler to be invoked.</param>
        /// <param name="sender">The object which raises the event.</param>
        /// <param name="args">The arguments associated with the event.</param>
        public static void Call(
            [CanBeNull] this EventHandler handler,
            [CanBeNull] object sender,
            [CanBeNull] EventArgs args)
        {
            //  Avoid a mutation between the reads.
            var currentHandler = Volatile.Read(ref handler);
            currentHandler?.Invoke(sender, args);
        }

        /// <summary>
        ///     Performs invocation of the delegate making sure that it exists first.
        ///     Can be called on null.
        /// </summary>
        /// <param name="handler">The event handler to be invoked.</param>
        /// <param name="sender">The object which raises the event.</param>
        public static void Call([CanBeNull] this EventHandler handler, [CanBeNull] object sender = null)
        {
            var currentHandler = Volatile.Read(ref handler);
            currentHandler?.Invoke(sender, EventArgs.Empty);
        }

        /// <summary>
        ///     Performs invocation of the delegate making sure that it exists first.
        ///     Can be called on null.
        /// </summary>
        /// <typeparam name="TArgs">The type of the arguments associated with the event.</typeparam>
        /// <param name="handler">The event handler to be invoked.</param>
        /// <param name="sender">The object which raises the event.</param>
        /// <param name="args">The arguments associated with the event.</param>
        public static void Call<TArgs>(
            [CanBeNull] this EventHandler<TArgs> handler,
            [CanBeNull] object sender,
            [CanBeNull] TArgs args)
        {
            var currentHandler = Volatile.Read(ref handler);
            currentHandler?.Invoke(sender, args);
        }

        /// <summary>
        ///     Performs invocation of the delegate making sure that it exists first.
        ///     Can be called on null.
        /// </summary>
        /// <typeparam name="TArgs">The type of the arguments associated with the event.</typeparam>
        /// <param name="handler">The event handler to be invoked.</param>
        /// <param name="args">The arguments associated with the event.</param>
        public static void Call<TArgs>([CanBeNull] this EventHandler<TArgs> handler, [CanBeNull] TArgs args)
        {
            var currentHandler = Volatile.Read(ref handler);
            currentHandler?.Invoke(null, args);
        }

        /// <summary>
        ///     Performs invocation of the delegate making sure that it exists first.
        ///     Can be called on null.
        /// </summary>
        /// <param name="handler">The event handler to be invoked.</param>
        public static void Call([CanBeNull] this Action handler)
        {
            var currentHandler = Volatile.Read(ref handler);
            currentHandler?.Invoke();
        }

        /// <summary>
        ///     Performs invocation of the delegate making sure that it exists first.
        ///     Can be called on null.
        /// </summary>
        /// <param name="handler">The event handler to be invoked.</param>
        /// <param name="arg">The argument associated with the event.</param>
        public static void Call<TArg>([CanBeNull] this Action<TArg> handler, [CanBeNull] TArg arg)
        {
            var currentHandler = Volatile.Read(ref handler);
            currentHandler?.Invoke(arg);
        }

        /// <summary>
        ///     Performs invocation of the delegate making sure that it exists first.
        ///     Can be called on null.
        /// </summary>
        /// <param name="handler">The event handler to be invoked.</param>
        /// <param name="arg1">The first argument associated with the event.</param>
        /// <param name="arg2">The second argument associated with the event.</param>
        public static void Call<TArg1, TArg2>(
            [CanBeNull] this Action<TArg1, TArg2> handler,
            [CanBeNull] TArg1 arg1,
            [CanBeNull] TArg2 arg2)
        {
            var currentHandler = Volatile.Read(ref handler);
            currentHandler?.Invoke(arg1, arg2);
        }

        /// <summary>
        ///     Performs invocation of the delegate making sure that it exists first.
        ///     Can be called on null.
        /// </summary>
        /// <param name="handler">The event handler to be invoked.</param>
        /// <param name="arg1">The first argument associated with the event.</param>
        /// <param name="arg2">The second argument associated with the event.</param>
        /// <param name="arg3">The third argument associated with the event.</param>
        public static void Call<TArg1, TArg2, TArg3>(
            [CanBeNull] this Action<TArg1, TArg2, TArg3> handler,
            [CanBeNull] TArg1 arg1,
            [CanBeNull] TArg2 arg2,
            [CanBeNull] TArg3 arg3)
        {
            var currentHandler = Volatile.Read(ref handler);
            currentHandler?.Invoke(arg1, arg2, arg3);
        }

        /// <summary>
        ///     Performs invocation of the delegate making sure that it exists first.
        ///     Can be called on null.
        /// </summary>
        /// <param name="handler">The event handler to be invoked.</param>
        /// <param name="arg1">The first argument associated with the event.</param>
        /// <param name="arg2">The second argument associated with the event.</param>
        /// <param name="arg3">The third argument associated with the event.</param>
        /// <param name="arg4">The fourth argument associated with the event.</param>
        public static void Call<TArg1, TArg2, TArg3, TArg4>(
            [CanBeNull] this Action<TArg1, TArg2, TArg3, TArg4> handler,
            [CanBeNull] TArg1 arg1,
            [CanBeNull] TArg2 arg2,
            [CanBeNull] TArg3 arg3,
            [CanBeNull] TArg4 arg4)
        {
            var currentHandler = Volatile.Read(ref handler);
            currentHandler?.Invoke(arg1, arg2, arg3, arg4);
        }

        /// <summary>
        ///     Performs invocation of the delegate making sure that it exists first.
        ///     Can be called on null.
        /// </summary>
        /// <param name="handler">The event handler to be invoked.</param>
        /// <param name="arg1">The first argument associated with the event.</param>
        /// <param name="arg2">The second argument associated with the event.</param>
        /// <param name="arg3">The third argument associated with the event.</param>
        /// <param name="arg4">The fourth argument associated with the event.</param>
        /// <param name="arg5">The fifth argument associated with the event.</param>
        public static void Call<TArg1, TArg2, TArg3, TArg4, TArg5>(
            [CanBeNull] this Action<TArg1, TArg2, TArg3, TArg4, TArg5> handler,
            [CanBeNull] TArg1 arg1,
            [CanBeNull] TArg2 arg2,
            [CanBeNull] TArg3 arg3,
            [CanBeNull] TArg4 arg4,
            [CanBeNull] TArg5 arg5)
        {
            var currentHandler = Volatile.Read(ref handler);
            currentHandler?.Invoke(arg1, arg2, arg3, arg4, arg5);
        }

        /// <summary>
        ///     Performs invocation of the delegate making sure that it exists first.
        ///     Can be called on null.
        /// </summary>
        /// <param name="handler">The event handler to be invoked.</param>
        /// <param name="arg1">The first argument associated with the event.</param>
        /// <param name="arg2">The second argument associated with the event.</param>
        /// <param name="arg3">The third argument associated with the event.</param>
        /// <param name="arg4">The fourth argument associated with the event.</param>
        /// <param name="arg5">The fifth argument associated with the event.</param>
        /// <param name="arg6">The sixth argument associated with the event.</param>
        public static void Call<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(
            [CanBeNull] this Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> handler,
            [CanBeNull] TArg1 arg1,
            [CanBeNull] TArg2 arg2,
            [CanBeNull] TArg3 arg3,
            [CanBeNull] TArg4 arg4,
            [CanBeNull] TArg5 arg5,
            [CanBeNull] TArg6 arg6)
        {
            var currentHandler = Volatile.Read(ref handler);
            currentHandler?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6);
        }

        /// <summary>
        ///     Performs invocation of the delegate making sure that it exists first.
        ///     Can be called on null.
        /// </summary>
        /// <param name="handler">The event handler to be invoked.</param>
        /// <param name="defaultResult">The default result to use when handler was null.</param>
        /// <returns>Event result.</returns>
        [CanBeNull]
        public static TResult Call<TResult>(
            [CanBeNull] this Func<TResult> handler,
            [CanBeNull] TResult defaultResult = default(TResult))
        {
            var currentHandler = Volatile.Read(ref handler);
            return currentHandler != null ? currentHandler() : defaultResult;
        }

        /// <summary>
        ///     Performs invocation of the delegate making sure that it exists first.
        ///     Can be called on null.
        /// </summary>
        /// <param name="handler">The event handler to be invoked.</param>
        /// <param name="arg">The argument associated with the event.</param>
        /// <param name="defaultResult">The default result to use when handler was null.</param>
        /// <returns>Event result.</returns>
        [CanBeNull]
        public static TResult Call<TArg, TResult>(
            [CanBeNull] this Func<TArg, TResult> handler,
            [CanBeNull] TArg arg,
            [CanBeNull] TResult defaultResult = default(TResult))
        {
            var currentHandler = Volatile.Read(ref handler);
            return currentHandler != null ? currentHandler(arg) : defaultResult;
        }

        /// <summary>
        ///     Performs invocation of the delegate making sure that it exists first.
        ///     Can be called on null.
        /// </summary>
        /// <param name="handler">The event handler to be invoked.</param>
        /// <param name="arg1">The first argument associated with the event.</param>
        /// <param name="arg2">The second argument associated with the event.</param>
        /// <param name="defaultResult">The default result to use when handler was null.</param>
        /// <returns>Event result.</returns>
        [CanBeNull]
        public static TResult Call<TArg1, TArg2, TResult>(
            [CanBeNull] this Func<TArg1, TArg2, TResult> handler,
            [CanBeNull] TArg1 arg1,
            [CanBeNull] TArg2 arg2,
            [CanBeNull] TResult defaultResult = default(TResult))
        {
            var currentHandler = Volatile.Read(ref handler);
            return currentHandler != null ? currentHandler(arg1, arg2) : defaultResult;
        }

        /// <summary>
        ///     Performs invocation of the delegate making sure that it exists first.
        ///     Can be called on null.
        /// </summary>
        /// <param name="handler">The event handler to be invoked.</param>
        /// <param name="arg1">The first argument associated with the event.</param>
        /// <param name="arg2">The second argument associated with the event.</param>
        /// <param name="arg3">The third argument associated with the event.</param>
        /// <param name="defaultResult">The default result to use when handler was null.</param>
        /// <returns>Event result.</returns>
        [CanBeNull]
        public static TResult Call<TArg1, TArg2, TArg3, TResult>(
            [CanBeNull] this Func<TArg1, TArg2, TArg3, TResult> handler,
            [CanBeNull] TArg1 arg1,
            [CanBeNull] TArg2 arg2,
            [CanBeNull] TArg3 arg3,
            [CanBeNull] TResult defaultResult = default(TResult))
        {
            var currentHandler = Volatile.Read(ref handler);
            return currentHandler != null ? currentHandler(arg1, arg2, arg3) : defaultResult;
        }

        /// <summary>
        ///     Performs invocation of the delegate making sure that it exists first.
        ///     Can be called on null.
        /// </summary>
        /// <param name="handler">The event handler to be invoked.</param>
        /// <param name="arg1">The first argument associated with the event.</param>
        /// <param name="arg2">The second argument associated with the event.</param>
        /// <param name="arg3">The third argument associated with the event.</param>
        /// <param name="arg4">The fourth argument associated with the event.</param>
        /// <param name="defaultResult">The default result to use when handler was null.</param>
        /// <returns>Event result.</returns>
        [CanBeNull]
        public static TResult Call<TArg1, TArg2, TArg3, TArg4, TResult>(
            [CanBeNull] this Func<TArg1, TArg2, TArg3, TArg4, TResult> handler,
            [CanBeNull] TArg1 arg1,
            [CanBeNull] TArg2 arg2,
            [CanBeNull] TArg3 arg3,
            [CanBeNull] TArg4 arg4,
            [CanBeNull] TResult defaultResult = default(TResult))
        {
            var currentHandler = Volatile.Read(ref handler);
            return currentHandler != null ? currentHandler(arg1, arg2, arg3, arg4) : defaultResult;
        }

        /// <summary>
        ///     Performs invocation of the delegate making sure that it exists first.
        ///     Can be called on null.
        /// </summary>
        /// <param name="handler">The event handler to be invoked.</param>
        /// <param name="arg1">The first argument associated with the event.</param>
        /// <param name="arg2">The second argument associated with the event.</param>
        /// <param name="arg3">The third argument associated with the event.</param>
        /// <param name="arg4">The fourth argument associated with the event.</param>
        /// <param name="arg5">The fifth argument associated with the event.</param>
        /// <param name="defaultResult">The default result to use when handler was null.</param>
        /// <returns>Event result.</returns>
        [CanBeNull]
        public static TResult Call<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>(
            [CanBeNull] this Func<TArg1, TArg2, TArg3, TArg4, TArg5, TResult> handler,
            [CanBeNull] TArg1 arg1,
            [CanBeNull] TArg2 arg2,
            [CanBeNull] TArg3 arg3,
            [CanBeNull] TArg4 arg4,
            [CanBeNull] TArg5 arg5,
            [CanBeNull] TResult defaultResult = default(TResult))
        {
            var currentHandler = Volatile.Read(ref handler);
            return currentHandler != null ? currentHandler(arg1, arg2, arg3, arg4, arg5) : defaultResult;
        }

        /// <summary>
        ///     Performs invocation of the delegate making sure that it exists first.
        ///     Can be called on null.
        /// </summary>
        /// <param name="handler">The event handler to be invoked.</param>
        /// <param name="arg1">The first argument associated with the event.</param>
        /// <param name="arg2">The second argument associated with the event.</param>
        /// <param name="arg3">The third argument associated with the event.</param>
        /// <param name="arg4">The fourth argument associated with the event.</param>
        /// <param name="arg5">The fifth argument associated with the event.</param>
        /// <param name="arg6">The sixth argument associated with the event.</param>
        /// <param name="defaultResult">The default result to use when handler was null.</param>
        /// <returns>Event result.</returns>
        [CanBeNull]
        public static TResult Call<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>(
            [CanBeNull] this Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult> handler,
            [CanBeNull] TArg1 arg1,
            [CanBeNull] TArg2 arg2,
            [CanBeNull] TArg3 arg3,
            [CanBeNull] TArg4 arg4,
            [CanBeNull] TArg5 arg5,
            [CanBeNull] TArg6 arg6,
            [CanBeNull] TResult defaultResult = default(TResult))
        {
            var currentHandler = Volatile.Read(ref handler);
            return currentHandler != null ? currentHandler(arg1, arg2, arg3, arg4, arg5, arg6) : defaultResult;
        }

        /// <summary>
        ///     Performs invocation of the <see cref="PropertyChangedEventHandler" /> making sure
        ///     that it exists first. Can be called on null.
        /// </summary>
        /// <param name="handler">The event handler, can be null.</param>
        /// <param name="instance">The object which raises the event.</param>
        /// <param name="propertyName">The name of the property that was changed.</param>
        public static void Call(
            [CanBeNull] this PropertyChangedEventHandler handler,
            [CanBeNull] object instance,
            [CanBeNull] string propertyName)
        {
            var currentHandler = Volatile.Read(ref handler);
            currentHandler?.Invoke(instance, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        ///     Performs invocation of the <see cref="PropertyChangedEventHandler" /> making sure
        ///     that it exists first. Can be called on null.
        /// </summary>
        /// <typeparam name="T">The type of the property that was changed.</typeparam>
        /// <param name="handler">The event handler, can be null.</param>
        /// <param name="propertyGetterExpression">An expression that returns the value of the property.</param>
        /// <exception cref="ArgumentException">Property getter expression appears not to get any property</exception>
        public static void Call<T>(
            [CanBeNull] this PropertyChangedEventHandler handler,
            [NotNull] Expression<Func<T>> propertyGetterExpression)
        {
            Check.NotNull(propertyGetterExpression);

            var currentHandler = Volatile.Read(ref handler);
            if (currentHandler != null) {
                MemberExpression expression;
                try {
                    expression = (MemberExpression)propertyGetterExpression.Body;
                }
                catch (InvalidCastException) {
                    throw new ArgumentException("Property getter expression appears not to get any property",
                                                nameof(propertyGetterExpression));
                }

                var instanceExpression = (ConstantExpression)expression.Expression;
                var propertyName = expression.Member.Name;
                currentHandler(instanceExpression.Value, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}