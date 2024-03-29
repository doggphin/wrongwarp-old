using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace WrongWarp
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private WWNetworkManager networkManager;
        [Header("UI")]
        [SerializeField] private GameObject SelectPlayType;
        [SerializeField] private TMP_InputField ipAddressInputField;

        public void HostServer()
        {
            networkManager.StartHost();
        }
    }
}