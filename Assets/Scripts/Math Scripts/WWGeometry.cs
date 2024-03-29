using UnityEngine;

namespace WrongWarp
{
    public static class WWGeometry
    {
        public static float GetDistanceToRectangle()
        {
            return 0;
        }

        public static Vector3 GetPointOnPlaneToOrigin()
        {
            return new Vector3();
        }
        
        /// <summary>
        /// Treating a transform as an original origin, get the position of the point if it used a different transform with the same offset from the original transform.
        /// </summary>
        /// <param name="point"> Point in world space.</param>
        /// <param name="originalOrigin"> The original "origin" </param>
        /// <param name="newOrigin"> The destination to rotate and move the point around. </param>
        public static Vector3 ChangeOriginOfPoint(Vector3 point, Transform originalOrigin, Transform newOrigin)
        {
            // Read from bottom to top
            return newOrigin.position + 
                (newOrigin.rotation * 
                (Quaternion.Inverse(originalOrigin.rotation) * 
                (point - originalOrigin.position)
                ));
        }

        public static Vector3 RotToNormal(Quaternion quaternion)
        {
            return quaternion * Vector3.forward;
        }
    }
}
