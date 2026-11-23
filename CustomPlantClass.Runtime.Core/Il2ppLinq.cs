using System;
using Il2CppSystem.Collections.Generic;

namespace CustomPlantClass.Runtime.Core
{
    /// <summary>
    /// Provides LINQ-style query operators for IL2CPP lists and hash sets.
    /// </summary>
    public static partial class Il2CPPEnumerable
    {
        static List<T> ToIl2CppList<T>(System.Collections.Generic.IEnumerable<T> source)
        {
            ArgumentNullException.ThrowIfNull(source);
            
            var result = new List<T>();
            foreach (T item in source) result.Add(item);
            return result;
        }

        static List<T> ToIl2CppList<T>(IEnumerable<T> source)
        {
            return Il2CppSystem.Linq.Enumerable.ToList(source);
        }

        static System.Collections.Generic.IEnumerable<T> GetManagedEnumerable<T>(List<T> source)
        {
            ArgumentNullException.ThrowIfNull(source);
            return Enumerate(source);
        }

        static System.Collections.Generic.IEnumerable<T> GetManagedEnumerable<T>(HashSet<T> source)
        {
            ArgumentNullException.ThrowIfNull(source);
            return Enumerate(source);
        }

        static System.Collections.Generic.IEnumerable<T> GetManagedEnumerable<T>(IEnumerable<T> source)
        {
            ArgumentNullException.ThrowIfNull(source);
            return Enumerate(source);
        }

        static System.Collections.Generic.IEnumerable<T> Enumerate<T>(List<T> source)
        {
            foreach (T item in source) yield return item;
        }

        static System.Collections.Generic.IEnumerable<T> Enumerate<T>(HashSet<T> source)
        {
            foreach (T item in source) yield return item;
        }

        static System.Collections.Generic.IEnumerable<T> Enumerate<T>(IEnumerable<T> source)
        {
            List<T> items = Il2CppSystem.Linq.Enumerable.ToList(source);
            foreach (T item in items) yield return item;
        }

        static IEnumerable<T> GetIl2CppEnumerable<T>(List<T> source)
        {
            ArgumentNullException.ThrowIfNull(source);
            return source.Cast<IEnumerable<T>>();
        }

        static IEnumerable<T> GetIl2CppEnumerable<T>(HashSet<T> source)
        {
            ArgumentNullException.ThrowIfNull(source);
            return source.Cast<IEnumerable<T>>();
        }

        static IEnumerable<T> GetIl2CppEnumerable<T>(IEnumerable<T> source)
        {
            ArgumentNullException.ThrowIfNull(source);
            return source;
        }

        /// <summary>
        /// Returns the input typed as IEnumerable.
        /// </summary>
        /// <returns>The input sequence typed as IEnumerable.</returns>
        public static IEnumerable<T> AsEnumerable<T>(this List<T> source)
        {
            ArgumentNullException.ThrowIfNull(source);
            return source.Cast<IEnumerable<T>>();
        }

        /// <summary>
        /// Returns the input typed as IEnumerable.
        /// </summary>
        /// <returns>The input sequence typed as IEnumerable.</returns>
        public static IEnumerable<T> AsEnumerable<T>(this IEnumerable<T> source)
        {
            ArgumentNullException.ThrowIfNull(source);
            return source;
        }

        /// <summary>
        /// Returns the input typed as IEnumerable.
        /// </summary>
        /// <returns>The input sequence typed as IEnumerable.</returns>
        public static IEnumerable<T> AsEnumerable<T>(this ICollection<T> source)
        {
            ArgumentNullException.ThrowIfNull(source);
            return source.Cast<IEnumerable<T>>();
        }

        /// <summary>
        /// Returns the input typed as IEnumerable.
        /// </summary>
        /// <returns>The input sequence typed as IEnumerable.</returns>
        public static IEnumerable<T> AsEnumerable<T>(this IReadOnlyList<T> source)
        {
            ArgumentNullException.ThrowIfNull(source);
            return source.Cast<IEnumerable<T>>();
        }

        /// <summary>
        /// Returns the input typed as IEnumerable.
        /// </summary>
        /// <returns>The input sequence typed as IEnumerable.</returns>
        public static IEnumerable<T> AsEnumerable<T>(this IReadOnlyCollection<T> source)
        {
            ArgumentNullException.ThrowIfNull(source);
            return source.Cast<IEnumerable<T>>();
        }

        /// <summary>
        /// Returns the input typed as IEnumerable.
        /// </summary>
        /// <returns>The input sequence typed as IEnumerable.</returns>
        public static IEnumerable<T> AsEnumerable<T>(this IList<T> source)
        {
            ArgumentNullException.ThrowIfNull(source);
            return source.Cast<IEnumerable<T>>();
        }

        /// <summary>
        /// Returns the input typed as IEnumerable.
        /// </summary>
        /// <returns>The input sequence typed as IEnumerable.</returns>
        public static IEnumerable<T> AsEnumerable<T>(this HashSet<T> source)
        {
            ArgumentNullException.ThrowIfNull(source);
            return source.Cast<IEnumerable<T>>();
        }

        /// <summary>
        /// Creates a List from an IEnumerable.
        /// </summary>
        /// <returns>A List that contains elements from the input sequence.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static List<T> ToList<T>(this IEnumerable<T> source)
        {
            return Il2CppSystem.Linq.Enumerable.ToList(source);
        }

        /// <summary>
        /// Creates a List from an IEnumerable.
        /// </summary>
        /// <returns>A List that contains elements from the input sequence.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static List<T> ToList<T>(this HashSet<T> source)
        {
            return Il2CppSystem.Linq.Enumerable.ToList(GetIl2CppEnumerable(source));
        }

        /// <summary>
        /// Creates a List from an IEnumerable.
        /// </summary>
        /// <returns>A List that contains elements from the input sequence.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static List<T> ToList<T>(this List<T> source)
        {
            return Il2CppSystem.Linq.Enumerable.ToList(GetIl2CppEnumerable(source));
        }

        /// <summary>
        /// Creates a HashSet from an IEnumerable.
        /// </summary>
        /// <returns>A System.Collections.Generic.HashSet that contains values of type TSource selected from the input sequence.</returns>
        public static HashSet<T> ToHashSet<T>(this HashSet<T> source)
        {
            return Il2CppSystem.Linq.Enumerable.ToHashSet(GetIl2CppEnumerable(source));
        }

        /// <summary>
        /// Creates a HashSet from an IEnumerable.
        /// </summary>
        /// <returns>A System.Collections.Generic.HashSet that contains values of type TSource selected from the input sequence.</returns>
        public static HashSet<T> ToHashSet<T>(this List<T> source)
        {
            return Il2CppSystem.Linq.Enumerable.ToHashSet(GetIl2CppEnumerable(source));
        }

        /// <summary>
        /// Creates a HashSet from an IEnumerable.
        /// </summary>
        /// <returns>A System.Collections.Generic.HashSet that contains values of type TSource selected from the input sequence.</returns>
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            return Il2CppSystem.Linq.Enumerable.ToHashSet(source);
        }

        /// <summary>
        /// Creates a HashSet from an IEnumerable.
        /// </summary>
        /// <returns>A System.Collections.Generic.HashSet that contains values of type TSource selected from the input sequence.</returns>
        public static HashSet<T> ToHashSet<T>(this List<T> source, IEqualityComparer<T> comparer)
        {
            return Il2CppSystem.Linq.Enumerable.ToHashSet(GetIl2CppEnumerable(source), comparer);
        }

        /// <summary>
        /// Creates a HashSet from an IEnumerable.
        /// </summary>
        /// <returns>A System.Collections.Generic.HashSet that contains values of type TSource selected from the input sequence.</returns>
        public static HashSet<T> ToHashSet<T>(this HashSet<T> source, IEqualityComparer<T> comparer)
        {
            return Il2CppSystem.Linq.Enumerable.ToHashSet(GetIl2CppEnumerable(source), comparer);
        }

        /// <summary>
        /// Creates a HashSet from an IEnumerable.
        /// </summary>
        /// <returns>A System.Collections.Generic.HashSet that contains values of type TSource selected from the input sequence.</returns>
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer)
        {
            return Il2CppSystem.Linq.Enumerable.ToHashSet(source, comparer);
        }

        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>
        /// <returns>An IEnumerable that contains elements from the input sequence that satisfy the condition.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static List<T> Where<T>(this List<T> source, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Where(GetIl2CppEnumerable(source), predicate));
        }

        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>
        /// <returns>An IEnumerable that contains elements from the input sequence that satisfy the condition.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static List<T> Where<T>(this HashSet<T> source, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Where(GetIl2CppEnumerable(source), predicate));
        }

        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>
        /// <returns>An IEnumerable that contains elements from the input sequence that satisfy the condition.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static List<T> Where<T>(this List<T> source, Func<T, int, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            int index = 0;
            Func<T, bool> indexed = item => predicate(item, index++);
            Il2CppSystem.Func<T, bool> indexedPredicate = indexed;
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Where(GetIl2CppEnumerable(source), indexedPredicate));
        }

        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>
        /// <returns>An IEnumerable that contains elements from the input sequence that satisfy the condition.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static List<T> Where<T>(this HashSet<T> source, Func<T, int, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            int index = 0;
            Func<T, bool> indexed = item => predicate(item, index++);
            Il2CppSystem.Func<T, bool> indexedPredicate = indexed;
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Where(GetIl2CppEnumerable(source), indexedPredicate));
        }

        /// <summary>
        /// Projects each element of a sequence into a new form by incorporating the element's index.
        /// </summary>
        /// <returns>An IEnumerable whose elements are the result of invoking the transform function on each element of source.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static List<TResult> Select<TSource, TResult>(this List<TSource> source, Func<TSource, TResult> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Select<TSource, TResult>(GetIl2CppEnumerable(source), selector));
        }

        /// <summary>
        /// Projects each element of a sequence into a new form by incorporating the element's index.
        /// </summary>
        /// <returns>An IEnumerable whose elements are the result of invoking the transform function on each element of source.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static List<TResult> Select<TSource, TResult>(this HashSet<TSource> source, Func<TSource, TResult> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Select<TSource, TResult>(GetIl2CppEnumerable(source), selector));
        }

        /// <summary>
        /// Projects each element of a sequence into a new form by incorporating the element's index.
        /// </summary>
        /// <returns>An IEnumerable whose elements are the result of invoking the transform function on each element of source.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static List<TResult> Select<TSource, TResult>(this List<TSource> source, Func<TSource, int, TResult> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Select<TSource, TResult>(GetIl2CppEnumerable(source), selector));
        }

        /// <summary>
        /// Projects each element of a sequence into a new form by incorporating the element's index.
        /// </summary>
        /// <returns>An IEnumerable whose elements are the result of invoking the transform function on each element of source.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static List<TResult> Select<TSource, TResult>(this HashSet<TSource> source, Func<TSource, int, TResult> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Select<TSource, TResult>(GetIl2CppEnumerable(source), selector));
        }

        /// <summary>
        /// Projects each source element to a sequence and flattens the resulting sequences into one sequence.
        /// </summary>
        public static List<TResult> SelectMany<TSource, TResult>(this List<TSource> source, Func<TSource, List<TResult>> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            Func<TSource, IEnumerable<TResult>> selectorAdapter =
                item =>
                {
                    List<TResult> selected = selector(item);
                    if (selected == null) throw new InvalidOperationException("The collection selector returned null.");
                    return selected.Cast<IEnumerable<TResult>>();
                };
            Il2CppSystem.Func<TSource, IEnumerable<TResult>> il2CppSelector = selectorAdapter;
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.SelectMany(GetIl2CppEnumerable(source), il2CppSelector));
        }

        /// <summary>
        /// Projects each source element to a sequence and flattens the resulting sequences into one sequence.
        /// </summary>
        public static List<TResult> SelectMany<TSource, TResult>(this HashSet<TSource> source, Func<TSource, List<TResult>> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            Func<TSource, IEnumerable<TResult>> selectorAdapter =
                item =>
                {
                    List<TResult> selected = selector(item);
                    if (selected == null) throw new InvalidOperationException("The collection selector returned null.");
                    return selected.Cast<IEnumerable<TResult>>();
                };
            Il2CppSystem.Func<TSource, IEnumerable<TResult>> il2CppSelector = selectorAdapter;
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.SelectMany(GetIl2CppEnumerable(source), il2CppSelector));
        }

        /// <summary>
        /// Determines whether the source contains any elements or whether any element satisfies a condition.
        /// </summary>
        public static bool Any<T>(this List<T> source)
        {
            ArgumentNullException.ThrowIfNull(source);
            return source.Count != 0;
        }

        /// <summary>
        /// Determines whether the source contains any elements or whether any element satisfies a condition.
        /// </summary>
        public static bool Any<T>(this HashSet<T> source)
        {
            ArgumentNullException.ThrowIfNull(source);
            return source.Count != 0;
        }

        /// <summary>
        /// Determines whether the source contains any elements or whether any element satisfies a condition.
        /// </summary>
        public static bool Any<T>(this List<T> source, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return Il2CppSystem.Linq.Enumerable.Any(GetIl2CppEnumerable(source), predicate);
        }

        /// <summary>
        /// Determines whether the source contains any elements or whether any element satisfies a condition.
        /// </summary>
        public static bool Any<T>(this HashSet<T> source, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return Il2CppSystem.Linq.Enumerable.Any(GetIl2CppEnumerable(source), predicate);
        }

        /// <summary>
        /// Determines whether all elements of the source satisfy a condition.
        /// </summary>
        public static bool All<T>(this List<T> source, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return Il2CppSystem.Linq.Enumerable.All(GetIl2CppEnumerable(source), predicate);
        }

        /// <summary>
        /// Determines whether all elements of the source satisfy a condition.
        /// </summary>
        public static bool All<T>(this HashSet<T> source, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return Il2CppSystem.Linq.Enumerable.All(GetIl2CppEnumerable(source), predicate);
        }

        /// <summary>
        /// Returns the number of elements in the source or the number that satisfy a condition.
        /// </summary>
        public static int Count<T>(this List<T> source)
        {
            ArgumentNullException.ThrowIfNull(source);
            return source.Count;
        }

        /// <summary>
        /// Returns the number of elements in the source or the number that satisfy a condition.
        /// </summary>
        public static int Count<T>(this HashSet<T> source)
        {
            ArgumentNullException.ThrowIfNull(source);
            return source.Count;
        }

        /// <summary>
        /// Returns the number of elements in the source or the number that satisfy a condition.
        /// </summary>
        public static int Count<T>(this List<T> source, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return Il2CppSystem.Linq.Enumerable.Count(GetIl2CppEnumerable(source), predicate);
        }

        /// <summary>
        /// Returns the number of elements in the source or the number that satisfy a condition.
        /// </summary>
        public static int Count<T>(this HashSet<T> source, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return Il2CppSystem.Linq.Enumerable.Count(GetIl2CppEnumerable(source), predicate);
        }

        /// <summary>
        /// Determines whether the source contains a specified element.
        /// </summary>
        public static bool Contains<T>(this List<T> source, T value)
        {
            return Il2CppSystem.Linq.Enumerable.Contains(GetIl2CppEnumerable(source), value);
        }

        /// <summary>
        /// Determines whether the source contains a specified element.
        /// </summary>
        public static bool Contains<T>(this HashSet<T> source, T value)
        {
            ArgumentNullException.ThrowIfNull(source);
            return source.Contains(value);
        }

        /// <summary>
        /// Returns the first element of the source or the first element that satisfies a condition.
        /// </summary>
        public static T First<T>(this List<T> source)
        {
            return Il2CppSystem.Linq.Enumerable.First(GetIl2CppEnumerable(source));
        }

        /// <summary>
        /// Returns the first element of the source or the first element that satisfies a condition.
        /// </summary>
        public static T First<T>(this HashSet<T> source)
        {
            return Il2CppSystem.Linq.Enumerable.First(GetIl2CppEnumerable(source));
        }

        /// <summary>
        /// Returns the first element of the source or the first element that satisfies a condition.
        /// </summary>
        public static T First<T>(this List<T> source, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return Il2CppSystem.Linq.Enumerable.First(GetIl2CppEnumerable(source), predicate);
        }

        /// <summary>
        /// Returns the first element of the source or the first element that satisfies a condition.
        /// </summary>
        public static T First<T>(this HashSet<T> source, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return Il2CppSystem.Linq.Enumerable.First(GetIl2CppEnumerable(source), predicate);
        }

        /// <summary>
        /// Returns the first element of the source or the first element that satisfies a condition, or the default value if no such element exists.
        /// </summary>
        public static T FirstOrDefault<T>(this List<T> source)
        {
            return Il2CppSystem.Linq.Enumerable.FirstOrDefault(GetIl2CppEnumerable(source));
        }

        /// <summary>
        /// Returns the first element of the source or the first element that satisfies a condition, or the default value if no such element exists.
        /// </summary>
        public static T FirstOrDefault<T>(this HashSet<T> source)
        {
            return Il2CppSystem.Linq.Enumerable.FirstOrDefault(GetIl2CppEnumerable(source));
        }

        /// <summary>
        /// Returns the first element of the source or the first element that satisfies a condition, or the default value if no such element exists.
        /// </summary>
        public static T FirstOrDefault<T>(this List<T> source, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return Il2CppSystem.Linq.Enumerable.FirstOrDefault(GetIl2CppEnumerable(source), predicate);
        }

        /// <summary>
        /// Returns the first element of the source or the first element that satisfies a condition, or the default value if no such element exists.
        /// </summary>
        public static T FirstOrDefault<T>(this HashSet<T> source, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return Il2CppSystem.Linq.Enumerable.FirstOrDefault(GetIl2CppEnumerable(source), predicate);
        }

        /// <summary>
        /// Returns the only element of the source or the only element that satisfies a condition, and throws if there is not exactly one.
        /// </summary>
        public static T Single<T>(this List<T> source)
        {
            return Il2CppSystem.Linq.Enumerable.Single(GetIl2CppEnumerable(source));
        }

        /// <summary>
        /// Returns the only element of the source or the only element that satisfies a condition, and throws if there is not exactly one.
        /// </summary>
        public static T Single<T>(this HashSet<T> source)
        {
            return Il2CppSystem.Linq.Enumerable.Single(GetIl2CppEnumerable(source));
        }

        /// <summary>
        /// Returns the only element of the source or the only element that satisfies a condition, or the default value if no such element exists; throws if more than one exists.
        /// </summary>
        public static T SingleOrDefault<T>(this List<T> source)
        {
            return Il2CppSystem.Linq.Enumerable.SingleOrDefault(GetIl2CppEnumerable(source));
        }

        /// <summary>
        /// Returns the only element of the source or the only element that satisfies a condition, or the default value if no such element exists; throws if more than one exists.
        /// </summary>
        public static T SingleOrDefault<T>(this HashSet<T> source)
        {
            return Il2CppSystem.Linq.Enumerable.SingleOrDefault(GetIl2CppEnumerable(source));
        }

        /// <summary>
        /// Returns the only element of the source or the only element that satisfies a condition, and throws if there is not exactly one.
        /// </summary>
        public static T Single<T>(this List<T> source, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return Il2CppSystem.Linq.Enumerable.Single(GetIl2CppEnumerable(source), predicate);
        }

        /// <summary>
        /// Returns the only element of the source or the only element that satisfies a condition, and throws if there is not exactly one.
        /// </summary>
        public static T Single<T>(this HashSet<T> source, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return Il2CppSystem.Linq.Enumerable.Single(GetIl2CppEnumerable(source), predicate);
        }

        /// <summary>
        /// Returns the only element of the source or the only element that satisfies a condition, or the default value if no such element exists; throws if more than one exists.
        /// </summary>
        public static T SingleOrDefault<T>(this List<T> source, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return Il2CppSystem.Linq.Enumerable.SingleOrDefault(GetIl2CppEnumerable(source), predicate);
        }

        /// <summary>
        /// Returns the only element of the source or the only element that satisfies a condition, or the default value if no such element exists; throws if more than one exists.
        /// </summary>
        public static T SingleOrDefault<T>(this HashSet<T> source, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return Il2CppSystem.Linq.Enumerable.SingleOrDefault(GetIl2CppEnumerable(source), predicate);
        }

        /// <summary>
        /// Returns the last element of the source or the last element that satisfies a condition.
        /// </summary>
        public static T Last<T>(this List<T> source)
        {
            return Il2CppSystem.Linq.Enumerable.Last(GetIl2CppEnumerable(source));
        }

        /// <summary>
        /// Returns the last element of the source or the last element that satisfies a condition.
        /// </summary>
        public static T Last<T>(this HashSet<T> source)
        {
            return Il2CppSystem.Linq.Enumerable.Last(GetIl2CppEnumerable(source));
        }

        /// <summary>
        /// Returns the last element of the source or the last element that satisfies a condition.
        /// </summary>
        public static T Last<T>(this List<T> source, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return Il2CppSystem.Linq.Enumerable.Last(
                Il2CppSystem.Linq.Enumerable.Where(GetIl2CppEnumerable(source), predicate));
        }

        /// <summary>
        /// Returns the last element of the source or the last element that satisfies a condition.
        /// </summary>
        public static T Last<T>(this HashSet<T> source, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return Il2CppSystem.Linq.Enumerable.Last(
                Il2CppSystem.Linq.Enumerable.Where(GetIl2CppEnumerable(source), predicate));
        }

        /// <summary>
        /// Returns the last element of the source or the last element that satisfies a condition, or the default value if no such element exists.
        /// </summary>
        public static T LastOrDefault<T>(this List<T> source)
        {
            return Il2CppSystem.Linq.Enumerable.LastOrDefault(GetIl2CppEnumerable(source));
        }

        /// <summary>
        /// Returns the last element of the source or the last element that satisfies a condition, or the default value if no such element exists.
        /// </summary>
        public static T LastOrDefault<T>(this HashSet<T> source)
        {
            return Il2CppSystem.Linq.Enumerable.LastOrDefault(GetIl2CppEnumerable(source));
        }

        /// <summary>
        /// Returns the last element of the source or the last element that satisfies a condition, or the default value if no such element exists.
        /// </summary>
        public static T LastOrDefault<T>(this List<T> source, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return Il2CppSystem.Linq.Enumerable.LastOrDefault(
                Il2CppSystem.Linq.Enumerable.Where(GetIl2CppEnumerable(source), predicate));
        }

        /// <summary>
        /// Returns the last element of the source or the last element that satisfies a condition, or the default value if no such element exists.
        /// </summary>
        public static T LastOrDefault<T>(this HashSet<T> source, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return Il2CppSystem.Linq.Enumerable.LastOrDefault(
                Il2CppSystem.Linq.Enumerable.Where(GetIl2CppEnumerable(source), predicate));
        }

        /// <summary>
        /// Returns the element at a specified index in the source.
        /// </summary>
        public static T ElementAt<T>(this List<T> source, int index)
        {
            return Il2CppSystem.Linq.Enumerable.ElementAt(GetIl2CppEnumerable(source), index);
        }

        /// <summary>
        /// Returns the element at a specified index in the source.
        /// </summary>
        public static T ElementAt<T>(this HashSet<T> source, int index)
        {
            return Il2CppSystem.Linq.Enumerable.ElementAt(GetIl2CppEnumerable(source), index);
        }

        /// <summary>
        /// Returns the element at a specified index, or the default value if the index is out of range.
        /// </summary>
        public static T ElementAtOrDefault<T>(this List<T> source, int index)
        {
            return System.Linq.Enumerable.ElementAtOrDefault(GetManagedEnumerable(source), index);
        }

        /// <summary>
        /// Returns the element at a specified index, or the default value if the index is out of range.
        /// </summary>
        public static T ElementAtOrDefault<T>(this HashSet<T> source, int index)
        {
            return System.Linq.Enumerable.ElementAtOrDefault(GetManagedEnumerable(source), index);
        }

        /// <summary>
        /// Returns an Int64 representing the number of elements in the source or the number that satisfy a condition.
        /// </summary>
        public static long LongCount<T>(this List<T> source)
        {
            ArgumentNullException.ThrowIfNull(source);
            return source.Count;
        }

        /// <summary>
        /// Returns an Int64 representing the number of elements in the source or the number that satisfy a condition.
        /// </summary>
        public static long LongCount<T>(this HashSet<T> source)
        {
            ArgumentNullException.ThrowIfNull(source);
            return source.Count;
        }

        /// <summary>
        /// Returns an Int64 representing the number of elements in the source or the number that satisfy a condition.
        /// </summary>
        public static long LongCount<T>(this List<T> source, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return System.Linq.Enumerable.LongCount(GetManagedEnumerable(source), predicate);
        }

        /// <summary>
        /// Returns an Int64 representing the number of elements in the source or the number that satisfy a condition.
        /// </summary>
        public static long LongCount<T>(this HashSet<T> source, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return System.Linq.Enumerable.LongCount(GetManagedEnumerable(source), predicate);
        }

        /// <summary>
        /// Returns the source elements, or a sequence containing the specified default value if the source is empty.
        /// </summary>
        public static List<T> DefaultIfEmpty<T>(this List<T> source)
        {
            return ToIl2CppList(System.Linq.Enumerable.DefaultIfEmpty(GetManagedEnumerable(source)));
        }

        /// <summary>
        /// Returns the source elements, or a sequence containing the specified default value if the source is empty.
        /// </summary>
        public static List<T> DefaultIfEmpty<T>(this HashSet<T> source)
        {
            return ToIl2CppList(System.Linq.Enumerable.DefaultIfEmpty(GetManagedEnumerable(source)));
        }

        /// <summary>
        /// Returns the source elements, or a sequence containing the specified default value if the source is empty.
        /// </summary>
        public static List<T> DefaultIfEmpty<T>(this List<T> source, T defaultValue)
        {
            return ToIl2CppList(System.Linq.Enumerable.DefaultIfEmpty(GetManagedEnumerable(source), defaultValue));
        }

        /// <summary>
        /// Returns the source elements, or a sequence containing the specified default value if the source is empty.
        /// </summary>
        public static List<T> DefaultIfEmpty<T>(this HashSet<T> source, T defaultValue)
        {
            return ToIl2CppList(System.Linq.Enumerable.DefaultIfEmpty(GetManagedEnumerable(source), defaultValue));
        }

        /// <summary>
        /// Returns the specified number of consecutive elements from the start of the source.
        /// </summary>
        public static List<T> Take<T>(this List<T> source, int count)
        {
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Take(GetIl2CppEnumerable(source), count));
        }

        /// <summary>
        /// Returns the specified number of consecutive elements from the start of the source.
        /// </summary>
        public static List<T> Take<T>(this HashSet<T> source, int count)
        {
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Take(GetIl2CppEnumerable(source), count));
        }

        /// <summary>
        /// Skips the specified number of elements and returns the remaining elements.
        /// </summary>
        public static List<T> Skip<T>(this List<T> source, int count)
        {
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Skip(GetIl2CppEnumerable(source), count));
        }

        /// <summary>
        /// Skips the specified number of elements and returns the remaining elements.
        /// </summary>
        public static List<T> Skip<T>(this HashSet<T> source, int count)
        {
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Skip(GetIl2CppEnumerable(source), count));
        }

        /// <summary>
        /// Returns elements from the source while a condition is true.
        /// </summary>
        public static List<T> TakeWhile<T>(this List<T> source, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return ToIl2CppList(System.Linq.Enumerable.TakeWhile(GetManagedEnumerable(source), predicate));
        }

        /// <summary>
        /// Returns elements from the source while a condition is true.
        /// </summary>
        public static List<T> TakeWhile<T>(this HashSet<T> source, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return ToIl2CppList(System.Linq.Enumerable.TakeWhile(GetManagedEnumerable(source), predicate));
        }

        /// <summary>
        /// Skips elements while a condition is true and returns the remaining elements.
        /// </summary>
        public static List<T> SkipWhile<T>(this List<T> source, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return ToIl2CppList(System.Linq.Enumerable.SkipWhile(GetManagedEnumerable(source), predicate));
        }

        /// <summary>
        /// Skips elements while a condition is true and returns the remaining elements.
        /// </summary>
        public static List<T> SkipWhile<T>(this HashSet<T> source, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return ToIl2CppList(System.Linq.Enumerable.SkipWhile(GetManagedEnumerable(source), predicate));
        }

        /// <summary>
        /// Reverses the order of the elements in the source.
        /// </summary>
        public static List<T> Reverse<T>(this List<T> source)
        {
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Reverse(GetIl2CppEnumerable(source)));
        }

        /// <summary>
        /// Reverses the order of the elements in the source.
        /// </summary>
        public static List<T> Reverse<T>(this HashSet<T> source)
        {
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Reverse(GetIl2CppEnumerable(source)));
        }

        /// <summary>
        /// Returns distinct elements from the source, using the specified equality comparer when provided.
        /// </summary>
        public static List<T> Distinct<T>(this List<T> source)
        {
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Distinct(GetIl2CppEnumerable(source)));
        }

        /// <summary>
        /// Returns distinct elements from the source, using the specified equality comparer when provided.
        /// </summary>
        public static List<T> Distinct<T>(this HashSet<T> source)
        {
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Distinct(GetIl2CppEnumerable(source)));
        }

        /// <summary>
        /// Returns distinct elements from the source, using the specified equality comparer when provided.
        /// </summary>
        public static List<T> Distinct<T>(this List<T> source, IEqualityComparer<T> comparer)
        {
            return DistinctCore(GetManagedEnumerable(source), comparer);
        }

        /// <summary>
        /// Returns distinct elements from the source, using the specified equality comparer when provided.
        /// </summary>
        public static List<T> Distinct<T>(this HashSet<T> source, IEqualityComparer<T> comparer)
        {
            return DistinctCore(GetManagedEnumerable(source), comparer);
        }

        static List<T> DistinctCore<T>(
            System.Collections.Generic.IEnumerable<T> source,
            IEqualityComparer<T> comparer)
        {
            var seen = comparer == null ? new HashSet<T>() : new HashSet<T>(comparer);
            var result = new List<T>();
            foreach (T item in source)
            {
                if (seen.Add(item)) result.Add(item);
            }
            return result;
        }

        /// <summary>
        /// Concatenates two sequences.
        /// </summary>
        public static List<T> Concat<T>(this List<T> first, List<T> second)
        {
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Concat(GetIl2CppEnumerable(first), GetIl2CppEnumerable(second)));
        }

        /// <summary>
        /// Concatenates two sequences.
        /// </summary>
        public static List<T> Concat<T>(this List<T> first, HashSet<T> second)
        {
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Concat(GetIl2CppEnumerable(first), GetIl2CppEnumerable(second)));
        }

        /// <summary>
        /// Concatenates two sequences.
        /// </summary>
        public static List<T> Concat<T>(this HashSet<T> first, List<T> second)
        {
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Concat(GetIl2CppEnumerable(first), GetIl2CppEnumerable(second)));
        }

        /// <summary>
        /// Concatenates two sequences.
        /// </summary>
        public static List<T> Concat<T>(this HashSet<T> first, HashSet<T> second)
        {
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Concat(GetIl2CppEnumerable(first), GetIl2CppEnumerable(second)));
        }

        /// <summary>
        /// Produces the set union of two sequences.
        /// </summary>
        public static List<T> Union<T>(this List<T> first, List<T> second)
        {
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Union(GetIl2CppEnumerable(first), GetIl2CppEnumerable(second)));
        }

        /// <summary>
        /// Produces the set union of two sequences.
        /// </summary>
        public static List<T> Union<T>(this List<T> first, HashSet<T> second)
        {
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Union(GetIl2CppEnumerable(first), GetIl2CppEnumerable(second)));
        }

        /// <summary>
        /// Produces the set union of two sequences.
        /// </summary>
        public static List<T> Union<T>(this HashSet<T> first, List<T> second)
        {
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Union(GetIl2CppEnumerable(first), GetIl2CppEnumerable(second)));
        }

        /// <summary>
        /// Produces the set union of two sequences.
        /// </summary>
        public static List<T> Union<T>(this HashSet<T> first, HashSet<T> second)
        {
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Union(GetIl2CppEnumerable(first), GetIl2CppEnumerable(second)));
        }

        /// <summary>
        /// Produces the set intersection of two sequences.
        /// </summary>
        public static List<T> Intersect<T>(this List<T> first, List<T> second)
        {
            return ToIl2CppList(System.Linq.Enumerable.Intersect(GetManagedEnumerable(first), GetManagedEnumerable(second)));
        }

        /// <summary>
        /// Produces the set intersection of two sequences.
        /// </summary>
        public static List<T> Intersect<T>(this List<T> first, HashSet<T> second)
        {
            return ToIl2CppList(System.Linq.Enumerable.Intersect(GetManagedEnumerable(first), GetManagedEnumerable(second)));
        }

        /// <summary>
        /// Produces the set intersection of two sequences.
        /// </summary>
        public static List<T> Intersect<T>(this HashSet<T> first, List<T> second)
        {
            return ToIl2CppList(System.Linq.Enumerable.Intersect(GetManagedEnumerable(first), GetManagedEnumerable(second)));
        }

        /// <summary>
        /// Produces the set intersection of two sequences.
        /// </summary>
        public static List<T> Intersect<T>(this HashSet<T> first, HashSet<T> second)
        {
            return ToIl2CppList(System.Linq.Enumerable.Intersect(GetManagedEnumerable(first), GetManagedEnumerable(second)));
        }

        /// <summary>
        /// Produces the set difference of two sequences.
        /// </summary>
        public static List<T> Except<T>(this List<T> first, List<T> second)
        {
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Except(GetIl2CppEnumerable(first), GetIl2CppEnumerable(second)));
        }

        /// <summary>
        /// Produces the set difference of two sequences.
        /// </summary>
        public static List<T> Except<T>(this List<T> first, HashSet<T> second)
        {
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Except(GetIl2CppEnumerable(first), GetIl2CppEnumerable(second)));
        }

        /// <summary>
        /// Produces the set difference of two sequences.
        /// </summary>
        public static List<T> Except<T>(this HashSet<T> first, List<T> second)
        {
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Except(GetIl2CppEnumerable(first), GetIl2CppEnumerable(second)));
        }

        /// <summary>
        /// Produces the set difference of two sequences.
        /// </summary>
        public static List<T> Except<T>(this HashSet<T> first, HashSet<T> second)
        {
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Except(GetIl2CppEnumerable(first), GetIl2CppEnumerable(second)));
        }

        /// <summary>
        /// Sorts the elements of the source in ascending order by a key.
        /// </summary>
        public static List<T> OrderBy<T, TKey>(this List<T> source, Func<T, TKey> keySelector)
        {
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
            return Il2CppSystem.Linq.Enumerable.ToList(
                Il2CppSystem.Linq.Enumerable.OrderBy<T, TKey>(GetIl2CppEnumerable(source), keySelector).Cast<IEnumerable<T>>());
        }

        /// <summary>
        /// Sorts the elements of the source in ascending order by a key.
        /// </summary>
        public static List<T> OrderBy<T, TKey>(this HashSet<T> source, Func<T, TKey> keySelector)
        {
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
            return Il2CppSystem.Linq.Enumerable.ToList(
                Il2CppSystem.Linq.Enumerable.OrderBy<T, TKey>(GetIl2CppEnumerable(source), keySelector).Cast<IEnumerable<T>>());
        }

        /// <summary>
        /// Sorts the elements of the source in descending order by a key.
        /// </summary>
        public static List<T> OrderByDescending<T, TKey>(this List<T> source, Func<T, TKey> keySelector)
        {
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
            return Il2CppSystem.Linq.Enumerable.ToList(
                Il2CppSystem.Linq.Enumerable.OrderByDescending<T, TKey>(GetIl2CppEnumerable(source), keySelector).Cast<IEnumerable<T>>());
        }

        /// <summary>
        /// Sorts the elements of the source in descending order by a key.
        /// </summary>
        public static List<T> OrderByDescending<T, TKey>(this HashSet<T> source, Func<T, TKey> keySelector)
        {
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
            return Il2CppSystem.Linq.Enumerable.ToList(
                Il2CppSystem.Linq.Enumerable.OrderByDescending<T, TKey>(GetIl2CppEnumerable(source), keySelector).Cast<IEnumerable<T>>());
        }

        /// <summary>
        /// Determines whether two sequences are equal by comparing their elements in order.
        /// </summary>
        public static bool SequenceEqual<T>(this List<T> first, List<T> second)
        {
            return Il2CppSystem.Linq.Enumerable.SequenceEqual(GetIl2CppEnumerable(first), GetIl2CppEnumerable(second));
        }

        /// <summary>
        /// Determines whether two sequences are equal by comparing their elements in order.
        /// </summary>
        public static bool SequenceEqual<T>(this List<T> first, HashSet<T> second)
        {
            return Il2CppSystem.Linq.Enumerable.SequenceEqual(GetIl2CppEnumerable(first), GetIl2CppEnumerable(second));
        }

        /// <summary>
        /// Determines whether two sequences are equal by comparing their elements in order.
        /// </summary>
        public static bool SequenceEqual<T>(this HashSet<T> first, List<T> second)
        {
            return Il2CppSystem.Linq.Enumerable.SequenceEqual(GetIl2CppEnumerable(first), GetIl2CppEnumerable(second));
        }

        /// <summary>
        /// Determines whether two sequences are equal by comparing their elements in order.
        /// </summary>
        public static bool SequenceEqual<T>(this HashSet<T> first, HashSet<T> second)
        {
            return Il2CppSystem.Linq.Enumerable.SequenceEqual(GetIl2CppEnumerable(first), GetIl2CppEnumerable(second));
        }

        /// <summary>
        /// Applies a specified function to corresponding elements of two sequences, producing a sequence of results.
        /// </summary>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this List<TFirst> first,
            List<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector)
        {
            if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));
            return ToIl2CppList(System.Linq.Enumerable.Zip(GetManagedEnumerable(first), GetManagedEnumerable(second), resultSelector));
        }

        /// <summary>
        /// Applies a specified function to corresponding elements of two sequences, producing a sequence of results.
        /// </summary>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this List<TFirst> first,
            HashSet<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector)
        {
            if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));
            return ToIl2CppList(System.Linq.Enumerable.Zip(GetManagedEnumerable(first), GetManagedEnumerable(second), resultSelector));
        }

        /// <summary>
        /// Applies a specified function to corresponding elements of two sequences, producing a sequence of results.
        /// </summary>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this HashSet<TFirst> first,
            List<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector)
        {
            if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));
            return ToIl2CppList(System.Linq.Enumerable.Zip(GetManagedEnumerable(first), GetManagedEnumerable(second), resultSelector));
        }

        /// <summary>
        /// Applies a specified function to corresponding elements of two sequences, producing a sequence of results.
        /// </summary>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this HashSet<TFirst> first,
            HashSet<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector)
        {
            if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));
            return ToIl2CppList(System.Linq.Enumerable.Zip(GetManagedEnumerable(first), GetManagedEnumerable(second), resultSelector));
        }

        /// <summary>
        /// Applies an accumulator function over the source elements.
        /// </summary>
        public static T Aggregate<T>(this List<T> source, Func<T, T, T> accumulator)
        {
            if (accumulator == null) throw new ArgumentNullException(nameof(accumulator));
            return System.Linq.Enumerable.Aggregate(GetManagedEnumerable(source), accumulator);
        }

        /// <summary>
        /// Applies an accumulator function over the source elements.
        /// </summary>
        public static T Aggregate<T>(this HashSet<T> source, Func<T, T, T> accumulator)
        {
            if (accumulator == null) throw new ArgumentNullException(nameof(accumulator));
            return System.Linq.Enumerable.Aggregate(GetManagedEnumerable(source), accumulator);
        }

        /// <summary>
        /// Applies an accumulator function over the source elements.
        /// </summary>
        public static TAccumulate Aggregate<T, TAccumulate>(
            this List<T> source,
            TAccumulate seed,
            Func<TAccumulate, T, TAccumulate> accumulator)
        {
            if (accumulator == null) throw new ArgumentNullException(nameof(accumulator));
            return System.Linq.Enumerable.Aggregate(GetManagedEnumerable(source), seed, accumulator);
        }

        /// <summary>
        /// Applies an accumulator function over the source elements.
        /// </summary>
        public static TAccumulate Aggregate<T, TAccumulate>(
            this HashSet<T> source,
            TAccumulate seed,
            Func<TAccumulate, T, TAccumulate> accumulator)
        {
            if (accumulator == null) throw new ArgumentNullException(nameof(accumulator));
            return System.Linq.Enumerable.Aggregate(GetManagedEnumerable(source), seed, accumulator);
        }

        // These overloads provide shared implementations after collection interfaces are adapted to IEnumerable<T>.
        /// <inheritdoc/>
        public static List<T> Where<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Where(GetIl2CppEnumerable(source), predicate));
        }

        /// <inheritdoc/>
        public static List<T> Where<T>(this IEnumerable<T> source, Func<T, int, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            int index = 0;
            Func<T, bool> indexed = item => predicate(item, index++);
            Il2CppSystem.Func<T, bool> indexedPredicate = indexed;
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Where(GetIl2CppEnumerable(source), indexedPredicate));
        }

        /// <inheritdoc/>
        public static List<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Select<TSource, TResult>(GetIl2CppEnumerable(source), selector));
        }

        /// <inheritdoc/>
        public static List<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, TResult> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Select<TSource, TResult>(GetIl2CppEnumerable(source), selector));
        }

        /// <inheritdoc/>
        public static List<TResult> SelectMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, List<TResult>> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            Func<TSource, IEnumerable<TResult>> selectorAdapter =
                item =>
                {
                    List<TResult> selected = selector(item);
                    if (selected == null) throw new InvalidOperationException("The collection selector returned null.");
                    return selected.Cast<IEnumerable<TResult>>();
                };
            Il2CppSystem.Func<TSource, IEnumerable<TResult>> il2CppSelector = selectorAdapter;
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.SelectMany(GetIl2CppEnumerable(source), il2CppSelector));
        }

        /// <inheritdoc/>
        public static bool Any<T>(this IEnumerable<T> source)
        {
            ArgumentNullException.ThrowIfNull(source);
            return Il2CppSystem.Linq.Enumerable.Any(source);
        }

        /// <inheritdoc/>
        public static bool Any<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return Il2CppSystem.Linq.Enumerable.Any(GetIl2CppEnumerable(source), predicate);
        }

        /// <inheritdoc/>
        public static bool All<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return Il2CppSystem.Linq.Enumerable.All(GetIl2CppEnumerable(source), predicate);
        }

        /// <inheritdoc/>
        public static int Count<T>(this IEnumerable<T> source)
        {
            ArgumentNullException.ThrowIfNull(source);
            return Il2CppSystem.Linq.Enumerable.Count(source);
        }

        /// <inheritdoc/>
        public static int Count<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return Il2CppSystem.Linq.Enumerable.Count(GetIl2CppEnumerable(source), predicate);
        }

        /// <inheritdoc/>
        public static bool Contains<T>(this IEnumerable<T> source, T value)
        {
            return Il2CppSystem.Linq.Enumerable.Contains(GetIl2CppEnumerable(source), value);
        }

        /// <inheritdoc/>
        public static T First<T>(this IEnumerable<T> source)
        {
            return Il2CppSystem.Linq.Enumerable.First(GetIl2CppEnumerable(source));
        }

        /// <inheritdoc/>
        public static T First<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return Il2CppSystem.Linq.Enumerable.First(GetIl2CppEnumerable(source), predicate);
        }

        /// <inheritdoc/>
        public static T FirstOrDefault<T>(this IEnumerable<T> source)
        {
            return Il2CppSystem.Linq.Enumerable.FirstOrDefault(GetIl2CppEnumerable(source));
        }

        /// <inheritdoc/>
        public static T FirstOrDefault<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return Il2CppSystem.Linq.Enumerable.FirstOrDefault(GetIl2CppEnumerable(source), predicate);
        }

        /// <inheritdoc/>
        public static T Single<T>(this IEnumerable<T> source)
        {
            return Il2CppSystem.Linq.Enumerable.Single(GetIl2CppEnumerable(source));
        }

        /// <inheritdoc/>
        public static T SingleOrDefault<T>(this IEnumerable<T> source)
        {
            return Il2CppSystem.Linq.Enumerable.SingleOrDefault(GetIl2CppEnumerable(source));
        }

        /// <inheritdoc/>
        public static T Single<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return Il2CppSystem.Linq.Enumerable.Single(GetIl2CppEnumerable(source), predicate);
        }

        /// <inheritdoc/>
        public static T SingleOrDefault<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return Il2CppSystem.Linq.Enumerable.SingleOrDefault(GetIl2CppEnumerable(source), predicate);
        }

        /// <inheritdoc/>
        public static T Last<T>(this IEnumerable<T> source)
        {
            return Il2CppSystem.Linq.Enumerable.Last(GetIl2CppEnumerable(source));
        }

        /// <inheritdoc/>
        public static T Last<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return Il2CppSystem.Linq.Enumerable.Last(
                Il2CppSystem.Linq.Enumerable.Where(GetIl2CppEnumerable(source), predicate));
        }

        /// <inheritdoc/>
        public static T LastOrDefault<T>(this IEnumerable<T> source)
        {
            return Il2CppSystem.Linq.Enumerable.LastOrDefault(GetIl2CppEnumerable(source));
        }

        /// <inheritdoc/>
        public static T LastOrDefault<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return Il2CppSystem.Linq.Enumerable.LastOrDefault(
                Il2CppSystem.Linq.Enumerable.Where(GetIl2CppEnumerable(source), predicate));
        }

        /// <inheritdoc/>
        public static T ElementAt<T>(this IEnumerable<T> source, int index)
        {
            return Il2CppSystem.Linq.Enumerable.ElementAt(GetIl2CppEnumerable(source), index);
        }

        /// <inheritdoc/>
        public static T ElementAtOrDefault<T>(this IEnumerable<T> source, int index)
        {
            return System.Linq.Enumerable.ElementAtOrDefault(GetManagedEnumerable(source), index);
        }

        /// <inheritdoc/>
        public static long LongCount<T>(this IEnumerable<T> source)
        {
            return System.Linq.Enumerable.LongCount(GetManagedEnumerable(source));
        }

        /// <inheritdoc/>
        public static long LongCount<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return System.Linq.Enumerable.LongCount(GetManagedEnumerable(source), predicate);
        }

        /// <inheritdoc/>
        public static List<T> DefaultIfEmpty<T>(this IEnumerable<T> source)
        {
            return ToIl2CppList(System.Linq.Enumerable.DefaultIfEmpty(GetManagedEnumerable(source)));
        }

        /// <inheritdoc/>
        public static List<T> DefaultIfEmpty<T>(this IEnumerable<T> source, T defaultValue)
        {
            return ToIl2CppList(System.Linq.Enumerable.DefaultIfEmpty(GetManagedEnumerable(source), defaultValue));
        }

        /// <inheritdoc/>
        public static List<T> Take<T>(this IEnumerable<T> source, int count)
        {
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Take(GetIl2CppEnumerable(source), count));
        }

        /// <inheritdoc/>
        public static List<T> Skip<T>(this IEnumerable<T> source, int count)
        {
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Skip(GetIl2CppEnumerable(source), count));
        }

        /// <inheritdoc/>
        public static List<T> TakeWhile<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return ToIl2CppList(System.Linq.Enumerable.TakeWhile(GetManagedEnumerable(source), predicate));
        }

        /// <inheritdoc/>
        public static List<T> SkipWhile<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return ToIl2CppList(System.Linq.Enumerable.SkipWhile(GetManagedEnumerable(source), predicate));
        }

        /// <inheritdoc/>
        public static List<T> Reverse<T>(this IEnumerable<T> source)
        {
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Reverse(GetIl2CppEnumerable(source)));
        }

        /// <inheritdoc/>
        public static List<T> Distinct<T>(this IEnumerable<T> source)
        {
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Distinct(GetIl2CppEnumerable(source)));
        }

        /// <inheritdoc/>
        public static List<T> Distinct<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer)
        {
            return DistinctCore(GetManagedEnumerable(source), comparer);
        }

        /// <inheritdoc/>
        public static List<T> Concat<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Concat(
                GetIl2CppEnumerable(first), GetIl2CppEnumerable(second)));
        }

        /// <inheritdoc/>
        public static List<T> Union<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Union(
                GetIl2CppEnumerable(first), GetIl2CppEnumerable(second)));
        }

        /// <inheritdoc/>
        public static List<T> Intersect<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            return ToIl2CppList(System.Linq.Enumerable.Intersect(
                GetManagedEnumerable(first), GetManagedEnumerable(second)));
        }

        /// <inheritdoc/>
        public static List<T> Except<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Except(
                GetIl2CppEnumerable(first), GetIl2CppEnumerable(second)));
        }

        /// <inheritdoc/>
        public static List<T> OrderBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
        {
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
            return Il2CppSystem.Linq.Enumerable.ToList(
                Il2CppSystem.Linq.Enumerable.OrderBy<T, TKey>(GetIl2CppEnumerable(source), keySelector).Cast<IEnumerable<T>>());
        }

        /// <inheritdoc/>
        public static List<T> OrderByDescending<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
        {
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
            return Il2CppSystem.Linq.Enumerable.ToList(
                Il2CppSystem.Linq.Enumerable.OrderByDescending<T, TKey>(GetIl2CppEnumerable(source), keySelector).Cast<IEnumerable<T>>());
        }

        /// <inheritdoc/>
        public static bool SequenceEqual<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            return Il2CppSystem.Linq.Enumerable.SequenceEqual(
                GetIl2CppEnumerable(first), GetIl2CppEnumerable(second));
        }

        /// <inheritdoc/>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this IEnumerable<TFirst> first,
            IEnumerable<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector)
        {
            if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));
            return ToIl2CppList(System.Linq.Enumerable.Zip(
                GetManagedEnumerable(first), GetManagedEnumerable(second), resultSelector));
        }

        /// <inheritdoc/>
        public static T Aggregate<T>(this IEnumerable<T> source, Func<T, T, T> accumulator)
        {
            if (accumulator == null) throw new ArgumentNullException(nameof(accumulator));
            return System.Linq.Enumerable.Aggregate(GetManagedEnumerable(source), accumulator);
        }

        /// <inheritdoc/>
        public static TAccumulate Aggregate<T, TAccumulate>(
            this IEnumerable<T> source,
            TAccumulate seed,
            Func<TAccumulate, T, TAccumulate> accumulator)
        {
            if (accumulator == null) throw new ArgumentNullException(nameof(accumulator));
            return System.Linq.Enumerable.Aggregate(GetManagedEnumerable(source), seed, accumulator);
        }

        /// <inheritdoc/>
        public static List<T> ToList<T>(this IReadOnlyList<T> source) => ToList(source.AsEnumerable());

        /// <inheritdoc/>
        public static HashSet<T> ToHashSet<T>(this IReadOnlyList<T> source) => ToHashSet(source.AsEnumerable());

        /// <inheritdoc/>
        public static HashSet<T> ToHashSet<T>(this IReadOnlyList<T> source, IEqualityComparer<T> comparer) =>
            ToHashSet(source.AsEnumerable(), comparer);

        /// <inheritdoc/>
        public static List<T> Where<T>(this IReadOnlyList<T> source, Func<T, bool> predicate) =>
            Where(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static List<T> Where<T>(this IReadOnlyList<T> source, Func<T, int, bool> predicate) =>
            Where(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static List<TResult> Select<TSource, TResult>(this IReadOnlyList<TSource> source, Func<TSource, TResult> selector) =>
            Select(source.AsEnumerable(), selector);

        /// <inheritdoc/>
        public static List<TResult> Select<TSource, TResult>(this IReadOnlyList<TSource> source, Func<TSource, int, TResult> selector) =>
            Select(source.AsEnumerable(), selector);

        /// <inheritdoc/>
        public static List<TResult> SelectMany<TSource, TResult>(this IReadOnlyList<TSource> source, Func<TSource, List<TResult>> selector) =>
            SelectMany(source.AsEnumerable(), selector);

        /// <inheritdoc/>
        public static bool Any<T>(this IReadOnlyList<T> source) => Any(source.AsEnumerable());

        /// <inheritdoc/>
        public static bool Any<T>(this IReadOnlyList<T> source, Func<T, bool> predicate) =>
            Any(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static bool All<T>(this IReadOnlyList<T> source, Func<T, bool> predicate) =>
            All(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static int Count<T>(this IReadOnlyList<T> source) => Count(source.AsEnumerable());

        /// <inheritdoc/>
        public static int Count<T>(this IReadOnlyList<T> source, Func<T, bool> predicate) =>
            Count(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static bool Contains<T>(this IReadOnlyList<T> source, T value) =>
            Contains(source.AsEnumerable(), value);

        /// <inheritdoc/>
        public static T First<T>(this IReadOnlyList<T> source) => First(source.AsEnumerable());

        /// <inheritdoc/>
        public static T First<T>(this IReadOnlyList<T> source, Func<T, bool> predicate) =>
            First(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static T FirstOrDefault<T>(this IReadOnlyList<T> source) => FirstOrDefault(source.AsEnumerable());

        /// <inheritdoc/>
        public static T FirstOrDefault<T>(this IReadOnlyList<T> source, Func<T, bool> predicate) =>
            FirstOrDefault(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static T Single<T>(this IReadOnlyList<T> source) => Single(source.AsEnumerable());

        /// <inheritdoc/>
        public static T SingleOrDefault<T>(this IReadOnlyList<T> source) => SingleOrDefault(source.AsEnumerable());

        /// <inheritdoc/>
        public static T Single<T>(this IReadOnlyList<T> source, Func<T, bool> predicate) =>
            Single(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static T SingleOrDefault<T>(this IReadOnlyList<T> source, Func<T, bool> predicate) =>
            SingleOrDefault(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static T Last<T>(this IReadOnlyList<T> source) => Last(source.AsEnumerable());

        /// <inheritdoc/>
        public static T Last<T>(this IReadOnlyList<T> source, Func<T, bool> predicate) =>
            Last(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static T LastOrDefault<T>(this IReadOnlyList<T> source) => LastOrDefault(source.AsEnumerable());

        /// <inheritdoc/>
        public static T LastOrDefault<T>(this IReadOnlyList<T> source, Func<T, bool> predicate) =>
            LastOrDefault(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static T ElementAt<T>(this IReadOnlyList<T> source, int index) =>
            ElementAt(source.AsEnumerable(), index);

        /// <inheritdoc/>
        public static T ElementAtOrDefault<T>(this IReadOnlyList<T> source, int index) =>
            ElementAtOrDefault(source.AsEnumerable(), index);

        /// <inheritdoc/>
        public static long LongCount<T>(this IReadOnlyList<T> source) => LongCount(source.AsEnumerable());

        /// <inheritdoc/>
        public static long LongCount<T>(this IReadOnlyList<T> source, Func<T, bool> predicate) =>
            LongCount(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static List<T> DefaultIfEmpty<T>(this IReadOnlyList<T> source) =>
            DefaultIfEmpty(source.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> DefaultIfEmpty<T>(this IReadOnlyList<T> source, T defaultValue) =>
            DefaultIfEmpty(source.AsEnumerable(), defaultValue);

        /// <inheritdoc/>
        public static List<T> Take<T>(this IReadOnlyList<T> source, int count) =>
            Take(source.AsEnumerable(), count);

        /// <inheritdoc/>
        public static List<T> Skip<T>(this IReadOnlyList<T> source, int count) =>
            Skip(source.AsEnumerable(), count);

        /// <inheritdoc/>
        public static List<T> TakeWhile<T>(this IReadOnlyList<T> source, Func<T, bool> predicate) =>
            TakeWhile(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static List<T> SkipWhile<T>(this IReadOnlyList<T> source, Func<T, bool> predicate) =>
            SkipWhile(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static List<T> Reverse<T>(this IReadOnlyList<T> source) => Reverse(source.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Distinct<T>(this IReadOnlyList<T> source) => Distinct(source.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Distinct<T>(this IReadOnlyList<T> source, IEqualityComparer<T> comparer) =>
            Distinct(source.AsEnumerable(), comparer);

        /// <inheritdoc/>
        public static List<T> Concat<T>(this IReadOnlyList<T> first, IEnumerable<T> second) =>
            Concat(first.AsEnumerable(), second);

        /// <inheritdoc/>
        public static List<T> Union<T>(this IReadOnlyList<T> first, IEnumerable<T> second) =>
            Union(first.AsEnumerable(), second);

        /// <inheritdoc/>
        public static List<T> Intersect<T>(this IReadOnlyList<T> first, IEnumerable<T> second) =>
            Intersect(first.AsEnumerable(), second);

        /// <inheritdoc/>
        public static List<T> Except<T>(this IReadOnlyList<T> first, IEnumerable<T> second) =>
            Except(first.AsEnumerable(), second);

        /// <inheritdoc/>
        public static List<T> OrderBy<T, TKey>(this IReadOnlyList<T> source, Func<T, TKey> keySelector) =>
            OrderBy(source.AsEnumerable(), keySelector);

        /// <inheritdoc/>
        public static List<T> OrderByDescending<T, TKey>(this IReadOnlyList<T> source, Func<T, TKey> keySelector) =>
            OrderByDescending(source.AsEnumerable(), keySelector);

        /// <inheritdoc/>
        public static bool SequenceEqual<T>(this IReadOnlyList<T> first, IEnumerable<T> second) =>
            SequenceEqual(first.AsEnumerable(), second);

        /// <inheritdoc/>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this IReadOnlyList<TFirst> first,
            IEnumerable<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector) =>
            Zip(first.AsEnumerable(), second, resultSelector);

        /// <inheritdoc/>
        public static T Aggregate<T>(this IReadOnlyList<T> source, Func<T, T, T> accumulator) =>
            Aggregate(source.AsEnumerable(), accumulator);

        /// <inheritdoc/>
        public static TAccumulate Aggregate<T, TAccumulate>(
            this IReadOnlyList<T> source,
            TAccumulate seed,
            Func<TAccumulate, T, TAccumulate> accumulator) =>
            Aggregate(source.AsEnumerable(), seed, accumulator);

        /// <inheritdoc/>
        public static List<T> ToList<T>(this IReadOnlyCollection<T> source) => ToList(source.AsEnumerable());

        /// <inheritdoc/>
        public static HashSet<T> ToHashSet<T>(this IReadOnlyCollection<T> source) => ToHashSet(source.AsEnumerable());

        /// <inheritdoc/>
        public static HashSet<T> ToHashSet<T>(this IReadOnlyCollection<T> source, IEqualityComparer<T> comparer) =>
            ToHashSet(source.AsEnumerable(), comparer);

        /// <inheritdoc/>
        public static List<T> Where<T>(this IReadOnlyCollection<T> source, Func<T, bool> predicate) =>
            Where(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static List<T> Where<T>(this IReadOnlyCollection<T> source, Func<T, int, bool> predicate) =>
            Where(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static List<TResult> Select<TSource, TResult>(this IReadOnlyCollection<TSource> source, Func<TSource, TResult> selector) =>
            Select(source.AsEnumerable(), selector);

        /// <inheritdoc/>
        public static List<TResult> Select<TSource, TResult>(this IReadOnlyCollection<TSource> source, Func<TSource, int, TResult> selector) =>
            Select(source.AsEnumerable(), selector);

        /// <inheritdoc/>
        public static List<TResult> SelectMany<TSource, TResult>(this IReadOnlyCollection<TSource> source, Func<TSource, List<TResult>> selector) =>
            SelectMany(source.AsEnumerable(), selector);

        /// <inheritdoc/>
        public static bool Any<T>(this IReadOnlyCollection<T> source) => Any(source.AsEnumerable());

        /// <inheritdoc/>
        public static bool Any<T>(this IReadOnlyCollection<T> source, Func<T, bool> predicate) =>
            Any(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static bool All<T>(this IReadOnlyCollection<T> source, Func<T, bool> predicate) =>
            All(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static int Count<T>(this IReadOnlyCollection<T> source) => Count(source.AsEnumerable());

        /// <inheritdoc/>
        public static int Count<T>(this IReadOnlyCollection<T> source, Func<T, bool> predicate) =>
            Count(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static bool Contains<T>(this IReadOnlyCollection<T> source, T value) =>
            Contains(source.AsEnumerable(), value);

        /// <inheritdoc/>
        public static T First<T>(this IReadOnlyCollection<T> source) => First(source.AsEnumerable());

        /// <inheritdoc/>
        public static T First<T>(this IReadOnlyCollection<T> source, Func<T, bool> predicate) =>
            First(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static T FirstOrDefault<T>(this IReadOnlyCollection<T> source) => FirstOrDefault(source.AsEnumerable());

        /// <inheritdoc/>
        public static T FirstOrDefault<T>(this IReadOnlyCollection<T> source, Func<T, bool> predicate) =>
            FirstOrDefault(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static T Single<T>(this IReadOnlyCollection<T> source) => Single(source.AsEnumerable());

        /// <inheritdoc/>
        public static T SingleOrDefault<T>(this IReadOnlyCollection<T> source) => SingleOrDefault(source.AsEnumerable());

        /// <inheritdoc/>
        public static T Single<T>(this IReadOnlyCollection<T> source, Func<T, bool> predicate) =>
            Single(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static T SingleOrDefault<T>(this IReadOnlyCollection<T> source, Func<T, bool> predicate) =>
            SingleOrDefault(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static T Last<T>(this IReadOnlyCollection<T> source) => Last(source.AsEnumerable());

        /// <inheritdoc/>
        public static T Last<T>(this IReadOnlyCollection<T> source, Func<T, bool> predicate) =>
            Last(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static T LastOrDefault<T>(this IReadOnlyCollection<T> source) => LastOrDefault(source.AsEnumerable());

        /// <inheritdoc/>
        public static T LastOrDefault<T>(this IReadOnlyCollection<T> source, Func<T, bool> predicate) =>
            LastOrDefault(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static T ElementAt<T>(this IReadOnlyCollection<T> source, int index) =>
            ElementAt(source.AsEnumerable(), index);

        /// <inheritdoc/>
        public static T ElementAtOrDefault<T>(this IReadOnlyCollection<T> source, int index) =>
            ElementAtOrDefault(source.AsEnumerable(), index);

        /// <inheritdoc/>
        public static long LongCount<T>(this IReadOnlyCollection<T> source) => LongCount(source.AsEnumerable());

        /// <inheritdoc/>
        public static long LongCount<T>(this IReadOnlyCollection<T> source, Func<T, bool> predicate) =>
            LongCount(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static List<T> DefaultIfEmpty<T>(this IReadOnlyCollection<T> source) =>
            DefaultIfEmpty(source.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> DefaultIfEmpty<T>(this IReadOnlyCollection<T> source, T defaultValue) =>
            DefaultIfEmpty(source.AsEnumerable(), defaultValue);

        /// <inheritdoc/>
        public static List<T> Take<T>(this IReadOnlyCollection<T> source, int count) =>
            Take(source.AsEnumerable(), count);

        /// <inheritdoc/>
        public static List<T> Skip<T>(this IReadOnlyCollection<T> source, int count) =>
            Skip(source.AsEnumerable(), count);

        /// <inheritdoc/>
        public static List<T> TakeWhile<T>(this IReadOnlyCollection<T> source, Func<T, bool> predicate) =>
            TakeWhile(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static List<T> SkipWhile<T>(this IReadOnlyCollection<T> source, Func<T, bool> predicate) =>
            SkipWhile(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static List<T> Reverse<T>(this IReadOnlyCollection<T> source) => Reverse(source.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Distinct<T>(this IReadOnlyCollection<T> source) => Distinct(source.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Distinct<T>(this IReadOnlyCollection<T> source, IEqualityComparer<T> comparer) =>
            Distinct(source.AsEnumerable(), comparer);

        /// <inheritdoc/>
        public static List<T> Concat<T>(this IReadOnlyCollection<T> first, IEnumerable<T> second) =>
            Concat(first.AsEnumerable(), second);

        /// <inheritdoc/>
        public static List<T> Union<T>(this IReadOnlyCollection<T> first, IEnumerable<T> second) =>
            Union(first.AsEnumerable(), second);

        /// <inheritdoc/>
        public static List<T> Intersect<T>(this IReadOnlyCollection<T> first, IEnumerable<T> second) =>
            Intersect(first.AsEnumerable(), second);

        /// <inheritdoc/>
        public static List<T> Except<T>(this IReadOnlyCollection<T> first, IEnumerable<T> second) =>
            Except(first.AsEnumerable(), second);

        /// <inheritdoc/>
        public static List<T> OrderBy<T, TKey>(this IReadOnlyCollection<T> source, Func<T, TKey> keySelector) =>
            OrderBy(source.AsEnumerable(), keySelector);

        /// <inheritdoc/>
        public static List<T> OrderByDescending<T, TKey>(this IReadOnlyCollection<T> source, Func<T, TKey> keySelector) =>
            OrderByDescending(source.AsEnumerable(), keySelector);

        /// <inheritdoc/>
        public static bool SequenceEqual<T>(this IReadOnlyCollection<T> first, IEnumerable<T> second) =>
            SequenceEqual(first.AsEnumerable(), second);

        /// <inheritdoc/>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this IReadOnlyCollection<TFirst> first,
            IEnumerable<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector) =>
            Zip(first.AsEnumerable(), second, resultSelector);

        /// <inheritdoc/>
        public static T Aggregate<T>(this IReadOnlyCollection<T> source, Func<T, T, T> accumulator) =>
            Aggregate(source.AsEnumerable(), accumulator);

        /// <inheritdoc/>
        public static TAccumulate Aggregate<T, TAccumulate>(
            this IReadOnlyCollection<T> source,
            TAccumulate seed,
            Func<TAccumulate, T, TAccumulate> accumulator) =>
            Aggregate(source.AsEnumerable(), seed, accumulator);

        /// <inheritdoc/>
        public static List<T> ToList<T>(this IList<T> source) => ToList(source.AsEnumerable());

        /// <inheritdoc/>
        public static HashSet<T> ToHashSet<T>(this IList<T> source) => ToHashSet(source.AsEnumerable());

        /// <inheritdoc/>
        public static HashSet<T> ToHashSet<T>(this IList<T> source, IEqualityComparer<T> comparer) =>
            ToHashSet(source.AsEnumerable(), comparer);

        /// <inheritdoc/>
        public static List<T> Where<T>(this IList<T> source, Func<T, bool> predicate) =>
            Where(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static List<T> Where<T>(this IList<T> source, Func<T, int, bool> predicate) =>
            Where(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static List<TResult> Select<TSource, TResult>(this IList<TSource> source, Func<TSource, TResult> selector) =>
            Select(source.AsEnumerable(), selector);

        /// <inheritdoc/>
        public static List<TResult> Select<TSource, TResult>(this IList<TSource> source, Func<TSource, int, TResult> selector) =>
            Select(source.AsEnumerable(), selector);

        /// <inheritdoc/>
        public static List<TResult> SelectMany<TSource, TResult>(this IList<TSource> source, Func<TSource, List<TResult>> selector) =>
            SelectMany(source.AsEnumerable(), selector);

        /// <inheritdoc/>
        public static bool Any<T>(this IList<T> source) => Any(source.AsEnumerable());

        /// <inheritdoc/>
        public static bool Any<T>(this IList<T> source, Func<T, bool> predicate) =>
            Any(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static bool All<T>(this IList<T> source, Func<T, bool> predicate) =>
            All(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static int Count<T>(this IList<T> source) => Count(source.AsEnumerable());

        /// <inheritdoc/>
        public static int Count<T>(this IList<T> source, Func<T, bool> predicate) =>
            Count(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static bool Contains<T>(this IList<T> source, T value) =>
            Contains(source.AsEnumerable(), value);

        /// <inheritdoc/>
        public static T First<T>(this IList<T> source) => First(source.AsEnumerable());

        /// <inheritdoc/>
        public static T First<T>(this IList<T> source, Func<T, bool> predicate) =>
            First(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static T FirstOrDefault<T>(this IList<T> source) => FirstOrDefault(source.AsEnumerable());

        /// <inheritdoc/>
        public static T FirstOrDefault<T>(this IList<T> source, Func<T, bool> predicate) =>
            FirstOrDefault(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static T Single<T>(this IList<T> source) => Single(source.AsEnumerable());

        /// <inheritdoc/>
        public static T SingleOrDefault<T>(this IList<T> source) => SingleOrDefault(source.AsEnumerable());

        /// <inheritdoc/>
        public static T Single<T>(this IList<T> source, Func<T, bool> predicate) =>
            Single(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static T SingleOrDefault<T>(this IList<T> source, Func<T, bool> predicate) =>
            SingleOrDefault(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static T Last<T>(this IList<T> source) => Last(source.AsEnumerable());

        /// <inheritdoc/>
        public static T Last<T>(this IList<T> source, Func<T, bool> predicate) =>
            Last(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static T LastOrDefault<T>(this IList<T> source) => LastOrDefault(source.AsEnumerable());

        /// <inheritdoc/>
        public static T LastOrDefault<T>(this IList<T> source, Func<T, bool> predicate) =>
            LastOrDefault(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static T ElementAt<T>(this IList<T> source, int index) =>
            ElementAt(source.AsEnumerable(), index);

        /// <inheritdoc/>
        public static T ElementAtOrDefault<T>(this IList<T> source, int index) =>
            ElementAtOrDefault(source.AsEnumerable(), index);

        /// <inheritdoc/>
        public static long LongCount<T>(this IList<T> source) => LongCount(source.AsEnumerable());

        /// <inheritdoc/>
        public static long LongCount<T>(this IList<T> source, Func<T, bool> predicate) =>
            LongCount(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static List<T> DefaultIfEmpty<T>(this IList<T> source) =>
            DefaultIfEmpty(source.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> DefaultIfEmpty<T>(this IList<T> source, T defaultValue) =>
            DefaultIfEmpty(source.AsEnumerable(), defaultValue);

        /// <inheritdoc/>
        public static List<T> Take<T>(this IList<T> source, int count) =>
            Take(source.AsEnumerable(), count);

        /// <inheritdoc/>
        public static List<T> Skip<T>(this IList<T> source, int count) =>
            Skip(source.AsEnumerable(), count);

        /// <inheritdoc/>
        public static List<T> TakeWhile<T>(this IList<T> source, Func<T, bool> predicate) =>
            TakeWhile(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static List<T> SkipWhile<T>(this IList<T> source, Func<T, bool> predicate) =>
            SkipWhile(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static List<T> Reverse<T>(this IList<T> source) => Reverse(source.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Distinct<T>(this IList<T> source) => Distinct(source.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Distinct<T>(this IList<T> source, IEqualityComparer<T> comparer) =>
            Distinct(source.AsEnumerable(), comparer);

        /// <inheritdoc/>
        public static List<T> Concat<T>(this IList<T> first, IEnumerable<T> second) =>
            Concat(first.AsEnumerable(), second);

        /// <inheritdoc/>
        public static List<T> Union<T>(this IList<T> first, IEnumerable<T> second) =>
            Union(first.AsEnumerable(), second);

        /// <inheritdoc/>
        public static List<T> Intersect<T>(this IList<T> first, IEnumerable<T> second) =>
            Intersect(first.AsEnumerable(), second);

        /// <inheritdoc/>
        public static List<T> Except<T>(this IList<T> first, IEnumerable<T> second) =>
            Except(first.AsEnumerable(), second);

        /// <inheritdoc/>
        public static List<T> OrderBy<T, TKey>(this IList<T> source, Func<T, TKey> keySelector) =>
            OrderBy(source.AsEnumerable(), keySelector);

        /// <inheritdoc/>
        public static List<T> OrderByDescending<T, TKey>(this IList<T> source, Func<T, TKey> keySelector) =>
            OrderByDescending(source.AsEnumerable(), keySelector);

        /// <inheritdoc/>
        public static bool SequenceEqual<T>(this IList<T> first, IEnumerable<T> second) =>
            SequenceEqual(first.AsEnumerable(), second);

        /// <inheritdoc/>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this IList<TFirst> first,
            IEnumerable<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector) =>
            Zip(first.AsEnumerable(), second, resultSelector);

        /// <inheritdoc/>
        public static T Aggregate<T>(this IList<T> source, Func<T, T, T> accumulator) =>
            Aggregate(source.AsEnumerable(), accumulator);

        /// <inheritdoc/>
        public static TAccumulate Aggregate<T, TAccumulate>(
            this IList<T> source,
            TAccumulate seed,
            Func<TAccumulate, T, TAccumulate> accumulator) =>
            Aggregate(source.AsEnumerable(), seed, accumulator);

        /// <inheritdoc/>
        public static List<T> Concat<T>(this IEnumerable<T> first, IReadOnlyList<T> second) =>
            Concat(first, second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Concat<T>(this IEnumerable<T> first, IReadOnlyCollection<T> second) =>
            Concat(first, second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Concat<T>(this IEnumerable<T> first, IList<T> second) =>
            Concat(first, second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Union<T>(this IEnumerable<T> first, IReadOnlyList<T> second) =>
            Union(first, second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Union<T>(this IEnumerable<T> first, IReadOnlyCollection<T> second) =>
            Union(first, second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Union<T>(this IEnumerable<T> first, IList<T> second) =>
            Union(first, second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Intersect<T>(this IEnumerable<T> first, IReadOnlyList<T> second) =>
            Intersect(first, second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Intersect<T>(this IEnumerable<T> first, IReadOnlyCollection<T> second) =>
            Intersect(first, second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Intersect<T>(this IEnumerable<T> first, IList<T> second) =>
            Intersect(first, second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Except<T>(this IEnumerable<T> first, IReadOnlyList<T> second) =>
            Except(first, second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Except<T>(this IEnumerable<T> first, IReadOnlyCollection<T> second) =>
            Except(first, second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Except<T>(this IEnumerable<T> first, IList<T> second) =>
            Except(first, second.AsEnumerable());

        /// <inheritdoc/>
        public static bool SequenceEqual<T>(this IEnumerable<T> first, IReadOnlyList<T> second) =>
            SequenceEqual(first, second.AsEnumerable());

        /// <inheritdoc/>
        public static bool SequenceEqual<T>(this IEnumerable<T> first, IReadOnlyCollection<T> second) =>
            SequenceEqual(first, second.AsEnumerable());

        /// <inheritdoc/>
        public static bool SequenceEqual<T>(this IEnumerable<T> first, IList<T> second) =>
            SequenceEqual(first, second.AsEnumerable());

        /// <inheritdoc/>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this IEnumerable<TFirst> first,
            IReadOnlyList<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector) =>
            Zip(first, second.AsEnumerable(), resultSelector);

        /// <inheritdoc/>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this IEnumerable<TFirst> first,
            IReadOnlyCollection<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector) =>
            Zip(first, second.AsEnumerable(), resultSelector);

        /// <inheritdoc/>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this IEnumerable<TFirst> first,
            IList<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector) =>
            Zip(first, second.AsEnumerable(), resultSelector);

        /// <inheritdoc/>
        public static List<T> Concat<T>(this List<T> first, IReadOnlyList<T> second) =>
            Concat(GetIl2CppEnumerable(first), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Concat<T>(this List<T> first, IReadOnlyCollection<T> second) =>
            Concat(GetIl2CppEnumerable(first), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Concat<T>(this List<T> first, IList<T> second) =>
            Concat(GetIl2CppEnumerable(first), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Union<T>(this List<T> first, IReadOnlyList<T> second) =>
            Union(GetIl2CppEnumerable(first), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Union<T>(this List<T> first, IReadOnlyCollection<T> second) =>
            Union(GetIl2CppEnumerable(first), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Union<T>(this List<T> first, IList<T> second) =>
            Union(GetIl2CppEnumerable(first), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Intersect<T>(this List<T> first, IReadOnlyList<T> second) =>
            Intersect(GetIl2CppEnumerable(first), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Intersect<T>(this List<T> first, IReadOnlyCollection<T> second) =>
            Intersect(GetIl2CppEnumerable(first), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Intersect<T>(this List<T> first, IList<T> second) =>
            Intersect(GetIl2CppEnumerable(first), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Except<T>(this List<T> first, IReadOnlyList<T> second) =>
            Except(GetIl2CppEnumerable(first), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Except<T>(this List<T> first, IReadOnlyCollection<T> second) =>
            Except(GetIl2CppEnumerable(first), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Except<T>(this List<T> first, IList<T> second) =>
            Except(GetIl2CppEnumerable(first), second.AsEnumerable());

        /// <inheritdoc/>
        public static bool SequenceEqual<T>(this List<T> first, IReadOnlyList<T> second) =>
            SequenceEqual(GetIl2CppEnumerable(first), second.AsEnumerable());

        /// <inheritdoc/>
        public static bool SequenceEqual<T>(this List<T> first, IReadOnlyCollection<T> second) =>
            SequenceEqual(GetIl2CppEnumerable(first), second.AsEnumerable());

        /// <inheritdoc/>
        public static bool SequenceEqual<T>(this List<T> first, IList<T> second) =>
            SequenceEqual(GetIl2CppEnumerable(first), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this List<TFirst> first,
            IReadOnlyList<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector) =>
            Zip(GetIl2CppEnumerable(first), second.AsEnumerable(), resultSelector);

        /// <inheritdoc/>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this List<TFirst> first,
            IReadOnlyCollection<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector) =>
            Zip(GetIl2CppEnumerable(first), second.AsEnumerable(), resultSelector);

        /// <inheritdoc/>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this List<TFirst> first,
            IList<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector) =>
            Zip(GetIl2CppEnumerable(first), second.AsEnumerable(), resultSelector);

        /// <inheritdoc/>
        public static List<T> Concat<T>(this HashSet<T> first, IReadOnlyList<T> second) =>
            Concat(GetIl2CppEnumerable(first), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Concat<T>(this HashSet<T> first, IReadOnlyCollection<T> second) =>
            Concat(GetIl2CppEnumerable(first), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Concat<T>(this HashSet<T> first, IList<T> second) =>
            Concat(GetIl2CppEnumerable(first), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Union<T>(this HashSet<T> first, IReadOnlyList<T> second) =>
            Union(GetIl2CppEnumerable(first), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Union<T>(this HashSet<T> first, IReadOnlyCollection<T> second) =>
            Union(GetIl2CppEnumerable(first), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Union<T>(this HashSet<T> first, IList<T> second) =>
            Union(GetIl2CppEnumerable(first), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Intersect<T>(this HashSet<T> first, IReadOnlyList<T> second) =>
            Intersect(GetIl2CppEnumerable(first), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Intersect<T>(this HashSet<T> first, IReadOnlyCollection<T> second) =>
            Intersect(GetIl2CppEnumerable(first), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Intersect<T>(this HashSet<T> first, IList<T> second) =>
            Intersect(GetIl2CppEnumerable(first), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Except<T>(this HashSet<T> first, IReadOnlyList<T> second) =>
            Except(GetIl2CppEnumerable(first), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Except<T>(this HashSet<T> first, IReadOnlyCollection<T> second) =>
            Except(GetIl2CppEnumerable(first), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Except<T>(this HashSet<T> first, IList<T> second) =>
            Except(GetIl2CppEnumerable(first), second.AsEnumerable());

        /// <inheritdoc/>
        public static bool SequenceEqual<T>(this HashSet<T> first, IReadOnlyList<T> second) =>
            SequenceEqual(GetIl2CppEnumerable(first), second.AsEnumerable());

        /// <inheritdoc/>
        public static bool SequenceEqual<T>(this HashSet<T> first, IReadOnlyCollection<T> second) =>
            SequenceEqual(GetIl2CppEnumerable(first), second.AsEnumerable());

        /// <inheritdoc/>
        public static bool SequenceEqual<T>(this HashSet<T> first, IList<T> second) =>
            SequenceEqual(GetIl2CppEnumerable(first), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this HashSet<TFirst> first,
            IReadOnlyList<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector) =>
            Zip(GetIl2CppEnumerable(first), second.AsEnumerable(), resultSelector);

        /// <inheritdoc/>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this HashSet<TFirst> first,
            IReadOnlyCollection<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector) =>
            Zip(GetIl2CppEnumerable(first), second.AsEnumerable(), resultSelector);

        /// <inheritdoc/>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this HashSet<TFirst> first,
            IList<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector) =>
            Zip(GetIl2CppEnumerable(first), second.AsEnumerable(), resultSelector);

        /// <inheritdoc/>
        public static List<T> Concat<T>(this IReadOnlyList<T> first, IReadOnlyList<T> second) =>
            Concat(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Concat<T>(this IReadOnlyList<T> first, IReadOnlyCollection<T> second) =>
            Concat(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Concat<T>(this IReadOnlyList<T> first, IList<T> second) =>
            Concat(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Union<T>(this IReadOnlyList<T> first, IReadOnlyList<T> second) =>
            Union(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Union<T>(this IReadOnlyList<T> first, IReadOnlyCollection<T> second) =>
            Union(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Union<T>(this IReadOnlyList<T> first, IList<T> second) =>
            Union(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Intersect<T>(this IReadOnlyList<T> first, IReadOnlyList<T> second) =>
            Intersect(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Intersect<T>(this IReadOnlyList<T> first, IReadOnlyCollection<T> second) =>
            Intersect(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Intersect<T>(this IReadOnlyList<T> first, IList<T> second) =>
            Intersect(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Except<T>(this IReadOnlyList<T> first, IReadOnlyList<T> second) =>
            Except(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Except<T>(this IReadOnlyList<T> first, IReadOnlyCollection<T> second) =>
            Except(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Except<T>(this IReadOnlyList<T> first, IList<T> second) =>
            Except(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static bool SequenceEqual<T>(this IReadOnlyList<T> first, IReadOnlyList<T> second) =>
            SequenceEqual(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static bool SequenceEqual<T>(this IReadOnlyList<T> first, IReadOnlyCollection<T> second) =>
            SequenceEqual(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static bool SequenceEqual<T>(this IReadOnlyList<T> first, IList<T> second) =>
            SequenceEqual(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this IReadOnlyList<TFirst> first,
            IReadOnlyList<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector) =>
            Zip(first.AsEnumerable(), second.AsEnumerable(), resultSelector);

        /// <inheritdoc/>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this IReadOnlyList<TFirst> first,
            IReadOnlyCollection<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector) =>
            Zip(first.AsEnumerable(), second.AsEnumerable(), resultSelector);

        /// <inheritdoc/>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this IReadOnlyList<TFirst> first,
            IList<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector) =>
            Zip(first.AsEnumerable(), second.AsEnumerable(), resultSelector);

        /// <inheritdoc/>
        public static List<T> Concat<T>(this IReadOnlyCollection<T> first, IReadOnlyList<T> second) =>
            Concat(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Concat<T>(this IReadOnlyCollection<T> first, IReadOnlyCollection<T> second) =>
            Concat(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Concat<T>(this IReadOnlyCollection<T> first, IList<T> second) =>
            Concat(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Union<T>(this IReadOnlyCollection<T> first, IReadOnlyList<T> second) =>
            Union(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Union<T>(this IReadOnlyCollection<T> first, IReadOnlyCollection<T> second) =>
            Union(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Union<T>(this IReadOnlyCollection<T> first, IList<T> second) =>
            Union(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Intersect<T>(this IReadOnlyCollection<T> first, IReadOnlyList<T> second) =>
            Intersect(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Intersect<T>(this IReadOnlyCollection<T> first, IReadOnlyCollection<T> second) =>
            Intersect(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Intersect<T>(this IReadOnlyCollection<T> first, IList<T> second) =>
            Intersect(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Except<T>(this IReadOnlyCollection<T> first, IReadOnlyList<T> second) =>
            Except(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Except<T>(this IReadOnlyCollection<T> first, IReadOnlyCollection<T> second) =>
            Except(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Except<T>(this IReadOnlyCollection<T> first, IList<T> second) =>
            Except(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static bool SequenceEqual<T>(this IReadOnlyCollection<T> first, IReadOnlyList<T> second) =>
            SequenceEqual(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static bool SequenceEqual<T>(this IReadOnlyCollection<T> first, IReadOnlyCollection<T> second) =>
            SequenceEqual(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static bool SequenceEqual<T>(this IReadOnlyCollection<T> first, IList<T> second) =>
            SequenceEqual(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this IReadOnlyCollection<TFirst> first,
            IReadOnlyList<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector) =>
            Zip(first.AsEnumerable(), second.AsEnumerable(), resultSelector);

        /// <inheritdoc/>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this IReadOnlyCollection<TFirst> first,
            IReadOnlyCollection<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector) =>
            Zip(first.AsEnumerable(), second.AsEnumerable(), resultSelector);

        /// <inheritdoc/>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this IReadOnlyCollection<TFirst> first,
            IList<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector) =>
            Zip(first.AsEnumerable(), second.AsEnumerable(), resultSelector);

        /// <inheritdoc/>
        public static List<T> Concat<T>(this IList<T> first, IReadOnlyList<T> second) =>
            Concat(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Concat<T>(this IList<T> first, IReadOnlyCollection<T> second) =>
            Concat(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Concat<T>(this IList<T> first, IList<T> second) =>
            Concat(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Union<T>(this IList<T> first, IReadOnlyList<T> second) =>
            Union(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Union<T>(this IList<T> first, IReadOnlyCollection<T> second) =>
            Union(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Union<T>(this IList<T> first, IList<T> second) =>
            Union(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Intersect<T>(this IList<T> first, IReadOnlyList<T> second) =>
            Intersect(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Intersect<T>(this IList<T> first, IReadOnlyCollection<T> second) =>
            Intersect(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Intersect<T>(this IList<T> first, IList<T> second) =>
            Intersect(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Except<T>(this IList<T> first, IReadOnlyList<T> second) =>
            Except(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Except<T>(this IList<T> first, IReadOnlyCollection<T> second) =>
            Except(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Except<T>(this IList<T> first, IList<T> second) =>
            Except(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static bool SequenceEqual<T>(this IList<T> first, IReadOnlyList<T> second) =>
            SequenceEqual(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static bool SequenceEqual<T>(this IList<T> first, IReadOnlyCollection<T> second) =>
            SequenceEqual(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static bool SequenceEqual<T>(this IList<T> first, IList<T> second) =>
            SequenceEqual(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this IList<TFirst> first,
            IReadOnlyList<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector) =>
            Zip(first.AsEnumerable(), second.AsEnumerable(), resultSelector);

        /// <inheritdoc/>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this IList<TFirst> first,
            IReadOnlyCollection<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector) =>
            Zip(first.AsEnumerable(), second.AsEnumerable(), resultSelector);

        /// <inheritdoc/>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this IList<TFirst> first,
            IList<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector) =>
            Zip(first.AsEnumerable(), second.AsEnumerable(), resultSelector);

        /// <inheritdoc/>
        public static List<T> ToList<T>(this ICollection<T> source) => ToList(source.AsEnumerable());

        /// <inheritdoc/>
        public static HashSet<T> ToHashSet<T>(this ICollection<T> source) => ToHashSet(source.AsEnumerable());

        /// <inheritdoc/>
        public static HashSet<T> ToHashSet<T>(this ICollection<T> source, IEqualityComparer<T> comparer) =>
            ToHashSet(source.AsEnumerable(), comparer);

        /// <inheritdoc/>
        public static List<T> Where<T>(this ICollection<T> source, Func<T, bool> predicate) =>
            Where(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static List<T> Where<T>(this ICollection<T> source, Func<T, int, bool> predicate) =>
            Where(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static List<TResult> Select<TSource, TResult>(this ICollection<TSource> source, Func<TSource, TResult> selector) =>
            Select(source.AsEnumerable(), selector);

        /// <inheritdoc/>
        public static List<TResult> Select<TSource, TResult>(this ICollection<TSource> source, Func<TSource, int, TResult> selector) =>
            Select(source.AsEnumerable(), selector);

        /// <inheritdoc/>
        public static List<TResult> SelectMany<TSource, TResult>(this ICollection<TSource> source, Func<TSource, List<TResult>> selector) =>
            SelectMany(source.AsEnumerable(), selector);

        /// <inheritdoc/>
        public static bool Any<T>(this ICollection<T> source) => Any(source.AsEnumerable());

        /// <inheritdoc/>
        public static bool Any<T>(this ICollection<T> source, Func<T, bool> predicate) =>
            Any(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static bool All<T>(this ICollection<T> source, Func<T, bool> predicate) =>
            All(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static int Count<T>(this ICollection<T> source) => Count(source.AsEnumerable());

        /// <inheritdoc/>
        public static int Count<T>(this ICollection<T> source, Func<T, bool> predicate) =>
            Count(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static bool Contains<T>(this ICollection<T> source, T value) =>
            Contains(source.AsEnumerable(), value);

        /// <inheritdoc/>
        public static T First<T>(this ICollection<T> source) => First(source.AsEnumerable());

        /// <inheritdoc/>
        public static T First<T>(this ICollection<T> source, Func<T, bool> predicate) =>
            First(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static T FirstOrDefault<T>(this ICollection<T> source) => FirstOrDefault(source.AsEnumerable());

        /// <inheritdoc/>
        public static T FirstOrDefault<T>(this ICollection<T> source, Func<T, bool> predicate) =>
            FirstOrDefault(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static T Single<T>(this ICollection<T> source) => Single(source.AsEnumerable());

        /// <inheritdoc/>
        public static T SingleOrDefault<T>(this ICollection<T> source) => SingleOrDefault(source.AsEnumerable());

        /// <inheritdoc/>
        public static T Single<T>(this ICollection<T> source, Func<T, bool> predicate) =>
            Single(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static T SingleOrDefault<T>(this ICollection<T> source, Func<T, bool> predicate) =>
            SingleOrDefault(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static T Last<T>(this ICollection<T> source) => Last(source.AsEnumerable());

        /// <inheritdoc/>
        public static T Last<T>(this ICollection<T> source, Func<T, bool> predicate) =>
            Last(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static T LastOrDefault<T>(this ICollection<T> source) => LastOrDefault(source.AsEnumerable());

        /// <inheritdoc/>
        public static T LastOrDefault<T>(this ICollection<T> source, Func<T, bool> predicate) =>
            LastOrDefault(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static T ElementAt<T>(this ICollection<T> source, int index) =>
            ElementAt(source.AsEnumerable(), index);

        /// <inheritdoc/>
        public static T ElementAtOrDefault<T>(this ICollection<T> source, int index) =>
            ElementAtOrDefault(source.AsEnumerable(), index);

        /// <inheritdoc/>
        public static long LongCount<T>(this ICollection<T> source) => LongCount(source.AsEnumerable());

        /// <inheritdoc/>
        public static long LongCount<T>(this ICollection<T> source, Func<T, bool> predicate) =>
            LongCount(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static List<T> DefaultIfEmpty<T>(this ICollection<T> source) =>
            DefaultIfEmpty(source.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> DefaultIfEmpty<T>(this ICollection<T> source, T defaultValue) =>
            DefaultIfEmpty(source.AsEnumerable(), defaultValue);

        /// <inheritdoc/>
        public static List<T> Take<T>(this ICollection<T> source, int count) =>
            Take(source.AsEnumerable(), count);

        /// <inheritdoc/>
        public static List<T> Skip<T>(this ICollection<T> source, int count) =>
            Skip(source.AsEnumerable(), count);

        /// <inheritdoc/>
        public static List<T> TakeWhile<T>(this ICollection<T> source, Func<T, bool> predicate) =>
            TakeWhile(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static List<T> SkipWhile<T>(this ICollection<T> source, Func<T, bool> predicate) =>
            SkipWhile(source.AsEnumerable(), predicate);

        /// <inheritdoc/>
        public static List<T> Reverse<T>(this ICollection<T> source) => Reverse(source.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Distinct<T>(this ICollection<T> source) => Distinct(source.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Distinct<T>(this ICollection<T> source, IEqualityComparer<T> comparer) =>
            Distinct(source.AsEnumerable(), comparer);

        /// <inheritdoc/>
        public static List<T> Concat<T>(this ICollection<T> first, IEnumerable<T> second) =>
            Concat(first.AsEnumerable(), second);

        /// <inheritdoc/>
        public static List<T> Union<T>(this ICollection<T> first, IEnumerable<T> second) =>
            Union(first.AsEnumerable(), second);

        /// <inheritdoc/>
        public static List<T> Intersect<T>(this ICollection<T> first, IEnumerable<T> second) =>
            Intersect(first.AsEnumerable(), second);

        /// <inheritdoc/>
        public static List<T> Except<T>(this ICollection<T> first, IEnumerable<T> second) =>
            Except(first.AsEnumerable(), second);

        /// <inheritdoc/>
        public static List<T> OrderBy<T, TKey>(this ICollection<T> source, Func<T, TKey> keySelector) =>
            OrderBy(source.AsEnumerable(), keySelector);

        /// <inheritdoc/>
        public static List<T> OrderByDescending<T, TKey>(this ICollection<T> source, Func<T, TKey> keySelector) =>
            OrderByDescending(source.AsEnumerable(), keySelector);

        /// <inheritdoc/>
        public static bool SequenceEqual<T>(this ICollection<T> first, IEnumerable<T> second) =>
            SequenceEqual(first.AsEnumerable(), second);

        /// <inheritdoc/>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this ICollection<TFirst> first,
            IEnumerable<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector) =>
            Zip(first.AsEnumerable(), second, resultSelector);

        /// <inheritdoc/>
        public static T Aggregate<T>(this ICollection<T> source, Func<T, T, T> accumulator) =>
            Aggregate(source.AsEnumerable(), accumulator);

        /// <inheritdoc/>
        public static TAccumulate Aggregate<T, TAccumulate>(
            this ICollection<T> source,
            TAccumulate seed,
            Func<TAccumulate, T, TAccumulate> accumulator) =>
            Aggregate(source.AsEnumerable(), seed, accumulator);

        /// <inheritdoc/>
        public static List<T> Concat<T>(this IEnumerable<T> first, ICollection<T> second) =>
            Concat(first, second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Union<T>(this IEnumerable<T> first, ICollection<T> second) =>
            Union(first, second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Intersect<T>(this IEnumerable<T> first, ICollection<T> second) =>
            Intersect(first, second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Except<T>(this IEnumerable<T> first, ICollection<T> second) =>
            Except(first, second.AsEnumerable());

        /// <inheritdoc/>
        public static bool SequenceEqual<T>(this IEnumerable<T> first, ICollection<T> second) =>
            SequenceEqual(first, second.AsEnumerable());

        /// <inheritdoc/>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this IEnumerable<TFirst> first,
            ICollection<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector) =>
            Zip(first, second.AsEnumerable(), resultSelector);

        /// <inheritdoc/>
        public static List<T> Concat<T>(this List<T> first, ICollection<T> second) =>
            Concat(GetIl2CppEnumerable(first), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Union<T>(this List<T> first, ICollection<T> second) =>
            Union(GetIl2CppEnumerable(first), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Intersect<T>(this List<T> first, ICollection<T> second) =>
            Intersect(GetIl2CppEnumerable(first), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Except<T>(this List<T> first, ICollection<T> second) =>
            Except(GetIl2CppEnumerable(first), second.AsEnumerable());

        /// <inheritdoc/>
        public static bool SequenceEqual<T>(this List<T> first, ICollection<T> second) =>
            SequenceEqual(GetIl2CppEnumerable(first), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this List<TFirst> first,
            ICollection<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector) =>
            Zip(GetIl2CppEnumerable(first), second.AsEnumerable(), resultSelector);

        /// <inheritdoc/>
        public static List<T> Concat<T>(this HashSet<T> first, ICollection<T> second) =>
            Concat(GetIl2CppEnumerable(first), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Union<T>(this HashSet<T> first, ICollection<T> second) =>
            Union(GetIl2CppEnumerable(first), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Intersect<T>(this HashSet<T> first, ICollection<T> second) =>
            Intersect(GetIl2CppEnumerable(first), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Except<T>(this HashSet<T> first, ICollection<T> second) =>
            Except(GetIl2CppEnumerable(first), second.AsEnumerable());

        /// <inheritdoc/>
        public static bool SequenceEqual<T>(this HashSet<T> first, ICollection<T> second) =>
            SequenceEqual(GetIl2CppEnumerable(first), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this HashSet<TFirst> first,
            ICollection<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector) =>
            Zip(GetIl2CppEnumerable(first), second.AsEnumerable(), resultSelector);

        /// <inheritdoc/>
        public static List<T> Concat<T>(this IReadOnlyList<T> first, ICollection<T> second) =>
            Concat(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Union<T>(this IReadOnlyList<T> first, ICollection<T> second) =>
            Union(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Intersect<T>(this IReadOnlyList<T> first, ICollection<T> second) =>
            Intersect(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Except<T>(this IReadOnlyList<T> first, ICollection<T> second) =>
            Except(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static bool SequenceEqual<T>(this IReadOnlyList<T> first, ICollection<T> second) =>
            SequenceEqual(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this IReadOnlyList<TFirst> first,
            ICollection<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector) =>
            Zip(first.AsEnumerable(), second.AsEnumerable(), resultSelector);

        /// <inheritdoc/>
        public static List<T> Concat<T>(this IReadOnlyCollection<T> first, ICollection<T> second) =>
            Concat(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Union<T>(this IReadOnlyCollection<T> first, ICollection<T> second) =>
            Union(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Intersect<T>(this IReadOnlyCollection<T> first, ICollection<T> second) =>
            Intersect(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Except<T>(this IReadOnlyCollection<T> first, ICollection<T> second) =>
            Except(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static bool SequenceEqual<T>(this IReadOnlyCollection<T> first, ICollection<T> second) =>
            SequenceEqual(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this IReadOnlyCollection<TFirst> first,
            ICollection<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector) =>
            Zip(first.AsEnumerable(), second.AsEnumerable(), resultSelector);

        /// <inheritdoc/>
        public static List<T> Concat<T>(this IList<T> first, ICollection<T> second) =>
            Concat(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Union<T>(this IList<T> first, ICollection<T> second) =>
            Union(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Intersect<T>(this IList<T> first, ICollection<T> second) =>
            Intersect(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Except<T>(this IList<T> first, ICollection<T> second) =>
            Except(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static bool SequenceEqual<T>(this IList<T> first, ICollection<T> second) =>
            SequenceEqual(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this IList<TFirst> first,
            ICollection<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector) =>
            Zip(first.AsEnumerable(), second.AsEnumerable(), resultSelector);

        /// <inheritdoc/>
        public static List<T> Concat<T>(this ICollection<T> first, IReadOnlyList<T> second) =>
            Concat(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Concat<T>(this ICollection<T> first, IReadOnlyCollection<T> second) =>
            Concat(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Concat<T>(this ICollection<T> first, IList<T> second) =>
            Concat(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Union<T>(this ICollection<T> first, IReadOnlyList<T> second) =>
            Union(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Union<T>(this ICollection<T> first, IReadOnlyCollection<T> second) =>
            Union(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Union<T>(this ICollection<T> first, IList<T> second) =>
            Union(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Intersect<T>(this ICollection<T> first, IReadOnlyList<T> second) =>
            Intersect(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Intersect<T>(this ICollection<T> first, IReadOnlyCollection<T> second) =>
            Intersect(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Intersect<T>(this ICollection<T> first, IList<T> second) =>
            Intersect(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Except<T>(this ICollection<T> first, IReadOnlyList<T> second) =>
            Except(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Except<T>(this ICollection<T> first, IReadOnlyCollection<T> second) =>
            Except(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Except<T>(this ICollection<T> first, IList<T> second) =>
            Except(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static bool SequenceEqual<T>(this ICollection<T> first, IReadOnlyList<T> second) =>
            SequenceEqual(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static bool SequenceEqual<T>(this ICollection<T> first, IReadOnlyCollection<T> second) =>
            SequenceEqual(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static bool SequenceEqual<T>(this ICollection<T> first, IList<T> second) =>
            SequenceEqual(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this ICollection<TFirst> first,
            IReadOnlyList<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector) =>
            Zip(first.AsEnumerable(), second.AsEnumerable(), resultSelector);

        /// <inheritdoc/>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this ICollection<TFirst> first,
            IReadOnlyCollection<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector) =>
            Zip(first.AsEnumerable(), second.AsEnumerable(), resultSelector);

        /// <inheritdoc/>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this ICollection<TFirst> first,
            IList<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector) =>
            Zip(first.AsEnumerable(), second.AsEnumerable(), resultSelector);

        /// <inheritdoc/>
        public static List<T> Concat<T>(this ICollection<T> first, ICollection<T> second) =>
            Concat(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Union<T>(this ICollection<T> first, ICollection<T> second) =>
            Union(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Intersect<T>(this ICollection<T> first, ICollection<T> second) =>
            Intersect(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<T> Except<T>(this ICollection<T> first, ICollection<T> second) =>
            Except(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static bool SequenceEqual<T>(this ICollection<T> first, ICollection<T> second) =>
            SequenceEqual(first.AsEnumerable(), second.AsEnumerable());

        /// <inheritdoc/>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this ICollection<TFirst> first,
            ICollection<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector) =>
            Zip(first.AsEnumerable(), second.AsEnumerable(), resultSelector);

        /// <inheritdoc/>
        public static List<T> Concat<T>(this IEnumerable<T> first, List<T> second) =>
            Concat(first, GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static List<T> Concat<T>(this IEnumerable<T> first, HashSet<T> second) =>
            Concat(first, GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static List<T> Concat<T>(this List<T> first, IEnumerable<T> second) =>
            Concat(GetIl2CppEnumerable(first), second);

        /// <inheritdoc/>
        public static List<T> Concat<T>(this HashSet<T> first, IEnumerable<T> second) =>
            Concat(GetIl2CppEnumerable(first), second);

        /// <inheritdoc/>
        public static List<T> Concat<T>(this IReadOnlyList<T> first, List<T> second) =>
            Concat(first.AsEnumerable(), GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static List<T> Concat<T>(this IReadOnlyList<T> first, HashSet<T> second) =>
            Concat(first.AsEnumerable(), GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static List<T> Concat<T>(this IReadOnlyCollection<T> first, List<T> second) =>
            Concat(first.AsEnumerable(), GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static List<T> Concat<T>(this IReadOnlyCollection<T> first, HashSet<T> second) =>
            Concat(first.AsEnumerable(), GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static List<T> Concat<T>(this IList<T> first, List<T> second) =>
            Concat(first.AsEnumerable(), GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static List<T> Concat<T>(this IList<T> first, HashSet<T> second) =>
            Concat(first.AsEnumerable(), GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static List<T> Concat<T>(this ICollection<T> first, List<T> second) =>
            Concat(first.AsEnumerable(), GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static List<T> Concat<T>(this ICollection<T> first, HashSet<T> second) =>
            Concat(first.AsEnumerable(), GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static List<T> Union<T>(this IEnumerable<T> first, List<T> second) =>
            Union(first, GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static List<T> Union<T>(this IEnumerable<T> first, HashSet<T> second) =>
            Union(first, GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static List<T> Union<T>(this List<T> first, IEnumerable<T> second) =>
            Union(GetIl2CppEnumerable(first), second);

        /// <inheritdoc/>
        public static List<T> Union<T>(this HashSet<T> first, IEnumerable<T> second) =>
            Union(GetIl2CppEnumerable(first), second);

        /// <inheritdoc/>
        public static List<T> Union<T>(this IReadOnlyList<T> first, List<T> second) =>
            Union(first.AsEnumerable(), GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static List<T> Union<T>(this IReadOnlyList<T> first, HashSet<T> second) =>
            Union(first.AsEnumerable(), GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static List<T> Union<T>(this IReadOnlyCollection<T> first, List<T> second) =>
            Union(first.AsEnumerable(), GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static List<T> Union<T>(this IReadOnlyCollection<T> first, HashSet<T> second) =>
            Union(first.AsEnumerable(), GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static List<T> Union<T>(this IList<T> first, List<T> second) =>
            Union(first.AsEnumerable(), GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static List<T> Union<T>(this IList<T> first, HashSet<T> second) =>
            Union(first.AsEnumerable(), GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static List<T> Union<T>(this ICollection<T> first, List<T> second) =>
            Union(first.AsEnumerable(), GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static List<T> Union<T>(this ICollection<T> first, HashSet<T> second) =>
            Union(first.AsEnumerable(), GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static List<T> Intersect<T>(this IEnumerable<T> first, List<T> second) =>
            Intersect(first, GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static List<T> Intersect<T>(this IEnumerable<T> first, HashSet<T> second) =>
            Intersect(first, GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static List<T> Intersect<T>(this List<T> first, IEnumerable<T> second) =>
            Intersect(GetIl2CppEnumerable(first), second);

        /// <inheritdoc/>
        public static List<T> Intersect<T>(this HashSet<T> first, IEnumerable<T> second) =>
            Intersect(GetIl2CppEnumerable(first), second);

        /// <inheritdoc/>
        public static List<T> Intersect<T>(this IReadOnlyList<T> first, List<T> second) =>
            Intersect(first.AsEnumerable(), GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static List<T> Intersect<T>(this IReadOnlyList<T> first, HashSet<T> second) =>
            Intersect(first.AsEnumerable(), GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static List<T> Intersect<T>(this IReadOnlyCollection<T> first, List<T> second) =>
            Intersect(first.AsEnumerable(), GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static List<T> Intersect<T>(this IReadOnlyCollection<T> first, HashSet<T> second) =>
            Intersect(first.AsEnumerable(), GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static List<T> Intersect<T>(this IList<T> first, List<T> second) =>
            Intersect(first.AsEnumerable(), GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static List<T> Intersect<T>(this IList<T> first, HashSet<T> second) =>
            Intersect(first.AsEnumerable(), GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static List<T> Intersect<T>(this ICollection<T> first, List<T> second) =>
            Intersect(first.AsEnumerable(), GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static List<T> Intersect<T>(this ICollection<T> first, HashSet<T> second) =>
            Intersect(first.AsEnumerable(), GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static List<T> Except<T>(this IEnumerable<T> first, List<T> second) =>
            Except(first, GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static List<T> Except<T>(this IEnumerable<T> first, HashSet<T> second) =>
            Except(first, GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static List<T> Except<T>(this List<T> first, IEnumerable<T> second) =>
            Except(GetIl2CppEnumerable(first), second);

        /// <inheritdoc/>
        public static List<T> Except<T>(this HashSet<T> first, IEnumerable<T> second) =>
            Except(GetIl2CppEnumerable(first), second);

        /// <inheritdoc/>
        public static List<T> Except<T>(this IReadOnlyList<T> first, List<T> second) =>
            Except(first.AsEnumerable(), GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static List<T> Except<T>(this IReadOnlyList<T> first, HashSet<T> second) =>
            Except(first.AsEnumerable(), GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static List<T> Except<T>(this IReadOnlyCollection<T> first, List<T> second) =>
            Except(first.AsEnumerable(), GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static List<T> Except<T>(this IReadOnlyCollection<T> first, HashSet<T> second) =>
            Except(first.AsEnumerable(), GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static List<T> Except<T>(this IList<T> first, List<T> second) =>
            Except(first.AsEnumerable(), GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static List<T> Except<T>(this IList<T> first, HashSet<T> second) =>
            Except(first.AsEnumerable(), GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static List<T> Except<T>(this ICollection<T> first, List<T> second) =>
            Except(first.AsEnumerable(), GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static List<T> Except<T>(this ICollection<T> first, HashSet<T> second) =>
            Except(first.AsEnumerable(), GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static bool SequenceEqual<T>(this IEnumerable<T> first, List<T> second) =>
            SequenceEqual(first, GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static bool SequenceEqual<T>(this IEnumerable<T> first, HashSet<T> second) =>
            SequenceEqual(first, GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static bool SequenceEqual<T>(this List<T> first, IEnumerable<T> second) =>
            SequenceEqual(GetIl2CppEnumerable(first), second);

        /// <inheritdoc/>
        public static bool SequenceEqual<T>(this HashSet<T> first, IEnumerable<T> second) =>
            SequenceEqual(GetIl2CppEnumerable(first), second);

        /// <inheritdoc/>
        public static bool SequenceEqual<T>(this IReadOnlyList<T> first, List<T> second) =>
            SequenceEqual(first.AsEnumerable(), GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static bool SequenceEqual<T>(this IReadOnlyList<T> first, HashSet<T> second) =>
            SequenceEqual(first.AsEnumerable(), GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static bool SequenceEqual<T>(this IReadOnlyCollection<T> first, List<T> second) =>
            SequenceEqual(first.AsEnumerable(), GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static bool SequenceEqual<T>(this IReadOnlyCollection<T> first, HashSet<T> second) =>
            SequenceEqual(first.AsEnumerable(), GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static bool SequenceEqual<T>(this IList<T> first, List<T> second) =>
            SequenceEqual(first.AsEnumerable(), GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static bool SequenceEqual<T>(this IList<T> first, HashSet<T> second) =>
            SequenceEqual(first.AsEnumerable(), GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static bool SequenceEqual<T>(this ICollection<T> first, List<T> second) =>
            SequenceEqual(first.AsEnumerable(), GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static bool SequenceEqual<T>(this ICollection<T> first, HashSet<T> second) =>
            SequenceEqual(first.AsEnumerable(), GetIl2CppEnumerable(second));

        /// <inheritdoc/>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this IEnumerable<TFirst> first,
            List<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector) =>
            Zip(first, GetIl2CppEnumerable(second), resultSelector);

        /// <inheritdoc/>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this IEnumerable<TFirst> first,
            HashSet<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector) =>
            Zip(first, GetIl2CppEnumerable(second), resultSelector);

        /// <inheritdoc/>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this List<TFirst> first,
            IEnumerable<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector) =>
            Zip(GetIl2CppEnumerable(first), second, resultSelector);

        /// <inheritdoc/>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this HashSet<TFirst> first,
            IEnumerable<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector) =>
            Zip(GetIl2CppEnumerable(first), second, resultSelector);

        /// <inheritdoc/>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this IReadOnlyList<TFirst> first,
            List<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector) =>
            Zip(first.AsEnumerable(), GetIl2CppEnumerable(second), resultSelector);

        /// <inheritdoc/>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this IReadOnlyList<TFirst> first,
            HashSet<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector) =>
            Zip(first.AsEnumerable(), GetIl2CppEnumerable(second), resultSelector);

        /// <inheritdoc/>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this IReadOnlyCollection<TFirst> first,
            List<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector) =>
            Zip(first.AsEnumerable(), GetIl2CppEnumerable(second), resultSelector);

        /// <inheritdoc/>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this IReadOnlyCollection<TFirst> first,
            HashSet<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector) =>
            Zip(first.AsEnumerable(), GetIl2CppEnumerable(second), resultSelector);

        /// <inheritdoc/>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this IList<TFirst> first,
            List<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector) =>
            Zip(first.AsEnumerable(), GetIl2CppEnumerable(second), resultSelector);

        /// <inheritdoc/>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this IList<TFirst> first,
            HashSet<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector) =>
            Zip(first.AsEnumerable(), GetIl2CppEnumerable(second), resultSelector);

        /// <inheritdoc/>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this ICollection<TFirst> first,
            List<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector) =>
            Zip(first.AsEnumerable(), GetIl2CppEnumerable(second), resultSelector);

        /// <inheritdoc/>
        public static List<TResult> Zip<TFirst, TSecond, TResult>(
            this ICollection<TFirst> first,
            HashSet<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector) =>
            Zip(first.AsEnumerable(), GetIl2CppEnumerable(second), resultSelector);
    }
}
