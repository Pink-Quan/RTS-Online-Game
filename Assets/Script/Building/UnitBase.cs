using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTSOnlineGame
{
    public class UnitBase : NetworkBehaviour
    {
        [SerializeField] private Health heath;
        public static event Action<UnitBase> serverOnBaseSpawn; 
        public static event Action<UnitBase> serverOnBaseDespawn;
        public static event Action<int> serverOnPlayerDie;

        #region Server

        public override void OnStartServer()
        {
            serverOnBaseSpawn?.Invoke(this);
            heath.serverOnDie += HandleServerDie;
        }

        public override void OnStopServer()
        {
            serverOnBaseDespawn?.Invoke(this);
            heath.serverOnDie -= HandleServerDie;
        }

        [Server]
        private void HandleServerDie()
        {
            serverOnPlayerDie?.Invoke(connectionToClient.connectionId);
            NetworkServer.Destroy(gameObject);
        }

        #endregion

        #region Client



        #endregion
    }

}