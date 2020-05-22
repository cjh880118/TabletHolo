using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig.Models;
using System;
using CellBig.UI.Event;

namespace CellBig.Contents
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