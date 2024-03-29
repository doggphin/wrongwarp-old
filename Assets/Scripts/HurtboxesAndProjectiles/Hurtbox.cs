using UnityEngine;
using System;
using Mirror;

namespace WrongWarp
{
    public class Hurtbox : MonoBehaviour
    {
        [field: SerializeField] public float DamageMultiplier { get; private set; }

        [field: SerializeField] public Collider HurtboxCollider { get; private set; }

        private DamageableEntity parentEntity;

        [field: SerializeField] public int Priority { get; private set; } = 0;

        private void Start()
        {
            if(!(HurtboxCollider = GetComponent<Collider>()))
            {
                throw new Exception("No collider attached to hurtbox.");
            }
        }

        public void SetEntityParent(DamageableEntity parent)
        {
            if(parentEntity == null) { parentEntity = parent; }
        }

        private void OnValidate()
        {
            HurtboxCollider = GetComponent<Collider>();
        }

        [Server]
        public virtual void ServerDamage(float damage)
        {
            parentEntity.ServerDamage(damage);
        }

        [Client]
        public virtual void ClientDamage(float damage)
        {
            //attachedEntity.ClientDamage(damage);
        }
    }
}
