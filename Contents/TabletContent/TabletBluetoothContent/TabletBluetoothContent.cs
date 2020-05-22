using JHchoi.Constants;
using JHchoi.UI.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JHchoi.Models;
using Midiazen;
using System.Linq;

namespace JHchoi.Contents
{
    public class TabletBluetoothContent : IContent
    {
        PlayerInventoryModel playerInventoryModel;
        HolostarSettingModel holostarSettingModel;
        CalendarModel calendarModel;
        SettingModel settingModel;
        Coroutine corCalenderCheck;

        protected override void OnLoadStart()
        {
            playerInventoryModel = Model.First<PlayerInventoryModel>();
            holostarSettingModel = Model.First<HolostarSettingModel>();
            calendarModel = Model.First<CalendarModel>();
            settingModel = Model.First<SettingModel>();
            SetLoadComplete();
        }

        protected override void OnLoadComplete()
        {
            for (int i = 0; i < calendarModel.AlarmEvents.Count; i++)
            {
                if (DateTime.Now.Ticks > calendarModel.AlarmEvents[i].tick)
                {
                    calendarModel.AlarmEvents.RemoveAt(i);
                }
            }

            corCalenderCheck = StartCoroutine(CalenderCheck());
        }


        IEnumerator CalenderCheck()
        {
            while (true)
            {
                yield return null;
                if (calendarModel.AlarmEvents.Count > 0 && DateTime.Now.Ticks > calendarModel.AlarmEvents[0].tick)
                {
                    if (holostarSettingModel.IsSchedule)
                        Message.Send<InfoMsg>(new InfoMsg(calendarModel.AlarmEvents[0].title));

                    if (holostarSettingModel.IsTTSReceive)
                        Message.Send<TTSSendMsg>(new TTSSendMsg(null, calendarModel.AlarmEvents[0].title + "일정 시간입니다.", playerInventoryModel.NowCharacter));

                    calendarModel.AlarmEvents.RemoveAt(0);
                }
            }
        }

        protected override void OnEnter()
        {
            WindowBluetooth.GetInstance().OpenPort(settingModel.Port, settingModel.Baud, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
            AddMessage();
        }

        private void AddMessage()
        {
            Message.AddListener<TabletReceiveMsg>(TabletReceive);
            Message.AddListener<TabletMotionMsg>(TabletMotion);
        }

        //립모션으로 메뉴 이동시
        private void TabletMotion(TabletMotionMsg msg)
        {
            BluetoothData data = JsonUtility.FromJson<BluetoothData>(msg.msg);
            SENDMSGTYPE tempMsgType = data.dataType;
            IContent.RequestContentExit<TabletHolostarContent>();
            IContent.RequestContentExit<TabletWatchContent>();
            IContent.RequestContentExit<TabletMusicContent>();
            IContent.RequestContentExit<TabletRhythmContent>();
            if (tempMsgType == SENDMSGTYPE.MENU)
            {
                try
                {
                    Menu tempMenu = data.menu;

                    if (tempMenu == Menu.Main)
                    {
                        Debug.Log("메인 메뉴");
                        settingModel.NowMenu = Menu.Main;
                    }
                    else if (tempMenu == Menu.Watch)
                    {
                        Debug.Log("시계");
                        settingModel.NowMenu = Menu.Watch;
                        IContent.RequestContentEnter<TabletWatchContent>();
                    }
                    else if (tempMenu == Menu.Music)
                    {
                        Debug.Log("음악");
                        settingModel.NowMenu = Menu.Music;
                        IContent.RequestContentEnter<TabletMusicContent>();
                    }
                    else if (tempMenu == Menu.Game)
                    {
                        Debug.Log("게임");
                        settingModel.NowMenu = Menu.Game;
                    }
                    else if (tempMenu == Menu.HoloStar)
                    {
                        Debug.Log("홀로스타");
                        settingModel.NowMenu = Menu.HoloStar;
                        IContent.RequestContentEnter<TabletHolostarContent>();
                    }
                    else if (tempMenu == Menu.Option)
                    {
                        Debug.Log("옵션");
                        settingModel.NowMenu = Menu.Option;
                    }

                    WindowBluetooth.GetInstance().SendBluetoothMsg("", SENDMSGTYPE.MENU, MUSICINFO.None, tempMenu);
                    Message.Send<RunMenuMsg>(new RunMenuMsg(tempMenu));
                }
                catch (Exception e)
                {
                    Debug.Log(e.ToString());
                }
            }
            else if (tempMsgType == SENDMSGTYPE.GAME)
            {
                settingModel.NowMenu = Menu.Game;
                GameType gameType = (GameType)Enum.Parse(typeof(GameType), data.msg);

                if (gameType == GameType.AlongGame)
                {
                    Debug.Log("방치형");
                    IContent.RequestContentEnter<TabletAloneContent>();
                }
                else if (gameType == GameType.RhythmGame)
                {
                    Debug.Log("리듬");
                    IContent.RequestContentEnter<TabletRhythmContent>();
                    Message.Send<RunMenuMsg>(new RunMenuMsg(Menu.Game));
                    Message.Send<RhythmGameStartMsg>(new RhythmGameStartMsg());
                }
            }
        }

        private void TabletReceive(TabletReceiveMsg msg)
        {
            ReceiveMsg(msg.msg);
        }

        void ReceiveMsg(string msg)
        {
            if (msg == "DisConnect")
            {
                settingModel.IsBluetoothConnet = false;
                Message.Send<InfoMsg>(new InfoMsg("Bluetooth 연결이 끊어졌습니다."));
                return;
            }

            BluetoothData data = JsonUtility.FromJson<BluetoothData>(msg);
            SENDMSGTYPE tempMsgType = data.dataType;

            WindowBluetooth.GetInstance().SendBluetoothMsg("", SENDMSGTYPE.RECEIVE);

            if (tempMsgType == SENDMSGTYPE.MENU)
            {
                try
                {
                    Menu tempMenu = data.menu;

                    IContent.RequestContentExit<TabletHolostarContent>();
                    IContent.RequestContentExit<TabletWatchContent>();
                    IContent.RequestContentExit<TabletMusicContent>();
                    IContent.RequestContentExit<TabletRhythmContent>();

                    if (tempMenu == Menu.Main)
                    {
                        Debug.Log("메인 메뉴");
                        settingModel.NowMenu = Menu.Main;
                    }
                    else if (tempMenu == Menu.Watch)
                    {
                        Debug.Log("시계");
                        settingModel.NowMenu = Menu.Watch;
                        IContent.RequestContentEnter<TabletWatchContent>();
                    }
                    else if (tempMenu == Menu.Music)
                    {
                        Debug.Log("음악");
                        settingModel.NowMenu = Menu.Music;
                        IContent.RequestContentEnter<TabletMusicContent>();
                    }
                    else if (tempMenu == Menu.Game)
                    {
                        Debug.Log("게임");
                        settingModel.NowMenu = Menu.Game;
                    }
                    else if (tempMenu == Menu.HoloStar)
                    {
                        Debug.Log("홀로스타");
                        settingModel.NowMenu = Menu.HoloStar;
                        IContent.RequestContentEnter<TabletHolostarContent>();
                    }
                    else if (tempMenu == Menu.Option)
                    {
                        Debug.Log("옵션");
                        settingModel.NowMenu = Menu.Option;
                    }
                    Message.Send<RunMenuMsg>(new RunMenuMsg(tempMenu));

                    if (holostarSettingModel.IsMusicPlay && tempMenu == Menu.Music)
                    { }
                    else
                        Message.Send<SetCharacterAnimationMsg>(new SetCharacterAnimationMsg(AnimationType.Idel1, false));
                }
                catch (Exception e)
                {
                    Debug.Log(e.ToString());
                }
            }
            else if (tempMsgType == SENDMSGTYPE.GAME)
            {
                IContent.RequestContentExit<TabletAloneContent>();
                IContent.RequestContentExit<TabletRhythmContent>();
                settingModel.NowMenu = Menu.Game;
                GameType gameType = (GameType)Enum.Parse(typeof(GameType), data.msg);
                if (gameType == GameType.AlongGame)
                {
                    Debug.Log("방치형");
                    IContent.RequestContentEnter<TabletAloneContent>();
                }
                else if (gameType == GameType.RhythmGame)
                {
                    Debug.Log("리듬");
                    IContent.RequestContentEnter<TabletRhythmContent>();
                }

            }
            else if (tempMsgType == SENDMSGTYPE.MSG)
            {
                if (holostarSettingModel.IsMMSReceive)
                    Message.Send<InfoMsg>(new InfoMsg(data.msg));
                if (holostarSettingModel.IsTTSReceive)
                    Message.Send<TTSSendMsg>(new TTSSendMsg(null, data.msg, playerInventoryModel.NowCharacter));
            }
            else if (tempMsgType == SENDMSGTYPE.MUSIC)
            {
                Message.Send<MusicRequestMsg>(new MusicRequestMsg(data.musicInfo, data.msg));
            }
            else if (tempMsgType == SENDMSGTYPE.CHARINFO)
            {
                //블루트스 커넥터가 false인 상태에서 최초 정보 수신 연결 확인 알림

                //모바일에 인벤토리 정보를 저장
                PlayerInventory tempInven = JsonUtility.FromJson<PlayerInventory>(data.msg);
                playerInventoryModel.PlayerInventory = tempInven;

                //모델 정보로 케릭터 셋팅
                Message.Send<SetCharacterDressMsg>(new SetCharacterDressMsg(playerInventoryModel.PlayerInventory.nowCharacter,
                    playerInventoryModel.PlayerInventory.nowSkin));

                string tempSetting = JsonUtility.ToJson(holostarSettingModel.HoloStarSetting);
                WindowBluetooth.GetInstance().SendBluetoothMsg(tempSetting, SENDMSGTYPE.SETTING);
            }
            else if (tempMsgType == SENDMSGTYPE.LOCATION)
            {
                string[] location = data.msg.Split('&');
                Message.Send<LocationMsg>(new LocationMsg(location[0], location[1]));
                //위치 정보 저장
            }
            else if (tempMsgType == SENDMSGTYPE.CALENDAR)
            {

                if (data.msg == "start")
                {
                    calendarModel.AlarmEvents.Clear();
                }
                else if (data.msg == "end")
                {

                }
                else
                {
                    AlarmEvent alarmEvent = JsonUtility.FromJson<AlarmEvent>(data.msg);
                    calendarModel.AlarmEvents.Add(alarmEvent);
                }

                //WindowBluetooth.GetInstance().SendBluetoothMsg("", SENDMSGTYPE.RECEIVE);

                //                CalanderEvent alarmEvent = JsonUtility.FromJson<CalanderEvent>(data.msg);
                //calendarModel.AlarmEvents = alarmEvent.alarmEvents.ToList();
                //calendarModel.AlarmEvents.Add
                //일정 저장
            }
            else if (tempMsgType == SENDMSGTYPE.SETTING)
            {
                settingModel.NowMenu = Menu.Option;
                Message.Send<RunMenuMsg>(new RunMenuMsg(Menu.Option));
                HoloStarSetting tempSetting = JsonUtility.FromJson<HoloStarSetting>(data.msg);
                holostarSettingModel.HoloStarSetting = tempSetting;
            }
            else if (tempMsgType == SENDMSGTYPE.ANIMATION)
            {
                //애니메이션
                AnimationType aniType = (AnimationType)Enum.Parse(typeof(AnimationType), data.msg);
                Message.Send<SetCharacterAnimationMsg>(new SetCharacterAnimationMsg(aniType, true));
            }
            else if (tempMsgType == SENDMSGTYPE.CONNECTION)
            {
                if (Convert.ToBoolean(data.msg))
                {
                    Debug.Log("접속 완료");
                    Message.Send<InfoMsg>(new InfoMsg("Bluetooth 연결되었습니다."));
                    settingModel.IsBluetoothConnet = true;
                    //Debug.Log("메인");
                    BluetoothData bluetoothData = new BluetoothData();
                    bluetoothData.msg = "";
                    bluetoothData.dataType = SENDMSGTYPE.MENU;
                    bluetoothData.musicInfo = MUSICINFO.None;
                    bluetoothData.menu = Menu.Main;
                    string dataMsg = JsonUtility.ToJson(bluetoothData);
                    Message.Send<TabletMotionMsg>(new TabletMotionMsg(dataMsg));
                    Message.Send<RunMenuMsg>(new RunMenuMsg(Menu.Main));
                    WindowBluetooth.GetInstance().SendBluetoothMsg("", SENDMSGTYPE.CHARINFO);
                }
                else
                {
                    Debug.Log("접속 종료");
                    Message.Send<InfoMsg>(new InfoMsg("Bluetooth 연결이 끊어졌습니다."));
                    settingModel.IsBluetoothConnet = false;
                }
            }
            else if (tempMsgType == SENDMSGTYPE.INTENT)
            {
                Message.Send<STTReceiveMsg>(new STTReceiveMsg(data.msg,""));
            }
        }
        protected override void OnExit()
        {
            RemoveMessage();
        }

        private void RemoveMessage()
        {
            Message.RemoveListener<TabletReceiveMsg>(TabletReceive);
        }
    }
}