using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

namespace WrongWarp
{
    public class Health : NetworkBehaviour
    {
        [SerializeField] private float maxHealth = 100f;
        //[SerializeField] public Damageable[] hurtboxes;

        [SyncVar(hook = nameof(HandleHealthUpdated))]
        [ShowInInspector] private float actualHealth;
        [ShowInInspector] private float clientPredictedHealth;


        public static event EventHandler<DeathEventArgs> OnDeath;
        public static event EventHandler<HealthChangedEventArgs> OnHealthChanged;

        public bool IsDeadOnServer => actualHealth == 0f;
        public bool IsDeadOnClient => clientPredictedHealth == 0f;

        public override void OnStartServer()
        {
            Init();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
        }

        [Server]
        public void Init(float health = 0)
        {
            if(health == 0)
            {
                actualHealth = maxHealth;
            } else
            {
                if (health < 0)
                {
                    Debug.LogError($"Tried to initialize health at {health}, which isn't possible.");
                    actualHealth = 0;
                }
                actualHealth = health;
            }
        }

        //Makes player "die" instead of disconnect
        [ServerCallback]
        private void OnDisable()
        {
            OnDeath?.Invoke(this, new DeathEventArgs(connectionToClient));
        }

        [Server]
        public void Add(float amt)
        {
            amt = Mathf.Max(amt, 0);

            actualHealth = Mathf.Min(actualHealth + amt, maxHealth);
        }

        [Server]
        public void Remove(float amt)
        {
            amt = Mathf.Max(amt, 0);
            actualHealth = Mathf.Max(actualHealth - amt, 0);

            if (actualHealth == 0)
            {
                OnDeath?.Invoke(this, new DeathEventArgs(connectionToClient));

                RpcHandleDeath();
            }
        }

        public void SetHurtboxesActive(bool enabled)
        {
            //if(enabled)
            //{
            //    foreach(Damageable hurtbox in hurtboxes)
            //    {
            //        hurtbox.gameObject.layer = 30;
            //    }
            //}
            //else
            //{
            //    foreach (Damageable hurtbox in hurtboxes)
            //    {
            //        hurtbox.gameObject.layer = 0;
            //    }
            //}  
        }

        private void HandleHealthUpdated(float oldAmount, float newAmount)
        {
            OnHealthChanged?.Invoke(this, new HealthChangedEventArgs(oldAmount, newAmount));
        }

        [ClientRpc]
        private void RpcHandleHealthUpdated(float amount)
        {
            clientPredictedHealth = amount;
        }

        [ClientRpc]
        private void RpcHandleDeath()
        {
            gameObject.SetActive(false);
        }

        public struct DeathEventArgs
        {
            public NetworkConnection conn;

            public DeathEventArgs(NetworkConnection conn)
            {
                this.conn = conn;
            }
        }

        public struct HealthChangedEventArgs
        {
            public float health;
            public float maxHealth;

            public HealthChangedEventArgs(float health, float maxHealth)
            {
                this.health = health;
                this.maxHealth = maxHealth;
            }
        }

    }
}