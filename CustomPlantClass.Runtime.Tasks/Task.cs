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
