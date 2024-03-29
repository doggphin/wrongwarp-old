using UnityEngine;
using Mirror;

namespace WrongWarp
{
    public class DamageableEntity : NetworkEntity
    {
        [field:  Space]
        [field: SerializeField] protected bool IsDamageable { get; private set; } = true;
        [field: SerializeField] protected bool IsHealable { get; private set; } = true;
        [field: SerializeField] protected float MaxHealth { get; private set; } = 100;
        [field: SerializeField] protected float currentHealth = 100;

        [field:SerializeField] public Hurtbox[] Hurtboxes { get; private set; }

        public float Health
        {
            get => currentHealth;
            set
            {
                if (Health > currentHealth) { ServerHeal(Health - currentHealth); }
                else if (Health < currentHealth) { ServerDamage(currentHealth - Health); }
                else { Debug.LogWarning($"Tried to set health to what it already was on object {gameObject.name}."); }
            }
        }

        void Start()
        {
            for(int i=0; i<Hurtboxes.Length; i++)
            {
                Hurtboxes[i].SetEntityParent(this);
            }
        }

        private void OnValidate()
        {
            if (currentHealth <= 0) { currentHealth = 1; }
            if (MaxHealth < currentHealth) { currentHealth = MaxHealth; }
        }

        private void Awake()
        {
            if (isServer)
            {
                Health = Mathf.Min(MaxHealth, Health);
                if (Health <= 0)
                {
                    ServerKill();
                }
            }
        }

        [Server]
        public void ServerDamage(float amount)
        {
            if (IsDamageable)
            {
                currentHealth = Mathf.Max(currentHealth - amount, 0);

                if (Health == 0)
                {
                    ServerKill();
                    frameDataCache.slices.Add(new(ADDataType.Empty, (int)FrameActionID.Heal));
                }
            }
        }

        [Server]
        public void ServerHeal(float amount)
        {
            if (IsHealable)
            {
                currentHealth = Mathf.Min(MaxHealth, currentHealth + amount);
                frameDataCache.slices.Add(new(ADDataType.Empty, (int)FrameActionID.Heal));
            }
        }

        public enum DamageableEntityToggles
        {
            IsDamageable,
            IsHealable,
        }
    }
}
