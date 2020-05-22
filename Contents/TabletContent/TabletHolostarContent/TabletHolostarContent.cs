using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JHchoi.Models;
using System;
using JHchoi.UI.Event;
using JHchoi.Constants;
using Midiazen;

namespace JHchoi.Contents
{
    public class TabletHolostarContent : IContent
    {
        PlayerInventoryModel playerInventoryModel;
        SettingModel settingModel;
        AudioSource tts;
        HolostartCharacter_Controller character_Controller;
        Coroutine corTTsSpeak;
        GameObject gra;

        protected override void OnLoadStart()
        {
            tts = GameObject.Find("MidiazenTTS(Clone)").GetComponent<AudioSource>();
            playerInventoryModel = Model.First<PlayerInventoryModel>();
            settingModel = Model.First<SettingModel>();
            StartCoroutine(LoadInitialData());
        }

        IEnumerator LoadInitialData()
        {
            string path = "Object/Face/Face";
            yield return StartCoroutine(ResourceLoader.Instance.Load<GameObject>(path,
               o =>
               {
                   var gameObject = Instantiate(o) as GameObject;
                   character_Controller = gameObject.GetComponent<HolostartCharacter_Controller>();
                   gameObject.transform.position = new Vector3(0, 0, 0);
               }));

            path = "Object/NewUI/Gra";
            yield return StartCoroutine(ResourceLoader.Instance.Load<GameObject>(path,
               o =>
               {
                   gra = Instantiate(o) as GameObject;
                   gra.SetActive(false);
               }));

            SetLoadComplete();
        }

        protected override void OnEnter()
        {
            character_Controller.SetCharacter(playerInventoryModel.NowCharacter);
            AddMessage();
        }

        void AddMessage()
        {
            Message.AddListener<TTSSendMsg>(TTSSend);
            Message.AddListener<CameraZoomMsg>(CameraZoom);
            Message.AddListener<SetCharacterAnimationMsg>(SetCharacterAnimation);
            Message.AddListener<EmotionAniMationMsg>(EmotionAniMation);
        }

        private void TTSSend(TTSSendMsg msg)
        {
            if (msg.msg != null)
            {
                if (corTTsSpeak != null)
                {
                    StopCoroutine(TTsDelay());
                    corTTsSpeak = null;
                }

                corTTsSpeak = StartCoroutine(TTsDelay());
            }
        }

        IEnumerator TTsDelay()
        {
            while (!tts.isPlaying)
            {
                yield return null;
            }

            SetCharacterAnimation(new SetCharacterAnimationMsg(AnimationType.Talk, false));
            StartCoroutine(TTsSpeakCheck());
        }

        IEnumerator TTsSpeakCheck()
        {
            while (tts.isPlaying)
            {
                yield return null;
            }

            SetCharacterAnimation(new SetCharacterAnimationMsg(AnimationType.Smile, false));
        }

        private void CameraZoom(CameraZoomMsg msg)
        {
            if (msg.isZoom)
                gra.SetActive(true);
            else
                gra.SetActive(false);

            character_Controller.ZoomInOut(msg.isZoom);
        }

        private void SetCharacterAnimation(SetCharacterAnimationMsg msg)
        {
            int tempAniNum = (int)msg.animationType;
            character_Controller.SetAniMation(tempAniNum, msg.isBluetoothCommand);

            if (settingModel.IsBluetoothConnet && !msg.isBluetoothCommand)
            {
                WindowBluetooth.GetInstance().SendBluetoothMsg(msg.animationType.ToString(), SENDMSGTYPE.ANIMATION);
            }
        }

        private void EmotionAniMation(EmotionAniMationMsg msg)
        {
            SetCharacterAnimation(new SetCharacterAnimationMsg(AnimationType.Talk, false));
            StartCoroutine(EmotionSpeakCheck(msg.index));
        }

        IEnumerator EmotionSpeakCheck(int index)
        {
            yield return new WaitForSeconds(0.5f);

            while (SoundManager.Instance.IsPlaySound(index))
            {
                yield return null;
            }

            SetCharacterAnimation(new SetCharacterAnimationMsg(AnimationType.Smile, false));
        }

        protected override void OnExit()
        {
            gra.SetActive(false);
            character_Controller.CharacterDestory();
            RemoveMessage();
        }

        private void RemoveMessage()
        {
            Message.RemoveListener<TTSSendMsg>(TTSSend);
            Message.RemoveListener<CameraZoomMsg>(CameraZoom);
            Message.RemoveListener<SetCharacterAnimationMsg>(SetCharacterAnimation);
            Message.RemoveListener<EmotionAniMationMsg>(EmotionAniMation);
        }

    }
}