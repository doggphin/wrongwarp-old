using UnityEngine;

namespace WrongWarp
{
    public class CircleSlotGenerator : MonoBehaviour, IPositionGenerator
    {
        /// <summary> Generates an evenly spaced array of positions along a circle. </summary>
        public Vector2[] GenerateSlotPositions(int amountOfSlots, float scale)
        {
            // If 0, 1 or 2 slots, return special cases
            if (amountOfSlots == 0) { Debug.Log(" ERROR :Tried to generate slot positions for 0 slots."); return new Vector2[0]; }   // Return null if amountOfSlots is 0 for some reason
            if (amountOfSlots == 1) { return new Vector2[1]; }                                                                       // Not technically necessary but saves computations
            if (amountOfSlots == 2) { return new Vector2[2] { new Vector2(-0.5f, 0), new Vector2(0.5f, 0) }; }                       // Makes slots 1 distance instead of sqrt2 distance

            if (amountOfSlots % 2 == 0)
            {
                // If even amount of slots, start at 180/amountOfSlots
                return GenerateCircleIntervals(amountOfSlots, scale, 180 / amountOfSlots, 360 / amountOfSlots);
            }
            else
            {
                // If odd amount of slots, start at 0 degrees
                return GenerateCircleIntervals(amountOfSlots, scale, 0, 360 / amountOfSlots);
            }
        }

        /// <summary> Generates an array of positions along a circle starting at the top. </summary>
        // Might want to move this to VectorMath
        private Vector2[] GenerateCircleIntervals(int amountOfSlots, float scale, float startingDegrees, float degreesToRotate)
        {
            // Initialize pointer and list of vectors
            Vector2 pointer = Vector2.up;
            Vector2[] positions = new Vector2[amountOfSlots];

            // Find scale factor necessary to make sure squares don't touch,
            // Even in the worst case scenario where they're still "touching" but farthest away from one another (diagonal):
            // [Slot]
            //       [Slot]
            // Where the distance between slot origins would be Sqrt2. Divide the array length by the distance they would be without any scaling to make them this far apart, always.
            float seperationLocalScale = Mathf.Sqrt(2) / (RotateVector(pointer, degreesToRotate) - pointer).magnitude;

            // Scale up pointer by this value and add initial scaled, rotated pointer to positions
            pointer = Vector2.up * seperationLocalScale;
            if (startingDegrees != 0) { pointer = RotateVector(pointer, startingDegrees); }
            positions[0] = pointer;

            // For each interval (starting at the second iteration)
            for (int i = 1; i < amountOfSlots; i++)
            {
                // Rotate the current pointer, then add it to positions
                pointer = RotateVector(pointer, degreesToRotate);
                positions[i] = pointer;
            }

            // Multiply the points by scale if needed
            if (scale != 1)
            {
                for (int i = 0; i < amountOfSlots; i++)
                {
                    positions[i] *= scale;
                }
            }

            // Convert positions to an array
            return positions;
        }

        /// <summary> Returns a rotated version of the vector. </summary>
        private Vector2 RotateVector(Vector2 vector, float angle)
        {
            float radians = angle * Mathf.Deg2Rad;
            float _x = vector.x * Mathf.Cos(radians) - vector.y * Mathf.Sin(radians);
            float _y = vector.x * Mathf.Sin(radians) + vector.y * Mathf.Cos(radians);
            return new Vector2(_x, _y);
        }
    }
}