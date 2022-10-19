using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTSOnlineGame
{
    public class UnitProjectile : NetworkBehaviour
    {
        [SerializeField] private Rigidbody thisRigidbody;
        [SerializeField] private float launchForce = 20f;
        [SerializeField] private float despawnTime = 5f;
        [SerializeField] private int damge = 20;

        private void Start()
        {
            thisRigidbody.velocity = transform.forward * launchForce;
        }

        public override void OnStartServer()
        {
            Invoke("DestroySelf", despawnTime);
        }

        private void DestroySelf()
        {
            NetworkServer.Destroy(gameObject);
        }

        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out NetworkIdentity you))
            {
                if (you.connectionToClient == connectionToClient) return;
            }

            if(other.TryGetComponent(out Health enemyHealth))
            {
                enemyHealth.DealDamge(damge);
                DestroySelf();
            }

        }
    }
}

