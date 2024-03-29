using UnityEngine;
using Mirror;

namespace WrongWarp
{
    public class PlayerInitializer : NetworkBehaviour
    {
        bool isInitialized = false;
        [SerializeField] private UIManager uiManager;
        [SerializeField] private MainCamera mainCamera;

        /// <summary> Initializes this player on the server, then tells the client to initialize itself. </summary>
        [Server]
        public void Init()
        {
            if (isInitialized) { Debug.Log("ERROR: Player already initialized."); return; }

            TargetInitialize(GetComponent<NetworkIdentity>().connectionToClient);
            GetComponent<InventoryManager>().ServerInit();
            TargetShowHotbar(GetComponent<NetworkIdentity>().connectionToClient);

            isInitialized = true;
        }

        /// <summary> Initializes this player on the client. </summary>
        [TargetRpc]
        public void TargetInitialize(NetworkConnection target)
        {
            uiManager.GetComponent<UIManager>().ClientInit();
            GetComponent<InventoryManager>().ClientInit();

            GetComponent<NetworkEntity>().enabled = false;

            GetComponent<PlayerMovement>().InitAsClient();
            GetComponent<EmotePlayer>().Init();
            GetComponentInChildren<PlayerCamera>().InitAsClient();
            InputManager.PlayerControls.Enable();
            InputManager.UIInput.Enable();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            mainCamera.Init();
        }


        /// <summary> Jank fix to make sure the hotbar shows up on a client and inventory is preloaded. </summary>
        [TargetRpc]
        private void TargetShowHotbar(NetworkConnection target)
        {
            uiManager.GetComponent<UIManager>().ToggleUI(UIType.InventoryMenu);
            uiManager.GetComponent<UIManager>().ToggleUI(UIType.InventoryMenu);
        }
    }
}