using UnityEngine;

namespace WrongWarp
{
    public static class WWPhysics
    {
        /// <summary>
        /// Casts spherecasts in an arc with a set start and end position around a point.
        /// </summary>
        /// <param name="pointA"> The point on the sphere to start spherecasts. </param>
        /// <param name="pointB"> The point on the sphere to end spherecasts. </param>
        /// <param name="center"> The center of the sphere. </param>
        /// <param name="castsPerCircle"> How many casts would be done in a full circle? </param>
        public static bool ArcSphereCast(Vector3 pointA, Vector3 pointB, Vector3 center, float castsPerCircle, float radius, out RaycastHit hitInfo, int layerMask)
        {
            Vector3 pointAFixed = pointA - center;
            Vector3 pointBFixed = pointB - center;
            int castsToPerform = (int)Mathf.Min(castsPerCircle * (Vector3.Angle(pointAFixed, pointBFixed) / 360), 1);
            float interval = 1 / castsToPerform;

            Vector3[] points = new Vector3[castsToPerform + 1];
            for(int i=0; i<castsToPerform + 1; i++)
            {
                points[i] = Vector3.Slerp(pointAFixed, pointBFixed, interval * i);
            }
            float distanceBetweenPoints = (points[1] - points[0]).magnitude;
            
            for(int i=0; i<castsToPerform; i++)
            {
                if (Physics.SphereCast(new Ray(points[i], points[i + 1]), radius, out hitInfo, distanceBetweenPoints, layerMask))
                {
                    return true;
                }
            }
            hitInfo = default;
            return false;
        }
    }
}
