using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace RTSOnlineGame
{
    public class BuildingButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private Building building;
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text priceText;
        [SerializeField] private LayerMask flourLayerMask;

        private Camera mainCamera;
        private RTSPlayer player;
        private GameObject buildingPreviewInstance;
        private Renderer buildingRendererInstance;
        private BoxCollider buildingColider;

        private void Start()
        {
            mainCamera = Camera.main;

            iconImage.sprite = building.GetIcon();
            priceText.text= building.GetPrice().ToString();

            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();

            buildingColider = building.GetComponent<BoxCollider>();
        }

        private void Update()
        {

            if (buildingPreviewInstance == null) return;

            UpdateBuildingPreview();

        }


        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;

            if (player.GetResources() < building.GetPrice()) return;

            buildingPreviewInstance = Instantiate(building.GetBuildingPreview());
            buildingRendererInstance = buildingPreviewInstance.transform.GetChild(0).GetComponent<Renderer>();
            buildingPreviewInstance.SetActive(false);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (buildingPreviewInstance == null) return;

            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if(Physics.Raycast(ray,out RaycastHit hit,Mathf.Infinity,flourLayerMask))
            {
                player.CmdTryPlaceBuilding(building.GetId(),hit.point);
            }

            Destroy(buildingPreviewInstance);
        }

        private void UpdateBuildingPreview()
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, flourLayerMask)) return;

            buildingPreviewInstance.transform.position = hit.point;

            if(!buildingPreviewInstance.activeSelf)
            {
                buildingPreviewInstance.SetActive(true);
            }

            Color color = player.CanPlanceBuilding(buildingColider, hit.point) ? Color.green : Color.red;
            
            buildingRendererInstance.material.SetColor("_BaseColor", color);
        }
    }

}