using System;
using UnityEngine;
using Mirror;

namespace WrongWarp
{
    public class WandEquippable : BaseEquippable
    {
        [SerializeField] AudioClip clip;

        public void OnServerInitialized()
        {
            
        }

        public override void Use()
        {
            Debug.Log("needs to be fixed");
            AudioManager.PlaySound(transform, clip, 20);
            //Fire(transform.position, origin.rotation, clientSimpleItem);
            // What needs to be here instead is a converted IDBtag to fire with
        }

        //public override ProjectilePreset GetPreset()
        //{
        //    return null;
        //}
    }
}