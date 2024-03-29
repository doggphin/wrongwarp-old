using UnityEngine;

namespace WrongWarp
{

    [CreateAssetMenu(fileName = "New Slot Type", menuName = "Assets/Inventory/SlotType")]
    public class SlotType : ScriptableObject
    {
        [Space]
        [Header("Slot Visuals")]
        [Tooltip("The sprite to use for inventory slots.")]
        [SerializeField] public Sprite slotSprite;

        [Tooltip("The sprite to use for inventory slots' highlights.")]
        [SerializeField] public Sprite slotHighlightSprite;

        [Tooltip("The sprite to use for a highlighted inventory slot.")]
        [SerializeField] public Sprite slotSpriteHighlighted;

        [Tooltip("Is the highlight an outline, or is it a new slot sprite?")]
        public bool isOutline = true;

        [Tooltip("Does the highlight glow in and out?")]
        public bool glows = true;

        [Tooltip("How big should each slot be?")]
        [SerializeField] private float padding = 85;

        /// <summary> Multiplies an array of slot positions by the padding of this slotType. </summary>
        public Vector2[] PadPositions(Vector2[] slotPositions)
        {
            for (int i = 0; i < slotPositions.Length; i++)
            {
                slotPositions[i] *= padding;
            }
            return slotPositions;
        }

        public Vector2 GetBackgroundSize(Vector2[] slotPositions)
        {
            float maxY = 0;
            float minY = 0;

            //float minX = 0;
            //float maxX = 0;

            // Find the maximum and minimum Y positions between all the item slots
            foreach (Vector2 position in slotPositions)
            {

                if (position.y > maxY) { maxY = position.y; }
                else if (position.y < minY) { minY = position.y; }

                //if (position.x > maxX) { maxX = position.x; }
                //else if (position.x < minX) { minX = position.x; }
            }

            float heightToSet = maxY - minY;

            // Sets width to default 6 slots of padding.
            return new Vector2(560, maxY - minY + padding + 50);    // 6 * 85 + 50 = 560
        }

        public Vector2 GetSlotScale()
        {
            return new Vector2(padding * 0.01f, padding * 0.01f);
        }
    }
}