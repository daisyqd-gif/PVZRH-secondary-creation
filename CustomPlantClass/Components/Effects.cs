namespace CustomPlantClass
{
    /// <summary>
    /// Provides extension methods for adding, retrieving, and removing
    /// custom effects on supported Unity objects.
    /// </summary>
    public static class EffectsMgr
    {
        private static void Abort(CustomEffect effect)
        {
            if (effect != null && !effect.IsDestroyed())
            {
                Object.DestroyImmediate(effect);
            }
        }
        /// <summary>
        /// Adds or retrieves a custom effect on a component, validating that
        /// the component is attached to an object compatible with the effect.
        /// </summary>
        /// <typeparam name="T">The custom effect type to add or retrieve.</typeparam>
        /// <param name="self">The component on which to add the effect.</param>
        /// <param name="cycleTimer">An amount to add to the effect's cycle timer when positive.</param>
        /// <param name="removeTimer">An amount to add to the effect's removal timer when positive.</param>
        /// <returns>The effect component, or <see langword="null"/> if the target is invalid or incompatible.</returns>
        public static CustomEffect AddEffect<T>(this MonoBehaviour self, float cycleTimer = -1f, float removeTimer = -1f) where T : CustomEffect
        {
            if (self == null || self.IsDestroyed()) return null;
            if (PlantMgr.IsNotNullMonoBehaviour(self.GetOrAddComponent<T>(), out var comp))
            {
                switch (comp.Usage)
                {
                    case CustomEffectUsage.Plant:
                        {
                            if (!self.TryGetComponent<Plant>(out var _))
                            {
                                Abort(comp);
                                return null;
                            }
                            break;
                        }
                    case CustomEffectUsage.Zombie:
                        {
                            if (!self.TryGetComponent<Zombie>(out var _))
                            {
                                Abort(comp);
                                return null;
                            }
                            break;
                        }
                    case CustomEffectUsage.Bullet:
                        {
                            if (!self.TryGetComponent<Bullet>(out var _))
                            {
                                Abort(comp);
                                return null;
                            }
                            break;
                        }
                    case CustomEffectUsage.GridItem:
                        {
                            if (!self.TryGetComponent<GridItem>(out var _))
                            {
                                Abort(comp);
                                return null;
                            }
                            break;
                        }
                }
                if (cycleTimer > 0f)
                {
                    comp.CycleCountDown += cycleTimer;
                }
                if (removeTimer > 0f)
                {
                    comp.RemoveCountDown += removeTimer;
                }
                return comp;
            }
            return null;
        }
        /// <summary>
        /// Attempts to retrieve a custom effect component from a
        /// <see cref="MonoBehaviour"/>.
        /// </summary>
        /// <typeparam name="T">The custom effect type to retrieve.</typeparam>
        /// <param name="self">The component to search.</param>
        /// <param name="effect">Receives the effect if one is found.</param>
        /// <returns><see langword="true"/> if the effect component was found.</returns>
        public static bool TryGetCustomEffect<T>(this MonoBehaviour self, out T effect) where T : CustomEffect => self.TryGetComponent(out effect);
        /// <summary>
        /// Invokes the effect's removal callback and removes it from
        /// the specified <see cref="MonoBehaviour"/>.
        /// </summary>
        /// <typeparam name="T">The custom effect type to remove.</typeparam>
        /// <param name="self">The component from which to remove the effect.</param>
        public static void RemoveEffect<T>(this MonoBehaviour self) where T : CustomEffect
        {
            if (!self.TryGetComponent<T>(out var component)) return;
            component.OnRemoveEffect();
            Object.Destroy(component);
        }
        /// <summary>
        /// Adds or retrieves a custom effect on a transform, validating that
        /// the object is compatible with the effect.
        /// </summary>
        /// <typeparam name="T">The custom effect type to add or retrieve.</typeparam>
        /// <param name="self">The transform on which to add the effect.</param>
        /// <param name="cycleTimer">An amount to add to the effect's cycle timer when positive.</param>
        /// <param name="removeTimer">An amount to add to the effect's removal timer when positive.</param>
        /// <returns>The effect component, or <see langword="null"/> if the target is invalid or incompatible.</returns>
        public static CustomEffect AddEffect<T>(this Transform self, float cycleTimer = -1f, float removeTimer = -1f) where T : CustomEffect
        {
            if (self == null || self.IsDestroyed()) return null;
            if (PlantMgr.IsNotNullMonoBehaviour(self.GetOrAddComponent<T>(), out var comp))
            {
                switch (comp.Usage)
                {
                    case CustomEffectUsage.Plant:
                        {
                            if (!self.TryGetComponent<Plant>(out var _))
                            {
                                Abort(comp);
                                return null;
                            }
                            break;
                        }
                    case CustomEffectUsage.Zombie:
                        {
                            if (!self.TryGetComponent<Zombie>(out var _))
                            {
                                Abort(comp);
                                return null;
                            }
                            break;
                        }
                    case CustomEffectUsage.Bullet:
                        {
                            if (!self.TryGetComponent<Bullet>(out var _))
                            {
                                Abort(comp);
                                return null;
                            }
                            break;
                        }
                    case CustomEffectUsage.GridItem:
                        {
                            if (!self.TryGetComponent<GridItem>(out var _))
                            {
                                Abort(comp);
                                return null;
                            }
                            break;
                        }
                }
                if (cycleTimer > 0f)
                {
                    comp.CycleCountDown += cycleTimer;
                }
                if (removeTimer > 0f)
                {
                    comp.RemoveCountDown += removeTimer;
                }
                return comp;
            }
            return null;
        }
        /// <summary>
        /// Attempts to retrieve a custom effect component from
        /// a <see cref="Transform"/>.
        /// </summary>
        /// <typeparam name="T">The custom effect type to retrieve.</typeparam>
        /// <param name="self">The transform to search.</param>
        /// <param name="effect">Receives the effect if one is found.</param>
        /// <returns><see langword="true"/> if the effect component was found.</returns>
        public static bool TryGetCustomEffect<T>(this Transform self, out T effect) where T : CustomEffect => self.TryGetComponent(out effect);
        /// <summary>
        /// Invokes the effect's removal callback and removes it from
        /// the game object associated with the transform.
        /// </summary>
        /// <typeparam name="T">The custom effect type to remove.</typeparam>
        /// <param name="self">The transform whose game object holds the effect.</param>
        public static void RemoveEffect<T>(this Transform self) where T : CustomEffect
        {
            if (!self.TryGetComponent<T>(out var component)) return;
            component.OnRemoveEffect();
            Object.Destroy(component);
        }
        /// <summary>
        /// Adds or retrieves a custom effect on a game object, validating
        /// that the object is compatible with the effect.
        /// </summary>
        /// <typeparam name="T">The custom effect type to add or retrieve.</typeparam>
        /// <param name="self">The game object on which to add the effect.</param>
        /// <param name="cycleTimer">An amount to add to the effect's cycle timer when positive.</param>
        /// <param name="removeTimer">An amount to add to the effect's removal timer when positive.</param>
        /// <returns>The effect component, or <see langword="null"/> if the target is invalid or incompatible.</returns>
        public static CustomEffect AddEffect<T>(this GameObject self, float cycleTimer = -1f, float removeTimer = -1f) where T : CustomEffect
        {
            if (self == null || self.IsDestroyed()) return null;
            if (PlantMgr.IsNotNullMonoBehaviour(self.GetOrAddComponent<T>(), out var comp))
            {
                switch (comp.Usage)
                {
                    case CustomEffectUsage.Plant:
                        {
                            if (!self.TryGetComponent<Plant>(out var _))
                            {
                                Abort(comp);
                                return null;
                            }
                            break;
                        }
                    case CustomEffectUsage.Zombie:
                        {
                            if (!self.TryGetComponent<Zombie>(out var _))
                            {
                                Abort(comp);
                                return null;
                            }
                            break;
                        }
                    case CustomEffectUsage.Bullet:
                        {
                            if (!self.TryGetComponent<Bullet>(out var _))
                            {
                                Abort(comp);
                                return null;
                            }
                            break;
                        }
                    case CustomEffectUsage.GridItem:
                        {
                            if (!self.TryGetComponent<GridItem>(out var _))
                            {
                                Abort(comp);
                                return null;
                            }
                            break;
                        }
                }
                if (cycleTimer > 0f)
                {
                    comp.CycleCountDown += cycleTimer;
                }
                if (removeTimer > 0f)
                {
                    comp.RemoveCountDown += removeTimer;
                }
                return comp;
            }
            return null;
        }
        /// <summary>
        /// Attempts to retrieve a custom effect component from
        /// a <see cref="GameObject"/>.
        /// </summary>
        /// <typeparam name="T">The custom effect type to retrieve.</typeparam>
        /// <param name="self">The game object to search.</param>
        /// <param name="effect">Receives the effect if one is found.</param>
        /// <returns><see langword="true"/> if the effect component was found.</returns>
        public static bool TryGetCustomEffect<T>(this GameObject self, out T effect) where T : CustomEffect => self.TryGetComponent(out effect);
        /// <summary>
        /// Invokes the effect's removal callback and removes it from
        /// the specified game object.
        /// </summary>
        /// <typeparam name="T">The custom effect type to remove.</typeparam>
        /// <param name="self">The game object from which to remove the effect.</param>
        public static void RemoveEffect<T>(this GameObject self) where T : CustomEffect
        {
            if (!self.TryGetComponent<T>(out var component)) return;
            component.OnRemoveEffect();
            Object.Destroy(component);
        }
    }
    /// <summary>
    /// Base component for custom effects, with optional cycle and timed
    /// removal countdowns and lifecycle callbacks.
    /// </summary>
    public class CustomEffect : MonoBehaviour
    {
        /// <summary>
        /// Gets whether the effect's cycle countdown should be processed
        /// during fixed updates.
        /// </summary>
        public virtual bool CanCountDown { get; } = false;
        /// <summary>
        /// Gets whether the effect should be removed when its removal
        /// countdown reaches zero.
        /// </summary>
        public virtual bool TimedEffect { get; } = false;
        /// <summary>
        /// Gets or sets the interval used to reset the cycle countdown
        /// after its timer callback runs.
        /// </summary>
        public virtual float CycleCountDown { get; set; } = 0f;
        /// <summary>
        /// Gets or sets the countdown interval before this effect
        /// is removed when timed removal is enabled.
        /// </summary>
        public virtual float RemoveCountDown { get; set; } = 0f;
        /// <summary>
        /// Gets the kind of object this effect is designed to be used on.
        /// </summary>
        public virtual CustomEffectUsage Usage { get => CustomEffectUsage.Other; }
        private float AttrCountDown = 0f;
        private float RemoveCd = 0f;
        /// <summary>
        /// Initializes the effect's countdowns and invokes its add callback.
        /// </summary>
        public void Start()
        {
            AttrCountDown = CycleCountDown;
            RemoveCd = RemoveCountDown;
            OnAddEffect();
        }
        /// <summary>
        /// Updates the effect's cycle and removal countdowns
        /// and invokes the corresponding callbacks when they expire.
        /// </summary>
        public void FixedUpdate()
        {
            if (CanCountDown)
            {
                AttrCountDown -= Time.deltaTime;
                if (AttrCountDown <= 0)
                {
                    OnTimerZero();
                    AttrCountDown = CycleCountDown;
                }
            }
            if (TimedEffect)
            {
                RemoveCd -= Time.deltaTime;
                if (RemoveCd <= 0)
                {
                    OnRemoveEffect();
                    Destroy(this);
                }
            }
        }
        /// <summary>
        /// Called when the effect is added and initialized.
        /// Override this method to perform effect setup.
        /// </summary>
        public virtual void OnAddEffect() { }
        /// <summary>
        /// Called whenever an enabled cycle countdown reaches zero.
        /// Override this method to handle the cycle event.
        /// </summary>
        public virtual void OnTimerZero() { }
        /// <summary>
        /// Called when the effect is removed, including timed removal.
        /// Override this method to perform effect cleanup.
        /// </summary>
        public virtual void OnRemoveEffect() { }
    }
    /// <summary>
    /// Identifies the category of game object compatible with
    /// a custom effect.
    /// </summary>
    public enum CustomEffectUsage
    {
#pragma warning disable CS1591 // Enum members are intentionally excluded from XML documentation
        Plant = 0,
        Zombie = 1,
        Bullet = 2,
        GridItem = 3,
        Other = 4
#pragma warning restore CS1591
    }
}
