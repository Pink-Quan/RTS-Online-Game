using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTSOnlineGame
{
    public class TeamColorSetter : NetworkBehaviour
    {
        [SerializeField] private Renderer[] colorRenderer;
        [SyncVar(hook =nameof(HandleTeamColorUpdated))] private Color teamColor;


        #region Server

        public override void OnStartServer()
        {
            RTSPlayer player = connectionToClient.identity.GetComponent<RTSPlayer>();

            teamColor = player.GetTeamColor();
        }

        #endregion

        #region Client

        private void HandleTeamColorUpdated(Color oldValue,Color newValue)
        {
            foreach (var renderer in colorRenderer)
            {
                renderer.material.SetColor("_BaseColor",newValue);
            }    
        }    

        #endregion
    }

}