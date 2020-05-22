using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig.UI.Event;
using CellBig.Constants;
using Android;

namespace CellBig.Contents
{
    public class MenuContent : IContent
    {
        static string TAG = "MenuContent ::";

        protected override void OnEnter()
        {
            UI.IDialog.RequestDialogEnter<UI.MainMenuDialog>();

            IContent.RequestContentExit<WatchContent>();
            IContent.RequestContentExit<AloneGameContent>();
            IContent.RequestContentExit<RhythmGameContent>();
            IContent.RequestContentExit<HolostarContent>();
            IContent.RequestContentExit<WatchContent>();
            IContent.RequestContentExit<SettingContnet>();
            IContent.RequestContentExit<StoreContent>();
            IContent.RequestContentExit<MusicContent>();

            //블루투스 목록 호출

            AddMessage();
        }

        private void AddMessage()
        {
            Message.AddListener<RunMenuMsg>(RunMenu);
            Message.AddListener<RunGameMsg>(RunGame);
            Message.AddListener<GameSelectCloseMsg>(GameSelectClose);
        }

        private void RunMenu(RunMenuMsg msg)
        {
            Debug.Log(TAG + "RunMenu");
            if (msg.menu == Menu.Watch)
                IContent.RequestContentEnter<WatchContent>();
            else if (msg.menu == Menu.Music)
                IContent.RequestContentEnter<MusicContent>();
            else if (msg.menu == Menu.Game)
                UI.IDialog.RequestDialogEnter<UI.GameSelectDialog>();
            else if (msg.menu == Menu.Store)
                IContent.RequestContentEnter<StoreContent>();
            else if (msg.menu == Menu.HoloStar)
                IContent.RequestContentEnter<HolostarContent>();
            else if (msg.menu == Menu.Option)
                IContent.RequestContentEnter<SettingContnet>();

            AndroidTrasferMgr.Instance.BluetoothSendMsg(msg.menu.ToString(), SENDMSGTYPE.MENU);
        }

        private void RunGame(RunGameMsg msg)
        {
            if (msg.menu == GameType.AlongGame)
            {
                IContent.RequestContentEnter<AloneGameContent>();
            }
            else if (msg.menu == GameType.AlongGame)
            {
                IContent.RequestContentEnter<RhythmGameContent>();
            }
        }

        private void GameSelectClose(GameSelectCloseMsg msg)
        {
            UI.IDialog.RequestDialogExit<UI.GameSelectDialog>();
            UI.IDialog.RequestDialogEnter<UI.MainMenuDialog>();
        }

        protected override void OnExit()
        {
            DialogAllClose();
            RemoveMessage();
        }

        private void RemoveMessage()
        {
            Message.RemoveListener<RunMenuMsg>(RunMenu);
            Message.RemoveListener<RunGameMsg>(RunGame);
            Message.RemoveListener<GameSelectCloseMsg>(GameSelectClose);
        }
    }
}