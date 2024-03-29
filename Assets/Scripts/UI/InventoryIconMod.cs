using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WrongWarp
{
    public class InventoryIconMod : MonoBehaviour
    {
        [SerializeField] private List<Sprite> spriteDatabase;
        [SerializeField] private Image modImage;

        public void Initialize(short spriteID)
        {
            modImage.sprite = spriteDatabase[spriteID % spriteDatabase.Count];
        }
    }
}