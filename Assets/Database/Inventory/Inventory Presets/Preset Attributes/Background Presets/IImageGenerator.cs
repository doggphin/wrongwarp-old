using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WrongWarp
{
    public interface IImageGenerator
    {
        public Sprite GenerateSprite(int width, int height, ServerSimpleItem[] simplifiedInventoryItems);
        public bool AveragePositionsX { get; }
        public bool AveragePositionsY { get; }
    }
}
