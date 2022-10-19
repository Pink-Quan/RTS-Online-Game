using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RTSOnlineGame
{
    public class LobbyMenu : MonoBehaviour
    {
        [SerializeField] private GameObject lobbyUI;
        [SerializeField] private Button startGameButton;
        [SerializeField] private TMP_Text[] playerNameTexts;

        private void Start()
        {
            RTSNetworkManager.ClientOnConnetted += HandleClientConnected;
            RTSPlayer.AuthorityOnPartyOwnerStateUpdate += AuthorityHandlePartyOwnerStateUpdated;
            RTSPlayer.ClientOnInforUpdate += ClientHandleInforUpdated;
        }

        private void OnDestroy()
        {
            RTSNetworkManager.ClientOnConnetted -= HandleClientConnected;
            RTSPlayer.AuthorityOnPartyOwnerStateUpdate -= AuthorityHandlePartyOwnerStateUpdated;
            RTSPlayer.ClientOnInforUpdate -= ClientHandleInforUpdated;
        }

        private void ClientHandleInforUpdated()
        {
            List<RTSPlayer> list = ((RTSNetworkManager)NetworkManager.singleton).playerList;
            for(int i = 0; i < list.Count; i++)
            {
                playerNameTexts[i].text = list[i].GetDisplayName();
            }
            for(int i = list.Count; i < playerNameTexts.Length; i++)
            {
                playerNameTexts[i].text = "Waiting for player ...";
            }
            startGameButton.interactable=list.Count > 1;
        }

        private void AuthorityHandlePartyOwnerStateUpdated(bool obj)
        {
            startGameButton.gameObject.SetActive(obj);
        }

        private void HandleClientConnected()
        {
            lobbyUI.SetActive(true);
        }
        public void StartGame()
        {
            NetworkClient.connection.identity.GetComponent<RTSPlayer>().CmdStartGame();
        }

        public void LeaveLobbyy()
        {
            if(NetworkServer.active&&NetworkClient.isConnected)
            {
                NetworkManager.singleton.StopHost();
            }
            else
            {
                NetworkManager.singleton.StopClient();
                SceneManager.LoadScene(0);
            }
        }
    }
}