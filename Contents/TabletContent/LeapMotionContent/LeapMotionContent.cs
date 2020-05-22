using CellBig.Constants;
using CellBig.UI.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig.Models;
using Leap.Unity;
using CellBig.Module;
using System.Linq;

namespace CellBig.Contents
{
    public class LeapMotionContent : IContent
    {
        HandModelManager leapHand;
        Dictionary<Chirality, HandModel> Rigidhands = new Dictionary<Chirality, HandModel>();
        Dictionary<Chirality, Coroutine> trace = new Dictionary<Chirality, Coroutine>();
        List<bool> ListNowFingerActive = new List<bool>();
        List<bool> ListPrevFingerActive = new List<bool>();
        float Timer = 0;

        SettingModel settingModel;
        HolostarSettingModel holostarSettingModel;
        Menu_Controller tablet_Menu;
        GameObject LeapModule;
        GameObject tabletMainMenu;
        GameObject tabletMusicMenu;
        GameObject tabletOptionMenu;
        bool isLeftHandIn;
        bool isRightHandIn;

        Coroutine corHandOut;
        float handOutTime;

        protected override void OnLoadStart()
        {
            LeapModule = GameObject.Find("LeapMotion");
            settingModel = Model.First<SettingModel>();
            holostarSettingModel = Model.First<HolostarSettingModel>();
            ResourcesLoad();
            StartCoroutine(LoadInitialData());
        }

        IEnumerator LoadInitialData()
        {
            string path = "Object/NewUI/TabletMenu";
            yield return StartCoroutine(ResourceLoader.Instance.Load<GameObject>(path,
               o =>
               {
                   var tabletMenu = Instantiate(o) as GameObject;
                   tabletMenu.transform.position = new Vector3(0, 0, 0.5f);
                   tablet_Menu = tabletMenu.GetComponent<Menu_Controller>();
                   tablet_Menu.InitMenu(LeapModule);
                   tabletMainMenu = tabletMenu.transform.GetChild(0).gameObject;
                   tabletMusicMenu = tabletMenu.transform.GetChild(1).gameObject;
                   tabletOptionMenu = tabletMenu.transform.GetChild(2).gameObject;

               }));

            SetLoadComplete();
        }

        protected override void OnLoadComplete()
        {
            corHandOut = StartCoroutine(HandInOutCheck());
        }

        IEnumerator HandInOutCheck()
        {
            while (true)
            {
                yield return null;
                if (!isRightHandIn && !isLeftHandIn)  
                {
                    handOutTime += Time.deltaTime;
                    if (handOutTime > 1.5)
                    {
                        handOutTime = 0;
                        tabletMainMenu.SetActive(false);
                        tabletMusicMenu.SetActive(false);
                        settingModel.isOpenMenu = false;

                        //손이 없어질시 메뉴 셋팅
                        if (settingModel.NowMenu == Menu.Watch)
                            UI.IDialog.RequestDialogEnter<UI.TabletWatchDialog>();

                        else if (settingModel.NowMenu == Menu.HoloStar)
                            Message.Send<CameraZoomMsg>(new CameraZoomMsg(true));

                        else if (settingModel.NowMenu == Menu.Option)
                            tablet_Menu.SetMenu(settingModel.NowMenu, settingModel.IsBluetoothConnet, holostarSettingModel.IsMMSReceive, holostarSettingModel.IsSchedule, holostarSettingModel.IsMute, holostarSettingModel.TabletVolume);
                    }
                }
                else
                {
                    handOutTime = 0;
                }
            }
        }

        void ResourcesLoad()
        {
            leapHand = ModuleManager.Instance.GetComponentInChildren<HandModelManager>();
            var hands = leapHand.GetComponentsInChildren<HandModel>(true);
            foreach (var item in hands)
            {
                Debug.Log("" + item.Handedness + " name : " + item.name);
                Rigidhands.Add(item.Handedness, item);
            }

            Rigidhands[Chirality.Left].OnBegin += OnLeft;
            Rigidhands[Chirality.Left].OnFinish += OffLeft;
            Rigidhands[Chirality.Right].OnBegin += OnRight;
            Rigidhands[Chirality.Right].OnFinish += OffRight;

            if (leapHand == null)
                Debug.LogError("module error");
        }

        bool IsIndexOpen;
        bool IsMiddleOpen;
        bool IsRingOpen;
        bool IsThumbOpen;
        bool IsPinkyOpen;

        IEnumerator TraceHandPos(Chirality hand)
        {
            Debug.Log("hand : " + hand);
            while (true)
            {
                yield return new WaitForFixedUpdate();
                Vector3 handpos = Rigidhands[hand].GetPalmPosition();
                Vector3 indexFingerLast = new Vector3();
                Vector3 indexFingerFirst = new Vector3();
                Vector3 middleFingerLast = new Vector3();
                Vector3 middleFingerFirst = new Vector3();
                Vector3 ringFingerLast = new Vector3();
                Vector3 ringFingerFirst = new Vector3();
                Vector3 thumbFingerLast = new Vector3();
                Vector3 thumbFingerFirst = new Vector3();
                Vector3 pinkyFingerLast = new Vector3();
                Vector3 pinkyFingerFirst = new Vector3();

                ListNowFingerActive.Clear();

                foreach (var item in Rigidhands[hand].fingers)
                {
                    if (item.fingerType == Leap.Finger.FingerType.TYPE_INDEX)
                    {
                        indexFingerLast = item.bones[3].position;
                        indexFingerFirst = Rigidhands[hand].palm.position; //item.bones[0].position;
                    }
                    else if (item.fingerType == Leap.Finger.FingerType.TYPE_MIDDLE)
                    {
                        middleFingerLast = item.bones[3].position;
                        middleFingerFirst = Rigidhands[hand].palm.position;
                    }
                    else if (item.fingerType == Leap.Finger.FingerType.TYPE_RING)
                    {
                        ringFingerLast = item.bones[3].position;
                        ringFingerFirst = Rigidhands[hand].palm.position;
                    }
                    else if (item.fingerType == Leap.Finger.FingerType.TYPE_THUMB)
                    {
                        thumbFingerLast = item.bones[3].position;
                        thumbFingerFirst = Rigidhands[hand].palm.position;
                    }
                    else if (item.fingerType == Leap.Finger.FingerType.TYPE_PINKY)
                    {
                        pinkyFingerLast = item.bones[3].position;
                        pinkyFingerFirst = Rigidhands[hand].palm.position;
                    }

                    //검지 손가락
                    if (Vector3.Distance(indexFingerFirst, indexFingerLast) < 0.2f &&
                        Vector3.Distance(indexFingerFirst, indexFingerLast) != 0)
                    {
                        ListNowFingerActive.Add(false);
                        IsIndexOpen = false;
                    }
                    else if (Vector3.Distance(indexFingerFirst, indexFingerLast) > 0.2f &&
                        Vector3.Distance(indexFingerFirst, indexFingerLast) != 0)
                    {
                        ListNowFingerActive.Add(true);
                        IsIndexOpen = true;
                    }

                    //중지 손가락
                    if (Vector3.Distance(middleFingerLast, middleFingerFirst) < 0.2f &&
                        Vector3.Distance(middleFingerLast, middleFingerFirst) != 0)
                    {
                        ListNowFingerActive.Add(false);
                        IsMiddleOpen = false;
                        //Debug.Log("중지 접");
                    }
                    else if (Vector3.Distance(middleFingerLast, middleFingerFirst) > 0.2f &&
                        Vector3.Distance(middleFingerLast, middleFingerFirst) != 0)
                    {
                        ListNowFingerActive.Add(true);
                        IsMiddleOpen = true;
                        //Debug.Log("중지 펴");
                    }

                    //약지 손가락
                    if (Vector3.Distance(ringFingerLast, ringFingerFirst) < 0.2f &&
                        Vector3.Distance(ringFingerLast, ringFingerFirst) != 0)
                    {
                        ListNowFingerActive.Add(false);
                        IsRingOpen = false;
                        //Debug.Log("약지 접");
                    }
                    else if (Vector3.Distance(ringFingerLast, ringFingerFirst) > 0.2f &&
                         Vector3.Distance(ringFingerLast, ringFingerFirst) != 0)
                    {
                        ListNowFingerActive.Add(true);
                        IsRingOpen = true;
                        //Debug.Log("약지 펴");
                    }

                    //엄지 손가락
                    if (Vector3.Distance(thumbFingerLast, thumbFingerFirst) < 0.165f &&
                        Vector3.Distance(thumbFingerLast, thumbFingerFirst) != 0)
                    {
                        ListNowFingerActive.Add(false);
                        IsThumbOpen = false;
                        //Debug.Log("엄지 접");
                    }
                    else if (Vector3.Distance(thumbFingerLast, thumbFingerFirst) > 0.165f &&
                       Vector3.Distance(thumbFingerLast, thumbFingerFirst) != 0)
                    {
                        ListNowFingerActive.Add(true);
                        IsThumbOpen = true;
                        //Debug.Log("엄지 펴");
                    }

                    //새끼 손가락
                    if (Vector3.Distance(pinkyFingerLast, pinkyFingerFirst) < 0.15f &&
                        Vector3.Distance(pinkyFingerLast, pinkyFingerFirst) != 0)
                    {
                        ListNowFingerActive.Add(false);
                        IsPinkyOpen = false;
                        //Debug.Log("새끼 접");
                    }
                    else if (Vector3.Distance(pinkyFingerLast, pinkyFingerFirst) > 0.15f &&
                        Vector3.Distance(pinkyFingerLast, pinkyFingerFirst) != 0)
                    {
                        ListNowFingerActive.Add(true);
                        IsPinkyOpen = true;
                        //Debug.Log("새끼 펴");
                    }
                }

                bool same = ListPrevFingerActive.SequenceEqual(ListNowFingerActive);

                if (ListPrevFingerActive == null || !same)
                {
                    Debug.Log("유지 실패");
                    Timer = 0;
                    isMenuInfo = false;
                    ListPrevFingerActive.Clear();
                    foreach (var o in ListNowFingerActive)
                        ListPrevFingerActive.Add(o);
                }
                else if (same)
                {
                    Timer += Time.deltaTime;
                    if (Timer > 1 && !isMenuInfo)
                    {
                        isMenuInfo = true;
                        MenuInfo();
                    }

                    if (Timer > 2)
                    {
                        Timer = 0;
                        isMenuInfo = false;
                        RunMenuCheck();
                    }
                }
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                LeapMotion(Menu.Game);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                Message.Send<InfoMsg>(new InfoMsg("게임이 종료됩니다."));
                Message.Send<SetCharacterAnimationMsg>(new SetCharacterAnimationMsg(AnimationType.Idel1, false));
                settingModel.IsPlayGame = false;
                LeapMotion(Menu.Main);
            }
        }

        bool isMenuInfo = false;

        void RunMenuCheck()
        {
            //주먹 메뉴 등장
            if (!IsIndexOpen && !IsMiddleOpen && !IsPinkyOpen && !IsRingOpen && !IsThumbOpen)
            {
                if (settingModel.NowMenu == Menu.Watch)
                    UI.IDialog.RequestDialogExit<UI.TabletWatchDialog>();
                else if (settingModel.NowMenu == Menu.Game)
                {
                    Message.Send<InfoMsg>(new InfoMsg("게임이 종료됩니다."));
                    Message.Send<SetCharacterAnimationMsg>(new SetCharacterAnimationMsg(AnimationType.Idel1, false));
                    settingModel.IsPlayGame = false;
                    LeapMotion(Menu.Main);
                }

                tablet_Menu.SetMenu(Menu.Main);
                return;
            }

            //그외는 기타 메뉴이동
            Menu tempMenu = Menu.None;

            if (!IsIndexOpen && !IsMiddleOpen && !IsPinkyOpen && !IsRingOpen && IsThumbOpen && settingModel.NowMenu != Menu.Main)
            {
                //엄지 상위 메뉴
                Debug.Log("메인");
                tempMenu = Menu.Main;
                LeapMotion(tempMenu);
            }

            if (!tabletMainMenu.activeSelf && !tabletMusicMenu.activeSelf && !tabletOptionMenu.activeSelf)
            {

                if (IsIndexOpen && !IsMiddleOpen && !IsPinkyOpen && !IsRingOpen && !IsThumbOpen && settingModel.NowMenu != Menu.Watch)
                {
                    //검지 시계
                    Debug.Log("시계");
                    tempMenu = Menu.Watch;
                    LeapMotion(tempMenu);
                }
                else if (IsIndexOpen && !IsMiddleOpen && !IsPinkyOpen && !IsRingOpen && IsThumbOpen && settingModel.NowMenu != Menu.Game)
                {
                    //검지 중지 약지 게임이동
                    Debug.Log("게임");
                    tempMenu = Menu.Game;
                    LeapMotion(tempMenu);
                }
                else if (IsIndexOpen && IsMiddleOpen && IsPinkyOpen && IsRingOpen && !IsThumbOpen && settingModel.NowMenu != Menu.HoloStar)
                {
                    //검지 중지 약지 새끼 홀로스타
                    Debug.Log("홀로스타");
                    tempMenu = Menu.HoloStar;
                    LeapMotion(tempMenu);
                }
                else if (IsIndexOpen && IsMiddleOpen && !IsPinkyOpen && !IsRingOpen && !IsThumbOpen && settingModel.NowMenu != Menu.Music)
                {
                    //검지 중지 음악재생
                    Debug.Log("음악재생");
                    tempMenu = Menu.Music;
                    LeapMotion(tempMenu);
                }
                else if (IsIndexOpen && IsMiddleOpen && IsPinkyOpen && IsRingOpen && IsThumbOpen && settingModel.NowMenu != Menu.Option)
                {
                    //엄지 검지 중지 약지 새끼 옵션
                    Debug.Log("옵션");
                    tempMenu = Menu.Option;
                    LeapMotion(tempMenu);
                }
            }
        }

        void MenuInfo()
        {
            if (!IsIndexOpen && !IsMiddleOpen && !IsPinkyOpen && !IsRingOpen && IsThumbOpen && settingModel.NowMenu != Menu.Main && !settingModel.IsPlayGame)
            {
                //엄지 상위 메뉴
                Message.Send<InfoMsg>(new InfoMsg("모션을 유지하면 메인으로 이동합니다."));
            }

            if ((tabletMainMenu.activeSelf || tabletMusicMenu.activeSelf || tabletOptionMenu.activeSelf) || settingModel.IsPlayGame)
                return;

            else if (IsIndexOpen && !IsMiddleOpen && !IsPinkyOpen && !IsRingOpen && !IsThumbOpen && settingModel.NowMenu != Menu.Watch)
            {
                //검지 시계
                Message.Send<InfoMsg>(new InfoMsg("모션을 유지하면 탁상시계로 이동합니다."));
            }
            else if (IsIndexOpen && !IsMiddleOpen && !IsPinkyOpen && !IsRingOpen && IsThumbOpen && settingModel.NowMenu != Menu.Game)
            {
                //검지 엄지 게임이동
                Message.Send<InfoMsg>(new InfoMsg("모션을 유지하면 리듬게임으로 이동합니다."));
            }
            else if (IsIndexOpen && IsMiddleOpen && IsPinkyOpen && IsRingOpen && !IsThumbOpen && settingModel.NowMenu != Menu.HoloStar)
            {
                //검지 중지 약지 새끼 홀로스타
                Message.Send<InfoMsg>(new InfoMsg("모션을 유지하면 홀로스타로 이동합니다."));
            }
            else if (IsIndexOpen && IsMiddleOpen && !IsPinkyOpen && !IsRingOpen && !IsThumbOpen && settingModel.NowMenu != Menu.Music)
            {
                //검지 중지 음악재생
                Message.Send<InfoMsg>(new InfoMsg("모션을 유지하면 음악재생으로 이동합니다."));
            }
            else if (IsIndexOpen && IsMiddleOpen && IsPinkyOpen && IsRingOpen && IsThumbOpen && settingModel.NowMenu != Menu.Option)
            {
                //엄지 검지 중지 약지 새끼 옵션
                Message.Send<InfoMsg>(new InfoMsg("모션을 유지하면 옵션으로 이동합니다."));
            }

        }

        void LeapMotion(Menu menu)
        {
            //yield return null;
            BluetoothData data = new BluetoothData();
            data.msg = "";
            data.dataType = SENDMSGTYPE.MENU;
            data.musicInfo = MUSICINFO.None;
            string dataMsg;

            if (!tabletMainMenu.activeSelf && !settingModel.IsPlayGame)
            {
                if (menu == Menu.Main)
                {
                    //엄지 상위 메뉴
                    Debug.Log("메인");
                    data.menu = Menu.Main;
                    dataMsg = JsonUtility.ToJson(data);
                    Message.Send<TabletMotionMsg>(new TabletMotionMsg(dataMsg));
                    RunMenu(new RunMenuMsg(Menu.None));
                }
                else if (menu == Menu.Watch)
                {
                    //검지 시계
                    Debug.Log("시계");
                    data.menu = Menu.Watch;
                    dataMsg = JsonUtility.ToJson(data);
                    Message.Send<TabletMotionMsg>(new TabletMotionMsg(dataMsg));
                    RunMenu(new RunMenuMsg(Menu.Watch));
                }
                else if (menu == Menu.Game)
                {
                    //검지 중지 약지 게임이동
                    Debug.Log("게임");
                    data.dataType = SENDMSGTYPE.GAME;
                    data.msg = GameType.RhythmGame.ToString(); ;
                    dataMsg = JsonUtility.ToJson(data);
                    Message.Send<TabletMotionMsg>(new TabletMotionMsg(dataMsg));
                }
                else if (menu == Menu.HoloStar)
                {
                    //검지 중지 약지 새끼 홀로스타
                    Debug.Log("홀로스타");
                    data.menu = Menu.HoloStar;
                    dataMsg = JsonUtility.ToJson(data);
                    Message.Send<TabletMotionMsg>(new TabletMotionMsg(dataMsg));
                    RunMenu(new RunMenuMsg(Menu.HoloStar));
                }
                else if (menu == Menu.Music)
                {
                    //검지 중지 음악재생
                    Debug.Log("음악재생");
                    data.menu = Menu.Music;
                    dataMsg = JsonUtility.ToJson(data);
                    Message.Send<TabletMotionMsg>(new TabletMotionMsg(dataMsg));
                    RunMenu(new RunMenuMsg(Menu.Music));
                }
                else if (menu == Menu.Option)
                {
                    //엄지 검지 중지 약지 새끼 옵션
                    Debug.Log("옵션");
                    data.menu = Menu.Option;
                    dataMsg = JsonUtility.ToJson(data);
                    Message.Send<TabletMotionMsg>(new TabletMotionMsg(dataMsg));
                    RunMenu(new RunMenuMsg(Menu.Option));
                }
                //tablet_Menu.MenuPosition(menu);
            }
            Debug.Log("제스쳐 인식");
        }

        void OnLeft()
        {
            Message.Send<CameraZoomMsg>(new CameraZoomMsg(false));
            Debug.Log(settingModel.NowMenu);
            if (settingModel.NowMenu == Menu.Music)
            {
                tablet_Menu.SetMenu(settingModel.NowMenu, settingModel.IsBluetoothConnet, holostarSettingModel.IsMMSReceive, holostarSettingModel.IsSchedule, holostarSettingModel.IsMute, holostarSettingModel.TabletVolume);
            }

            Debug.Log("왼손 인");
            isLeftHandIn = true;
            InputActive(Chirality.Left);
        }

        void OffLeft()
        {
            Debug.Log("왼손 아웃");
            isLeftHandIn = false;
            InputInactive(Chirality.Left);
        }

        void OnRight()
        {
            Message.Send<CameraZoomMsg>(new CameraZoomMsg(false));
            Debug.Log(settingModel.NowMenu);
            Debug.Log("오른손 인");
            if (settingModel.NowMenu == Menu.Music)
            {
                tablet_Menu.SetMenu(settingModel.NowMenu, settingModel.IsBluetoothConnet, holostarSettingModel.IsMMSReceive, holostarSettingModel.IsSchedule, holostarSettingModel.IsMute, holostarSettingModel.TabletVolume);
            }
            isRightHandIn = true;
            InputActive(Chirality.Right);
        }

        void OffRight()
        {
            Debug.Log("오른손 아웃");
            isRightHandIn = false;
            InputInactive(Chirality.Right);
        }

        void InputActive(Chirality hand)
        {
            ListPrevFingerActive.Clear();
            ListNowFingerActive.Clear();
            trace.Add(hand, StartCoroutine(TraceHandPos(hand)));
        }

        void InputInactive(Chirality hand)
        {
            if (trace.ContainsKey(hand))
            {
                StopCoroutine(trace[hand]);
                trace.Remove(hand);
            }
        }

        protected override void OnEnter()
        {
            AddMessage();
        }

        private void AddMessage()
        {
            Message.AddListener<STTBtnSetMsg>(STTBtnSet);
            Message.AddListener<VolumeChangeMsg>(VolumeChange);
            Message.AddListener<RunMenuMsg>(RunMenu);
            Message.AddListener<TabletOptionMsg>(TabletOption);
        }

        private void STTBtnSet(STTBtnSetMsg msg)
        {
            //tablet_Menu.SetSTTbtn(msg.isSet);
        }

        private void VolumeChange(VolumeChangeMsg msg)
        {
            Debug.Log(holostarSettingModel.TabletVolume);
            if (holostarSettingModel.TabletVolume >= 1)
            {
                holostarSettingModel.TabletVolume = 0;
            }
            else
            {
                holostarSettingModel.TabletVolume += 0.003f;
            }

            Message.Send<SetVolumeValueMsg>(new SetVolumeValueMsg(holostarSettingModel.TabletVolume));
        }

        private void RunMenu(RunMenuMsg msg)
        {
            if (msg.menu == Menu.Main)
            {
                tablet_Menu.SetMenu(Menu.None);
            }
            else if (msg.menu == Menu.Watch)
            {
                tablet_Menu.SetMenu(Menu.Watch);
            }
            else if (msg.menu == Menu.Game)
            {
                tablet_Menu.SetMenu(Menu.Game);
            }
            else if (msg.menu == Menu.HoloStar)
            {
                tablet_Menu.SetMenu(Menu.HoloStar);
            }
            else if (msg.menu == Menu.Music)
            {
                tablet_Menu.SetMenu(Menu.Music, settingModel.IsBluetoothConnet, holostarSettingModel.IsMMSReceive, holostarSettingModel.IsSchedule, holostarSettingModel.IsMute, holostarSettingModel.TabletVolume, holostarSettingModel.IsMusicPlay);
            }
            else if (msg.menu == Menu.Option)
            {
                tablet_Menu.SetMenu(Menu.Option, settingModel.IsBluetoothConnet, holostarSettingModel.IsMMSReceive, holostarSettingModel.IsSchedule, holostarSettingModel.IsMute, holostarSettingModel.TabletVolume, holostarSettingModel.IsMusicPlay);
            }
        }

        private void TabletOption(TabletOptionMsg msg)
        {
            if (msg.Option == OptionSet.Bluetooth)
            {

            }
            else if (msg.Option == OptionSet.Schedule)
            {
                holostarSettingModel.IsSchedule = !holostarSettingModel.IsSchedule;
            }
            else if (msg.Option == OptionSet.SMS)
            {
                holostarSettingModel.IsMMSReceive = !holostarSettingModel.IsMMSReceive;
            }
            else if (msg.Option == OptionSet.Sound)
            {
                holostarSettingModel.IsMute = !holostarSettingModel.IsMute;
            }

            tablet_Menu.SetMenu(settingModel.NowMenu, settingModel.IsBluetoothConnet, holostarSettingModel.IsMMSReceive, holostarSettingModel.IsSchedule, holostarSettingModel.IsMute, holostarSettingModel.TabletVolume);
        }

        protected override void OnExit()
        {
            RemoveMessage();
        }

        private void RemoveMessage()
        {
            Message.RemoveListener<STTBtnSetMsg>(STTBtnSet);
            Message.RemoveListener<VolumeChangeMsg>(VolumeChange);
            Message.RemoveListener<RunMenuMsg>(RunMenu);
            Message.RemoveListener<TabletOptionMsg>(TabletOption);
        }
    }
}