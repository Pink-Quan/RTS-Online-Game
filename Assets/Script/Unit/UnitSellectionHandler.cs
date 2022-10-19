using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RTSOnlineGame
{
    public class UnitSellectionHandler : MonoBehaviour
    {
        [SerializeField] private RectTransform unitSelectionArea_RectTranform;
        private RTSPlayer player;

        private Vector2 startPos;

        [SerializeField] private LayerMask unit_LayerMask;
        private Camera mainCamera;
        public List<Unit> sellectedUnits_List { get; private set; } = new List<Unit>();
        private void Start()
        {
            mainCamera = Camera.main;

            Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitSpawn;
            GameOverHandler.clientOnGameOver += ClientOnGameover;
            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();

        }

        private void OnDestroy()
        {
            Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitSpawn;
            GameOverHandler.clientOnGameOver -= ClientOnGameover;
        }


        private void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                DesellectAllUnit();
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                ClearAllSellectionArea();
            }
            else if (Mouse.current.leftButton.isPressed)
            {
                UpdateSelectionArea();
            }

        }

        private void UpdateSelectionArea()
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            float areaWidth = (mousePos.x - startPos.x);
            float areaHeight = (mousePos.y - startPos.y);

            unitSelectionArea_RectTranform.sizeDelta = new Vector2(Mathf.Abs(areaWidth), Mathf.Abs(areaHeight));
            unitSelectionArea_RectTranform.anchoredPosition = startPos + new Vector2(areaWidth, areaHeight) / 2;
        }

        private void DesellectAllUnit()
        {
            if (!Keyboard.current.leftShiftKey.isPressed)
            {
                foreach (Unit u in sellectedUnits_List)
                {
                    u.DesellectUnit();
                }
                sellectedUnits_List.Clear();
            }
            unitSelectionArea_RectTranform.gameObject.SetActive(true);
            startPos = Mouse.current.position.ReadValue();

            UpdateSelectionArea();
        }

        private void ClearAllSellectionArea()
        {
            unitSelectionArea_RectTranform.gameObject.SetActive(false);

            if (unitSelectionArea_RectTranform.sizeDelta.magnitude == 0)
            {
                Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
                if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, unit_LayerMask)) return;
                if (!hit.collider.TryGetComponent(out Unit unit)) return;
                if (!unit.hasAuthority) return;

                sellectedUnits_List.Add(unit);
                foreach (Unit u in sellectedUnits_List)
                {
                    u.SellectUnit();
                }
                return;
            }

            Vector2 min = unitSelectionArea_RectTranform.anchoredPosition - unitSelectionArea_RectTranform.sizeDelta / 2;
            Vector2 max = unitSelectionArea_RectTranform.anchoredPosition + unitSelectionArea_RectTranform.sizeDelta / 2;

            foreach (Unit u in player.GetMyUnitList())
            {
                if (sellectedUnits_List.Contains(u))
                    continue;

                Vector3 screenPos = mainCamera.WorldToScreenPoint(u.transform.position);

                if (screenPos.x < max.x && screenPos.x > min.x && screenPos.y > min.y && screenPos.y < max.y)
                {
                    sellectedUnits_List.Add(u);
                    u.SellectUnit();
                }
            }
        }

        private void AuthorityHandleUnitSpawn(Unit obj)
        {
            sellectedUnits_List.Remove(obj);
        }


        private void ClientOnGameover(string obj)
        {
            enabled = false;
        }
    }
}

