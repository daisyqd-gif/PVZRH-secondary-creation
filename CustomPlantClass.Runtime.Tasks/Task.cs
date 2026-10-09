namespace CustomPlantClass.Runtime.Tasks
{
    /// <summary>
    /// Represents an awaitable delay that completes after a scheduled duration.
    /// </summary>
    public class Delay : IDelay
    {
        private bool _isCompleted;
        private Action _continuation;
        private readonly CancellationToken _token;

        /// <summary>
        /// Initializes a delay that can be canceled by the specified token.
        /// </summary>
        /// <param name="token">The token that can cancel the delay.</param>
        public Delay(CancellationToken token)
        {
            _token = token;
        }

        /// <summary>
        /// Initializes a delay that cannot be canceled.
        /// </summary>
        public Delay()
        {
            _token = null;
        }

        /// <summary>
        /// Gets whether the delay has completed or its cancellation token has been canceled.
        /// </summary>
        public bool IsCompleted => _isCompleted || (_token?.IsCanceled ?? false);

        /// <summary>
        /// Registers the continuation to invoke when the delay completes.
        /// </summary>
        /// <param name="continuation">The continuation to invoke.</param>
        public void OnCompleted(Action continuation)
        {
            _continuation = continuation;
        }

        /// <summary>
        /// Marks the delay as complete and invokes its registered continuation unless canceled.
        /// </summary>
        public void Complete()
        {
            if (_token?.IsCanceled ?? false)
            {
                _isCompleted = true;
                return;
            }

            _isCompleted = true;
            _continuation?.Invoke();
        }

        /// <summary>
        /// Gets the result of the completed delay.
        /// </summary>
        public void GetResult() { }
    }
    /// <summary>
    /// Represents an awaitable delay whose countdown is adjusted by a speed multiplier.
    /// </summary>
    public class DelayScaled : IDelay
    {
        private bool _isCompleted;
        private Action _continuation;
        private readonly CancellationToken _token;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public float remaining;
        public Func<float> speedMultiplier; // dynamic multiplier
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Initializes a scaled delay with a duration, a dynamic speed multiplier, and an optional cancellation token.
        /// </summary>
        /// <param name="seconds">The initial duration in seconds.</param>
        /// <param name="speed">A function that supplies the multiplier applied to elapsed time.</param>
        /// <param name="token">An optional token that can cancel the delay.</param>
        public DelayScaled(float seconds, Func<float> speed, CancellationToken token = null)
        {
            remaining = seconds;
            speedMultiplier = speed;
            _token = token;
        }

        /// <summary>
        /// Gets whether the delay has completed or its cancellation token has been canceled.
        /// </summary>
        public bool IsCompleted => _isCompleted || (_token?.IsCanceled ?? false);

        /// <summary>
        /// Registers the continuation to invoke when the delay completes.
        /// </summary>
        /// <param name="continuation">The continuation to invoke.</param>
        public void OnCompleted(Action continuation) => _continuation = continuation;

        /// <summary>
        /// Marks the delay as complete and invokes its registered continuation.
        /// </summary>
        public void Complete()
        {
            if (_isCompleted) return;
            _isCompleted = true;
            _continuation?.Invoke();
        }

        /// <summary>
        /// Gets the result of the completed delay.
        /// </summary>
        public void GetResult() { }
    }
    /// <summary>
    /// Represents an awaitable operation that completes when a predicate becomes true.
    /// </summary>
    public class WaitUntil : IDelay
    {
        private bool _isCompleted;
        private Action _continuation;
        private readonly CancellationToken _token;
        private readonly Func<bool> _predicate;

        /// <summary>
        /// Initializes a wait that can be canceled by the specified token.
        /// </summary>
        /// <param name="predicate">The condition that must become true to complete the wait.</param>
        /// <param name="token">The token that can cancel the wait.</param>
        public WaitUntil(Func<bool> predicate, CancellationToken token)
        {
            _predicate = predicate;
            _token = token;
        }

        /// <summary>
        /// Initializes a wait that completes when the specified predicate becomes true.
        /// </summary>
        /// <param name="predicate">The condition that must become true to complete the wait.</param>
        public WaitUntil(Func<bool> predicate)
        {
            _predicate = predicate;
            _token = null;
        }

        /// <summary>
        /// Gets whether the wait has completed or its cancellation token has been canceled.
        /// </summary>
        public bool IsCompleted => _isCompleted || (_token?.IsCanceled ?? false);

        /// <summary>
        /// Registers the continuation to invoke when the wait completes.
        /// </summary>
        /// <param name="continuation">The continuation to invoke.</param>
        public void OnCompleted(Action continuation)
        {
            _continuation = continuation;
        }

        internal bool Check()
        {
            if (_isCompleted) return true;
            if (_token?.IsCanceled ?? false)
            {
                _isCompleted = true;
                return true;
            }

            if (_predicate != null && _predicate())
            {
                Complete();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Marks the wait as complete and invokes its registered continuation.
        /// </summary>
        public void Complete()
        {
            if (_isCompleted) return;

            _isCompleted = true;
            _continuation?.Invoke();
        }

        /// <summary>
        /// Gets the result of the completed wait.
        /// </summary>
        public void GetResult() { }
    }
    /// <summary>
    /// Provides an awaitable handle for a predicate-based wait.
    /// </summary>
    public readonly struct WaitUntilTask
    {
        private readonly WaitUntil _awaiter;

        /// <summary>
        /// Initializes the handle with the specified awaiter.
        /// </summary>
        /// <param name="awaiter">The awaiter that completes when its predicate is satisfied.</param>
        public WaitUntilTask(WaitUntil awaiter)
        {
            _awaiter = awaiter;
        }

        /// <summary>
        /// Gets the awaiter used by this handle.
        /// </summary>
        /// <returns>The underlying predicate-based awaiter.</returns>
        public WaitUntil GetAwaiter() => _awaiter;

        /// <summary>
        /// Schedules a wait that completes when the predicate is true or the token is canceled.
        /// </summary>
        /// <param name="predicate">The condition to wait for.</param>
        /// <param name="token">The token that can cancel the wait.</param>
        /// <returns>An awaitable handle for the scheduled wait.</returns>
        public static WaitUntilTask WaitUntil(Func<bool> predicate, CancellationToken token)
        {
            var awaiter = new WaitUntil(predicate, token);
            WaitUntilScheduler.Schedule(awaiter);
            return new WaitUntilTask(awaiter);
        }

        /// <summary>
        /// Schedules a wait that completes when the predicate becomes true.
        /// </summary>
        /// <param name="predicate">The condition to wait for.</param>
        /// <returns>An awaitable handle for the scheduled wait.</returns>
        public static WaitUntilTask WaitUntil(Func<bool> predicate)
        {
            var awaiter = new WaitUntil(predicate);
            WaitUntilScheduler.Schedule(awaiter);
            return new WaitUntilTask(awaiter);
        }
    }
    /// <summary>
    /// Provides awaitable operations for timed and fixed-update delays.
    /// </summary>
    public struct DelayTask
    {
        private readonly IDelay _awaiter;

        /// <summary>
        /// Initializes the handle with the specified delay awaiter.
        /// </summary>
        /// <param name="awaiter">The awaiter that represents the delay.</param>
        public DelayTask(IDelay awaiter)
        {
            _awaiter = awaiter;
        }

        /// <summary>
        /// Gets the awaiter used by this handle.
        /// </summary>
        /// <returns>The underlying delay awaiter.</returns>
        public IDelay GetAwaiter() => _awaiter;

        /// <summary>
        /// Schedules a delay that can be canceled by the specified token.
        /// </summary>
        /// <param name="seconds">The duration of the delay in seconds.</param>
        /// <param name="token">The token that can cancel the delay.</param>
        /// <returns>An awaitable handle for the scheduled delay.</returns>
        public static DelayTask Delay(float seconds, CancellationToken token)
        {
            var awaiter = new Delay(token);
            DelayScheduler.Schedule(seconds, awaiter);
            return new DelayTask(awaiter);
        }

        /// <summary>
        /// Schedules a delay for the specified duration.
        /// </summary>
        /// <param name="seconds">The duration of the delay in seconds.</param>
        /// <returns>An awaitable handle for the scheduled delay.</returns>
        public static DelayTask Delay(float seconds)
        {
            var awaiter = new Delay();
            DelayScheduler.Schedule(seconds, awaiter);
            return new DelayTask(awaiter);
        }

        /// <summary>
        /// Schedules a delay that completes on the next fixed update and can be canceled.
        /// </summary>
        /// <param name="token">The token that can cancel the delay.</param>
        /// <returns>An awaitable handle for the scheduled delay.</returns>
        public static DelayTask WaitForFixedUpdate(CancellationToken token)
        {
            var awaiter = new Delay(token);
            DelayScheduler.ScheduleFixedUpate(awaiter);
            return new DelayTask(awaiter);
        }

        /// <summary>
        /// Schedules a delay that completes on the next fixed update.
        /// </summary>
        /// <returns>An awaitable handle for the scheduled delay.</returns>
        public static DelayTask WaitForFixedUpdate()
        {
            var awaiter = new Delay();
            DelayScheduler.ScheduleFixedUpate(awaiter);
            return new DelayTask(awaiter);
        }

        /// <summary>
        /// Schedules a delay for the specified number of fixed updates with cancellation support.
        /// </summary>
        /// <param name="steps">The number of fixed updates to wait for.</param>
        /// <param name="token">The token that can cancel the delay.</param>
        /// <returns>An awaitable handle for the scheduled delay.</returns>
        public static DelayTask WaitForFixedUpdate(int steps, CancellationToken token)
        {
            var awaiter = new Delay(token);
            for (int i = 0; i < steps; i++)
                DelayScheduler.ScheduleFixedUpate(awaiter);
            return new DelayTask(awaiter);
        }

        /// <summary>
        /// Schedules a delay for the specified number of fixed updates.
        /// </summary>
        /// <param name="steps">The number of fixed updates to wait for.</param>
        /// <returns>An awaitable handle for the scheduled delay.</returns>
        public static DelayTask WaitForFixedUpdate(int steps)
        {
            var awaiter = new Delay();
            for (int i = 0; i < steps; i++)
                DelayScheduler.ScheduleFixedUpate(awaiter);
            return new DelayTask(awaiter);
        }
        /// <summary>
        /// Schedules a delay whose countdown is adjusted by a dynamic speed multiplier.
        /// </summary>
        /// <param name="seconds">The initial duration in seconds.</param>
        /// <param name="speed">A function that supplies the multiplier applied to elapsed time.</param>
        /// <param name="token">An optional token that can cancel the delay.</param>
        /// <returns>An awaitable handle for the scheduled delay.</returns>
        public static DelayTask DelayScaled(float seconds, Func<float> speed, CancellationToken token = null)
        {
            var awaiter = new DelayScaled(seconds, speed, token);
            DelayScheduler.ScheduleScaled(awaiter);
            return new DelayTask(awaiter);
        }
    }
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
    /// <summary>
    /// Signals cancellation for supported scheduled operations.
    /// </summary>
    public class CancellationToken
    {
        /// <summary>
        /// Gets whether cancellation has been requested.
        /// </summary>
        public bool IsCanceled { get; private set; }

        /// <summary>
        /// Requests cancellation.
        /// </summary>
        public void Cancel() => IsCanceled = true;

        /// <summary>
        /// Creates a token that is canceled on the next fixed update.
        /// </summary>
        /// <returns>The scheduled cancellation token.</returns>
        public static CancellationToken CancelAfterFixedUpate()
        {
            var token = new CancellationToken();
            DelayScheduler.ScheduleFixedUpate(new Delay(), () => token.Cancel());
            return token;
        }
        /// <summary>
        /// Creates a token that is canceled after the specified number of fixed updates.
        /// </summary>
        /// <param name="steps">The number of fixed updates before cancellation.</param>
        /// <returns>The scheduled cancellation token.</returns>
        public static CancellationToken CancelAfterFixedUpate(int steps)
        {
            var token = new CancellationToken();
            _ = WaitForFixedUpdates(steps,token);
            return token;
        }
        private async static Task WaitForFixedUpdates(int steps,CancellationToken token)
        {
            await DelayTask.WaitForFixedUpdate(steps);
            token.Cancel();
        }
        /// <summary>
        /// Creates a token that is canceled after the specified duration.
        /// </summary>
        /// <param name="seconds">The duration in seconds before cancellation.</param>
        /// <returns>The scheduled cancellation token.</returns>
        public static CancellationToken CancelAfterSeconds(float seconds)
        {
            var token = new CancellationToken();
            DelayScheduler.Schedule(seconds, new Delay(), () => token.Cancel());
            return token;
        }
        /// <summary>
        /// Creates a token that is canceled when the specified predicate becomes true.
        /// </summary>
        /// <param name="predicate">The condition that triggers cancellation.</param>
        /// <returns>The scheduled cancellation token.</returns>
        public static CancellationToken CancelWhen(Func<bool> predicate)
        {
            var token = new CancellationToken();
            WaitUntilScheduler.Schedule(new WaitUntil(predicate), () => token.Cancel());
            return token;
        }
    }
    /// <summary>
    /// Provides extension methods for creating cancellation tokens tied to Unity object lifetimes.
    /// </summary>
    public static class CancellationTokenExt
    {
        /// <summary>
        /// Creates a cancellation token that is canceled when the component is destroyed.
        /// </summary>
        /// <param name="self">The component whose destruction triggers cancellation.</param>
        /// <returns>A token canceled when the component is destroyed.</returns>
        public static CancellationToken CreateCancellationToken(this MonoBehaviour self)
        {
            var token = new CancellationToken();
            WaitUntilScheduler.Schedule(new WaitUntil(() => self.destroyCancellationToken.IsCancellationRequested), () => token.Cancel());
            return token;
        }
        /// <summary>
        /// Creates a cancellation token associated with the specified game object's component lifetime.
        /// </summary>
        /// <param name="self">The game object for which to create the token.</param>
        /// <returns>A token canceled when its associated component is destroyed.</returns>
        public static CancellationToken CreateCancellationToken(this GameObject self) =>
            self.TryGetComponent<MonoBehaviour>(out var mono)
                ? CreateCancellationToken(mono)
                : self.GetOrAddComponent<MonobehaviourCancellationToken>().Token;
        /// <summary>
        /// Creates a cancellation token associated with the specified component's lifetime.
        /// </summary>
        /// <param name="self">The component for which to create the token.</param>
        /// <returns>A token canceled when its associated component is destroyed.</returns>
        public static CancellationToken CreateCancellationToken(this Component self) =>
            self.TryGetComponent<MonoBehaviour>(out var mono)
                ? CreateCancellationToken(mono)
                : self.GetOrAddComponent<MonobehaviourCancellationToken>().Token;
        /// <summary>
        /// Provides a cancellation token that is canceled when this component is destroyed.
        /// </summary>
        public class MonobehaviourCancellationToken : MonoBehaviour
        {
            /// <summary>
            /// Gets the token associated with this component's lifetime.
            /// </summary>
            public CancellationToken Token { get; private set; }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            public void Awake()
            {
                Token = new CancellationToken();
            }

            public void OnDestroy()
            {
                Token.Cancel();
            }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        }
    }
    /// <summary>
    /// Defines an awaiter for a delay or other scheduled operation.
    /// </summary>
    public interface IDelay : INotifyCompletion
    {
        /// <summary>
        /// Gets whether the operation has completed.
        /// </summary>
        public bool IsCompleted { get; }

        /// <summary>
        /// Marks the operation as complete.
        /// </summary>
        public void Complete();

        /// <summary>
        /// Gets the result of the completed operation.
        /// </summary>
        public void GetResult();
    }
}
