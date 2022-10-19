using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RTSOnlineGame
{
    public class UnitCommandGiver : MonoBehaviour
    {
        [SerializeField] private UnitSellectionHandler unitSellectionHandler;
        [SerializeField] private LayerMask unit_LayerMask;
        private Camera mainCamera;

        private void Start()
        {
            mainCamera = Camera.main;
            GameOverHandler.clientOnGameOver += ClientHandleGameover;
        }
        private void OnDestroy()
        {
            GameOverHandler.clientOnGameOver -= ClientHandleGameover;
        }


        private void Update()
        {
            if (!Mouse.current.rightButton.wasPressedThisFrame) return;

            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, unit_LayerMask)) return;
            if (hit.collider.TryGetComponent(out Targetable target))
            {
                if (target.hasAuthority)
                {
                    TryMoveUnit(hit.point);
                    return;
                }
                else
                {
                    TryTarget(target);
                    return;
                }
            }
            TryMoveUnit(hit.point);
        }

        private void TryTarget(Targetable target)
        {
            foreach (var i in unitSellectionHandler.sellectedUnits_List)
            {
                i.GetTargeter().CmdSetTarget(target.gameObject);
            }
        }

        private void TryMoveUnit(Vector3 destination_Vector3)
        {
            foreach (var i in unitSellectionHandler.sellectedUnits_List)
            {
                i.GetUnitMove().CmdMove(destination_Vector3);
            }
        }

        private void ClientHandleGameover(string obj)
        {
             enabled = false;
        }
    }
}
