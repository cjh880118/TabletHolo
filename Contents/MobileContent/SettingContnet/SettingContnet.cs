using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CellBig.Contents
{
    public class SettingContnet : IContent
    {
        static string TAG = "AloneGameContent :: ";

        protected override void OnEnter()
        {
            Debug.Log(TAG + "OnEnter");
            AddMessage();
            IContent.RequestContentExit<MenuContent>();

            UI.IDialog.RequestDialogEnter<UI.SettingMainDialog>();
        }

        private void AddMessage()
        {
           
        }

        protected override void OnExit()
        {
            DialogAllClose();
        }
    }
}