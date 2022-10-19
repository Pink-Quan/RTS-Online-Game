using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTSOnlineGame
{
    public class Targetable : NetworkBehaviour
    {
        [SerializeField] private Transform aimAtPoint_Tranform;

        public Transform GetAimAtPoint() => aimAtPoint_Tranform;

    }

}
