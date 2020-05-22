using CellBig.Constants;
using CellBig.Models;
using CellBig.UI.Event;
using Midiazen;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace CellBig.Contents
{
    public class TabletSTTContent : IContent
    {
        SettingModel settingModel;
        CalendarModel calendarModel;
        HolostarSettingModel holostarSettingModel;
        PlayerInventoryModel playerInventoryModel;

        protected override void OnLoadStart()
        {
            settingModel = Model.First<SettingModel>();
            calendarModel = Model.First<CalendarModel>();
            holostarSettingModel = Model.First<HolostarSettingModel>();
            playerInventoryModel = Model.First<PlayerInventoryModel>();
            SetLoadComplete();
        }

        protected override void OnEnter()
        {
            AddMessage();
        }

        private void AddMessage()
        {
            Message.AddListener<STTReceiveMsg>(STTReceive);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                StartCoroutine(RecordInputDelay());
          
            }
            else if (Input.GetKeyDown(KeyCode.Z))
            {
                Message.Send<WeatherRequestMsg>(new WeatherRequestMsg(playerInventoryModel.NowCharacter));
                //Message.Send<LocationMsg>(new LocationMsg(PlayerPrefs.GetString("lat"), PlayerPrefs.GetString("lon")));
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                string dateEvent = string.Format("오늘은 {0}개의 일정이 있습니다.", calendarModel.GetTodayEvent());
                Message.Send<InfoMsg>(new InfoMsg(dateEvent));
                Message.Send<TTSSendMsg>(new TTSSendMsg("", dateEvent, playerInventoryModel.NowCharacter));
            }
        }

        IEnumerator RecordInputDelay()
        {
            Message.Send<TTSSendMsg>(new TTSSendMsg("", " 네?", playerInventoryModel.NowCharacter));
            Message.Send<InfoMsg>(new InfoMsg("음성을 입력해주세요."));
            yield return new WaitForSeconds(settingModel.RecordDelay);
            Message.Send<STTRecord>(new STTRecord());
        }

        //todo..
        //음성 인식 처리 부분.. 추후 음성 추가에 따라 처리
        private void STTReceive(STTReceiveMsg msg)
        {
            int index = 0;
            try
            {
                index = int.Parse(msg.intent);
            }
            catch (Exception E)
            {
                Debug.Log(E.ToString());
            }

            BluetoothData data = new BluetoothData();
            data.msg = "";
            data.dataType = SENDMSGTYPE.MENU;
            data.musicInfo = MUSICINFO.None;
            string dataMsg;

            //홀로스타
            if (index == 10000)//메인 메뉴
            {
                Message.Send<InfoMsg>(new InfoMsg("메인 메뉴로 이동합니다."));
                Debug.Log("메인");
                data.menu = Menu.Main;
                dataMsg = JsonUtility.ToJson(data);
                Message.Send<TabletMotionMsg>(new TabletMotionMsg(dataMsg));
                Message.Send<RunMenuMsg>(new RunMenuMsg(Menu.None));
                MenuMoveResponse();
            }
            else if (index == 10001)//메인 메뉴
            {
                Message.Send<InfoMsg>(new InfoMsg("최상위 메뉴로 이동합니다."));
                Debug.Log("메인");
                data.menu = Menu.Main;
                dataMsg = JsonUtility.ToJson(data);
                Message.Send<TabletMotionMsg>(new TabletMotionMsg(dataMsg));
                Message.Send<RunMenuMsg>(new RunMenuMsg(Menu.None));
                MenuMoveResponse();
            }
            else if (index == 11000)//시계
            {
                Debug.Log("시계");
                Message.Send<InfoMsg>(new InfoMsg("탁상시계 화면으로 이동합니다."));
                data.menu = Menu.Watch;
                dataMsg = JsonUtility.ToJson(data);
                Message.Send<TabletMotionMsg>(new TabletMotionMsg(dataMsg));
                Message.Send<RunMenuMsg>(new RunMenuMsg(Menu.Watch));
                MenuMoveResponse();
            }
            else if (index == 13200 || index == 13000)//게임
            {
                Debug.Log("게임");
                //Message.Send<InfoMsg>(new InfoMsg(" 이동합니다."));
                data.dataType = SENDMSGTYPE.GAME;
                data.msg = GameType.RhythmGame.ToString(); ;
                dataMsg = JsonUtility.ToJson(data);
                Message.Send<TabletMotionMsg>(new TabletMotionMsg(dataMsg));

                MenuMoveResponse();
            }
            else if (index == 12000)//음악
            {
                //검지 중지 음악재생
                Message.Send<InfoMsg>(new InfoMsg("음악재생으로 이동합니다."));
                Debug.Log("음악재생");
                data.menu = Menu.Music;
                dataMsg = JsonUtility.ToJson(data);
                Message.Send<TabletMotionMsg>(new TabletMotionMsg(dataMsg));
                Message.Send<RunMenuMsg>(new RunMenuMsg(Menu.Music));
                MenuMoveResponse();
            }
            else if (index == 14000)//홀로스타
            {
                //검지 중지 약지 새끼 홀로스타
                Message.Send<InfoMsg>(new InfoMsg("홀로스타 모드로 이동합니다."));
                Debug.Log("홀로스타");
                data.menu = Menu.HoloStar;
                dataMsg = JsonUtility.ToJson(data);
                Message.Send<TabletMotionMsg>(new TabletMotionMsg(dataMsg));
                Message.Send<RunMenuMsg>(new RunMenuMsg(Menu.HoloStar));
                MenuMoveResponse();
            }
            else if (index == 15000)//옵션
            {
                //엄지 검지 중지 약지 새끼 옵션
                Message.Send<InfoMsg>(new InfoMsg("환경설정으로 이동합니다."));
                Debug.Log("옵션");
                data.menu = Menu.Option;
                dataMsg = JsonUtility.ToJson(data);
                Message.Send<TabletMotionMsg>(new TabletMotionMsg(dataMsg));
                Message.Send<RunMenuMsg>(new RunMenuMsg(Menu.Option));
                MenuMoveResponse();
            }
            else if (index == 12100 || index == 12200)//음악재생
            {
                Message.Send<InfoMsg>(new InfoMsg("음악재생으로 이동합니다."));
                //검지 중지 음악재생
                Debug.Log("음악재생");
                data.menu = Menu.Music;
                dataMsg = JsonUtility.ToJson(data);
                Message.Send<TabletMotionMsg>(new TabletMotionMsg(dataMsg));
                Message.Send<RunMenuMsg>(new RunMenuMsg(Menu.Music));
                MenuMoveResponse();
            }
            else if (index == 20003 && settingModel.NowMenu == Menu.Option)//음소거
            {
                MenuMoveResponse();
                Message.Send<InfoMsg>(new InfoMsg("음소거 되었습니다."));
                holostarSettingModel.HoloOptionSetting.isMute = true;
                //사용
            }
            else if (index == 20004 && settingModel.NowMenu == Menu.Option)//음소거 해제
            {
                MenuMoveResponse();
                Message.Send<InfoMsg>(new InfoMsg("음소거가 해제되었습니다."));
                holostarSettingModel.HoloOptionSetting.isMute = false;
            }
            else if (index == 20005 && settingModel.NowMenu == Menu.Option)//볼륨 업
            {
                holostarSettingModel.HoloOptionSetting.TabletVolume += 0.2f;
                if (holostarSettingModel.HoloOptionSetting.TabletVolume > 1)
                {
                    holostarSettingModel.HoloOptionSetting.TabletVolume = 1f;
                }
                Message.Send<InfoMsg>(new InfoMsg("현재 볼륨은 " + holostarSettingModel.HoloOptionSetting.TabletVolume + " 입니다."));
            }
            else if (index == 20006 && settingModel.NowMenu == Menu.Option)//볼륨 다운
            {
                holostarSettingModel.HoloOptionSetting.TabletVolume -= 0.2f;
                if (holostarSettingModel.HoloOptionSetting.TabletVolume < 0)
                {
                    holostarSettingModel.HoloOptionSetting.TabletVolume = 0;
                }
                Message.Send<InfoMsg>(new InfoMsg("현재 볼륨은 " + holostarSettingModel.HoloOptionSetting.TabletVolume + " 입니다."));
            }
            else if (index == 31011)
            {
                //날씨
                Message.Send<WeatherRequestMsg>(new WeatherRequestMsg(playerInventoryModel.NowCharacter));
            }
            else if (index == 31021)
            {
                //시간
                string time = string.Format("현재 시간은 {0}시 {1}분 입니다.", DateTime.Now.ToString("HH"), DateTime.Now.ToString("mm"));
                Message.Send<TTSSendMsg>(new TTSSendMsg("", time, playerInventoryModel.NowCharacter));
            }
            else if (index == 31031)
            {
                string dateEvent = string.Format("오늘은 {0}개의 일정이 있습니다.", calendarModel.GetTodayEvent());
                Message.Send<InfoMsg>(new InfoMsg(dateEvent));
                Message.Send<TTSSendMsg>(new TTSSendMsg("", dateEvent, playerInventoryModel.NowCharacter));
                //오늘 일정
            }
            else if (index == 31032)
            {
                //내일 일정
                string dateEvent = string.Format("내일은 {0}개의 일정이 있습니다.", calendarModel.GetTomorrowEvent());
                Message.Send<InfoMsg>(new InfoMsg(dateEvent));
                Message.Send<TTSSendMsg>(new TTSSendMsg("", dateEvent, playerInventoryModel.NowCharacter));
            }

            else if (index == 0)
            {
                Message.Send<TTSSendMsg>(new TTSSendMsg("", "다시 말씀해 주시겠어요?", playerInventoryModel.NowCharacter));
            }
            else
            {
                if(playerInventoryModel.NowCharacter == Character.Girl)
                    SoundManager.Instance.PlaySound(index);
                else
                {
                    index = index + 10000;
                    SoundManager.Instance.PlaySound(index);
                }
                Message.Send<EmotionAniMationMsg>(new EmotionAniMationMsg(index));

            }

            if(settingModel.UseWakeUp)
                Message.Send<WakeUpMsg>(new WakeUpMsg(true));

            Message.Send<STTBtnSetMsg>(new STTBtnSetMsg(true));

            string receive = string.Format("STT 수신 Intent : {0} text : {1}", msg.intent, msg.text);
            Log.Instance.log(receive);
        }

        void MenuMoveResponse()
        {
            int soundIndex = 16000;
            if (playerInventoryModel.NowCharacter == Character.Girl)
                SoundManager.Instance.PlaySound(soundIndex);
            else
            {
                soundIndex = soundIndex + 10000;
                SoundManager.Instance.PlaySound(soundIndex);
            }
        }

        IEnumerator Rerecord()
        {
            yield return new WaitForSeconds(1.5f);
            Message.Send<STTRecord>(new STTRecord());
        }

        protected override void OnExit()
        {
            RemoveMessage();
        }

        private void RemoveMessage()
        {
            Message.RemoveListener<STTReceiveMsg>(STTReceive);
        }
    }
}
