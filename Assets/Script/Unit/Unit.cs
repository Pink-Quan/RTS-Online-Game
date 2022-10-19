using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RTSOnlineGame
{
    public class Unit : NetworkBehaviour
    {
        [SerializeField] private UnityEvent onSellect_UnityEvent;
        [SerializeField] private UnityEvent onDesellect_UnityEvent;
        [SerializeField] private Targeter targeter;
        [SerializeField] private UnitMoverment unitMoverment;
        [SerializeField] private Health health;
        [SerializeField] private int resourcesCost = 10;

        public static event Action<Unit> ServerOnUnitSpawned;
        public static event Action<Unit> ServerOnUnitDespawned;

        public static event Action<Unit> AuthorityOnUnitSpawned;
        public static event Action<Unit> AuthorityOnUnitDespawned;

        public Targeter GetTargeter() => targeter;
        public UnitMoverment GetUnitMove() => unitMoverment;
        public int GetResourceCost() => resourcesCost;

        #region Server

        public override void OnStartServer()
        {
            ServerOnUnitSpawned?.Invoke(this);
            health.serverOnDie += ServerHandleDie;
        }

        public override void OnStopServer()
        {
            ServerOnUnitDespawned?.Invoke(this);
            health.serverOnDie -= ServerHandleDie;
        }

        [Server]
        private void ServerHandleDie()
        {
            NetworkServer.Destroy(gameObject);
        }

        #endregion

        #region Client

        [Client]
        public void SellectUnit()
        {
            if (!hasAuthority) return;

            onSellect_UnityEvent?.Invoke();
        }

        [Client]
        public void DesellectUnit()
        {
            if (!hasAuthority) return;

            onDesellect_UnityEvent?.Invoke();
        }
        public override void OnStartAuthority()
        {
            AuthorityOnUnitSpawned?.Invoke(this);
        }

        public override void OnStopClient()
        {
            if (!hasAuthority) return;
            AuthorityOnUnitDespawned?.Invoke(this);
        }

        #endregion
    }
}

