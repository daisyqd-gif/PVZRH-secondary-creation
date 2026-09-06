namespace CustomPlantClass.Main
{
    public static class GeneralTools
    {
        /// <summary>
        /// Returns a list of colliders within a cone-shaped area defined by the origin, direction, maximum distance, and cone angle.
        /// </summary>
        /// <param name="origin">The origin point of the cone</param>
        /// <param name="direction">The direction the cone is facing</param>
        /// <param name="maxDistance">The maximum distance of the cone</param>
        /// <param name="coneAngle">The angle of the cone in degrees</param>
        /// <param name="layerMask">The layer mask to use for the raycasts</param>
        /// <param name="rayCount">The number of rays to cast within the cone</param>
        /// <returns>A null checked list of colliders within a cone-shaped area</returns>
        public static List<Collider2D> GetCollidersInCone(Vector2 origin, Vector2 direction, float maxDistance, float coneAngle, LayerMask layerMask, int rayCount = 10)
        {
            List<Collider2D> collidersInCone = new List<Collider2D>();

            // Normalize the direction vector
            direction.Normalize();

            // Calculate the half angle of the cone in radians
            float halfAngle = coneAngle * 0.5f * Mathf.Deg2Rad;

            // Calculate the angle step for raycasting
            float angleStep = coneAngle / (rayCount - 1);

            // Loop through each ray in the cone
            for (int i = 0; i < rayCount; i++)
            {
                // Calculate the current angle for this ray
                float currentAngle = -halfAngle + (i * angleStep * Mathf.Deg2Rad);

                // Rotate the direction vector by the current angle
                Vector2 rotatedDirection = new Vector2(
                    direction.x * Mathf.Cos(currentAngle) - direction.y * Mathf.Sin(currentAngle),
                    direction.x * Mathf.Sin(currentAngle) + direction.y * Mathf.Cos(currentAngle)
                );

                // Perform a raycast in the rotated direction
                RaycastHit2D hit = Physics2D.Raycast(origin, rotatedDirection, maxDistance, layerMask);
                if (hit.collider != null && !collidersInCone.Contains(hit.collider))
                {
                    collidersInCone.Add(hit.collider);
                }
            }

            return collidersInCone;
        }
        public static List<Collider2D> GetCollider2DsInOval(Vector2 center, float radiusX, float radiusY, LayerMask layerMask)
        {
            List<Collider2D> collidersInOval = new List<Collider2D>();

            // Calculate the bounding box of the oval
            Vector2 min = new Vector2(center.x - radiusX, center.y - radiusY);
            Vector2 max = new Vector2(center.x + radiusX, center.y + radiusY);

            // Get all colliders in the bounding box
            Collider2D[] colliders = Physics2D.OverlapAreaAll(min, max, layerMask);

            // Filter colliders that are actually within the oval
            foreach (var collider in colliders)
            {
                Vector2 closestPoint = collider.ClosestPoint(center);
                float normalizedX = (closestPoint.x - center.x) / radiusX;
                float normalizedY = (closestPoint.y - center.y) / radiusY;

                if ((normalizedX * normalizedX) + (normalizedY * normalizedY) <= 1f)
                {
                    collidersInOval.Add(collider);
                }
            }

            return collidersInOval;
        }
    }
}