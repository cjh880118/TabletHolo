using JHchoi.Constants;
using JHchoi.Models;
using JHchoi.UI.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace JHchoi.Contents
{
    public class TabletRhythmContent : IContent
    {
        static string TAG = "RhythmGameContent :: ";
        BT_Sound bT_Sound;
        NoteModel noteModel;
        PlayerInventoryModel playerInventoryModel;
        HolostarSettingModel holostarSettingModel;
        SettingModel settingModel;
        List<BT_Sound.Param> ListRhythmGameMusic = new List<BT_Sound.Param>();
        List<Coroutine> ListCoroutine = new List<Coroutine>();
        List<Dictionary<string, object>> noteData;
        RhythmGame_Controller rhythmGame_Controller;
        GameObject LeapModule;

        int nowCombo;
        int maxCombo;
        int perfect;
        int good;
        int normal;
        int bad;
        float score;

        protected override void OnLoadStart()
        {
            LeapModule = GameObject.Find("LeapMotion");
            noteModel = Model.First<NoteModel>();
            playerInventoryModel = Model.First<PlayerInventoryModel>();
            holostarSettingModel = Model.First<HolostarSettingModel>();
            settingModel = Model.First<SettingModel>();
            bT_Sound = TableManager.Instance.GetTableClass<BT_Sound>();
            foreach (var o in bT_Sound.sheets[0].list)
            {
                if (o.isGame)
                    ListRhythmGameMusic.Add(o);
            }

            StartCoroutine(LoadInitialData());
        }

        IEnumerator LoadInitialData()
        {
            string path = "Object/NewUI/RythmObject";
            yield return StartCoroutine(ResourceLoader.Instance.Load<GameObject>(path,
               o =>
               {
                   var inGameObject = Instantiate(o) as GameObject;
                   rhythmGame_Controller = inGameObject.GetComponent<RhythmGame_Controller>();
               }));

            SetLoadComplete();
        }


        protected override void OnEnter()
        {
            Debug.Log("리듬게임 시작");
            AddMessage();
        }

        private void AddMessage()
        {
            Message.AddListener<RhythmGameStartMsg>(RhythmGameStart);
            Message.AddListener<RhythmGameNoteJudgeMsg>(RhythmGameNoteJudge);
            Message.AddListener<RhythmGameMusicSelectMsg>(RhythmGameMusicSelect);
        }

        private void RhythmGameStart(RhythmGameStartMsg msg)
        {
            Message.Send<InfoMsg>(new InfoMsg("잠시후에 게임이 시작됩니다."));
            WindowBluetooth.GetInstance().SendBluetoothMsg("rhythm", SENDMSGTYPE.MENU, MUSICINFO.None, Menu.Game);
            StartCoroutine(StartGaemDelay());
        }

        IEnumerator StartGaemDelay()
        {
            yield return new WaitForSeconds(3.0f);
            rhythmGame_Controller.gameObject.SetActive(true);
            rhythmGame_Controller.InitRhythmGameNote();
            int num = UnityEngine.Random.Range(5, 10);
            RhythmGameMusicSelect(new RhythmGameMusicSelectMsg(num));
            settingModel.IsPlayGame = true;
        }

        private void RhythmGameNoteJudge(RhythmGameNoteJudgeMsg msg)
        {
            if (msg.rhythmNote == RhythmNote.Bad)
            {
                nowCombo = 0;
                bad++;
                Debug.Log("bad : " + bad);
            }
            else if (msg.rhythmNote == RhythmNote.Normal)
            {
                nowCombo++;
                normal++;
                Debug.Log("Normal : " + normal);
            }
            else if (msg.rhythmNote == RhythmNote.Good)
            {
                nowCombo++;
                good++;
                Debug.Log("Good : " + good);
            }
            else if (msg.rhythmNote == RhythmNote.Perfect)
            {
                nowCombo++;
                perfect++;
                Debug.Log("Perfect : " + perfect);
            }

            if (nowCombo > maxCombo)
                maxCombo = nowCombo;

        }

        private void RhythmGameMusicSelect(RhythmGameMusicSelectMsg msg)
        {
            nowCombo = 0;
            maxCombo = 0;
            perfect = 0;
            good = 0;
            normal = 0;
            bad = 0;
            score = 0;

            SoundManager.Instance.PlaySound(msg.musicIndex, holostarSettingModel.HoloOptionSetting.TabletVolume);
            Debug.Log("음악 번호 : " + msg.musicIndex);
            int aniNum = UnityEngine.Random.Range((int)AnimationType.Motion1, (int)AnimationType.Motion7 + 1);
            Message.Send<SetCharacterAnimationMsg>(new SetCharacterAnimationMsg((AnimationType)aniNum, false));
            foreach (var o in ListRhythmGameMusic)
            {
                if (o.Index == msg.musicIndex)
                {
                    noteData = noteModel.GetNoteData(o.Title);
                    break;
                }
            }

            ListCoroutine.Add(StartCoroutine(PlayNote()));
        }

        public IEnumerator PlayNote()
        {
            int currentNoteCount = 0;
            float noteTerm = noteModel.NoteTerm;

            while (currentNoteCount < noteData.Count)
            {
                float term = 0;
                float currentTime = float.Parse(noteData[currentNoteCount]["Time"].ToString());

                // 데이터 테이블의 첫번쨰 인자(시간) 동안 대기.
                if (currentNoteCount > 0)
                    term = (currentTime - noteTerm) - (float.Parse(noteData[currentNoteCount - 1]["Time"].ToString()) - noteTerm);
                else
                    term = (currentTime - noteTerm);


                yield return new WaitForSeconds(term);

                if (noteData[currentNoteCount]["0"].ToString() == "1")
                {
                    //왼쪽노트
                    Message.Send<RhythmGameNoteCreateMsg>(new RhythmGameNoteCreateMsg(false, currentNoteCount));
                }

                if (noteData[currentNoteCount]["1"].ToString() == "1")
                {
                    //오른쪽 노트
                    Message.Send<RhythmGameNoteCreateMsg>(new RhythmGameNoteCreateMsg(true, currentNoteCount));
                }

                currentNoteCount++;
            }

            StartCoroutine(RhythmGameEnd());
        }

        IEnumerator RhythmGameEnd()
        {
            yield return new WaitForSeconds(3.0f);
            int aniNum = UnityEngine.Random.Range((int)AnimationType.Idel1, (int)AnimationType.Idel3 + 1);
            Message.Send<SetCharacterAnimationMsg>(new SetCharacterAnimationMsg((AnimationType)aniNum, false));
            UI.IDialog.RequestDialogEnter<UI.RhythmGameResultDialog>();
            score = ((((normal + good + perfect) / noteData.Count) * 0.8f) +
                (((good + perfect) / noteData.Count) * 0.07f) +
                (((perfect) / noteData.Count) * 0.13f)) * 100;
            Message.Send<RhythmGameResultMsg>(new RhythmGameResultMsg(bad, normal, good, perfect, maxCombo, score));
            StartCoroutine(RhythmGameResultClose());
        }

        IEnumerator RhythmGameResultClose()
        {
            yield return new WaitForSeconds(5.0f);
            UI.IDialog.RequestDialogExit<UI.RhythmGameResultDialog>();
            settingModel.IsPlayGame = false;
            rhythmGame_Controller.GameEnd();

            BluetoothData data = new BluetoothData();
            data.msg = "";
            data.dataType = SENDMSGTYPE.MENU;
            data.musicInfo = MUSICINFO.None;
            string dataMsg;
            data.menu = Menu.Main;
            dataMsg = JsonUtility.ToJson(data);
            Message.Send<TabletMotionMsg>(new TabletMotionMsg(dataMsg));
            Message.Send<RunMenuMsg>(new RunMenuMsg(Menu.None));
        }

        protected override void OnExit()
        {
            foreach (var o in ListCoroutine)
            {
                if (o != null)
                    StopCoroutine(o);
            }

            settingModel.IsPlayGame = false;
            rhythmGame_Controller.GameEnd();
            rhythmGame_Controller.gameObject.SetActive(false);
            SoundManager.Instance.StopAllSound();
            DialogAllClose();
            RemoveMessage();
        }

        private void RemoveMessage()
        {
            Message.RemoveListener<RhythmGameStartMsg>(RhythmGameStart);
            Message.RemoveListener<RhythmGameNoteJudgeMsg>(RhythmGameNoteJudge);
            Message.RemoveListener<RhythmGameMusicSelectMsg>(RhythmGameMusicSelect);
        }
    }
}
