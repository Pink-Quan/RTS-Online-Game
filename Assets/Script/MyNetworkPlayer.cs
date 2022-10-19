using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

namespace MultiplayerTesting
{
    public class MyNetworkPlayer : NetworkBehaviour
    {
        [SerializeField] private TMP_Text playerText;
        [SerializeField] private Renderer playerRenderer;

        [SyncVar(hook = nameof(HandleDisplayName))] private string myPlayerName = "Missing Name";
        [SyncVar(hook = nameof(HandleDisplayColorUpdate))] private Color playerColor = Color.white;

        #region Server

        [Server]
        public void SetDisplayName(string name)
        {
            myPlayerName = name;
        }
        [Server]
        public void SetColor(Color color)
        {
            playerColor = color;
        }
        [Command]
        public void CmdSetDisplayName(string Name)
        {
            SetDisplayName(Name);
        }

        #endregion

        #region Client

        private void HandleDisplayColorUpdate(Color oldColor, Color newColor)
        {
            playerRenderer.material.color = newColor;
        }
        private void HandleDisplayName(string oldName, string newName)
        {
            playerText.text = newName;
        }

        [ContextMenu("SetMyName")]
        private void SetMyName()
        {
            CmdSetDisplayName("My Name");
        }

        #endregion
    }
}



