using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RTSOnlineGame
{
    public class RTSNetworkManager : NetworkManager
    {
        [SerializeField] private GameObject basePrefab;
        [SerializeField] private GameOverHandler gameOverHandlerPrefab;

        public static event Action ClientOnConnetted;
        public static event Action ClientOnDisconnetted;

        public List<RTSPlayer> playerList =new List<RTSPlayer>();
        private bool isGameInProgress=false;

        #region Server

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            if (!isGameInProgress) return;
            conn.Disconnect();
        }
        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            RTSPlayer player=conn.identity.GetComponent<RTSPlayer>();
            playerList.Remove(player);
            base.OnServerDisconnect(conn);
        }

        public override void OnStopServer()
        {
            playerList.Clear();
            isGameInProgress=false;
        }

        public void StarGame()
        {
            if (playerList.Count < 2) return;
            isGameInProgress = true;
            ServerChangeScene("MapScene");
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);

            RTSPlayer player=conn.identity.GetComponent<RTSPlayer>(); 
            playerList.Add(player);
            player.SetTeamColor(new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 1));
            player.SetPartyOwner(playerList.Count == 1);
            player.SetDisplayName("Player "+playerList.Count.ToString());
        }

        public override void OnServerSceneChanged(string sceneName)
        {
            if (SceneManager.GetActiveScene().name.StartsWith("MapScene"))
            {
                GameOverHandler gameOverHandlerTemp = Instantiate(gameOverHandlerPrefab);
                NetworkServer.Spawn(gameOverHandlerTemp.gameObject);

                foreach (var player in playerList)
                {
                    var playerBase = Instantiate(basePrefab,GetStartPosition().position,Quaternion.identity);
                    NetworkServer.Spawn(playerBase,player.connectionToClient);
                }
            }
        }
        #endregion

        #region Client
        public override void OnClientConnect()
        {
            base.OnClientConnect();
            ClientOnConnetted?.Invoke();
        }

        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
            ClientOnDisconnetted?.Invoke();
        }

        public override void OnStartClient()
        {
            playerList.Clear();
        }

        #endregion
    }
}


