using UnityEngine;

namespace WrongWarp
{
    public class EquippableHolder : MonoBehaviour
    {
        public GameObject heldItem { get; private set; }

        // Returns whether the held item changed.
        /// <summary>
        /// Sets the equipped equippable.
        /// </summary>
        /// <param name="itemID"> The item ID to set the held item to. </param>
        /// <param name="caresAboutTransform"> Does the client care about setting the origin of the equippable for projectiles/hitboxes? </param>
        /// <returns> Returns whether the item was changed. </returns>
        public bool SetNewEquip(ushort itemID, bool caresAboutTransform)
        {
            Debug.Log($"Setting equip to {itemID}.");
            if(itemID == 0)
            {
                if (heldItem) { Destroy(heldItem); heldItem = null; return true; }
                return false;
            }

            Item item = DatabaseLookup.ItemByID(itemID);

            if(item.equippable == null)         // Check if trying to switch to nothing
            {
                if(heldItem == null) { return false; }      // If nothing was already equipped, do nothing and return false

                Destroy(heldItem);                          // Otherwise item was something, so delete what was being held
                return true;                                // Return that something changed
            }
            if(item.equippable.equippableObject != heldItem)     // Check if item trying to switch to is different from the one already being held
            {
                if (heldItem) { Destroy(heldItem); }                                        // If so, delete currently held item,
                heldItem = Instantiate(item.equippable.equippableObject, transform);        // then replace it with the new item.
                if(caresAboutTransform) { heldItem.GetComponent<BaseEquippable>().origin = transform; }
                return true;                                                                // Return that something changed.
            }
            return false;                                   // Otherwise trying to switch to same item. Return that nothing changed.
        }
    }
}
