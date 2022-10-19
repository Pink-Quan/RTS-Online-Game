using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTSOnlineGame
{
    public class Building : NetworkBehaviour
    {
        [SerializeField] private GameObject buildingPreview;
        [SerializeField] private Sprite icon;
        [SerializeField] private int id = -1;
        [SerializeField] private int price = 100;

        public static event Action<Building> serverOnBuildingSpawned;
        public static event Action<Building> serverOnBuildingDespawned;

        public static event Action<Building> authorityOnBuildingSpawned;
        public static event Action<Building> authorityOnBuildingDespawned;

        public Sprite GetIcon() => icon;
        public int GetId() => id;
        public int GetPrice() => price;
        public GameObject GetBuildingPreview() => buildingPreview;

        #region Server

        public override void OnStartServer()
        {
            serverOnBuildingSpawned?.Invoke(this);
        }

        public override void OnStopServer()
        {
            serverOnBuildingDespawned?.Invoke(this);
        }

        #endregion

        #region Client

        public override void OnStartClient()
        {
            authorityOnBuildingSpawned?.Invoke(this);
        }

        public override void OnStopClient()
        {
            authorityOnBuildingDespawned?.Invoke(this);
        }

        #endregion
    }
}
