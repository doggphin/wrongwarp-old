using UnityEngine;

namespace WrongWarp
{
    [CreateAssetMenu(fileName = "New Equippable", menuName = "Assets/Equippables")]
    public class Equippable : ScriptableObject
    {
        public GameObject equippableObject;
        public BaseEquippable EquippableObjectComponent
        {
            get { return equippableObject.GetComponent<BaseEquippable>(); }
        }
        //public ProjectilePreset projectilePreset;
        public AudioClip useSound;
        public ItemType ammoType;
    }
}
