using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CustomPlantClass.Runtime.Tasks
{
    public class Delay : IDelay
    {
        private bool _isCompleted;
        private Action _continuation;
        private readonly CancellationToken _token;

        // Mode 1: with cancellation
        public Delay(CancellationToken token)
        {
            _token = token;
        }

        // Mode 2: without cancellation
        public Delay()
        {
            _token = null;
        }

        public bool IsCompleted => _isCompleted || (_token?.IsCanceled ?? false);

        public void OnCompleted(Action continuation)
        {
            _continuation = continuation;
        }

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

        public void GetResult() { }
    }
    public class DelayScaled : IDelay
    {
        private bool _isCompleted;
        private Action _continuation;
        private readonly CancellationToken _token;
        public float remaining;
        public Func<float> speedMultiplier; // dynamic multiplier

        public DelayScaled(float seconds, Func<float> speed, CancellationToken token = null)
        {
            remaining = seconds;
            speedMultiplier = speed;
            _token = token;
        }

        public bool IsCompleted => _isCompleted || (_token?.IsCanceled ?? false);

        public void OnCompleted(Action continuation) => _continuation = continuation;

        public void Complete()
        {
            if (_isCompleted) return;
            _isCompleted = true;
            _continuation?.Invoke();
        }

        public void GetResult() { }
    }
    public class WaitUntil : IDelay
    {
        private bool _isCompleted;
        private Action _continuation;
        private readonly CancellationToken _token;
        private readonly Func<bool> _predicate;

        // with cancellation
        public WaitUntil(Func<bool> predicate, CancellationToken token)
        {
            _predicate = predicate;
            _token = token;
        }

        // without cancellation
        public WaitUntil(Func<bool> predicate)
        {
            _predicate = predicate;
            _token = null;
        }

        public bool IsCompleted => _isCompleted || (_token?.IsCanceled ?? false);

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

        public void Complete()
        {
            if (_isCompleted) return;

            _isCompleted = true;
            _continuation?.Invoke();
        }

        public void GetResult() { }
    }
    public readonly struct WaitUntilTask
    {
        private readonly WaitUntil _awaiter;

        public WaitUntilTask(WaitUntil awaiter)
        {
            _awaiter = awaiter;
        }

        public WaitUntil GetAwaiter() => _awaiter;

        // with cancellation
        public static WaitUntilTask WaitUntil(Func<bool> predicate, CancellationToken token)
        {
            var awaiter = new WaitUntil(predicate, token);
            WaitUntilScheduler.Schedule(awaiter);
            return new WaitUntilTask(awaiter);
        }

        // without cancellation
        public static WaitUntilTask WaitUntil(Func<bool> predicate)
        {
            var awaiter = new WaitUntil(predicate);
            WaitUntilScheduler.Schedule(awaiter);
            return new WaitUntilTask(awaiter);
        }
    }
    public struct DelayTask
    {
        private readonly IDelay _awaiter;

        public DelayTask(IDelay awaiter)
        {
            _awaiter = awaiter;
        }

        public IDelay GetAwaiter() => _awaiter;

        // Mode 1: with cancellation
        public static DelayTask Delay(float seconds, CancellationToken token)
        {
            var awaiter = new Delay(token);
            DelayScheduler.Schedule(seconds, awaiter);
            return new DelayTask(awaiter);
        }

        // Mode 2: without cancellation
        public static DelayTask Delay(float seconds)
        {
            var awaiter = new Delay();
            DelayScheduler.Schedule(seconds, awaiter);
            return new DelayTask(awaiter);
        }

        // FixedUpdate (with cancellation)
        public static DelayTask WaitForFixedUpdate(CancellationToken token)
        {
            var awaiter = new Delay(token);
            DelayScheduler.ScheduleFixedUpate(awaiter);
            return new DelayTask(awaiter);
        }

        // FixedUpdate (without cancellation)
        public static DelayTask WaitForFixedUpdate()
        {
            var awaiter = new Delay();
            DelayScheduler.ScheduleFixedUpate(awaiter);
            return new DelayTask(awaiter);
        }

        // FixedUpdate steps (with cancellation)
        public static DelayTask WaitForFixedUpdate(int steps, CancellationToken token)
        {
            var awaiter = new Delay(token);
            for (int i = 0; i < steps; i++)
                DelayScheduler.ScheduleFixedUpate(awaiter);
            return new DelayTask(awaiter);
        }

        // FixedUpdate steps (without cancellation)
        public static DelayTask WaitForFixedUpdate(int steps)
        {
            var awaiter = new Delay();
            for (int i = 0; i < steps; i++)
                DelayScheduler.ScheduleFixedUpate(awaiter);
            return new DelayTask(awaiter);
        }
        public static DelayTask DelayScaled(float seconds, Func<float> speed, CancellationToken token = null)
        {
            var awaiter = new DelayScaled(seconds, speed, token);
            DelayScheduler.ScheduleScaled(awaiter);
            return new DelayTask(awaiter);
        }
    }
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

        public void Update()
        {
            float dt = Time.deltaTime;

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
    }
    public class WaitUntilScheduler : MonoBehaviour
    {
        private class Entry
        {
            public WaitUntil awaiter;
            public Action action; // null if no action
        }

        private static readonly List<Entry> entries = new();

        public static void Schedule(WaitUntil awaiter)
        {
            entries.Add(new Entry { awaiter = awaiter, action = null });
        }

        public static void Schedule(WaitUntil awaiter, Action action)
        {
            entries.Add(new Entry { awaiter = awaiter, action = action });
        }

        public void Update()
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
    }
    public class CancellationToken
    {
        public bool IsCanceled { get; private set; }

        public void Cancel() => IsCanceled = true;

        public static CancellationToken CancelAfterFixedUpate()
        {
            var token = new CancellationToken();
            DelayScheduler.ScheduleFixedUpate(new Delay(), () => token.Cancel());
            return token;
        }
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
        public static CancellationToken CancelAfterSeconds(float seconds)
        {
            var token = new CancellationToken();
            DelayScheduler.Schedule(seconds, new Delay(), () => token.Cancel());
            return token;
        }
        public static CancellationToken CancelWhen(Func<bool> predicate)
        {
            var token = new CancellationToken();
            WaitUntilScheduler.Schedule(new WaitUntil(predicate), () => token.Cancel());
            return token;
        }
    }
    public interface IDelay : INotifyCompletion
    {
        public bool IsCompleted { get; }
        public void Complete();
        public void GetResult();
    }
}
