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
            if (source == null) throw new ArgumentNullException(nameof(source));

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
            if (source == null) throw new ArgumentNullException(nameof(source));
            return Enumerate(source);
        }

        static System.Collections.Generic.IEnumerable<T> GetManagedEnumerable<T>(HashSet<T> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
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

        static IEnumerable<T> GetIl2CppEnumerable<T>(List<T> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            return source.Cast<IEnumerable<T>>();
        }

        static IEnumerable<T> GetIl2CppEnumerable<T>(HashSet<T> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            return source.Cast<IEnumerable<T>>();
        }

        /// <summary>
        /// Returns the source as an enumerable sequence.
        /// </summary>
        public static IEnumerable<T> AsEnumerable<T>(this List<T> source)
        {
            ArgumentNullException.ThrowIfNull(source);
            return source.Cast<IEnumerable<T>>();
        }

        /// <summary>
        /// Returns the source as an enumerable sequence.
        /// </summary>
        public static IEnumerable<T> AsEnumerable<T>(this HashSet<T> source)
        {
            ArgumentNullException.ThrowIfNull(source);
            return source.Cast<IEnumerable<T>>();
        }

        /// <summary>
        /// Creates a new IL2CPP list containing the source elements.
        /// </summary>
        public static List<T> ToList<T>(this IEnumerable<T> source)
        {
            return Il2CppSystem.Linq.Enumerable.ToList(source);
        }

        /// <summary>
        /// Creates a new IL2CPP list containing the source elements.
        /// </summary>
        public static List<T> ToList<T>(this HashSet<T> source)
        {
            return Il2CppSystem.Linq.Enumerable.ToList(GetIl2CppEnumerable(source));
        }

        /// <summary>
        /// Creates a new IL2CPP list containing the source elements.
        /// </summary>
        public static List<T> ToList<T>(this List<T> source)
        {
            return Il2CppSystem.Linq.Enumerable.ToList(GetIl2CppEnumerable(source));
        }

        /// <summary>
        /// Creates an IL2CPP hash set from the source elements.
        /// </summary>
        public static HashSet<T> ToHashSet<T>(this HashSet<T> source)
        {
            return Il2CppSystem.Linq.Enumerable.ToHashSet(GetIl2CppEnumerable(source));
        }

        /// <summary>
        /// Creates an IL2CPP hash set from the source elements, using the specified equality comparer when provided.
        /// </summary>
        public static HashSet<T> ToHashSet<T>(this List<T> source)
        {
            return Il2CppSystem.Linq.Enumerable.ToHashSet(GetIl2CppEnumerable(source));
        }

        /// <summary>
        /// Creates an IL2CPP hash set from the source elements, using the specified equality comparer when provided.
        /// </summary>
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            return Il2CppSystem.Linq.Enumerable.ToHashSet(source);
        }

        /// <summary>
        /// Creates an IL2CPP hash set from the source elements, using the specified equality comparer when provided.
        /// </summary>
        public static HashSet<T> ToHashSet<T>(this List<T> source, IEqualityComparer<T> comparer)
        {
            return Il2CppSystem.Linq.Enumerable.ToHashSet(GetIl2CppEnumerable(source), comparer);
        }

        /// <summary>
        /// Creates an IL2CPP hash set from the source elements, using the specified equality comparer when provided.
        /// </summary>
        public static HashSet<T> ToHashSet<T>(this HashSet<T> source, IEqualityComparer<T> comparer)
        {
            return Il2CppSystem.Linq.Enumerable.ToHashSet(GetIl2CppEnumerable(source), comparer);
        }

        /// <summary>
        /// Creates an IL2CPP hash set from the source elements, using the specified equality comparer when provided.
        /// </summary>
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer)
        {
            return Il2CppSystem.Linq.Enumerable.ToHashSet(source, comparer);
        }

        /// <summary>
        /// Filters the source elements based on a predicate.
        /// </summary>
        public static List<T> Where<T>(this List<T> source, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Where(GetIl2CppEnumerable(source), predicate));
        }

        /// <summary>
        /// Filters the source elements based on a predicate.
        /// </summary>
        public static List<T> Where<T>(this HashSet<T> source, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Where(GetIl2CppEnumerable(source), predicate));
        }

        /// <summary>
        /// Filters the source elements based on a predicate.
        /// </summary>
        public static List<T> Where<T>(this List<T> source, Func<T, int, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            int index = 0;
            Func<T, bool> indexed = item => predicate(item, index++);
            Il2CppSystem.Func<T, bool> indexedPredicate = indexed;
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Where(GetIl2CppEnumerable(source), indexedPredicate));
        }

        /// <summary>
        /// Filters the source elements based on a predicate.
        /// </summary>
        public static List<T> Where<T>(this HashSet<T> source, Func<T, int, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            int index = 0;
            Func<T, bool> indexed = item => predicate(item, index++);
            Il2CppSystem.Func<T, bool> indexedPredicate = indexed;
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Where(GetIl2CppEnumerable(source), indexedPredicate));
        }

        /// <summary>
        /// Projects each source element into a new form.
        /// </summary>
        public static List<TResult> Select<TSource, TResult>(this List<TSource> source, Func<TSource, TResult> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Select<TSource, TResult>(GetIl2CppEnumerable(source), selector));
        }

        /// <summary>
        /// Projects each source element into a new form.
        /// </summary>
        public static List<TResult> Select<TSource, TResult>(this HashSet<TSource> source, Func<TSource, TResult> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Select<TSource, TResult>(GetIl2CppEnumerable(source), selector));
        }

        /// <summary>
        /// Projects each source element into a new form.
        /// </summary>
        public static List<TResult> Select<TSource, TResult>(this List<TSource> source, Func<TSource, int, TResult> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            return ToIl2CppList(Il2CppSystem.Linq.Enumerable.Select<TSource, TResult>(GetIl2CppEnumerable(source), selector));
        }

        /// <summary>
        /// Projects each source element into a new form.
        /// </summary>
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
            if (source == null) throw new ArgumentNullException(nameof(source));
            return source.Count != 0;
        }

        /// <summary>
        /// Determines whether the source contains any elements or whether any element satisfies a condition.
        /// </summary>
        public static bool Any<T>(this HashSet<T> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
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
            if (source == null) throw new ArgumentNullException(nameof(source));
            return source.Count;
        }

        /// <summary>
        /// Returns the number of elements in the source or the number that satisfy a condition.
        /// </summary>
        public static int Count<T>(this HashSet<T> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
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
            if (source == null) throw new ArgumentNullException(nameof(source));
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
            if (source == null) throw new ArgumentNullException(nameof(source));
            return source.Count;
        }

        /// <summary>
        /// Returns an Int64 representing the number of elements in the source or the number that satisfy a condition.
        /// </summary>
        public static long LongCount<T>(this HashSet<T> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
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
    }
}
