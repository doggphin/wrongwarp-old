using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

namespace WrongWarp
{
    public class JoinServerMenu : MonoBehaviour
    {
        [SerializeField] private WWNetworkManager networkManager;
        [Header("UI")]
        [SerializeField] private TMP_InputField ipAddressInputField;
        [SerializeField] private GameObject joinServerPanel;

        private void OnEnable()
        {
            WWNetworkManager.OnClientConnected += HandleClientConnected;
            WWNetworkManager.OnClientDisconnected += HandleClientDisconnected;
        }
        private void OnDisable()
        {
            WWNetworkManager.OnClientConnected -= HandleClientConnected;
            WWNetworkManager.OnClientDisconnected -= HandleClientDisconnected;
        }

        public void JoinServer()
        {
            networkManager.networkAddress = ipAddressInputField.text;
            networkManager.StartClient();
            Debug.Log("Attempting to connect to " + ipAddressInputField.text);
        }

        private void HandleClientConnected()
        {
            joinServerPanel.SetActive(false);
        }

        private void HandleClientDisconnected()
        {
            joinServerPanel.SetActive(true);
        }
    }
}