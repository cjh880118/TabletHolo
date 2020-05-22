using CellBig.UI.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CellBig.UI
{
    public class TabletCharacterDialog : IDialog
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