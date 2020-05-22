using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig.Module;
using CellBig.Models;
using Leap.Unity;
using CellBig.Constants;

namespace CellBig.Module
{
    public class LeapMotionModule : IModule
    {
        private LeapServiceProvider leapService;
        private HandModelManager handModelManager;

        private void Awake()
        {
            if (leapService == null)
                leapService = GetComponentInChildren<LeapServiceProvider>();
            if (handModelManager == null)
                handModelManager = GetComponentInChildren<HandModelManager>();

        }
        protected override void OnLoadStart()
        {
            if (leapService == null)
                leapService = GetComponentInChildren<LeapServiceProvider>();
            if (handModelManager == null)
                handModelManager = GetComponentInChildren<HandModelManager>();

            var sm = Model.First<SettingModel>();
            if (!sm.UseLeapMotion)
            {
                leapService.gameObject.SetActive(false);
                handModelManager.gameObject.SetActive(false);
            }
            SetResourceLoadComplete();
        }
        protected override void OnUnload()
        {
        }

        public void OnModule()
        {
            leapService.gameObject.SetActive(true);
            handModelManager.gameObject.SetActive(true);
        }
        public void OffModule()
        {
            leapService.gameObject.SetActive(false);
            handModelManager.gameObject.SetActive(false);
        }

        public void LeapMotionPosition(Menu menu)
        {
            //if (menu == Menu.HoloStar)
            //{
            //    this.gameObject.transform.position = new Vector3(0, 1.3f, 5.5f);
            //    this.gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f) * 0.2f;
            //}
            //else
            //{
            //    this.gameObject.transform.position = new Vector3(0, 0, 5.5f);
            //    this.gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            //}
        }
    }
}