using UnityEngine;
using Mirror;

namespace WrongWarp
{
    public class Healable : NetworkBehaviour
    {
        [SerializeField] private Health health;
        public void Heal(float amt)
        {
            health.Add(amt);
        }
    }
}