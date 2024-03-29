using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WrongWarp
{
    [CreateAssetMenu(fileName = "New Position Generator", menuName = "Assets/Inventory/PositionGenerator")]
    public class PositionGenerator : ScriptableObject
    {
        [Tooltip("Initial slot positions to use. Next up are slotPositionGenerator slots.")]
        [SerializeField] private List<Vector2> initialPositions;

        [Tooltip("Generates slot positions with a script. If this also doesn't fill up the slots needed, will create default slots.")]
        [SerializeField] private GameObject slotPositionGenerator;

        [Tooltip("Should the slot positions be averaged along the X axis?")]
        [SerializeField] private bool averageSlotsX = false;

        [Space]
        [Tooltip("How many slots should there be per row when generated as default?")]
        [SerializeField] private int slotsPerRow = 6;
        public int SlotsPerRow
        {
            get
            {
                return slotsPerRow;
            }
        }

        /// <summary> Returns a list of Vector2 slot positions to use, where each slot is 1 unit x 1 unit. </summary>
        public Vector2[] GetSlotPositions(int amountOfSlots, float scale)
        {
            List<Vector2> slotsToReturn = new List<Vector2>();
            // If slot positions are set,
            if (initialPositions.Count != 0)
            {
                // Add as many as possible to slotsToReturn
                for (int i = 0; i < Mathf.Min(amountOfSlots, initialPositions.Count); i++)
                {
                    slotsToReturn.Add(initialPositions[i]);
                }
            }

            // If all the needed slot positions were generated, return it.
            // Trusts that the slots are optimally placed and don't need to be averaged.
            if (slotsToReturn.Count == amountOfSlots) { return slotsToReturn.ToArray(); }
            // Otherwise, if a slot position generator is set,
            if (slotPositionGenerator != null)
            {
                // Fill the rest of the slots using the generator
                var generatedSlots = slotPositionGenerator.GetComponent<IPositionGenerator>().GenerateSlotPositions(amountOfSlots - slotsToReturn.Count, scale);
                foreach (Vector2 position in generatedSlots)
                {
                    slotsToReturn.Add(position);
                }
            }

            // If slots are supposed to be averaged out along the X axis, average them.
            if (averageSlotsX) { AverageSlotPositions(slotsToReturn, false); }

            // If all the needed slot positions were generated, return it.
            if (slotsToReturn.Count == amountOfSlots) { return slotsToReturn.ToArray(); }

            // Otherwise, start filling up default slots.
            slotsToReturn = GenerateDefaultPositions(slotsToReturn, amountOfSlots);

            return slotsToReturn.ToArray();
        }

        /// <summary> Returns the list after filling it with default slot positions until it has amountOfSlots positions. </summary>
        private List<Vector2> GenerateDefaultPositions(List<Vector2> slotPositions, int amountOfSlots)
        {
            int initiallyGeneratedSlots = slotPositions.Count;

            int xPosition = 0;
            int yPosition = 0;
            for (int i = 0; i < amountOfSlots - initiallyGeneratedSlots; i++)
            {
                // Six positions across; make positions -2.5, -1.5, -0.5, 0.5, 1.5, 2.5
                slotPositions.Add(new Vector2(xPosition - 2.5f, yPosition));
                xPosition++;
                if (xPosition == slotsPerRow) { xPosition = 0; yPosition++; }
            }

            // Next, move up any slots that were moved in slotPositions by however much they need to be moved
            if (initiallyGeneratedSlots != 0)
            {
                // For every slot position,
                for (int i = 0; i < initiallyGeneratedSlots; i++)
                {
                    // Set itemSlotPool[i]'s position to slotPositions[i]
                    slotPositions[i] += (Vector2.up * yPosition) + Vector2.up;
                }
            }

            slotPositions = AverageSlotPositions(slotPositions, true);

            return slotPositions;
        }

        /// <summary> Corrects the positions of slots between startIndex and startIndex + amountToInclude to have an average X or Y position of 0.
        /// <para> Set isY to 'true' to average Y position. Set isY to 'false' to average X position. </para></summary>
        // Kinda jank way of selecting X or Y to average. Is there a better way to do it?
        private List<Vector2> AverageSlotPositions(List<Vector2> slotPositions, bool isY)
        {
            float offset = 0;

            // Find the total offset from of all the selected slots
            foreach (Vector2 position in slotPositions)
            {
                if (isY) { offset += position.y; }
                else { offset += position.x; }
            }

            // Get the amount to move, which is total offset divided by how many slots there are
            float amountToMove = -offset / slotPositions.Count;

            // If amount to move is substantial,
            if (Mathf.Abs(amountToMove) > 0.0001f)
            {
                // Move them in the direction chosen by the total offset divided by amount of slots having their position changed
                Vector2 offsetCorrectorVector = isY ? Vector3.up * amountToMove : Vector3.right * amountToMove;
                for (int i = 0; i < slotPositions.Count; i++)
                {
                    slotPositions[i] += offsetCorrectorVector;
                }
            }

            return slotPositions;
        }
    }
}