using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace RTSOnlineGame
{
    public class Minimap : MonoBehaviour,IPointerDownHandler,IDragHandler
    {
        [SerializeField] private RectTransform minimapRectTranform;
        [SerializeField] private float mapScale=100f;
        [SerializeField] private float offset = -5f;

        private Transform playerCamTranform;

        private void Update()
        {
            if (playerCamTranform != null) return;

            if(NetworkClient.connection.identity==null) return;

            
        }
        private void MoveCamera()
        {
            Vector2 mousePos=Mouse.current.position.ReadValue();
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(minimapRectTranform, mousePos, null, out Vector2 localPoint)) return;

            Vector2 lerp=new Vector2((localPoint.x-minimapRectTranform.rect.x)/minimapRectTranform.rect.width, (localPoint.y - minimapRectTranform.rect.y) / minimapRectTranform.rect.height);
            Vector3 newCamPos = new Vector3(Mathf.Lerp(-mapScale, mapScale, lerp.x), playerCamTranform.position.y, Mathf.Lerp(-mapScale, mapScale, lerp.y));
            playerCamTranform.position = newCamPos+new Vector3(0,0,offset);

        }

        public void OnPointerDown(PointerEventData eventData)
        {
            MoveCamera();
        }

        public void OnDrag(PointerEventData eventData)
        {
            MoveCamera();
        }
        private void Start()
        {
            playerCamTranform = NetworkClient.connection.identity.GetComponent<RTSPlayer>().GetCameraTranform();
        }
    }
}
