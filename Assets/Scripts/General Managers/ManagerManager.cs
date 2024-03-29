using UnityEngine;
using Mirror;

namespace WrongWarp
{
    public class ManagerManager : MonoBehaviour
    {
        [SerializeField] private GameObject[] clientManagers;
        [SerializeField] private GameObject[] networkedManagers;

        public static ManagerManager Instance { get; private set; }

        private void Awake()
        {
            if(Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }

            foreach(var manager in clientManagers)
            {
                Instantiate(manager);
            }

            DontDestroyOnLoad(gameObject);
        }

        [Server]
        public static void CreateServerManagers()
        {
            foreach(var manager in Instance.networkedManagers)
            {
                NetworkServer.Spawn(Instantiate(manager));
            }
        }
    }
}
