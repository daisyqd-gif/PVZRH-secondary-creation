namespace CustomPlantClass.Main
{
    /// <summary>
    /// Helpers for IEnumerables and lists
    /// </summary>
    public static class ListHelper
    {
        /// <summary>
        /// Works like python's range
        /// </summary>
        /// <param name="stop">The end of the range</param>
        /// <returns>An ienuerable containing the range</returns>
        public static IEnumerable<int> Range(int stop)
        {
            for (int i = 0; i < stop; i++)
                yield return i;
        }
        /// <summary>
        /// Works like python's range
        /// </summary>
        /// <param name="start">The start of the range</param>
        /// <param name="stop">The end of the range</param>
        /// <returns>An ienuerable containing the range</returns>
        public static IEnumerable<int> Range(int start, int stop)
        {
            for (int i = start; i < stop; i++)
                yield return i;
        }
        /// <summary>
        /// Works like python's range
        /// </summary>
        /// <param name="start">The start of the range</param>
        /// <param name="stop">The end of the range</param>
        /// <param name="step">The step of the range</param>
        /// <returns>An ienuerable containing the range</returns>
        public static IEnumerable<int> Range(int start, int stop, int step)
        {
            for (int i = start; i < stop; i += step)
                yield return i;
        }
        /// <summary>
        /// Enumerates a sequence while also returning the index of each element.
        /// </summary>
        /// <returns>A sequence that contains the index and the value</returns>
        public static IEnumerable<(int index, T value)> Enumerate<T>(this IEnumerable<T> seq)
        {
            int i = 0;
            foreach (var v in seq)
                yield return (i++, v);
        }
        /// <summary>
        /// Run an action an amount of times
        /// </summary>
        /// <param name="action"></param>
        /// <param name="count"></param>
        public static void Repeat(this Action action, int count)
        {
            for (int i = 0; i < count; i++)
                action();
        }
        [Obsolete("Use System.Linq's")]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static IEnumerable<List<T>> Chunks<T>(this IEnumerable<T> seq, int size)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            List<T> buffer = new(size);
            foreach (var x in seq)
            {
                buffer.Add(x);
                if (buffer.Count == size)
                {
                    yield return buffer;
                    buffer = new(size);
                }
            }
            if (buffer.Count > 0)
                yield return buffer;
        }
        /// <summary>
        /// Returns a list containing the original fusion pair and its mirrored version.
        /// </summary>
        public static List<(ID, ID)> MirrorTuple((ID, ID) input)
        {
            return new List<(ID, ID)>
            {
                (input.Item1, input.Item2),
                (input.Item2, input.Item1)
            };
        }

        /// <summary>
        /// Flattens an array of fusion lists into a single list.
        /// </summary>
        public static List<(ID, ID)> FlattenFusionArray(List<(ID, ID)>[] input)
        {
            var result = new List<(ID, ID)>();
            if (input == null) return result;

            foreach (var sublist in input)
            {
                if (sublist == null) continue;
                foreach (var pair in sublist)
                    result.Add(pair);
            }
            return result;
        }

        /// <summary>
        /// Mirrors every fusion pair in a list.
        /// </summary>
        public static List<(ID, ID)> MirrorList(List<(ID, ID)> input)
        {
            var result = new List<(ID, ID)>();
            if (input == null) return result;

            foreach (var pair in input)
            {
                result.Add((pair.Item1, pair.Item2));
                result.Add((pair.Item2, pair.Item1));
            }
            return result;
        }

        /// <summary>
        /// Removes duplicate fusion pairs.
        /// </summary>
        public static List<(ID, ID)> DeduplicateFusions(List<(ID, ID)> input)
        {
            var set = new HashSet<(int, int)>();
            var result = new List<(ID, ID)>();

            foreach (var pair in input)
            {
                var key = ((int)pair.Item1, (int)pair.Item2);
                if (set.Add(key))
                    result.Add(pair);
            }
            return result;
        }

        /// <summary>
        /// Creates a mirrored fusion list from simple pair definitions.
        /// </summary>
        public static List<(ID, ID)> Fusion(params (ID, ID)[] pairs)
        {
            var result = new List<(ID, ID)>();
            foreach (var p in pairs)
            {
                result.Add((p.Item1, p.Item2));
                result.Add((p.Item2, p.Item1));
            }
            return result;
        }
        /// <summary>
        /// Creates a list
        /// </summary>
        /// <returns>An IEnumerable</returns>
        public static IEnumerable<T> CreateList<T>(params T[] input) => [.. input];
        /// <summary>
        /// Adds multiple items to a sequence
        /// </summary>
        public static void AddMultiple<T>(this IList<T> self, params T[] input)
        {
            foreach (var i in input)
            {
                self.Add(i);
            }
        }
        /// <summary>
        /// Adds multiple items to a sequence
        /// </summary>
        public static void AddMultiple<T>(this HashSet<T> self, params T[] input)
        {
            foreach (var i in input)
            {
                self.Add(i);
            }
        }
        /// <summary>
        /// Adds multiple items to a sequence
        /// </summary>
        public static void AddMultiple<Tkey, Tvalue>(this Dictionary<Tkey, Tvalue> self, params KeyValuePair<Tkey, Tvalue>[] input)
        {
            foreach (var i in input)
            {
                if (!self.TryAdd(i.Key, i.Value))
                {
                    Debug.LogError($"Duplicate key {i.Key}");
                }
            }
        }
        /// <summary>
        /// Runs an action per item in a sequence
        /// </summary>
        public static void ActionPerItem<T>(this IEnumerable<T> self, Action<T> action)
        {
            foreach (T i in self)
            {
                try
                {
                    action(i);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
        }
        /// <summary>
        /// Runs an action per item in a sequence
        /// </summary>
        public static void ActionPerItem<T>(this IEnumerable<T> self, Func<T, bool> filter, Action<T> action)
        {
            foreach (T i in self)
            {
                try
                {
                    if (filter(i)) action(i);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
        }
        /// <summary>
        /// Partitions an ienumerable based on a predicate
        /// </summary>
        /// <returns>A ValueTuple of yes and no</returns>
        public static (IEnumerable<T> yes, IEnumerable<T> no) Partition<T>(this IEnumerable<T> seq, Func<T, bool> pred)
        {
            var yes = new List<T>();
            var no = new List<T>();
            foreach (var x in seq)
                (pred(x) ? yes : no).Add(x);
            return (yes, no);
        }
        /// <summary>
        /// Shuffles an IEnumerable
        /// </summary>
        /// <param name="self"></param>
        public static void Shuffle( this IEnumerable<int> self )
        {
            IList<int> list;
            if( self is IList<int> lst)
            {
                list = lst;
            }
            else
            {
                list = self.ToList();
            }
            var rng = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }
    }
}