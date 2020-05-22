using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JHchoi.UI.Event;
using JHchoi.Constants;

namespace JHchoi.UI
{
    public class StoreSkinDialog : IDialog
    {
        public Button bntClose;
        public Button bntBackGround;

        protected override void OnLoad()
        {
            bntClose.onClick.AddListener(() => Message.Send<CloseStoreDialogMsg>(new CloseStoreDialogMsg(StoreMenu.SkinShop)));
            bntBackGround.onClick.AddListener(() => Message.Send<CloseStoreDialogMsg>(new CloseStoreDialogMsg(StoreMenu.SkinShop)));
        }

        protected override void OnEnter()
        {
            AddMessage();
        }

        private void AddMessage()
        {
            Message.AddListener<SetSkinStoreMsg>(SetSkinStore);
        }

        private void SetSkinStore(SetSkinStoreMsg msg)
        {

        }

        protected override void OnExit()
        {
            RemoveMessage();
        }

        private void RemoveMessage()
        {
            Message.RemoveListener<SetSkinStoreMsg>(SetSkinStore);
        }
    }
}
