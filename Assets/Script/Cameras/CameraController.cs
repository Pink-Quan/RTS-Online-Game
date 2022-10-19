using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RTSOnlineGame
{
    public class CameraController : NetworkBehaviour
    {
        [SerializeField] private Transform playerCameraTranform;
        [SerializeField] private float speed = 20f;
        [SerializeField] private float screenBorderThickness = 10f;

        [SerializeField] private Vector2 screenXLimits;
        [SerializeField] private Vector2 screenZLimits;

        private Vector2 previousInput;

        private Controls controls;

        public override void OnStartAuthority()
        {
            playerCameraTranform.gameObject.SetActive(true);

            controls=new Controls();

            controls.Player.MoveCamera.performed += SetPreviousInput;
            controls.Player.MoveCamera.canceled += SetPreviousInput;
            controls.Enable();
        }

        private void SetPreviousInput(InputAction.CallbackContext ctx)
        {
            previousInput=ctx.ReadValue<Vector2>();
        }

        [ClientCallback]
        private void Update()
        {
            if (!hasAuthority||!Application.isFocused) return;

            UpdateCamraPosion();
        }

        private void UpdateCamraPosion()
        {
            Vector3 pos = playerCameraTranform.position;

            if(previousInput==Vector2.zero)
            {
                Vector3 cursorMoverment = Vector3.zero;
                Vector2 cursorPosion = Mouse.current.position.ReadValue();

                if(cursorPosion.y>=Screen.height-screenBorderThickness)
                {
                    cursorMoverment.z += 1;
                }
                else if(cursorPosion.y<=screenBorderThickness)
                {
                    cursorMoverment.z -= 1;
                }

                if (cursorPosion.x >= Screen.width - screenBorderThickness)
                {
                    cursorMoverment.x += 1;
                }
                else if (cursorPosion.x <= screenBorderThickness)
                {
                    cursorMoverment.x -= 1;
                }

                pos+=cursorMoverment.normalized*speed*Time.deltaTime;
            }
            else
            {
                pos+=new Vector3(previousInput.x, 0,previousInput.y)*speed*Time.deltaTime;
            }

            pos.x = Mathf.Clamp(pos.x, screenXLimits.x, screenXLimits.y);
            pos.z = Mathf.Clamp(pos.z, screenZLimits.x, screenZLimits.y);

            playerCameraTranform.position = pos;    

        }
    }
}