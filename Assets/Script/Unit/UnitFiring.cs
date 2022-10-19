using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTSOnlineGame
{
    public class UnitFiring : NetworkBehaviour
    {
        [SerializeField] private Targeter targeter;
        [SerializeField] private GameObject projectile_Prefab;
        [SerializeField] private Transform projectileSpawnPoint_Tranform;
        [SerializeField] private float fireRange_float = 10f;
        [SerializeField] private float fireRate_float = 1f;
        [SerializeField] private float rotationSpeed_float = 20f;

        private float lastFireTime_float;

        [ServerCallback]
        private void Update()
        {
            if(targeter.GetTarget()==null) return;
            if (!CanFireTarget()) return;

            Quaternion targetQuaternion = Quaternion.LookRotation(targeter.GetTarget().transform.position-transform.position);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetQuaternion, rotationSpeed_float * Time.deltaTime);

            if(Time.time > (1/fireRate_float)+lastFireTime_float)
            {
                Quaternion projectileRotation = Quaternion.LookRotation(targeter.GetTarget().GetAimAtPoint().position-projectileSpawnPoint_Tranform.position);
                GameObject tempProjectile_Prefab = Instantiate(projectile_Prefab, projectileSpawnPoint_Tranform.position,projectileRotation);

                NetworkServer.Spawn(tempProjectile_Prefab,connectionToClient);

                lastFireTime_float = Time.time;
            }
        }

        [Server]
        private bool CanFireTarget()
        {
            return (targeter.GetTarget().transform.position - transform.position).sqrMagnitude < fireRange_float * fireRange_float;
        }    
    }

}
