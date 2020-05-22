using System;
using UnityEngine;
using System.Collections.Generic;
using JHchoi.Constants;
using System.IO;

namespace JHchoi.Models
{
    public class SettingModel : Model
    {
        GameModel _owner;

        public LocalizingType LocalizingType = LocalizingType.KR;

        public void Setup(GameModel owner)
        {
            LoadSettingFile();
            _owner = owner;
        }

        private void LoadSettingFile()
        {
            string line;
            string pathBasic = Application.dataPath + "/StreamingAssets/";
            string path = "Setting/Setting.txt";
            using (System.IO.StreamReader file = new System.IO.StreamReader(@pathBasic + path))
            {
                while ((line = file.ReadLine()) != null)
                {
                    if (line.Contains(";") || string.IsNullOrEmpty(line))
                        continue;

                    if (line.StartsWith("Localizing"))
                        LocalizingType = (LocalizingType)int.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("port"))
                        port = line.Split('=')[1];
                    else if (line.StartsWith("baud"))
                        baud = int.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("IsSingle"))
                        isSingle = bool.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("IsLooking"))
                        UseLookingGless = bool.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("IsTablet"))
                        UseTablet = bool.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("UseWakeUp"))
                        UseWakeUp = bool.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("RecordDelay"))
                        RecordDelay = float.Parse(line.Split('=')[1]);
                }
                file.Close();
                line = string.Empty;
            }
        }

        Menu nowMenu = Menu.Main;
        float completeSpanTime = 30;               //second      
        float managementRecoverTime = 10;        //second
        int scheduleCount = 20;
        int manageMentTotalCount = 20;
        bool isBluetoothConnet = false;
        bool isSingle;
        string port;
        int baud;
        private bool isPlayGame = false;
        public bool UseLookingGless = true;
        public bool UseLeapMotion = true;
        public bool UseWakeUp = false;
        public bool UseTablet = false;
        public bool isOpenMenu;
        public float RecordDelay;

        public float CompleteSpanTime { get => completeSpanTime; set => completeSpanTime = value; }
        public float ManagementRecoverTime { get => managementRecoverTime; set => managementRecoverTime = value; }
        public int ScheduleCount { get => scheduleCount; set => scheduleCount = value; }
        public int ManageMentTotalCount { get => manageMentTotalCount; set => manageMentTotalCount = value; }
        public bool IsBluetoothConnet { get => isBluetoothConnet; set => isBluetoothConnet = value; }
        public string Port { get => port; set => port = value; }
        public int Baud { get => baud; set => baud = value; }
        public Menu NowMenu { get => nowMenu; set => nowMenu = value; }
        public bool IsPlayGame { get => isPlayGame; set => isPlayGame = value; }
        public bool IsSingle { get => isSingle; set => isSingle = value; }

        public string GetLocalizingPath()
        {
            string path = "";
            switch (LocalizingType)
            {
                case LocalizingType.KR: path = "KR"; break;
                case LocalizingType.JP: path = "JP"; break;
                case LocalizingType.EN: path = "EN"; break;
                case LocalizingType.CH: path = "CH"; break;
            }
            return LocalizingType.ToString();
        }

        public string GetPlayer()
        {
            if (UseTablet)
                return "Surface";
            else
                return "Pc";

        }
    }
}
