namespace CustomPlantClass.Runtime.Tasks
{
    /// <summary>
    /// Schedules and advances timed and fixed-update delay awaiters.
    /// </summary>
    public class DelayScheduler : MonoBehaviour
    {
        private class Entry
        {
            public IDelay awaiter;
            public float remaining;
            public bool useFixedUpdate;
            public bool isScaled;
            public Action WhenDone = null;
        }

        private static readonly List<Entry> entries = new();

        /// <summary>
        /// Schedules a delay for the specified duration.
        /// </summary>
        /// <param name="seconds">The duration of the delay in seconds.</param>
        /// <param name="awaiter">The delay awaiter to complete.</param>
        public static void Schedule(float seconds, Delay awaiter)
        {
            entries.Add(new Entry
            {
                awaiter = awaiter,
                remaining = seconds,
                useFixedUpdate = false,
                isScaled = false
            });
        }

        /// <summary>
        /// Schedules a delay whose countdown uses its configured speed multiplier.
        /// </summary>
        /// <param name="awaiter">The scaled delay awaiter to complete.</param>
        public static void ScheduleScaled(DelayScaled awaiter)
        {
            entries.Add(new Entry
            {
                awaiter = awaiter,
                remaining = awaiter.remaining,
                useFixedUpdate = false,
                isScaled = true
            });
        }

        /// <summary>
        /// Schedules a delay to complete on the next fixed update.
        /// </summary>
        /// <param name="awaiter">The delay awaiter to complete.</param>
        public static void ScheduleFixedUpate(Delay awaiter)
        {
            entries.Add(new Entry
            {
                awaiter = awaiter,
                remaining = 0f,
                useFixedUpdate = true,
                isScaled = false
            });
        }

        /// <summary>
        /// Schedules a delay and invokes an action after it completes.
        /// </summary>
        /// <param name="seconds">The duration of the delay in seconds.</param>
        /// <param name="awaiter">The delay awaiter to complete.</param>
        /// <param name="whenDone">The action to invoke after completion.</param>
        public static void Schedule(float seconds, Delay awaiter, Action whenDone)
        {
            entries.Add(new Entry
            {
                awaiter = awaiter,
                remaining = seconds,
                useFixedUpdate = false,
                isScaled = false,
                WhenDone = whenDone
            });
        }

        /// <summary>
        /// Schedules a scaled delay and invokes an action after it completes.
        /// </summary>
        /// <param name="awaiter">The scaled delay awaiter to complete.</param>
        /// <param name="whenDone">The action to invoke after completion.</param>
        public static void ScheduleScaled(DelayScaled awaiter, Action whenDone)
        {
            entries.Add(new Entry
            {
                awaiter = awaiter,
                remaining = awaiter.remaining,
                useFixedUpdate = false,
                isScaled = true,
                WhenDone = whenDone
            });
        }

        /// <summary>
        /// Schedules a delay for the next fixed update and invokes an action after it completes.
        /// </summary>
        /// <param name="awaiter">The delay awaiter to complete.</param>
        /// <param name="whenDone">The action to invoke after completion.</param>
        public static void ScheduleFixedUpate(Delay awaiter, Action whenDone)
        {
            entries.Add(new Entry
            {
                awaiter = awaiter,
                remaining = 0f,
                useFixedUpdate = true,
                isScaled = false,
                WhenDone = whenDone
            });
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public void FixedUpdate()
        {
            for (int i = entries.Count - 1; i >= 0; i--)
            {
                var e = entries[i];

                if (!e.useFixedUpdate)
                    continue;

                if (e.awaiter.IsCompleted)
                {
                    entries.RemoveAt(i);
                    continue;
                }

                e.awaiter.Complete();
                if(e.WhenDone != null)
                {
                    try
                    {
                        e.WhenDone();
                    }
                    catch(Exception ex)
                    {
                        Debug.LogError(ex.Message);
                    }
                }
                entries.RemoveAt(i);
            }
        }
        public static float prevTime = Time.time;
        /*
        public void Update()
        {
            OnPlayerLoop();
        }
        */
        public static void OnPlayerLoop()
        {
            float dt = Time.time - prevTime;
            prevTime = Time.time;

            for (int i = entries.Count - 1; i >= 0; i--)
            {
                var e = entries[i];

                if (e.useFixedUpdate)
                    continue;

                if (e.awaiter.IsCompleted)
                {
                    entries.RemoveAt(i);
                    continue;
                }

                float mult = 1f;

                if (e.isScaled && e.awaiter is DelayScaled scaled)
                    mult = scaled.speedMultiplier?.Invoke() ?? 1f;

                e.remaining -= dt * mult;

                if (e.remaining <= 0f)
                {
                    e.awaiter.Complete();
                    if(e.WhenDone != null)
                    {
                        try
                        {
                            e.WhenDone();
                        }
                        catch(Exception ex)
                        {
                            Debug.LogError(ex.Message);
                        }
                    }
                    entries.RemoveAt(i);
                }
            }
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
    /// <summary>
    /// Schedules and advances predicate-based waits.
    /// </summary>
    public class WaitUntilScheduler : MonoBehaviour
    {
        private class Entry
        {
            public WaitUntil awaiter;
            public Action action; // null if no action
        }

        private static readonly List<Entry> entries = new();

        /// <summary>
        /// Schedules a predicate-based wait.
        /// </summary>
        /// <param name="awaiter">The wait awaiter to check.</param>
        public static void Schedule(WaitUntil awaiter)
        {
            entries.Add(new Entry { awaiter = awaiter, action = null });
        }

        /// <summary>
        /// Schedules a predicate-based wait and invokes an action when it completes.
        /// </summary>
        /// <param name="awaiter">The wait awaiter to check.</param>
        /// <param name="action">The action to invoke after the wait completes.</param>
        public static void Schedule(WaitUntil awaiter, Action action)
        {
            entries.Add(new Entry { awaiter = awaiter, action = action });
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static void OnPlayerLoop()
        {
            for (int i = entries.Count - 1; i >= 0; i--)
            {
                var e = entries[i];
                var w = e.awaiter;

                if (w.IsCompleted || w.Check())
                {
                    // run action if present
                    if (e.action != null)
                    {
                        try
                        {
                            e.action();
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError(ex.ToString());
                        }
                    }

                    entries.RemoveAt(i);
                }
            }
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}