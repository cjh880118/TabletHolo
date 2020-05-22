using CellBig.Constants;
using CellBig.UI.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CellBig.UI
{
    public class StoreIdolDialog : IDialog
    {
        public Button bntClose;
        public Button bntBackGround;
        protected override void OnLoad()
        {
            bntClose.onClick.AddListener(() => Message.Send<CloseStoreDialogMsg>(new CloseStoreDialogMsg(StoreMenu.IdolShop)));
            bntBackGround.onClick.AddListener(() => Message.Send<CloseStoreDialogMsg>(new CloseStoreDialogMsg(StoreMenu.IdolShop)));
        }

        protected override void OnEnter()
        {
            AddMessage();
        }

        private void AddMessage()
        {
            Message.AddListener<SetCharacterStoreMsg>(SetCharacterStore);
        }

        private void SetCharacterStore(SetCharacterStoreMsg msg)
        {

        }

        protected override void OnExit()
        {
            RemoveMessage();
        }

        private void RemoveMessage()
        {
            Message.RemoveListener<SetCharacterStoreMsg>(SetCharacterStore);
        }
    }
}