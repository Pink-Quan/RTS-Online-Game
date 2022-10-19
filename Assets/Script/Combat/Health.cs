using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RTSOnlineGame
{
    public class Health : NetworkBehaviour
    {
        [SerializeField] private int maxHeath = 100;

        [SyncVar(hook =nameof(HandleHealthUpdated))]
        private int currentHeath;

        public event Action serverOnDie;
        public event Action<int, int> clientOnHeathUpdated;

        #region Server
        public override void OnStartServer()
        {
            currentHeath = maxHeath;
            UnitBase.serverOnPlayerDie += ServerHandlePlayerDie;
        }

        public override void OnStopServer()
        {
            UnitBase.serverOnPlayerDie -= ServerHandlePlayerDie;
        }

        [Server]
        private void ServerHandlePlayerDie(int conId)
        {
            if(conId==connectionToClient.connectionId)
            {
                DealDamge(currentHeath);
            }
        }

        [Server]
        public void DealDamge(int damgeDeal)
        {
            if (currentHeath <= 0) return;
            currentHeath = Mathf.Max(currentHeath- damgeDeal, 0);
            if (currentHeath != 0) return;

            serverOnDie?.Invoke();
        }   
        
        #endregion

        #region Client

        private void HandleHealthUpdated(int oldHealth,int newHealth)
        {
            clientOnHeathUpdated?.Invoke(newHealth,maxHeath);
        }

        #endregion
    }

}
