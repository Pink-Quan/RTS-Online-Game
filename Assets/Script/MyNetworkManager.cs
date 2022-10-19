using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerTesting
{
    public class MyNetworkManager : NetworkManager
    {
        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);

            MyNetworkPlayer player = conn.identity.GetComponent<MyNetworkPlayer>();

            player.SetDisplayName($"Player {numPlayers}");
            player.SetColor(new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1));
        }
    }
}

