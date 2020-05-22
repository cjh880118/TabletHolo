using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JHchoi.UI
{
    public class SettingMainDialog : IDialog
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
            RemoverMessage();
        }

        private void RemoverMessage()
        {
            
        }
    }
}