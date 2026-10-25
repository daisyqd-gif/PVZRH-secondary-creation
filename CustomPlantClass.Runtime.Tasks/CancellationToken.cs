namespace CustomPlantClass.Runtime.Tasks
{
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
        public static CancellationToken CreateCancellationToken(this MonoBehaviour self) => 
            self.GetOrAddComponent<MonobehaviourCancellationToken>().Token;
        /// <summary>
        /// Creates a cancellation token associated with the specified game object's component lifetime.
        /// </summary>
        /// <param name="self">The game object for which to create the token.</param>
        /// <returns>A token canceled when its associated component is destroyed.</returns>
        public static CancellationToken CreateCancellationToken(this GameObject self) => 
            self.GetOrAddComponent<MonobehaviourCancellationToken>().Token;
        /// <summary>
        /// Creates a cancellation token associated with the specified component's lifetime.
        /// </summary>
        /// <param name="self">The component for which to create the token.</param>
        /// <returns>A token canceled when its associated component is destroyed.</returns>
        public static CancellationToken CreateCancellationToken(this Component self) => 
            self.GetOrAddComponent<MonobehaviourCancellationToken>().Token;
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
}