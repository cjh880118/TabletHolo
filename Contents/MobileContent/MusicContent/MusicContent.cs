using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CellBig.Contents
{
    public class MusicContent : IContent
    {
        static string TAG = "MusicContent :: ";

        protected override void OnEnter()
        {
            Debug.Log(TAG + "OnEnter");
            IContent.RequestContentExit<MenuContent>();
            UI.IDialog.RequestDialogEnter<UI.MusicDialog>();
            AddMessage();
        }

        private void AddMessage()
        {
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                IContent.RequestContentEnter<MenuContent>();
            }
        }

        protected override void OnExit()
        {
            DialogAllClose();
            RemoveMessage();
        }

        private void RemoveMessage()
        {
        }




    }
}