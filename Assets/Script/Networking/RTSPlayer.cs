using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTSOnlineGame
{
    public class RTSPlayer : NetworkBehaviour
    {
        [SerializeField] private Transform cameraTranform;
        [SerializeField] private Building[] buildings;
        [SerializeField] private LayerMask buildingBlockLayer;
        [SerializeField] private float buildingRangeLimit =5f;

        [SyncVar(hook=nameof(ClientHandleResourcesUpdate))]
        private int resources = 500;
        [SyncVar(hook =nameof(AuthorityPartyOwnerStateUpdate))]
        private bool isPartyOwner=false;
        [SyncVar(hook =nameof(ClientDisplayNameUpdated))]
        private string displayName;

        private List<Unit> myUnit_List = new List<Unit>();
        private List<Building> myBuilding_List = new List<Building>();
        private Color teamColor;

        public List<Unit> GetMyUnitList() => myUnit_List;
        public List<Building> GetMyBuildingList() => myBuilding_List;

        public int GetResources() => resources;

        public Color GetTeamColor() => teamColor;
        public Transform GetCameraTranform() => cameraTranform;
        public bool GetIsPartyOwner() => isPartyOwner;
        public string GetDisplayName() => displayName;

        public        event Action<int>  ClientOnResourcesUpdate;
        public static event Action<bool> AuthorityOnPartyOwnerStateUpdate;
        public static event Action       ClientOnInforUpdate;

        public bool CanPlanceBuilding(BoxCollider buildingColider,Vector3 spawnPos)
        {
            if (Physics.CheckBox(spawnPos + buildingColider.center, buildingColider.size / 2, Quaternion.identity, buildingBlockLayer)) return false;

            foreach(Building m_building in myBuilding_List)
            {
                if((spawnPos-m_building.transform.position).sqrMagnitude <= buildingRangeLimit*buildingRangeLimit)
                    return true;
            }

            return false;
        }

        #region Server

        [Server]
        public void SetTeamColor(Color color)
        {
            teamColor = color;
        }

        public override void OnStartServer()
        {
            Unit.ServerOnUnitSpawned += ServerHandleUnitSpawner;
            Unit.ServerOnUnitDespawned += ServerHandleUnitDespawner;

            Building.serverOnBuildingSpawned += ServerHandleBuildingSpawn;
            Building.serverOnBuildingDespawned += ServerHandleBuildingDespawn;

            DontDestroyOnLoad(gameObject);
        }

        public override void OnStopServer()
        {
            Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawner;
            Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawner;

            Building.serverOnBuildingSpawned -= ServerHandleBuildingSpawn;
            Building.serverOnBuildingDespawned -= ServerHandleBuildingDespawn;
        }

        private void ServerHandleBuildingDespawn(Building obj)
        {
            if (obj.connectionToClient.connectionId != connectionToClient.connectionId) return;
            myBuilding_List.Remove(obj);
        }

        private void ServerHandleBuildingSpawn(Building obj)
        {
            if (obj.connectionToClient.connectionId != connectionToClient.connectionId) return;
            myBuilding_List.Add(obj);
        }

        private void ServerHandleUnitSpawner(Unit unit)
        {
            if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;
            myUnit_List.Add(unit);
        }

        private void ServerHandleUnitDespawner(Unit unit)
        {
            if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;
            myUnit_List.Remove(unit);
        }
        [Command]
        public void CmdStartGame()
        {
            if(!isPartyOwner) return;
            ((RTSNetworkManager)NetworkManager.singleton).StarGame();
        }

        [Command]
        public void CmdTryPlaceBuilding(int buildingId, Vector3 spawnPos)
        {
            Building buildingToPlace = null;
            for (int i = 0; i < buildings.Length; i++)
                if (buildingId == buildings[i].GetId())
                {
                    buildingToPlace = buildings[i];
                    break;
                }    

            if (buildingToPlace == null) return;    

            if (resources < buildingToPlace.GetPrice()) return;

            BoxCollider buildingColider=buildingToPlace.GetComponent<BoxCollider>();
            

            if(!CanPlanceBuilding(buildingColider,spawnPos)) return;

            Building buildingInstance = Instantiate(buildingToPlace, spawnPos,buildingToPlace.transform.rotation);
            NetworkServer.Spawn(buildingInstance.gameObject, connectionToClient);

            SetResources(resources - buildingToPlace.GetPrice());

        }

        [Server]
        public void SetResources(int newResources)
        {
            resources = newResources;
        }

        [Server]
        public void SetPartyOwner(bool state)
        {
            isPartyOwner = state;
        }

        [Server]
        public void SetDisplayName(string name)
        {
            displayName = name;
        }    

        #endregion

        #region Client

        private void AuthorityPartyOwnerStateUpdate(bool oldVaule,bool newValue)
        {
            if(!hasAuthority) return;
            AuthorityOnPartyOwnerStateUpdate?.Invoke(newValue);
        }

        public override void OnStartAuthority()
        {
            if (NetworkServer.active) return;

            Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawner;
            Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawner;

            Building.authorityOnBuildingSpawned += AuthorityHandleBuildingSpawned;
            Building.authorityOnBuildingDespawned += AuthorityHandleBuildingDespawned;
        }

        public override void OnStartClient()
        {
            if (NetworkServer.active) return;
            DontDestroyOnLoad(gameObject);
            ((RTSNetworkManager)NetworkManager.singleton).playerList.Add(this);
        }

        public override void OnStopClient()
        {
            ClientOnInforUpdate?.Invoke();
            if (!hasAuthority || !isClientOnly) return;

            ((RTSNetworkManager)NetworkManager.singleton).playerList.Remove(this);

            if (!hasAuthority) return;

            Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawner;
            Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawner;

            Building.authorityOnBuildingSpawned -= AuthorityHandleBuildingSpawned;
            Building.authorityOnBuildingDespawned -= AuthorityHandleBuildingDespawned;
        }

        private void AuthorityHandleUnitSpawner(Unit unit)
        {
            myUnit_List.Add(unit);
        }
        private void AuthorityHandleUnitDespawner(Unit unit)
        {
            myUnit_List.Remove(unit);
        }

        private void AuthorityHandleBuildingSpawned(Building obj)
        {
            myBuilding_List.Add(obj);
        }
        private void AuthorityHandleBuildingDespawned(Building obj)
        {
            myBuilding_List.Remove(obj);
        }

        private void ClientHandleResourcesUpdate(int oldValue,int newValue)
        {
            ClientOnResourcesUpdate?.Invoke(newValue);
        }
        private void ClientDisplayNameUpdated(string oldValue, string newValue)
        {
            ClientOnInforUpdate?.Invoke();
        }

        #endregion

    }
}

