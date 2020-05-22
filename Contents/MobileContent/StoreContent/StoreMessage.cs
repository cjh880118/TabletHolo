using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JHchoi.Constants;

namespace JHchoi.UI.Event
{
    public class OpenStoreDialogMsg : Message
    {
        public StoreMenu storeMenu;
        public OpenStoreDialogMsg(StoreMenu storeMenu)
        {
            this.storeMenu = storeMenu;
        }
    }

    public class CloseStoreDialogMsg : Message
    {
        public StoreMenu storeMenu;
        public CloseStoreDialogMsg(StoreMenu storeMenu)
        {
            this.storeMenu = storeMenu;
        }
    }

    public class SetCashStoreMsg : Message
    {
        public Cash_Table cash_Table;
        public SetCashStoreMsg(Cash_Table cash_Table)
        {
            this.cash_Table = cash_Table;
        }
    }

    public class SetCharacterStoreMsg : Message
    {
        public Character_Table character_Table;
        public SetCharacterStoreMsg(Character_Table character_Table)
        {
            this.character_Table = character_Table;
        }
    }
  
    public class SetSkinStoreMsg : Message
    {
        public Skin_Table skin_Table;
        public SetSkinStoreMsg(Skin_Table skin_Table)
        {
            this.skin_Table = skin_Table;
        }
    }
}