namespace CustomPlantClass.Main
{
    /// <summary>
    /// Helpers for math in mods without doing extreme math work
    /// </summary>
    public static class MathHelper
    {
        // ============================================================
        //  ROTATION / QUATERNION HELPERS
        // ============================================================
        /// <summary>
        /// Converts a 2D direction vector into a Z‑axis rotation.
        /// This is a direct replacement for Core.Lawnf.GetRotateFromSpeed().
        ///
        /// Formula:
        ///     angle = atan2(direction.y, direction.x)
        ///     rotation = Quaternion.Euler(0, 0, angle_in_degrees)
        /// </summary>
        /// <param name="direction">A 2D direction vector.</param>
        /// <returns>A Quaternion facing the direction.</returns>
        public static Quaternion DirectionToRotation(Vector2 direction)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            return Quaternion.Euler(0f, 0f, angle);
        }

        /// <summary>
        /// Computes the facing angle (in degrees) of a 2D direction vector.
        /// This is the scalar form of Core.Lawnf.GetRotateFromSpeed().
        ///
        /// Formula:
        ///     angle = atan2(direction.y, direction.x)
        /// </summary>
        /// <param name="direction">A 2D direction vector.</param>
        /// <returns>The facing angle in degrees.</returns>
        public static float DirectionToDegrees(Vector2 direction)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            return angle;
        }

        /// <summary>
        /// Converts a Z‑axis rotation into a normalized 2D direction vector.
        /// This is a direct replacement for Core.Lawnf.GetVectorFromQuaternion().
        ///
        /// Formula:
        ///     angle = rotation.eulerAngles.z * Deg2Rad
        ///     direction = (cos(angle), sin(angle)).normalized
        /// </summary>
        /// <param name="rotation">A Quaternion whose Z‑axis angle determines the direction.</param>
        /// <returns>A normalized Vector2 pointing in the facing direction.</returns>
        public static Vector2 RotationToDirection(Quaternion rotation)
        {
            float angle = rotation.eulerAngles.z * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
        }

        /// <summary>
        /// Converts a degree angle into a normalized 2D direction vector.
        /// This is the scalar form of Core.Lawnf.GetVectorFromQuaternion().
        ///
        /// Formula:
        ///     angle = rotationInDegrees * Deg2Rad
        ///     direction = (cos(angle), sin(angle)).normalized
        /// </summary>
        /// <param name="rotationInDegrees">A Z‑axis angle in degrees.</param>
        /// <returns>A normalized Vector2 pointing in the facing direction.</returns>
        public static Vector2 RotationToDirection(float rotationInDegrees)
        {
            float angle = rotationInDegrees * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
        }
        
        /// <summary>Creates a 2D rotation that faces from one position toward another.</summary>
        /// <param name="from">The starting position.</param>
        /// <param name="to">The position to face.</param>
        /// <returns>A quaternion representing the direction from <paramref name="from"/> to <paramref name="to"/>.</returns>
        public static Quaternion LookAt2D(Vector2 from, Vector2 to)
        {
            Vector2 dir = (to - from).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            return Quaternion.Euler(0, 0, angle);
        }

        /// <summary>Rotates a 2D orientation toward a target angle by at most a time-scaled amount.</summary>
        /// <param name="current">The current orientation.</param>
        /// <param name="targetAngle">The target Z-axis angle in degrees.</param>
        /// <param name="maxDegreesPerSecond">The maximum rotation speed in degrees per second.</param>
        /// <returns>The orientation after the bounded rotation step.</returns>
        public static Quaternion RotateTowards2D(
            Quaternion current,
            float targetAngle,
            float maxDegreesPerSecond)
        {
            Quaternion target = Quaternion.Euler(0f, 0f, targetAngle);
            return Quaternion.RotateTowards(
                current,
                target,
                maxDegreesPerSecond * Time.deltaTime
            );
        }

        /// <summary>Creates a random 2D rotation from zero up to 360 degrees.</summary>
        /// <returns>A quaternion with a random Z-axis rotation.</returns>
        public static Quaternion RandomRotation2D()
            => Quaternion.Euler(0, 0, Random.Range(0f, 360f));

        // ============================================================
        //  VECTOR HELPERS
        // ============================================================
        /// <summary>Calculates the squared distance between two points.</summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>The distance between the points, squared.</returns>
        public static float DistanceSq(Vector2 a, Vector2 b)
        {
            float dx = a.x - b.x;
            float dy = a.y - b.y;
            return dx * dx + dy * dy;
        }

        /// <summary>Rotates a vector by the specified angle.</summary>
        /// <param name="v">The vector to rotate.</param>
        /// <param name="degrees">The rotation angle in degrees.</param>
        /// <returns>The rotated vector.</returns>
        public static Vector2 RotateVector(Vector2 v, float degrees)
        {
            float rad = degrees * Mathf.Deg2Rad;
            float cs = Mathf.Cos(rad);
            float sn = Mathf.Sin(rad);
            return new Vector2(v.x * cs - v.y * sn, v.x * sn + v.y * cs);
        }

        /// <summary>Limits a vector's magnitude to the specified maximum.</summary>
        /// <param name="v">The vector to clamp.</param>
        /// <param name="max">The maximum allowed magnitude.</param>
        /// <returns>The original vector if within the limit, otherwise a vector with the maximum magnitude.</returns>
        public static Vector2 ClampMagnitude(Vector2 v, float max)
        {
            float mag = v.magnitude;
            return mag > max ? v * (max / mag) : v;
        }

        // ============================================================
        //  RANDOM HELPERS
        // ============================================================
        /// <summary>Selects a random item from a list, optionally filtering items first.</summary>
        /// <typeparam name="T">The type of list items.</typeparam>
        /// <param name="values">The list to select from.</param>
        /// <param name="selector">The filter to apply, or <see langword="null"/> to accept every item.</param>
        /// <returns>A randomly selected matching item, or the default value of <typeparamref name="T"/> if none match.</returns>
        public static T GetRandomValue<T>(List<T> values, Func<T, bool> selector = null)
        {
            selector ??= (_ => true);

            int count = 0;
            for (int i = 0; i < values.Count; i++)
                if (selector(values[i]))
                    count++;

            if (count == 0)
                return default;

            int target = Random.Range(0, count);

            for (int i = 0; i < values.Count; i++)
                if (selector(values[i]) && target-- == 0)
                    return values[i];

            return default;
        }

        /// <summary>Generates a random value between bounds using a distribution weighted toward the target mean.</summary>
        /// <param name="min">The lower bound.</param>
        /// <param name="max">The upper bound.</param>
        /// <param name="targetMean">The target mean, which must be strictly between the bounds.</param>
        /// <returns>A generated value, or <paramref name="min"/> if the target mean is outside the open interval.</returns>
        public static float GetRandomWithMean(float min, float max, float targetMean)
        {
            if (!(min < targetMean && targetMean < max))
                return min;

            float p = Random.value;
            float leftWeight = (max - targetMean) / (max - min);

            if (p < leftWeight)
            {
                if (min <= 0f)
                    return Random.Range(min, targetMean);

                float logMin = Mathf.Log(min);
                float logMean = Mathf.Log(targetMean);
                float r = Random.Range(logMin, logMean);
                return Mathf.Exp(r);
            }
            else
            {
                float logMean = Mathf.Log(targetMean);
                float logMax = Mathf.Log(max);
                float r = Random.Range(logMean, logMax);
                return Mathf.Exp(r);
            }
        }

        /// <summary>Generates a random value by sampling logarithmically between two ratios.</summary>
        /// <param name="minRatio">The lower ratio bound; negative values are replaced with a small positive value.</param>
        /// <param name="maxRatio">The upper ratio bound; negative values are replaced with a small positive value.</param>
        /// <returns>The logarithmically sampled value, shifted down by one.</returns>
        public static float GetRandomLogSymmetric(float minRatio, float maxRatio)
        {
            if (minRatio < 0f) minRatio = 0.01f;
            if (maxRatio < 0f) maxRatio = 0.01f;

            float logMin = Mathf.Log(minRatio);
            float logMax = Mathf.Log(maxRatio);

            float r = Random.Range(logMin, logMax);
            return Mathf.Exp(r) - 1f;
        }

        // ============================================================
        //  SCALAR HELPERS
        // ============================================================
        /// <summary>Maps a value linearly from one range to another without clamping.</summary>
        /// <param name="v">The value to remap.</param>
        /// <param name="a">The input range start.</param>
        /// <param name="b">The input range end.</param>
        /// <param name="c">The output range start.</param>
        /// <param name="d">The output range end.</param>
        /// <returns>The linearly remapped value.</returns>
        public static float Remap(float v, float a, float b, float c, float d)
            => c + (v - a) * (d - c) / (b - a);

        /// <summary>Maps a value from one range to another, clamping the input to its range.</summary>
        /// <param name="v">The value to remap.</param>
        /// <param name="a">The input range start.</param>
        /// <param name="b">The input range end.</param>
        /// <param name="c">The output range start.</param>
        /// <param name="d">The output range end.</param>
        /// <returns>The remapped value clamped to the output range.</returns>
        public static float RemapClamped(float v, float a, float b, float c, float d)
        {
            float t = Mathf.InverseLerp(a, b, v);
            return Mathf.Lerp(c, d, t);
        }

        /// <summary>Determines whether a value's absolute magnitude is less than a tolerance.</summary>
        /// <param name="v">The value to test.</param>
        /// <param name="eps">The comparison tolerance.</param>
        /// <returns><see langword="true"/> if the absolute value is below the tolerance.</returns>
        public static bool ApproximatelyZero(float v, float eps = 0.0001f)
            => Mathf.Abs(v) < eps;

        // ============================================================
        //  BALLISTIC HELPERS
        // ============================================================
        /// <summary>Calculates the initial velocity needed to intercept a moving target under gravity.</summary>
        /// <param name="projectilePos">The projectile's starting position.</param>
        /// <param name="targetVelocity">The target's velocity.</param>
        /// <param name="targetPos">The target's current position.</param>
        /// <param name="flightTime">The time until the projectile reaches the target.</param>
        /// <param name="gravity">The vertical acceleration applied to the projectile.</param>
        /// <returns>The projectile's initial velocity vector.</returns>
        public static Vector2 CalculateProjectileWithGravity(
            Vector2 projectilePos,
            Vector2 targetVelocity,
            Vector2 targetPos,
            float flightTime,
            float gravity)
        {
            float vx = (targetVelocity.x * flightTime + targetPos.x - projectilePos.x) / flightTime;

            float vy = (
                targetVelocity.y * flightTime + targetPos.y
                - projectilePos.y
                - gravity * 0.5f * flightTime * flightTime
            ) / flightTime;

            return new Vector2(vx, vy);
        }

        /// <summary>Calculates launch parameters for a projectile intercepting a moving target.</summary>
        /// <param name="projectilePos">The projectile's starting position.</param>
        /// <param name="targetVelocity">The target's velocity.</param>
        /// <param name="targetPos">The target's current position.</param>
        /// <param name="flightTime">The time until the projectile reaches the target.</param>
        /// <returns>An array containing launch angle in degrees, horizontal speed, vertical speed, and gravity.</returns>
        public static float[] CalculateProjectileWithSpeed(
            Vector2 projectilePos,
            Vector2 targetVelocity,
            Vector2 targetPos,
            float flightTime)
        {
            float dx = targetVelocity.x * flightTime + targetPos.x - projectilePos.x;
            float dy = targetVelocity.y * flightTime + targetPos.y - projectilePos.y;

            float g = Physics2D.gravity.y;

            dy = (dy - g * 1.5f * 0.5f * flightTime * flightTime) / flightTime;
            float vx = dx / flightTime;

            float angleDeg = Mathf.Atan2(dy, vx) * Mathf.Rad2Deg;

            return new float[]
            {
                angleDeg,
                vx,
                dy,
                g * 1.5f
            };
        }

        /// <summary>Estimates target velocity from two observations and calculates projectile launch parameters.</summary>
        /// <param name="startPos">The projectile's starting position.</param>
        /// <param name="t1">The time of the first target observation.</param>
        /// <param name="firstPlace">The target's position at the first observation.</param>
        /// <param name="t2">The time of the second target observation.</param>
        /// <param name="secondPlace">The target's position at the second observation.</param>
        /// <param name="flightTime">The time until the projectile reaches the target.</param>
        /// <returns>An array containing launch angle in degrees, horizontal speed, vertical speed, and gravity.</returns>
        public static float[] CalculateProjectileParameters(
            Vector2 startPos,
            float t1,
            Vector2 firstPlace,
            float t2,
            Vector2 secondPlace,
            float flightTime)
        {
            float dt = t2 - t1;
            float minDt = Time.deltaTime;
            if (dt < minDt)
                dt = minDt;

            Vector2 targetVelocity = (secondPlace - firstPlace) / dt;

            return CalculateProjectileWithSpeed(
                startPos,
                targetVelocity,
                secondPlace,
                flightTime
            );
        }

        // ============================================================
        //  COROUTINE HELPERS
        // ============================================================
        /// <summary>Gradually rotates a transform from its current orientation to a target orientation.</summary>
        /// <param name="t">The transform to rotate.</param>
        /// <param name="target">The target orientation.</param>
        /// <param name="smoothTime">The duration of the interpolation.</param>
        /// <param name="rotationSpeed">The multiplier applied to the fixed-step interpolation time.</param>
        /// <returns>An enumerator that performs the rotation over fixed updates.</returns>
        public static IEnumerator SmoothRotate(
            Transform t,
            Quaternion target,
            float smoothTime = 0.5f,
            float rotationSpeed = 0.6f)
        {
            if (t == null)
                yield break;

            Quaternion start = t.rotation;
            float elapsed = 0f;

            while (elapsed < smoothTime)
            {
                float tNorm = elapsed / smoothTime;
                t.rotation = Quaternion.Slerp(start, target, tNorm);

                elapsed += Time.fixedDeltaTime * rotationSpeed;
                yield return new WaitForFixedUpdate();
            }

            t.rotation = target;
        }
        /// <summary>Finds the first empty entry in a supported list or enumerable.</summary>
        /// <typeparam name="T">The type of collection entries.</typeparam>
        /// <param name="source">A managed list, IL2CPP list, or enumerable to inspect.</param>
        /// <param name="isEmpty">An optional predicate that identifies empty entries.</param>
        /// <returns>The index of the first empty entry, or the collection count if none is empty.</returns>
        /// <exception cref="NotSupportedException">The source is not a supported collection type.</exception>
        public static int NextEmptyIndex<T>(object source, Func<T, bool> isEmpty = null)
        {
            if (isEmpty == null)
            {
                isEmpty = (T entry) =>
                {
                    if (entry == null)
                        return true;

                    if (entry.Equals(null)) // UnityEngine.Object null
                        return true;

                    // Optional: support objects with IsDeleted/IsDestroyed flags
                    var type = entry.GetType();
                    var deletedField = type.GetField("IsDeleted");
                    if (deletedField != null && (bool)deletedField.GetValue(entry))
                        return true;

                    return false;
                };
            }
            // CASE 1 — Managed List<T>
            if (source is List<T> managedList)
            {
                for (int i = 0; i < managedList.Count; i++)
                    if (isEmpty(managedList[i]))
                        return i;

                return managedList.Count;
            }

            // CASE 2 — Il2Cpp List<T>
            if (source is Il2CppSystem.Collections.Generic.List<T> il2cppList)
            {
                int count = il2cppList.Count;
                for (int i = 0; i < count; i++)
                    if (isEmpty(il2cppList[i]))
                        return i;

                return count;
            }

            // CASE 4 — ANY IEnumerable<T> (managed or IL2CPP)
            if (source is IEnumerable<T> enumerable)
            {
                int index = 0;
                foreach (var entry in enumerable)
                {
                    if (isEmpty(entry))
                        return index;
                    index++;
                }
                return index;
            }

            throw new NotSupportedException("Unsupported list type for NextEmptyIndex");
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static Vector3 GetLevelButtonPosition(int col, int row)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            return new Vector3(
                -300f + col * 150f,
                160f - row * 130f

            );
        }
        /// <summary>Gets all declared values of an enum type.</summary>
        /// <typeparam name="T">The enum type whose values should be returned.</typeparam>
        /// <returns>A list containing the enum values.</returns>
        public static List<T> GetEnumValues<T>() where T : Enum => [.. (T[])typeof(T).GetEnumValues()];
        /// <summary>Formats a 64-bit integer using Chinese large-number units where applicable.</summary>
        /// <param name="num">The number to format.</param>
        /// <returns>The number represented with a Chinese large-number unit, or as digits if no unit applies.</returns>
        public static string FormatToChineseUnits(this long num)
        {
            // Chinese large-number units
            (long value, string unit)[] units =
            {
                (10_000L, "万"),
                (100_000_000L, "亿"),
                (1_000_000_000_000L, "兆"),
                (10_000_000_000_000_000L, "京")
            };

            // Find highest applicable unit
            for (int i = units.Length - 1; i >= 0; i--)
            {
                var (value, unit) = units[i];
                if (num >= value)
                    return $"{num / value}{unit}";
            }

            return num.ToString();
        }
        /// <summary>Formats a 32-bit integer using Chinese large-number units where applicable.</summary>
        /// <param name="num">The number to format.</param>
        /// <returns>The number represented with a Chinese large-number unit, or as digits if no unit applies.</returns>
        public static string FormatToChineseUnits(this int num)
        {
            // Chinese large-number units
            (long value, string unit)[] units =
            {
                (10_000L, "万"),
                (100_000_000L, "亿"),
                (1_000_000_000_000L, "兆"),
                (10_000_000_000_000_000L, "京")
            };

            // Find highest applicable unit
            for (int i = units.Length - 1; i >= 0; i--)
            {
                var (value, unit) = units[i];
                if (num >= value)
                    return $"{num / value}{unit}";
            }

            return num.ToString();
        }
        /// <summary>Formats a 64-bit integer in scientific notation when it is at least one billion.</summary>
        /// <param name="num">The number to format.</param>
        /// <returns>The number in scientific notation for large values, or as digits otherwise.</returns>
        public static string FormatToScientificNotation(this long num)
        {
            // For small numbers, just return the normal string
            if (num < 1_000_000_000L)
                return num.ToString();

            // Scientific notation, IL2CPP‑safe format
            return num.ToString("E2");
        }

        /// <summary>Formats a 32-bit integer in scientific notation when it is at least one billion.</summary>
        /// <param name="num">The number to format.</param>
        /// <returns>The number in scientific notation for large values, or as digits otherwise.</returns>
        public static string FormatToScientificNotation(this int num)
        {
            if (num < 1_000_000_000)
                return num.ToString();

            return num.ToString("E2");
        }
    }
}
