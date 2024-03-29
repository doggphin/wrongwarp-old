using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WrongWarp
{
    public class DraggableInventoryIcon : MonoBehaviour
    {
        [SerializeField] public TMP_Text stackSize;
        [SerializeField] public Image itemImage;

        private void Update()
        {
            transform.position = Input.mousePosition;
        }

        public void SetEnabled(bool enabled)
        {
            transform.position = Input.mousePosition;
            gameObject.SetActive(enabled);
        }

        public void SetItemData(Sprite itemSprite, string stackSize, Color32 color)
        {
            itemImage.sprite = itemSprite;
            this.stackSize.text = stackSize.ToString();
            this.stackSize.color = color;
        }
    }
}
