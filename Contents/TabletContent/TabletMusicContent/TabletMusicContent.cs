using CellBig.Constants;
using CellBig.UI.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Android;
using CellBig.Models;

namespace CellBig.Contents
{
    public class TabletMusicContent : IContent
    {
        int musicIndex;
        bool isRandomPlay = false;
        bool isPause = false;
        Playlist playlist;
        public AudioSource audioSource;
        HolostarSettingModel holostarSettingModel;
        SettingModel settingModel;
        GameObject audioVisualizer;
        Coroutine corMusicCheck;
        GameObject LeapModule;

        protected override void OnLoadStart()
        {
            LeapModule = GameObject.Find("LeapMotion");
            holostarSettingModel = Model.First<HolostarSettingModel>();
            settingModel = Model.First<SettingModel>();
            StartCoroutine(LoadInitialData());
        }

        IEnumerator LoadInitialData()
        {
            string path = "Object/UI/AudioVisualizer";
            yield return StartCoroutine(ResourceLoader.Instance.Load<GameObject>(path,
               o =>
               {
                   var inGameObject = Instantiate(o) as GameObject;
                   inGameObject.GetComponent<SpectrumElement>().InitAudioSpectrum();
                   inGameObject.transform.localPosition = new Vector3(0, 0, 0);
                   inGameObject.transform.localScale = new Vector3(0.06f, 0.06f, 1);
                   audioVisualizer = inGameObject;
                   audioVisualizer.SetActive(false);
               }));

            SetLoadComplete();
        }

        protected override void OnEnter()
        {
            AddMessage();
            SetAudioVisualizer();
            //LeapModule.transform.position = new Vector3(0, 0, 5.4f);
            playlist = WindowMusic.GetInstance().GetPlaylist();
            holostarSettingModel.HoloStarSetting.musicSetting.musicNowTime = audioSource.time;
            string tempJsonMsg = JsonUtility.ToJson(holostarSettingModel.HoloStarSetting);
            WindowBluetooth.GetInstance().SendBluetoothMsg(tempJsonMsg, SENDMSGTYPE.SETTING);

            if (holostarSettingModel.IsMusicPlay)
            {
                int motionNum = UnityEngine.Random.Range((int)AnimationType.Motion1, (int)AnimationType.Motion7 + 1);
                SetAniMation(motionNum);
                StartCoroutine(NowPlayingMusic());
            }
        }

        void SetAudioVisualizer()
        {
            audioVisualizer.SetActive(true);
            audioVisualizer.transform.parent = GameObject.Find("visualPar").transform;
            audioVisualizer.transform.localPosition = new Vector3(0, -100, 0f);
            audioVisualizer.transform.localScale = new Vector3(40f, 40f, 1);
        }

        IEnumerator NowPlayingMusic()
        {
            WindowBluetooth.GetInstance().SendBluetoothMsg(holostarSettingModel.MusicIndex.ToString(), SENDMSGTYPE.MUSIC, MUSICINFO.MUSIC_PLAY);
            yield return new WaitForSeconds(0.5f);
            corMusicCheck = StartCoroutine(CheckMusic());
        }

        private void AddMessage()
        {
            Message.AddListener<MusicRequestMsg>(MusicRequest);
        }

        private void MusicRequest(MusicRequestMsg msg)
        {
            if (msg.musicInfo == MUSICINFO.MUSIC_LIST)
            {
                //처음
                //string temp = JsonUtility.ToJson(playlist);
                //WindowBluetooth.GetInstance().SendBluetoothMsg(temp, SENDMSGTYPE.MUSIC, MUSICINFO.MUSIC_LIST);

                WindowBluetooth.GetInstance().SendBluetoothMsg("start", SENDMSGTYPE.MUSIC, MUSICINFO.MUSIC_LIST);


                for (int i = 0; i < playlist.data.Count; i++)
                {
                    string abc = JsonUtility.ToJson(playlist.data[i]);
                    Debug.Log(abc);
                    WindowBluetooth.GetInstance().SendBluetoothMsg(abc, SENDMSGTYPE.MUSIC, MUSICINFO.MUSIC_LIST);
                }

                WindowBluetooth.GetInstance().SendBluetoothMsg("end", SENDMSGTYPE.MUSIC, MUSICINFO.MUSIC_LIST);
            }
            else if (msg.musicInfo == MUSICINFO.MUSIC_PLAY)
            {
                if (isPause && audioSource.clip != null)
                    audioSource.UnPause();
                else
                {
                    holostarSettingModel.MusicIndex = 0;
                    StartCoroutine(GetAudio(0));
                }
                
                int motionNum = UnityEngine.Random.Range((int)AnimationType.Motion1, (int)AnimationType.Motion7 + 1);
                SetAniMation(motionNum);

                holostarSettingModel.IsMusicPlay = true;
                Message.Send<SetMusicPlayButtonMsg>(new SetMusicPlayButtonMsg(holostarSettingModel.IsMusicPlay));
                corMusicCheck = StartCoroutine(CheckMusic());
            }
            else if (msg.musicInfo == MUSICINFO.MUSIC_SELECT)
            {
                StartCoroutine(GetAudio((int.Parse(msg.msg))));
                holostarSettingModel.MusicIndex = int.Parse(msg.msg);
                holostarSettingModel.IsMusicPlay = true;
                Message.Send<SetMusicPlayButtonMsg>(new SetMusicPlayButtonMsg(holostarSettingModel.IsMusicPlay));
                int motionNum = UnityEngine.Random.Range((int)AnimationType.Motion1, (int)AnimationType.Motion7 + 1);
                SetAniMation(motionNum);
                corMusicCheck = StartCoroutine(CheckMusic());
            }
            else if (msg.musicInfo == MUSICINFO.MUSIC_PAUSE)
            {
                isPause = true;
                audioSource.Pause();
                holostarSettingModel.IsMusicPlay = false;
                Message.Send<SetMusicPlayButtonMsg>(new SetMusicPlayButtonMsg(holostarSettingModel.IsMusicPlay));
                int motionNum = UnityEngine.Random.Range((int)AnimationType.Idel1, (int)AnimationType.Idel2 + 1);
                SetAniMation(motionNum);
                if (corMusicCheck != null) StopCoroutine(corMusicCheck);
            }
            else if (msg.musicInfo == MUSICINFO.MUSIC_NEXT)
            {
                int tempIndex;
                audioSource.Stop();
                if (isRandomPlay)
                {
                    int listNum = 0;
                    for (int i = 0; i < playlist.data.Count; i++)
                    {
                        if (playlist.data[i].index == musicIndex)
                        {
                            listNum = i;
                            break;
                        }
                    }

                    if (listNum + 1 > playlist.data.Count - 1)
                    {
                        listNum = 0;
                    }
                    else
                    {
                        listNum = listNum + 1;
                    }
                    musicIndex = playlist.data[listNum].index;
                }
                else
                {
                    int musicNum;
                    do
                    {
                        tempIndex = UnityEngine.Random.Range(0, playlist.data.Count);
                        musicNum = playlist.data[tempIndex].index;
                    } while (playlist.data[tempIndex].index == musicIndex);

                    musicIndex = musicNum;
                }

                holostarSettingModel.MusicIndex = musicIndex;
                holostarSettingModel.IsMusicPlay = true;
                Message.Send<SetMusicPlayButtonMsg>(new SetMusicPlayButtonMsg(holostarSettingModel.IsMusicPlay));
                int motionNum = UnityEngine.Random.Range((int)AnimationType.Motion1, (int)AnimationType.Motion7 + 1);
                SetAniMation(motionNum);
                StartCoroutine(GetAudio(musicIndex));
            }
            else if (msg.musicInfo == MUSICINFO.MUSIC_PREV)
            {
                int tempIndex;
                audioSource.Stop();
                if (isRandomPlay)
                {
                    int listNum = 0;
                    for (int i = 0; i < playlist.data.Count; i++)
                    {
                        if (playlist.data[i].index == musicIndex)
                        {
                            listNum = i;
                            break;
                        }
                    }

                    if (listNum - 1 < 0)
                    {
                        listNum = playlist.data.Count - 1;
                    }
                    else
                    {
                        listNum = listNum - 1;
                    }
                    musicIndex = playlist.data[listNum].index;
                }
                else
                {
                    int musicNum;
                    do
                    {
                        tempIndex = UnityEngine.Random.Range(0, playlist.data.Count);
                        musicNum = playlist.data[tempIndex].index;
                    } while (playlist.data[tempIndex].index == musicIndex);
                    musicIndex = musicNum;
                }

                holostarSettingModel.MusicIndex = musicIndex;
                holostarSettingModel.IsMusicPlay = true;
                Message.Send<SetMusicPlayButtonMsg>(new SetMusicPlayButtonMsg(holostarSettingModel.IsMusicPlay));
                int motionNum = UnityEngine.Random.Range((int)AnimationType.Motion1, (int)AnimationType.Motion7 + 1);
                SetAniMation(motionNum);
                StartCoroutine(GetAudio(musicIndex));
            }
            else if (msg.musicInfo == MUSICINFO.MUSIC_VOLUME)
            {
                holostarSettingModel.MusicVolume = float.Parse(msg.msg);
                audioSource.volume = holostarSettingModel.MusicVolume;
            }
            else if (msg.musicInfo == MUSICINFO.MUSIC_MODE_REPEIT)
            {
                isRandomPlay = false;
                holostarSettingModel.IsRepeat = true;
            }
            else if (msg.musicInfo == MUSICINFO.MUSIC_MODE_SHUFFLE)
            {
                isRandomPlay = true;
                holostarSettingModel.IsRepeat = false;
            }

            holostarSettingModel.HoloStarSetting.musicSetting.musicNowTime = audioSource.time;
            string tempJsonMsg = JsonUtility.ToJson(holostarSettingModel.HoloStarSetting);
            WindowBluetooth.GetInstance().SendBluetoothMsg(tempJsonMsg, SENDMSGTYPE.SETTING);
        }

        void SetAniMation(int num)
        {
            Message.Send<SetCharacterAnimationMsg>(new SetCharacterAnimationMsg((AnimationType)num, false));

            if (settingModel.IsBluetoothConnet)
                WindowBluetooth.GetInstance().SendBluetoothMsg(num.ToString(), SENDMSGTYPE.ANIMATION);
        }

        IEnumerator GetAudio(int index)
        {
            musicIndex = index;
            //Music music = playlist.data[index];

            Music music = null;

            for (int i = 0; i < playlist.data.Count; i++)
            {
                if (playlist.data[i].index == musicIndex)
                {
                    //musicIndex = i;
                    music = playlist.data[i];
                    break;
                }
            }

            Debug.Log("file://" + music.path);
            WWW www = new WWW("file://" + music.path);
            yield return www;
            if (www.error != null)
                Debug.Log(www.error);
            else
            {
                Message.Send<InfoMsg>(new InfoMsg(music.title + " / " + music.artist));
                //test
                audioSource.clip = NAudioPlayer.FromMp3Data(www.bytes);
                audioSource.Play();
                audioSource.time = 0;
                WindowBluetooth.GetInstance().SendBluetoothMsg(musicIndex.ToString(), SENDMSGTYPE.MUSIC, MUSICINFO.MUSIC_SELECT);
            }
        }

        IEnumerator CheckMusic()
        {
            while (holostarSettingModel.IsMusicPlay)
            {
                yield return new WaitForSeconds(1.0f);
                int num = 0;
                for (int i = 0; i < playlist.data.Count; i++)
                {
                    if (musicIndex == playlist.data[i].index)
                    {
                        num = i;
                        break;
                    }
                }
                if (audioSource.time >= playlist.data[num].duration - 1f)
                {
                    holostarSettingModel.IsMusicPlay = false;
                    Message.Send<SetMusicPlayButtonMsg>(new SetMusicPlayButtonMsg(holostarSettingModel.IsMusicPlay));
                    MusicRequest(new MusicRequestMsg(MUSICINFO.MUSIC_NEXT, 0.ToString()));
                }

                WindowBluetooth.GetInstance().SendBluetoothMsg(audioSource.time.ToString(), SENDMSGTYPE.MUSIC, MUSICINFO.MUSIC_TIME);
            }
        }

        protected override void OnExit()
        {
            if (audioVisualizer != null)
                audioVisualizer.SetActive(false);

            if (corMusicCheck != null)
                StopCoroutine(corMusicCheck);

            //LeapModule.transform.position = new Vector3(0, 0, 0.3f);
            MusicRequest(new MusicRequestMsg(MUSICINFO.MUSIC_PAUSE, ""));
            RemoveMessage();
        }

        private void RemoveMessage()
        {
            Message.RemoveListener<MusicRequestMsg>(MusicRequest);
        }
    }
}
