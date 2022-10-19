using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RTSOnlineGame
{
    public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
    {
        [SerializeField] private Health health;
        [SerializeField] private Unit unit_Prefab;
        [SerializeField] private Transform unitSpawnPoint_Transform;

        [SerializeField] private TMP_Text remaningUnitText;
        [SerializeField] private Image unitProgressImage;
        [SerializeField] private int maxUnitQueue = 5;
        [SerializeField] private float spawnMoveRange = 7;
        [SerializeField] private float unitSpawnDuration =5f;

        [SyncVar(hook =nameof(ClientHandleQueueUnitUpdated))]
        private int queueUnit=0;
        [SyncVar]
        private float unitTimer;
        private float progressImageVelocity;

        #region Server

        public override void OnStartServer()
        {
            health.serverOnDie += ServerHandleDie;
        }

        public override void OnStopServer()
        {
            health.serverOnDie -= ServerHandleDie;
        }

        [Server]
        private void ServerHandleDie()
        {
            NetworkServer.Destroy(gameObject);
        }

        [Command]
        private void CmdSpawnUnit()
        {
            if (queueUnit == maxUnitQueue) return;

            RTSPlayer player= connectionToClient.identity.GetComponent<RTSPlayer>();

            if (player.GetResources() < unit_Prefab.GetResourceCost()) return;

            queueUnit+=1;
            player.SetResources(player.GetResources()- unit_Prefab.GetResourceCost());
        }
        [Server]
        private void ProduceUnit()
        {
            if(queueUnit == 0)
                return;
            unitTimer+=Time.deltaTime;

            if (unitTimer < unitSpawnDuration) return;

            Unit tempUnit_Prefab = Instantiate(unit_Prefab, unitSpawnPoint_Transform.position, unitSpawnPoint_Transform.rotation);
            NetworkServer.Spawn(tempUnit_Prefab.gameObject, connectionToClient);

            Vector3 spawnOffset = UnityEngine.Random.insideUnitSphere * spawnMoveRange;
            spawnOffset.y = unitSpawnPoint_Transform.position.y;

            UnitMoverment unitMoverment = tempUnit_Prefab.GetComponent<UnitMoverment>();
            unitMoverment.ServerMove(spawnOffset+unitSpawnPoint_Transform.position);

            queueUnit--;
            unitTimer = 0;
        }

        #endregion

        private void Update()
        {
            if (isServer)
            {
                ProduceUnit();
            }
            if (isClient)
            {
                UpdateTimeDisplayer();
            }
        }

        #region Client
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;

            if (!hasAuthority) return;

            CmdSpawnUnit();
        }

        private void ClientHandleQueueUnitUpdated(int oldValue, int newValue)
        {
            remaningUnitText.text= newValue.ToString();
            Debug.Log(oldValue.ToString()+"  "+ newValue.ToString());
        }

        private void UpdateTimeDisplayer()
        {
            float newProgress = unitTimer / unitSpawnDuration;

            if(newProgress < unitProgressImage.fillAmount)
            {
                unitProgressImage.fillAmount=newProgress;
            }   
            else
            {
                unitProgressImage.fillAmount = Mathf.SmoothDamp(unitProgressImage.fillAmount,newProgress,ref progressImageVelocity,0.1f);
            }    
        }


        #endregion
    }
}

