using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using System;

namespace RTSOnlineGame
{
    public class ResourcesDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text resourcesText;

        private RTSPlayer player;

        private void Start()
        {
            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
            ClientHandleResourcesUpdated(player.GetResources());
            player.ClientOnResourcesUpdate += ClientHandleResourcesUpdated;
        }

        private void OnDestroy()
        {
            player.ClientOnResourcesUpdate -= ClientHandleResourcesUpdated;
        }

        private void ClientHandleResourcesUpdated(int obj)
        {
            resourcesText.text = $"Resources = {obj}";
        }
    }

}
