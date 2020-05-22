using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JHchoi.Constants;
using System;

namespace JHchoi.UI.Event
{
    public class BluetoothListMsg : Message
    {
        public List<string> listBluetoothDevice;

        public BluetoothListMsg(List<string> listBluetoothDevice)
        {
            this.listBluetoothDevice = listBluetoothDevice;
        }
    }

    public class BluetoothListCloseMsg : Message
    {
    }

    public class BluetoothItemSelectMsg : Message
    {
        public string name;
        public BluetoothItemSelectMsg(string name)
        {
            this.name = name;
        }
    }

    public class BluetoothReceiveRunMenuMsg : Message
    {
        public Menu menu;
        public BluetoothReceiveRunMenuMsg(string menuName)
        {
            this.menu = (Menu)Enum.Parse(typeof(Menu), menuName);
        }
    }

    public class TabletReceiveMsg : Message
    {
        public string msg;
        public TabletReceiveMsg(string msg)
        {
            this.msg = msg;
        }
    }

    public class TabletMotionMsg : Message
    {
        public string msg;
        public TabletMotionMsg(string msg)
        {
            this.msg = msg;
        }
    }
}
