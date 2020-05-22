using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JHchoi.Constants;

namespace JHchoi.UI.Event
{
    public class SetCharacterDressMsg : Message
    {
        public Character character;
        public int dressNum;

        public SetCharacterDressMsg(Character character, int dressNum)
        {
            this.character = character;
            this.dressNum = dressNum;
        }
    }

    public class SetCharacterAnimationMsg : Message
    {
        public bool isBluetoothCommand;
        public AnimationType animationType;
        public SetCharacterAnimationMsg(AnimationType animationType, bool isBluetoothCommand)
        {
            this.animationType = animationType;
            this.isBluetoothCommand = isBluetoothCommand;
        }
    }

    public class SetSituationAniMsg : Message
    {
        public bool isSituationStart;
        public SetSituationAniMsg(bool isSituationStart)
        {
            this.isSituationStart = isSituationStart;
        }
    }

    public class CharacterTouchMsg : Message
    {
        public AnimationType aniType;
        public CharacterTouchMsg(AnimationType ani)
        {
            aniType = ani;
        }
    }

    public class WakeUpTTSMsg : Message
    {

    }

    public class MusicRequestMsg : Message
    {
        public MUSICINFO musicInfo;
        public string msg;
        public MusicRequestMsg(MUSICINFO musicInfo, string msg)
        {
            this.musicInfo = musicInfo;
            this.msg = msg;
        }
    }

    public class InfoMsg : Message
    {
        public string msg;
        public InfoMsg(string msg)
        {
            this.msg = msg;
        }
    }

    public class SetInfoPositionMsg : Message
    {
        public bool isSingle;
        public bool isTablet;
        public bool isLooking;

        public SetInfoPositionMsg(bool isSingle, bool isTablet, bool isLooking)
        {
            this.isSingle = isSingle;
            this.isTablet = isTablet;
            this.isLooking = isLooking;
        }

    }

    public class RhythmGameStartMsg : Message
    {

    }

    public class RhythmGameNoteJudgeMsg : Message
    {
        public RhythmNote rhythmNote;
        public RhythmGameNoteJudgeMsg(RhythmNote rhythmNote)
        {
            this.rhythmNote = rhythmNote;
        }
    }

    public class RhythmGameMusicSelectMsg : Message
    {
        public int musicIndex;
        public RhythmGameMusicSelectMsg(int musicIndex)
        {
            this.musicIndex = musicIndex;
        }
    }

    public class RhythmGameNoteCreateMsg : Message
    {
        public bool isRigth;
        public int noteCount;
        public RhythmGameNoteCreateMsg(bool isRight, int noteCount)
        {
            this.isRigth = isRight;
            this.noteCount = noteCount;
        }
    }

    public class RhythmGameBtnTouchMsg : Message
    {
        public bool isRight;
        public RhythmGameBtnTouchMsg(bool isRight)
        {
            this.isRight = isRight;
        }
    }

    public class RhythmGameResultMsg : Message
    {
        public int bad;
        public int normal;
        public int good;
        public int perfect;
        public int combo;
        public float score;
        public RhythmGameResultMsg(int bad, int normal, int good, int perfect, int combo, float score)
        {
            this.bad = bad;
            this.normal = normal;
            this.good = good;
            this.perfect = perfect;
            this.combo = combo;
            this.score = score;
        }
    }

    public class RhythmGameNoteDeleteMsg : Message
    {
        public RhythmNote rhythmNote;
        public int index;
        public GameObject note;
        public bool isRight;
        public bool isNoTouch;
        public RhythmGameNoteDeleteMsg(RhythmNote rhythmNote, int index, GameObject note, bool isRight, bool isNoTouch)
        {
            this.rhythmNote = rhythmNote;
            this.index = index;
            this.note = note;
            this.isRight = isRight;
            this.isNoTouch = isNoTouch;
        }
    }

    public class TabletMenuButtonSetMsg : Message
    {
        public Menu menu;
        public TabletMenuButtonSetMsg(Menu menu)
        {
            this.menu = menu;
        }
    }

    public class TabletOptionMsg : Message
    {
        public OptionSet Option;
        public TabletOptionMsg(OptionSet Option)
        {
            this.Option = Option;
        }
    }

    public class CameraZoomMsg : Message
    {
        public bool isZoom;
        public CameraZoomMsg(bool isZoom)
        {
            this.isZoom = isZoom;
        }
    }

    public class STTBtnSetMsg : Message
    {
        public bool isSet;
        public STTBtnSetMsg(bool isSet)
        {
            this.isSet = isSet;
        }
    }

    public class VolumeChangeMsg : Message
    {

    }

    public class SetVolumeValueMsg: Message
    {
        public float volume;
        public SetVolumeValueMsg(float volume)
        {
            this.volume = volume;
        }
    }

    public class SetMusicPlayButtonMsg : Message
    {
        public bool isPlay;
        public SetMusicPlayButtonMsg(bool isPlay)
        {
            this.isPlay = isPlay;
        }
    }

    public class WeatherRequestMsg : Message
    {
        public Character character;
        public WeatherRequestMsg(Character character)
        {
            this.character = character;
        }
    }

    public class EmotionAniMationMsg : Message
    {
        public int index;
        public EmotionAniMationMsg(int index)
        {
            this.index = index;
        }
    }
}