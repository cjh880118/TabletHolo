using JHchoi.UI.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JHchoi.UI
{
    public class TabletSingleDialog : IDialog
    {
        protected override void OnEnter()
        {
            AddMessage();
        }

        private void AddMessage()
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