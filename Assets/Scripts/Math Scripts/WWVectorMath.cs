using UnityEngine;

namespace WrongWarp
{
    /// <summary>
    /// A bunch of useful vector math functions, mostly for player movement
    /// </summary>
    public class WWVectorMath
    {
        //Simple calculations
        #region Shorthand Calculations
        /// <summary> Returns angle between two points' Vector2 and Vector3.down </summary>
        public static float GetAngle(Vector3 centerPosition, Vector3 hitPoint)
        {
            return Vector3.Angle(hitPoint - centerPosition, Vector3.down);
        }
        /// <summary> Returns angle between two points' Vector2 and Vector3.down </summary>
        public static float GetAngle(Vector3 vectorToPoint)
        {
            return Vector3.Angle(vectorToPoint, Vector3.down);
        }

        /// <summary> Returns the incline angle of what you're standing on </summary>
        public static float GroundIncline(Vector3 centerPosition, Vector3 hitPoint)
        {
            return 90 - Vector3.Angle(hitPoint - centerPosition, Vector3.down);
        }
        /// <summary> Returns the incline angle of what you're standing on </summary>
        public static float GroundIncline(Vector3 vectorToPoint)
        {
            return 90 - Vector3.Angle(vectorToPoint, Vector3.down);
        }

        /// <summary> Returns given Vector3 as a flattened Vector2 </summary>
        public static Vector2 Flatten2(Vector3 originalVector)
        {
            return new Vector2(originalVector.x, originalVector.z);
        }

        /// <summary> Returns the vector without vector.y </summary>
        public static Vector3 Flatten3(Vector3 originalVector)
        {
            return new Vector3(originalVector.x, 0, originalVector.z);
        }

        /// <summary> Turns a Vector2 into a flat Vector3 (x, 0, y) </summary>
        public static Vector3 MakeFlatVector3(Vector2 originalVector)
        {
            return new Vector3(originalVector.x, 0, originalVector.y);
        }

        /// <summary> Turns Vector2 movement and gravity into a total movement Vector3 </summary>
        /// Not very helpful
        public static Vector3 TotalMovement(Transform playerTransform, Vector2 inputVector, float gravity)
        {
            return playerTransform.right * inputVector.x + playerTransform.forward * inputVector.y + gravity * playerTransform.up;
        }
        #endregion Shorthand Calculations

        //Specific calculations
        #region "Advanced" Vector Calculations
        /// <summary> Returns the factor to multiply movement by based on slope stood on </summary>
        public static float MoveFactor(Vector2 direction, Vector3 centerPosition, Vector3 hitPoint)
        {
            Vector3 vectorToPoint = hitPoint - centerPosition;
            float dot = Mathf.Abs(Vector2.Dot(Flatten2(vectorToPoint).normalized, direction.normalized));
            return 1 - (dot * (1 - Mathf.Cos(GetAngle(vectorToPoint) * Mathf.Deg2Rad)));
        }

        /// <summary> [WIP] Finds distance to ground assuming standing on uniform angle terrain </summary> 
        /// Probably unnecessary
        public static float GetDistanceToGround(float skin, float angle, float hitDistance)
        {
            float distanceToGround = hitDistance / Mathf.Cos(angle);
            return Mathf.Abs(distanceToGround - skin);
        }

        /// <summary> [WIP] Returns the direction and amount by which to push player off slope </summary>
        /// Read paper drawing
        public static Vector3 SlipVelocitySet(Vector3 centerPosition, Vector3 hitPoint, float gravity)
        {
            Vector3 vectorToPoint = hitPoint - centerPosition;
            Vector3 downVelocity = Vector3.down * gravity;

            //Rotate towards Vector3.down by 90 degrees
            Vector3 newDirection = Vector3.RotateTowards(vectorToPoint, downVelocity, -4.71239f, 0f);
            Vector3 newVector = newDirection - Vector3.Project(newDirection, downVelocity);

            return newVector;
        }

        #endregion "Advanced" Vector Calculations
    }
}