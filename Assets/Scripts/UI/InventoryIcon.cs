using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace WrongWarp
{
    public class InventoryIcon : MonoBehaviour, ILeftClickable, ILeftClickDroppable, IRightClickable, IRightClickDroppable
    {
        [Header("References")]
        public Image itemImage;
        public Image backgroundImage;
        public TMP_Text stackSizeField;
        public RectTransform rectTransform;

        [SerializeField] private GameObject modTagIconReference;
        Dictionary<ushort, InventoryIconMod> modTagIcons = new();

        [SerializeField] private GameObject durabilityBarReference;
        HealthBar durabilityBar;


        private ushort index;
        private ushort invIndex;

        public static event Action<ushort, ushort, ushort?> SlotPickedUp = delegate { };
        public static event Action<ushort, ushort> SlotDropped = delegate { };

        /// <summary> Run when this inventory icon is created. </summary>
        public void Initialize(ushort invIndex, ushort index)
        {
            this.index = index;
            this.invIndex = invIndex;
            itemImage.color = Color.white;
        }

        public void SetVisualData(ClientSimpleItem item)
        {
            DeleteVisualTags();

            Item itemSO = DatabaseLookup.ItemByID(item.id);
            if (item.isEmpty)
            {
                SetItemVisual(null);
                SetStackVisual("", Color.red);
                //"", Color.red
            }
            else
            {
                SetItemVisual(itemSO.itemSprite);
                SetStackVisual(itemSO.GetSlotStackText(item.stackSize), itemSO.GetSlotStackColor(item.stackSize));
                //, itemToLookUp.GetSlotStackText(item.stackSize), itemToLookUp.GetSlotStackColor(item.stackSize)
            }

            if (item.idbTags != null)
            {
                foreach (var tag in item.idbTags.IDBTags)
                {
                    AddVisualTag(tag.Key, tag.Value);
                }
            }
        }

        /// <summary> Sets visuals for this slot's background. </summary>
        public void SetBackgroundVisuals(SlotType slotType, Vector2 slotScale)
        {
            backgroundImage.sprite = slotType.slotSprite;
            rectTransform.localScale = slotScale;
        }

        /// <summary> Sets visuals for this slot's item and amount. </summary>
        public void SetItemVisual(Sprite itemSprite)
        {
            if (itemSprite == null)
            {
                itemImage.enabled = false;
            }
            else
            {
                itemImage.sprite = itemSprite;
                itemImage.enabled = true;
            }
        }

        public void SetStackVisual(string amount, Color color)
        {
            stackSizeField.text = amount;
            stackSizeField.color = color;
        }

        public void AddVisualTag(ushort tagID, Tag tag)
        {
            // If tag is 0:durability, 1:ammo count or 4:color, try to update it
            if (modTagIcons.TryGetValue(tagID, out InventoryIconMod modTag) && tagID != 0 && tagID != 1 && tagID != 4)
            {
                return;
            }

            switch(tagID)
            {
                case 0:
                    float[] durabilityData = tag.DataAsFloats;
                    DurabilityModifier(true, durabilityData[0], durabilityData[1]);
                    break;
                case 1:
                    stackSizeField.text = tag.DataAsUShorts[0].ToString();
                    stackSizeField.color = Color.white;
                    break;
                case 4:
                    float[] colorData = tag.DataAsFloats;
                    itemImage.color = colorData.Length == 4 ? new Color(colorData[0], colorData[0], colorData[2], colorData[3]) : new Color(colorData[0], colorData[1], colorData[2]);
                    break;
            }
        }

        public void DurabilityModifier(bool enable, float durability, float maxDurability)
        {
            if (enable)
            {
                if (durabilityBar == null)
                {
                    // Instantiate a durability bar and move it to the correct position and rotation
                    durabilityBar = Instantiate(durabilityBarReference, transform, false).GetComponent<HealthBar>();
                    durabilityBar.Init(durability, maxDurability, null);

                    RectTransform durabilityBarTransform = durabilityBar.GetComponent<RectTransform>();
                    durabilityBarTransform.anchoredPosition = new Vector2(-46, 0);
                    durabilityBarTransform.localRotation = Quaternion.Euler(0, 0, 90); 
                }
                else
                {
                    // Update the durability bar's data
                    if (durabilityBar.maxHealth != maxDurability) { durabilityBar.maxHealth = maxDurability; }
                    durabilityBar.SetHealth(durability);
                }
            }
            else
            {
                // Otherwise destroy the durability bar
                if(durabilityBar != null)
                {
                    Destroy(durabilityBar.gameObject);
                    durabilityBar = null;
                }
            }
        }

        public void DeleteVisualTags()
        {
            if(durabilityBar) { Destroy(durabilityBar.gameObject); durabilityBar = null; }
            foreach(var modTagIcon in modTagIcons)
            {
                Destroy(modTagIcon.Value.gameObject);
            }
            modTagIcons = new();
        }

        public void SetPosition(Vector2 position)
        {
            rectTransform.anchoredPosition = position;
        }

        // ===========================================
        // Click Logic
        // ===========================================

        #region Click Logic

        public void OnLeftClicked()
        {
            SlotPickedUp(invIndex, index, null);
        }

        public void OnLeftClickDropped()
        {
            DropSlot();
        }

        public void OnRightClicked()
        {
            SlotPickedUp(invIndex, index, 1);
        }

        public void OnRightClickDropped()
        {
            DropSlot();
        }

        private void DropSlot()
        {
            SlotDropped(invIndex, index);
        }

        #endregion Click Logic

    }
}