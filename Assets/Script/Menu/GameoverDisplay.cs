using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RTSOnlineGame
{
    public class GameoverDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject gameoverDisplayParent;
        [SerializeField] private TMP_Text winnerNameText;
        private void Start()
        {
            GameOverHandler.clientOnGameOver += ClientHandleGameover;
        }

        private void OnDestroy()
        {
            GameOverHandler.clientOnGameOver -= ClientHandleGameover;
        }
        
        public void LeaveGame()
        {
            if(NetworkServer.active && NetworkClient.isConnected)
            {
                NetworkManager.singleton.StopHost();
            }
            else
            {
                NetworkManager.singleton.StopClient();
            }
        }

        private void ClientHandleGameover(string winner)
        {
            gameoverDisplayParent.SetActive(true);
            winnerNameText.text = $"{winner} has won";
        }
    }

}
