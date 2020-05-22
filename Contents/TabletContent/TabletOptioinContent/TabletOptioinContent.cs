using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JHchoi.Models;
using System;
using JHchoi.UI.Event;

namespace JHchoi.Contents
{
    public class TabletOptioinContent : IContent
    {
        HolostarSettingModel holostarSettingModel;

        protected override void OnLoadComplete()
        {
            holostarSettingModel = Model.First<HolostarSettingModel>();
            SetLoadComplete();
        }

        protected override void OnEnter()
        {
            AddMessage();
        }

        private void AddMessage()
        {
            
        }

        private void TabletSound(VolumeChangeMsg msg)
        {
            
        }

        protected override void OnExit()
        {
            RemoveMessage();
        }

        private void RemoveMessage()
        {
            
        }
    }
}