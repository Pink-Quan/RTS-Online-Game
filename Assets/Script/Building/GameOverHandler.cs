using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTSOnlineGame
{
    public class GameOverHandler : NetworkBehaviour
    {
        private List<UnitBase> unitBases = new List<UnitBase>();
        public static event Action<string> clientOnGameOver;
        public static event Action serverOnGameOver;

        #region Server

        public override void OnStartServer()
        {
            Debug.Log("OnStarServer");
            UnitBase.serverOnBaseSpawn += ServerHandleBaseSpawned;
            UnitBase.serverOnBaseDespawn += ServerHandleBaseDespawned; 
        }

        public override void OnStopServer()
        {
            UnitBase.serverOnBaseSpawn -= ServerHandleBaseSpawned;
            UnitBase.serverOnBaseDespawn -= ServerHandleBaseDespawned;
        }

        [Server]
        private void ServerHandleBaseSpawned(UnitBase unitBase)
        {
            unitBases.Add(unitBase);
        }

        [Server]
        private void ServerHandleBaseDespawned(UnitBase unitBase)
        {
            unitBases.Remove(unitBase);

            if (unitBases.Count != 1) return;

            Debug.Log("Gameover");

            string winnerID = unitBases[0].connectionToClient.connectionId.ToString();

            RPCGameover($"Player {winnerID}");

            serverOnGameOver?.Invoke();
            
        }

        #endregion

        #region Clients

        [ClientRpc]
        private void RPCGameover(string winner)
        {
            clientOnGameOver?.Invoke(winner);
        }

        #endregion
    }

}
