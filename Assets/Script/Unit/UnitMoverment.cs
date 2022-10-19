using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;
using System;

namespace RTSOnlineGame
{
    public class UnitMoverment : NetworkBehaviour
    {
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Targeter targeter;
        [SerializeField] private float chaseRange = 10f;

        #region Server
        public override void OnStartServer()
        {
            GameOverHandler.serverOnGameOver += ServerHandleGameover;
        }

        private void ServerHandleGameover()
        {
            agent.ResetPath();
        }

        [Server]
        public override void OnStopServer()
        {
            GameOverHandler.serverOnGameOver -= ServerHandleGameover;
            GameOverHandler.serverOnGameOver -= ServerHandleGameover;
        }

        [ServerCallback]
        private void Update()
        {
            Targetable target = targeter.GetTarget();
            if (target != null)
            {
                if ((target.transform.position - transform.position).sqrMagnitude >= chaseRange * chaseRange)
                {
                    agent.SetDestination(target.transform.position);
                }
                else if (agent.hasPath)
                {
                    agent.ResetPath();
                }

                return;
            }

            if (!agent.hasPath) return;
            if (agent.remainingDistance > agent.stoppingDistance) return;
            agent.ResetPath();
        }

        [Command]
        public void CmdMove(Vector3 pos)
        {
            ServerMove(pos);
        }
        [Server]
        public void ServerMove(Vector3 pos)
        {
            targeter.ClearTarget();

            if (!NavMesh.SamplePosition(pos, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            {
                return;
            }

            agent.SetDestination(hit.position);
        }    

        #endregion

        #region Client

        #endregion
    }
}


