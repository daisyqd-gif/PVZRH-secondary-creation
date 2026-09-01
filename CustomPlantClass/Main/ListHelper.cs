namespace CustomPlantClass.Main
{
    public static class ListHelper
    {
        public static IEnumerable<int> Range(int stop)
        {
            for (int i = 0; i < stop; i++)
                yield return i;
        }

        public static IEnumerable<int> Range(int start, int stop)
        {
            for (int i = start; i < stop; i++)
                yield return i;
        }

        public static IEnumerable<int> Range(int start, int stop, int step)
        {
            for (int i = start; i < stop; i += step)
                yield return i;
        }
        public static IEnumerable<(int index, T value)> Enumerate<T>(this IEnumerable<T> seq)
        {
            int i = 0;
            foreach (var v in seq)
                yield return (i++, v);
        }
        public static void Repeat(this Action action, int count)
        {
            for (int i = 0; i < count; i++)
                action();
        }
        public static IEnumerable<List<T>> Chunks<T>(this IEnumerable<T> seq, int size)
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

        public static IEnumerable<T> CreateList<T>(params T[] input) => [.. input];

        public static void AddMultiple<T>(this IList<T> self, params T[] input)
        {
            foreach (var i in input)
            {
                self.Add(i);
            }
        }
        public static void AddMultiple<T>(this HashSet<T> self, params T[] input)
        {
            foreach (var i in input)
            {
                self.Add(i);
            }
        }
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
        public static (IEnumerable<T> yes, IEnumerable<T> no) Partition<T>(this IEnumerable<T> seq, Func<T, bool> pred)
        {
            var yes = new List<T>();
            var no = new List<T>();
            foreach (var x in seq)
                (pred(x) ? yes : no).Add(x);
            return (yes, no);
        }
    }
}