using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RTSOnlineGame
{
    public class JoinLobbyMenu : MonoBehaviour
    {
        [SerializeField] private GameObject landingPagePanel;
        [SerializeField] private TMP_InputField addressInput;
        [SerializeField] private Button joinButton;

        private void OnEnable()
        {
            RTSNetworkManager.ClientOnConnetted += HandleClientConnected;
            RTSNetworkManager.ClientOnDisconnetted += HandleClientDisconneted;
        }

        private void OnDisable()
        {
            RTSNetworkManager.ClientOnConnetted -= HandleClientConnected;
            RTSNetworkManager.ClientOnDisconnetted -= HandleClientDisconneted;
        }

        public void Join()
        {
            string address=addressInput.text;
            NetworkManager.singleton.networkAddress = address;
            NetworkManager.singleton.StartClient();

            joinButton.interactable = false;
        }

        private void HandleClientConnected()
        {
            joinButton.interactable = true;
            gameObject.SetActive(false);
            landingPagePanel.SetActive(false);
        }

        private void HandleClientDisconneted()
        {
            joinButton.interactable = true;
        }

    }
}