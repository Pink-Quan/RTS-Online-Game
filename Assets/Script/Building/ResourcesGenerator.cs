using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTSOnlineGame
{
    public class ResourcesGenerator : NetworkBehaviour
    {
        [SerializeField] private Health health;
        [SerializeField] private int resourcesPerInterval = 10;
        [SerializeField] private float interval = 2f;

        private float timer;
        private RTSPlayer player;

        public override void OnStartServer()
        {
            timer = interval;
            player = connectionToClient.identity.GetComponent<RTSPlayer>();
            health.serverOnDie += ServerHandleDie;
            GameOverHandler.serverOnGameOver += ServerHandleGameover;
        }

        public override void OnStopServer()
        {
            health.serverOnDie -= ServerHandleDie;
            GameOverHandler.serverOnGameOver -= ServerHandleGameover;
        }

        private void ServerHandleDie()
        {
            NetworkServer.Destroy(gameObject);
        }

        private void ServerHandleGameover()
        {
            enabled = false;
        }

        [ServerCallback]
        private void Update()
        {
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                player.SetResources(player.GetResources()+resourcesPerInterval);
                timer = interval;
            }
        }
    }

}