using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig.Module;
using CellBig.Models;
using LookingGlass;
using CellBig.UI.Event;
using System;

namespace CellBig.Module
{
    public class LookingGlassModule : IModule
    {
        private Holoplay holoplay;
        float holoplaySize;
        Vector3 vec3Holoplay;
        private void Awake()
        {
            if (holoplay == null)
                holoplay = GetComponentInChildren<Holoplay>();
        }

        protected override void OnLoadStart()
        {
            if (holoplay == null)
                holoplay = GetComponentInChildren<Holoplay>();

            holoplaySize = holoplay.size;
            vec3Holoplay = holoplay.transform.localPosition;

            var sm = Model.First<SettingModel>();
            if (!sm.UseLookingGless)
                holoplay.gameObject.SetActive(false);

            SetResourceLoadComplete();

        }

        protected override void OnLoadComplete()
        {
            AddMessag();
        }

        void AddMessag()
        {
            Message.AddListener<CameraZoomMsg>(CameraZoom);
        }

        private void CameraZoom(CameraZoomMsg msg)
        {
            if (msg.isZoom)
            {
                holoplay.size = 0.2f;
                holoplay.transform.localPosition = new Vector3(0, 0.7f, -1);
            }
            else
            {
                holoplay.size = holoplaySize;
                holoplay.transform.localPosition = vec3Holoplay;
            }

            Debug.Log(msg.isZoom);
        }

        protected override void OnUnload()
        {
            RemoveMessage();
        }

        private void RemoveMessage()
        {
            Message.RemoveListener<CameraZoomMsg>(CameraZoom);
        }

        public RaycastHit[] RaycastToPosition(Vector3 pos)
        {
            Ray ray = holoplay.cam.ViewportPointToRay(holoplay.cam.WorldToViewportPoint(pos));
            Debug.DrawLine(ray.origin, ray.direction * 1000);
            
            return Physics.RaycastAll(ray);
            
        }
    }
}