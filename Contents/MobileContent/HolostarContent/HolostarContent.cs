using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CellBig.Contents
{
    public class HolostarContent : IContent
    {
        static string TAG = "HolostarContent :: ";

        protected override void OnEnter()
        {
            Debug.Log(TAG + "OnEnter");
            AddMessage();
            IContent.RequestContentExit<MenuContent>();

            UI.IDialog.RequestDialogEnter<UI.HoloStarMainDialog>();
        }

        private void AddMessage()
        {
        }

        protected override void OnExit()
        {
            DialogAllClose();
            RemoveMessage();
        }

        private void RemoveMessage()
        {
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                IContent.RequestContentEnter<MenuContent>();
            }
        }
    }
}