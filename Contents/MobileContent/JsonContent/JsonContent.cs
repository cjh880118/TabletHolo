using JHchoi.Constants;
using JHchoi.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace JHchoi.Contents
{
    public class JsonContent : IContent
    {
        //스케줄
        GameSchedule gameSchedule;
        List<AloneGameSchedule> listAlongGameSchedule;
        Dictionary<int, AloneGameSchedule> dicSchedule;
        //플레이어 정보
        PlayerStatus playerStatus;
        //플레이어 인벤토리
        PlayerInventory playerInventory;
        HoloStarSetting holostarSetting;

        ScheduleModel scheduleModel;
        PlayerStatusModel playerStatusModel;
        PlayerInventoryModel playerInventoryModel;
        SettingModel settingModel;
        HolostarSettingModel holostarSettingModel;
        CalendarModel calendarModel;
        CalanderEvent calanderEvent;

        protected override void OnLoadStart()
        {
            gameSchedule = new GameSchedule();
            listAlongGameSchedule = new List<AloneGameSchedule>();
            dicSchedule = new Dictionary<int, AloneGameSchedule>();
            playerStatus = new PlayerStatus();
            playerInventory = new PlayerInventory();
            holostarSetting = new HoloStarSetting();
            calanderEvent = new CalanderEvent();

            scheduleModel = Model.First<ScheduleModel>();
            playerStatusModel = Model.First<PlayerStatusModel>();
            playerInventoryModel = Model.First<PlayerInventoryModel>();
            settingModel = Model.First<SettingModel>();
            holostarSettingModel = Model.First<HolostarSettingModel>();
            calendarModel = Model.First<CalendarModel>();

            CheckFile("PlayerInventory");
            CheckFile("HoloStarSetting");
            CheckFile("Calendar");
            SetLoadComplete();
        }

        void CheckFile(string fileName)
        {
            string path = Application.dataPath + "/Resources/Json/" + fileName + ".json";
#if (UNITY_EDITOR)
            path = Application.dataPath + "/Resources/Json/" + fileName + ".json";

#elif(UNITY_STANDALONE_WIN)
            path = Application.dataPath + "/StreamingAssets/Json/" + fileName + ".json";

#elif (UNITY_ANDROID)

            string sDirPath;
            sDirPath = Application.persistentDataPath + "/Json";
            DirectoryInfo di = new DirectoryInfo(sDirPath);
            if (di.Exists == false)
            {
                di.Create();
            }

             path = Application.persistentDataPath + "/Json/" + fileName + ".json";
#endif
            FileInfo fi = new FileInfo(path);
            if (fi.Exists)
            {
                if (fileName == "PlayerStatus")
                {
                    playerStatus = LoadJsonFile<PlayerStatus>("PlayerStatus");
                    playerStatusModel.CharacterStatus = playerStatus;
                }
                else if (fileName == "Schedule")
                {
                    gameSchedule = LoadJsonFile<GameSchedule>("Schedule");
                    for (int i = 0; i < gameSchedule.alonGameShedule.Length; i++)
                    {
                        listAlongGameSchedule.Add(gameSchedule.alonGameShedule[i]);
                        dicSchedule.Add(i, gameSchedule.alonGameShedule[i]);
                    }
                    scheduleModel.DicSchedule = dicSchedule;
                }
                else if (fileName == "PlayerInventory")
                {
                    playerInventory = LoadJsonFile<PlayerInventory>("PlayerInventory");
                    playerInventoryModel.PlayerInventory = playerInventory;
                }
                else if (fileName == "HoloStarSetting")
                {
                    holostarSetting = LoadJsonFile<HoloStarSetting>("HoloStarSetting");
                    holostarSettingModel.HoloStarSetting = holostarSetting;
                }
                else if (fileName == "Calendar")
                {
                    calanderEvent = LoadJsonFile<CalanderEvent>("Calendar");
                    calendarModel.AlarmEvents = calanderEvent.alarmEvents.ToList();
                }
            }
            else
            {
                if (fileName == "PlayerStatus")
                {
                    CreatePlayerStatus();
                    playerStatusModel.CharacterStatus = playerStatus;
                }
                else if (fileName == "Schedule")
                {
                    CreateScheduleJson();

                    for (int i = 0; i < listAlongGameSchedule.Count; i++)
                    {
                        dicSchedule.Add(i, listAlongGameSchedule[i]);
                    }

                    scheduleModel.DicSchedule = dicSchedule;
                }
                else if (fileName == "PlayerInventory")
                {
                    CreatePlayerInventoryJson();
                    playerInventoryModel.PlayerInventory = playerInventory;
                }
                else if (fileName == "HoloStarSetting")
                {
                    CreateSettingJson();
                    holostarSettingModel.HoloStarSetting = holostarSetting;
                }
            }
        }

        T LoadJsonFile<T>(string fileName)
        {
            string path = Application.dataPath + "/Resources/Json/"; ;
#if (UNITY_EDITOR)
            path = Application.dataPath + "/Resources/Json/";
            //path = Application.dataPath + "/StreamingAssets/Json/";
#elif (UNITY_STANDALONE_WIN)
            path = Application.dataPath + "/StreamingAssets/Json/";
#elif (UNITY_ANDROID)
             path = Application.persistentDataPath + "/Json/";
#endif
            FileStream fileStream = new FileStream(string.Format("{0}/{1}.json", path, fileName), FileMode.Open);
            byte[] data = new byte[fileStream.Length];
            fileStream.Read(data, 0, data.Length);
            fileStream.Close();
            string jsonData = Encoding.UTF8.GetString(data);
            return JsonUtility.FromJson<T>(jsonData);
        }

        //최초 케릭터 정보 생성
        void CreatePlayerStatus()
        {
            playerStatus.vocal = 0;
            playerStatus.vocalGage = 0;
            playerStatus.vocalSkill = 0;
            playerStatus.dance = 0;
            playerStatus.danceGage = 0;
            playerStatus.danceSkill = 0;
            playerStatus.entertainment = 0;
            playerStatus.entertainmentGage = 0;
            playerStatus.intelligence = 0;
            playerStatus.intelligenceGage = 0;
            playerStatus.intelligenceSkill = 0;
            playerStatus.coin = 0;
            playerStatus.potential = 0;
            playerStatus.relaxSkill = 0;
            playerStatus.relaxSkill = 0;
            playerStatus.selfManagementSkill = 0;
            playerStatus.manageMentCout = 20;
            string temp = JsonUtility.ToJson(playerStatus);
            CreateJsonFile("PlayerStatus", temp);
        }

        //최초 스케줄 정보 생성
        void CreateScheduleJson()
        {
            for (int i = 0; i < settingModel.ScheduleCount; i++)
            {
                System.DateTime startTime = System.DateTime.Now.AddSeconds(i * settingModel.CompleteSpanTime);
                Schedule tempSchedule;

                if (i % 12 == 1 || i % 12 == 5)
                {
                    tempSchedule = Schedule.Meal;
                }
                else if (i % 12 == 9 || i % 12 == 10)
                {
                    tempSchedule = Schedule.Rest;
                }
                else
                {
                    tempSchedule = (Schedule)UnityEngine.Random.Range(0, 4);
                }
                listAlongGameSchedule.Add(CreateSchedule(i, tempSchedule, startTime.ToString()));
            }

            gameSchedule.alonGameShedule = listAlongGameSchedule.ToArray();
            string temp = JsonUtility.ToJson(gameSchedule);
            CreateJsonFile("Schedule", temp);
        }

        //최초 플레이어 인벤토리 생성
        private void CreatePlayerInventoryJson()
        {
            playerInventory.nowCharacter = Character.Girl;
            playerInventory.nowSkin = 1;

            List<Character> listBuyCharacter = new List<Character>();
            listBuyCharacter.Add(Character.Girl);
            playerInventory.buyCharacter = listBuyCharacter.ToArray();

            List<int> listBuySkin = new List<int>();
            listBuySkin.Add(0);
            listBuySkin.Add(1);
            playerInventory.buySkinNum = listBuySkin.ToArray();

            string temp = JsonUtility.ToJson(playerInventory);
            CreateJsonFile("PlayerInventory", temp);
        }

        AloneGameSchedule CreateSchedule(int index, Schedule schedule, string dateTime)
        {
            AloneGameSchedule alonGameShedule = new AloneGameSchedule();
            alonGameShedule.index = index;
            alonGameShedule.schedule = schedule;
            alonGameShedule.dateTime = dateTime;
            return alonGameShedule;
        }

        void CreateSettingJson()
        {
            holostarSetting.holoOptionSetting = new HoloOptionSetting();
            holostarSetting.musicSetting = new MusicSetting();

            holostarSetting.holoOptionSetting.isMMSReceive = false;
            holostarSetting.holoOptionSetting.isTTSReceive = false;
            holostarSetting.holoOptionSetting.isSchedule = false;
            holostarSetting.holoOptionSetting.isMute = false;
            holostarSetting.holoOptionSetting.TabletVolume = 1.0f;

            holostarSetting.musicSetting.isMusicPlay = false;
            holostarSetting.musicSetting.musicVolume = 1.0f;
            holostarSetting.musicSetting.musicIndex = 0;
            holostarSetting.musicSetting.isRepeat = true;
            holostarSetting.musicSetting.musicNowTime = 0;

            string temp = JsonUtility.ToJson(holostarSetting);
            CreateJsonFile("HoloStarSetting", temp);
        }

        void CreateJsonFile(string fileName, string jsonData)
        {
            string path = Application.dataPath + "/Resources/Json/"; ;
#if (UNITY_EDITOR)
            //path = Application.dataPath + "/StreamingAssets/Json/";
            path = Application.dataPath + "/Resources/Json/";

#elif (UNITY_STANDALONE_WIN)
            path = Application.dataPath + "/StreamingAssets/Json/";
#elif (UNITY_ANDROID)
             path = Application.persistentDataPath + "/Json/";
#endif

            FileStream fileStream = new FileStream(string.Format("{0}/{1}.json", path, fileName), FileMode.Create);

            byte[] data = Encoding.UTF8.GetBytes(jsonData);
            fileStream.Write(data, 0, data.Length);
            fileStream.Close();
        }

        //종료 직전 모델 데이터 json 으로 저장
        protected void OnDestroy()
        {
            holostarSettingModel.HoloStarSetting.musicSetting.isMusicPlay = false;
            SaveJson("PlayerInventory", playerInventoryModel.PlayerInventory);
            SaveJson("HoloStarSetting", holostarSettingModel.HoloStarSetting);
            CalanderEvent calanderEvent = new CalanderEvent();
            calanderEvent.alarmEvents = calendarModel.AlarmEvents.ToArray();
            SaveJson("Calendar", calanderEvent);
        }

        void SaveJson(string fileName, object o)
        {
            string temp = JsonUtility.ToJson(o);
            CreateJsonFile(fileName, temp);
        }

        protected override void OnEnter()
        {

        }

        protected override void OnExit()
        {

        }
    }
}