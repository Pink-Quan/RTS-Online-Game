using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

namespace RTSOnlineGame
{
    public class Targeter : NetworkBehaviour
    {
        private Targetable target;
        public Targetable GetTarget() => target;

        #region Server

        public override void OnStartServer()
        {
            GameOverHandler.serverOnGameOver += ServerHandleGameover;
        }

        public override void OnStopServer()
        {
            GameOverHandler.serverOnGameOver -= ServerHandleGameover;
        }

        [Server]
        private void ServerHandleGameover()
        {
            ClearTarget();
        }

        [Command]
        public void CmdSetTarget(GameObject targetGameObject)
        {
            if (!targetGameObject.TryGetComponent(out Targetable target)) return;
            this.target = target;

        }

        [Server]
        public void ClearTarget()
        {
            target = null;
        }

        #endregion

        #region Client



        #endregion
    }

}
