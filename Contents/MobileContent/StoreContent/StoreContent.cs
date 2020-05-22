using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JHchoi.UI.Event;
using JHchoi.Constants;
using System;

namespace JHchoi.Contents
{
    public class StoreContent : IContent
    {

        static string TAG = "StoreContent :: ";

        Cash_Table cash_Table;
        Character_Table character_Table;
        Skin_Table skin_Table;

        protected override void OnLoadStart()
        {
            cash_Table = TableManager.Instance.GetTableClass<Cash_Table>();
            character_Table = TableManager.Instance.GetTableClass<Character_Table>();
            skin_Table = TableManager.Instance.GetTableClass<Skin_Table>();
            SetLoadComplete();
        }

        protected override void OnEnter()
        {
            Debug.Log(TAG + "OnEnter");

            IContent.RequestContentExit<MenuContent>();
            UI.IDialog.RequestDialogEnter<UI.StoreMainDialog>();

            AddMessage();
        }

        private void AddMessage()
        {
            Message.AddListener<OpenStoreDialogMsg>(OpenStoreDialog);
            Message.AddListener<CloseStoreDialogMsg>(CloseStoreDialog);
        }

        private void OpenStoreDialog(OpenStoreDialogMsg msg)
        {
            if (msg.storeMenu == StoreMenu.IdolShop)
            {
                UI.IDialog.RequestDialogEnter<UI.StoreIdolDialog>();
                //Message.Send<SetCharacterStoreMsg>(new SetCharacterStoreMsg(character_Table));
            }
            else if (msg.storeMenu == StoreMenu.CashShop)
            {
                UI.IDialog.RequestDialogEnter<UI.StoreCashDialog>();
                Message.Send<SetCashStoreMsg>(new SetCashStoreMsg(cash_Table));
            }

            else if (msg.storeMenu == StoreMenu.SkinShop)
            {
                UI.IDialog.RequestDialogEnter<UI.StoreSkinDialog>();
                //Message.Send<SetSkinStoreMsg>(new SetSkinStoreMsg(skin_Table));
            }
        }

        private void CloseStoreDialog(CloseStoreDialogMsg msg)
        {
            if (msg.storeMenu == StoreMenu.IdolShop)
                UI.IDialog.RequestDialogExit<UI.StoreIdolDialog>();
            else if (msg.storeMenu == StoreMenu.CashShop)
                UI.IDialog.RequestDialogExit<UI.StoreCashDialog>();
            else if (msg.storeMenu == StoreMenu.SkinShop)
                UI.IDialog.RequestDialogExit<UI.StoreSkinDialog>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
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
            Message.RemoveListener<OpenStoreDialogMsg>(OpenStoreDialog);
            Message.RemoveListener<CloseStoreDialogMsg>(CloseStoreDialog);
        }
    }
}