using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;
using System.Linq;
using UnityEngine.SceneManagement;

namespace WrongWarp
{
    public class WWNetworkManager : NetworkManager
    {
        [Header("Game Objects")]
        [SerializeField] private GameObject gamePlayerPrefab;
        [SerializeField] private GameObject pickupableCrate;
        [SerializeField] private GameObject damageableObject;

        [Header("Scenes")]
        [Scene][SerializeField] private string gameScene;
        [Scene][SerializeField] private string menuScene;

        public static event Action OnClientDisconnected;
        public static event Action OnClientConnected;
        //public static event Action<NetworkConnection> OnServerReadied;

        public override void OnClientConnect()
        {
            base.OnClientConnect();
            OnClientConnected?.Invoke();
        }

        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
            OnClientDisconnected?.Invoke();
        }

        [Server]
        public override void OnStartServer()
        {
            ServerChangeScene("WorldMain");
            ManagerManager.CreateServerManagers();
        }

        [Server]
        public override void OnServerSceneChanged(string sceneName)
        {
            InstantiateSpawnedObjects();
        }

        [Server]
        public override void OnServerReady(NetworkConnectionToClient conn)
        {
            SpawnPlayer(conn);
        }

        [Server]
        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            base.OnServerConnect(conn);
        }

        [Server]
        private void SpawnPlayer(NetworkConnectionToClient conn)
        {
            if (conn.identity != null)
            {
                Destroy(conn.identity.gameObject);
                NetworkServer.RemovePlayerForConnection(conn, true);
            }

            GameObject playerInstance = Instantiate(playerPrefab, /*new Vector3(480, 2.5f, 3)*/ Vector3.zero, new Quaternion());

            NetworkServer.Spawn(playerInstance, conn);
            NetworkServer.AddPlayerForConnection(conn, playerInstance);

            playerInstance.GetComponent<PlayerInitializer>().Init();
        }

        public void SetScene(string sceneName)
        {
            switch (sceneName)
            {
                case "game":
                    ServerChangeScene(gameScene);
                    break;
                case "menu":
                    ServerChangeScene(menuScene);
                    break;
                default:
                    Debug.Log("Couldn't find the scene. Leaving as default.");
                    break;
            }
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Q))
            {
                AudioManager.RecalibrateAudioPlayers();
            }
            if(Input.GetKeyDown(KeyCode.T))
            {
                NetworkServer.Spawn(Instantiate(damageableObject, new Vector3(-48, 1.71f, 11), Quaternion.identity));
            }
        }

        private void InstantiateSpawnedObjects()
        {
            GameObject newCrate;

            for (ushort i = 1; i < DatabaseLookup.database.itemDatabase.Count; i++)
            {
                newCrate = Instantiate(pickupableCrate, new Vector3(0, 5, i), Quaternion.identity);
                newCrate.GetComponent<DroppedItem>().item = new ServerSimpleItem(i, (ushort)(DatabaseLookup.ItemByID(i).maxStackSize * (1 + UnityEngine.Random.Range(0, 1))));
                NetworkServer.Spawn(newCrate);
                if (i == 1)
                {
                    newCrate = Instantiate(pickupableCrate, new Vector3(0, 5, 0), Quaternion.identity);
                    newCrate.GetComponent<DroppedItem>().item = new ServerSimpleItem(i, (ushort)(DatabaseLookup.ItemByID(i).maxStackSize * (1 + UnityEngine.Random.Range(0, 1))));
                    NetworkServer.Spawn(newCrate);
                }
            }

            PortalManager.CreatePortalPair(new Vector3(-50f, 2.5f, 5), Quaternion.Euler(new Vector3(0, 0, 0)), new Vector3(480, 2.5f, 5), new Quaternion());
            NetworkServer.Spawn(Instantiate(damageableObject, new Vector3(-48, 1.71f, 11), Quaternion.identity));
        }
    }
}