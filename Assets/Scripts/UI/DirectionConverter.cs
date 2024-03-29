using UnityEngine;

namespace WrongWarp
{
    public static class DirectionConverter
    {
        /// <summary>
        /// Where right would typically be the beginning and return new Vector4(1, 0, 0, 0).
        /// </summary>
        public static Vector4 GetVector4(Direction direction, Direction startDirection = Direction.Right)
        {
            int directionValue = GetDirectionValue(direction, startDirection);

            return directionValue switch
            {
                0 => new Vector4(1, 0, 0, 0),
                1 => new Vector4(0, 1, 0, 0),
                2 => new Vector4(0, 0, 1, 0),
                3 => new Vector4(0, 0, 0, 1),
                _ => Vector4.zero,
            };
        }

        /// <summary>
        /// Where right would typically be the beginning and return new Vector4(1, 0).
        /// </summary>
        public static Vector2 GetVector2(Direction direction, Direction startDirection = Direction.Right)
        {
            int directionValue = GetDirectionValue(direction, startDirection);

            return directionValue switch
            {
                0 => new Vector2(1, 0),
                1 => new Vector2(0, 1),
                2 => new Vector2(-1, 0),
                3 => new Vector2(0, -1),
                _ => Vector4.zero,
            };
        }

        /// <summary>
        /// Returns the angle of a direction.
        /// </summary>
        public static float GetRotation(Direction direction)
        {
            return (ushort)direction * 90;
        }

        private static int GetDirectionValue(Direction direction, Direction startDirection)
        {
            return ((ushort)direction - (ushort)startDirection) % 4;
        }
    }
}
