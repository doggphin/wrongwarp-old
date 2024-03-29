using System;
using UnityEngine;

namespace WrongWarp
{
    /// <summary> An extremely unoptimized but very cool distorted color wheel generator. </summary>
    [Serializable]
    public class ColorWheel : MonoBehaviour, IImageGenerator
    {
        public bool averagePositionsX;
        public bool AveragePositionsX { get { return averagePositionsX; } }
        public bool averagePositionsY;
        public bool AveragePositionsY { get { return averagePositionsY; } }

        [Space]
        // Noise values
        public float offsetX = 100f;
        public float offsetY = 100f;
        public float scale = 1f;
        public float maxDistortionAngle = 10f;
        public float distortionSpeed = 0f;

        Vector2 pointer;
        // Set red vector to up, green to bottom right, and blue to bottom left
        Vector2 red = Vector2.up;
        Vector2 green = new Vector2(.866f, -0.5f);
        Vector2 blue = new Vector2(-.866f, -0.5f);

        /// <summary> Generates a color wheel sprite of width and height. </summary>
        public Sprite GenerateSprite(int width, int height, ServerSimpleItem[] items)
        {
            Texture2D colorWheelTexture = new Texture2D(width, height);

            // Finding half of width and height for future use
            int halfWidth = (width / 2);
            int halfHeight = (height / 2);


            // These should get set from the simplifiedNetworkItems
            int positionX = 0;
            int positionY = 0;


            // For every x coordinate of the image from left to right,
            for (int x = -halfWidth; x < halfWidth; x++)
            {
                // For every y coordinate of this x coordinate from bottom to top,
                for (int y = -halfHeight; y < halfHeight; y++)
                {
                    if (x == 0 && y == 0) { colorWheelTexture.SetPixel(x + halfWidth, y + halfHeight, new Color(1, 1, 1, 1)); }
                    // === COLOR POINT FINDER ===
                    // Create a vector starting at (0,0) to the point we're setting
                    pointer = new Vector2(x + positionX, y + positionY).normalized;
                    pointer = RotateVector(pointer, CalculateNoise(x + positionX, y + positionY, width, height) * maxDistortionAngle);

                    // Find degrees between pointer vector and red, green and blue maximums
                    float degreesToRed = Vector2.Angle(pointer, red);
                    float degreesToGreen = Vector2.Angle(pointer, green);
                    float degreesToBlue = Vector2.Angle(pointer, blue);

                    // Set red, green and blue values to 1-(the angle between the point and their maximum * ~0.00833)
                    // 0.00833 is ~=1/120 (faster to multiply than divide) and is used to make it hit 0 by the time it reaches another color
                    // For example, starting at the top of the circle red is 1, once it reaches yellow (60 degrees to the side), it will be 1-(120*0.00833)~=0
                    // DegreesToColor are multiplied to the power of 1.8 to make RGB colors fall off faster and make the secondary colors take up more of the wheel
                    float redColor = Mathf.Clamp((float)(1 - Mathf.Pow((float)(degreesToRed * 0.00833333), 1.8f)), 0, 1);
                    float greenColor = Mathf.Clamp((float)(1 - Mathf.Pow((float)(degreesToGreen * 0.00833333), 1.8f)), 0, 1);
                    float blueColor = Mathf.Clamp((float)(1 - Mathf.Pow((float)(degreesToBlue * 0.00833333), 1.8f)), 0, 1);
                    //Consider clamping between 0.05f and 0.95f

                    // Multiply all the colors to where at least one will be 1f
                    float factor = 1 / (Mathf.Max(redColor, greenColor, blueColor));

                    // Apply pixel color
                    colorWheelTexture.SetPixel(x + halfWidth, y + halfHeight, new Color(redColor * factor, greenColor * factor, blueColor * factor, 1f));
                }
            }

            colorWheelTexture.Apply();
            Sprite spriteToReturn = Sprite.Create(colorWheelTexture, new Rect(0, 0, width, height), new Vector2(width * 0.5f, height * 0.5f));
            return spriteToReturn;
        }

        /// <summary> Returns a random noise value. </summary>
        private float CalculateNoise(int x, int y, int width, int height)
        {
            if (width == 0 || height == 0) { return 0; }
            float xCoord = (x / (width * 0.5f)) * scale + offsetX;
            float yCoord = (y / (height * 0.5f)) * scale + offsetY;

            float value = Mathf.PerlinNoise(xCoord, yCoord);
            return (value - 0.5f) * 2;
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