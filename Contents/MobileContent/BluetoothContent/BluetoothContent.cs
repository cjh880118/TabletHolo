using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Android;
using JHchoi.UI.Event;
using System;
using JHchoi.Constants;
using JHchoi.Models;

namespace JHchoi.Contents
{
    public class BluetoothContent : IContent
    {
        SettingModel settingModel;

        protected override void OnLoadStart()
        {
            settingModel = Model.First<SettingModel>();
            SetLoadComplete();
        }

        protected override void OnEnter()
        {
            AddMessage();
            AndroidTrasferMgr.Instance.SearchDevice();
        }

        private void AddMessage()
        {
            Message.AddListener<BluetoothItemSelectMsg>(BluetoothItemSelect);
            Message.AddListener<BluetoothListCloseMsg>(BluetoothListClose);
        }

        private void BluetoothItemSelect(BluetoothItemSelectMsg msg)
        {
            AndroidTrasferMgr.Instance.SelectDevice(msg.name);
            UI.IDialog.RequestDialogEnter<UI.BluetoothLoadingDialog>();
        }

        private void BluetoothListClose(BluetoothListCloseMsg msg)
        {
            UI.IDialog.RequestDialogExit<UI.BluetoothListDialog>();
        }

        protected override void OnExit()
        {
            RemoveMessage();
        }

        private void RemoveMessage()
        {
            Message.RemoveListener<BluetoothItemSelectMsg>(BluetoothItemSelect);
        }

        //수신
        void BluetoothList(string msg)
        {
            string[] spstring = msg.Split('|');
            int deviceCnt = spstring.Length;
            List<string> bluetoothList = new List<string>();

            for (int i = 0; i < deviceCnt; i++)
            {
                bluetoothList.Add(spstring[i]);
            }

            //블루투스 다이얼로그 오픈
            UI.IDialog.RequestDialogEnter<UI.BluetoothListDialog>();
            Message.Send<BluetoothListMsg>(new BluetoothListMsg(bluetoothList));
        }

        void BluetoothConnectResult(string msg)
        {
            //연결 요청 결과
            UI.IDialog.RequestDialogExit<UI.BluetoothLoadingDialog>();
            if (msg == "ConnectedSuccess")
            {
                settingModel.IsBluetoothConnet = true;
                AndroidTrasferMgr.Instance.ShowToast("Bluetooth 연결에 성공하였습니다.");
                BluetoothListClose(new BluetoothListCloseMsg());
            }
            else
            {
                settingModel.IsBluetoothConnet = false;
                AndroidTrasferMgr.Instance.ShowToast("Bluetooth 연결에 연결할 HOLOSTAR를 찾지 못하였습니다.");
            }
        }

        //수신
        void BluetootEnableMsg(string msg)
        {
            if (msg == "Cancel")
            {
                AndroidTrasferMgr.Instance.ShowToast("블루투스를 키지 않으면 일부 기능은 사용할수 없습니다.");
            }
        }

        void ReceiveMsg(string msg)
        {
            string[] splitMsg = msg.Split('|');

            SENDMSGTYPE tempMsgType;

            tempMsgType = (SENDMSGTYPE)Enum.Parse(typeof(SENDMSGTYPE), splitMsg[0]);
            if (tempMsgType == SENDMSGTYPE.MENU)
            {
                try
                {
                    Menu tempMenu = (Menu)Enum.Parse(typeof(Menu), splitMsg[1]);
                    Debug.Log(tempMenu.ToString());
                }
                catch (Exception e)
                {
                    Debug.Log(e.ToString());
                }
            }
            else if (tempMsgType == SENDMSGTYPE.MSG) { }
            else if (tempMsgType == SENDMSGTYPE.MUSIC) { }
            else if (tempMsgType == SENDMSGTYPE.CHARINFO) { }
            else if (tempMsgType == SENDMSGTYPE.LOCATION) { }
            else if (tempMsgType == SENDMSGTYPE.CALENDAR) { }
            else if (tempMsgType == SENDMSGTYPE.SETTING) { }
        }
    }
}