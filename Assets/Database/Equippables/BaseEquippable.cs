using System;
using UnityEngine;
using Mirror;

namespace WrongWarp
{
    /// <summary>
    /// Class with base functions and variables meant to be overwritten to create a new equippable.
    /// </summary>
    public abstract class BaseEquippable : MonoBehaviour
    {
        //public static event Action<Vector3, Quaternion, ProjectileData> ItemFired = delegate {  };

        public ServerSimpleItem serverSimpleItem;
        public ClientSimpleItem clientSimpleItem;
        [SerializeField] public Transform origin;
        //[SerializeField] private ProjectilePreset projectilePreset;
        //[SerializeField] private ProjectilePreset[] projectilePresets;

        #region Initializers

        [Client]
        public virtual void Init(ClientSimpleItem clientSimpleItem, Transform origin)
        {
            this.clientSimpleItem = clientSimpleItem;
            this.origin = origin;
        }

        [Server]
        public void ServerInitialize(ServerSimpleItem serverSimpleItem, Transform origin)
        {
            this.serverSimpleItem = serverSimpleItem;
            this.origin = origin;
        }

        #endregion Initializers

        public abstract void Use();

        //public virtual ProjectilePreset GetPreset()
        //{
        //    return null;
        //}

        public virtual bool TryUseAmmo(ServerInventory ammoSource)
        {
            return true;
        }

        //public virtual void Fire(Vector3 position, Quaternion rotation, ProjectileData projectileData)
        //{
        //    ItemFired(position, rotation, projectileData);
        //}
    }
}
