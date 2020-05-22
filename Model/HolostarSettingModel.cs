using JHchoi.Contents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JHchoi.Models
{
    public class HolostarSettingModel : Model
    {
        GameModel _owner;

        public void Setup(GameModel owner)
        {
            _owner = owner;
        }

        HoloStarSetting setting;

        public bool IsMusicPlay { get => setting.musicSetting.isMusicPlay; set => setting.musicSetting.isMusicPlay = value; }
        public bool IsRepeat { get => setting.musicSetting.isRepeat; set => setting.musicSetting.isRepeat = value; }
        public int MusicIndex { get => setting.musicSetting.musicIndex; set => setting.musicSetting.musicIndex = value; }
        public double MusicNowTime { get => setting.musicSetting.musicNowTime; set => setting.musicSetting.musicNowTime = value; }
        public float MusicVolume { get => setting.musicSetting.musicVolume; set => setting.musicSetting.musicVolume = value; }

        public float TabletVolume { get => setting.holoOptionSetting.TabletVolume; set => setting.holoOptionSetting.TabletVolume = value; }

        public bool IsTTSReceive { get => setting.holoOptionSetting.isTTSReceive; set => setting.holoOptionSetting.isTTSReceive = value; }
        public bool IsMMSReceive { get => setting.holoOptionSetting.isMMSReceive; set => setting.holoOptionSetting.isMMSReceive = value; }
        public bool IsSchedule { get => setting.holoOptionSetting.isSchedule; set => setting.holoOptionSetting.isSchedule = value; }
        public bool IsMute { get => setting.holoOptionSetting.isMute; set => setting.holoOptionSetting.isMute = value; }

        public MusicSetting MusicSetting { get => setting.musicSetting; set => setting.musicSetting = value; }
        public HoloOptionSetting HoloOptionSetting { get => setting.holoOptionSetting; set => setting.holoOptionSetting = value; }
        public HoloStarSetting HoloStarSetting { get => setting; set => setting = value; }
    }
}