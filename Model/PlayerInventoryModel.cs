using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig.Constants;
using CellBig.Contents;

namespace CellBig.Models
{
    public class PlayerInventoryModel : Model
    {
        GameModel _owner;

        public void Setup(GameModel owner)
        {
            _owner = owner;
        }

        PlayerInventory playerInventory;

        public PlayerInventory PlayerInventory { get => playerInventory; set => playerInventory = value; }
        public Character NowCharacter { get => playerInventory.nowCharacter; set => playerInventory.nowCharacter = value; }
        public int NowSkin { get => playerInventory.nowSkin; set => playerInventory.nowSkin = value; }
        public Character[] ListBuyCharacter { get => playerInventory.buyCharacter; set => playerInventory.buyCharacter = value; }
        public int[] ListBuySkinNum { get => playerInventory.buySkinNum; set => playerInventory.buySkinNum = value; }
    }
}