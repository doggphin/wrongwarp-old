using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WrongWarp
{
    public interface IPositionGenerator
    {
        public Vector2[] GenerateSlotPositions(int amountOfSlots, float scale = 1);
    }
}
