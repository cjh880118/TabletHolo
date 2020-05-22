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
    public class TabletCharacterContent : IContent
    {
        Camera_Controller camera;
        PlayerInventoryModel playerInventoryModel;
        SettingModel settingModel;
        PlayerStatusModel playerStatusModel;
        
        Coroutine corSituationTime;
        Situation_Controller situation_Controller;

        GameObject tabletCharacter;
        GameObject character;
        GameObject effect;
        Schedule_Controller schedule_Controller;
        Character_Controller nowCharacter_Controller;

        #region Contents Load
        protected override void OnLoadStart()
        {
            playerInventoryModel = PlayerInventoryModel.First<PlayerInventoryModel>();
            settingModel = Model.First<SettingModel>();
            playerStatusModel = PlayerStatusModel.First<PlayerStatusModel>();
            StartCoroutine(LoadInitialData());
        }

        IEnumerator LoadInitialData()
        {
            string path = "Object/Tablet/TabletCharacter";
            yield return StartCoroutine(ResourceLoader.Instance.Load<GameObject>(path,
               o =>
               {
                   tabletCharacter = Instantiate(o) as GameObject;
                   tabletCharacter.transform.position = new Vector3(0, 0, 0);
                   camera = tabletCharacter.transform.GetChild(0).GetComponent<Camera_Controller>();
                   camera.InitCamera();
                   effect = tabletCharacter.transform.GetChild(1).gameObject;
                   schedule_Controller = tabletCharacter.transform.GetChild(2).GetComponent<Schedule_Controller>();
               }));

            path = "Object/Situation/Situation";
            yield return StartCoroutine(ResourceLoader.Instance.Load<GameObject>(path,
               o =>
               {
                   var inGameObject = Instantiate(o) as GameObject;
                   inGameObject.transform.position = new Vector3(0, 0, 0);
                   situation_Controller = inGameObject.GetComponent<Situation_Controller>();
               }));

            SetLoadComplete();
        }
        #endregion

        protected override void OnEnter()
        {
            AddMessage();
            corSituationTime = StartCoroutine(SituationTime());

            UI.IDialog.RequestDialogEnter<UI.TabletInfoDialog>();

            if (settingModel.IsSingle)
                UI.IDialog.RequestDialogEnter<UI.TabletSingleDialog>();
            else
                UI.IDialog.RequestDialogEnter<UI.TabletCharacterDialog>();

            if (settingModel.UseLookingGless)
            {
                UI.IDialog.RequestRotate<UI.TabletInfoDialog>();
                UI.IDialog.RequestDialogExit<UI.TabletSingleDialog>();
                UI.IDialog.RequestDialogExit<UI.TabletCharacterDialog>();
            }

            Message.Send<SetInfoPositionMsg>(new SetInfoPositionMsg(settingModel.IsSingle, settingModel.UseTablet, settingModel.UseLookingGless));

            SetCharacterDress(new SetCharacterDressMsg(playerInventoryModel.NowCharacter, playerInventoryModel.NowSkin));
        }

        private void AddMessage()
        {
            Message.AddListener<RunMenuMsg>(RunMenu);
            Message.AddListener<SetCharacterDressMsg>(SetCharacterDress);
            Message.AddListener<SetCharacterAnimationMsg>(SetCharacterAnimation);
            Message.AddListener<SetSituationAniMsg>(SetSituationAni);
            Message.AddListener<CameraZoomMsg>(CameraZoom);
            Message.AddListener<CharacterTouchMsg>(CharacterTouch);
            Message.AddListener<WakeUpTTSMsg>(WakeUpTTS);
        }

        private void WakeUpTTS(WakeUpTTSMsg msg)
        {
            Message.Send<TTSSendMsg>(new TTSSendMsg("", "네?", playerInventoryModel.NowCharacter));
        }

        private void RunMenu(RunMenuMsg msg)
        {
            Message.Send<CameraZoomMsg>(new CameraZoomMsg(false));

            if (msg.menu == Menu.HoloStar)
            {
                Message.Send<CameraZoomMsg>(new CameraZoomMsg(true));
                character.SetActive(false);
            }
            else if (msg.menu == Menu.Music)
            {
                character.SetActive(true);
                nowCharacter_Controller.EffectOff(false);
            }
            else
            {
                character.SetActive(true);
                nowCharacter_Controller.EffectOff(true);
            }
        }

        private void SetCharacterDress(SetCharacterDressMsg msg)
        {
            if ((nowCharacter_Controller == null || character == null) || !character.name.Contains(msg.character.ToString()))
            {
                if (character != null)
                {
                    Destroy(character);
                    Resources.UnloadUnusedAssets();
                    character = null;
                    nowCharacter_Controller = null;
                }

                StartCoroutine(LoadCharacter(msg.character, msg.dressNum));
            }
            else
            {
                if (msg.character == Character.Boy)
                    msg.dressNum -= 10;

                nowCharacter_Controller.SetDress(msg.dressNum);
                SetCharacterAnimation(new SetCharacterAnimationMsg(AnimationType.Idel1, false));
            }
        }

        IEnumerator LoadCharacter(Character name, int dressNum)
        {
            string path = "Object/Character/" + name.ToString();
            yield return StartCoroutine(ResourceLoader.Instance.Load<GameObject>(path,
               o =>
               {
                   character = Instantiate(o) as GameObject;
                   character.transform.parent = tabletCharacter.transform;
                   character.transform.position = new Vector3(0, 0, 0);
                   character.SetActive(true);
                   nowCharacter_Controller = character.GetComponent<Character_Controller>();
                   nowCharacter_Controller.SetCharacter(effect, schedule_Controller);

                   if (name == Character.Boy)
                       dressNum -= 10;

                   nowCharacter_Controller.SetDress(dressNum);
                   SetCharacterAnimation(new SetCharacterAnimationMsg(AnimationType.Idel1, false));

               }));
        }

        private void SetCharacterAnimation(SetCharacterAnimationMsg msg)
        {
            int tempAniNum = (int)msg.animationType;

            nowCharacter_Controller.SetAniMation(tempAniNum, msg.isBluetoothCommand);

            if (settingModel.IsBluetoothConnet && !msg.isBluetoothCommand)
            {
                WindowBluetooth.GetInstance().SendBluetoothMsg(msg.animationType.ToString(), SENDMSGTYPE.ANIMATION);
            }
        }

        private void SetSituationAni(SetSituationAniMsg msg)
        {
            if (msg.isSituationStart)
                character.SetActive(false);
            else
                character.SetActive(true);
        }

        IEnumerator SituationTime()
        {
            while (true)
            {
                if (settingModel.NowMenu == Menu.Main || settingModel.NowMenu == Menu.Watch)
                    situation_Controller.SetSituationAni(DateTime.Now);

                yield return null;
            }
        }

        private void CameraZoom(CameraZoomMsg msg)
        {
            //nowCharacter_Controller.ZoomInOut(msg.isZoom);
        }

        private void CharacterTouch(CharacterTouchMsg msg)
        {

            if (msg.aniType == AnimationType.Touch0)
            {
                Message.Send<TTSSendMsg>(new TTSSendMsg("", "룰루룰루루", playerInventoryModel.NowCharacter));
            }
            else if (msg.aniType == AnimationType.Touch1)
            {
                if (playerInventoryModel.NowCharacter == Character.Boy)
                {
                    Message.Send<TTSSendMsg>(new TTSSendMsg("", "전혀 힘들지 않습니다.", playerInventoryModel.NowCharacter));
                }
                else
                {
                    Message.Send<TTSSendMsg>(new TTSSendMsg("", "하나도 힘들지 않아요", playerInventoryModel.NowCharacter));
                }
            }
            else if (msg.aniType == AnimationType.Touch2)
            {
                Message.Send<TTSSendMsg>(new TTSSendMsg("", "으음 잘 모르겠네요.", playerInventoryModel.NowCharacter));
            }
            else if (msg.aniType == AnimationType.Touch3)
            {
                if (playerInventoryModel.NowCharacter == Character.Boy)
                {
                    Message.Send<TTSSendMsg>(new TTSSendMsg("", "어떤걸 도와드리면 좋으려나.", playerInventoryModel.NowCharacter));
                }
                else
                {
                    Message.Send<TTSSendMsg>(new TTSSendMsg("", "어떤걸 도와드리면 좋을까요.", playerInventoryModel.NowCharacter));
                }
            }
            else if (msg.aniType == AnimationType.Touch4)
            {
                if (playerInventoryModel.NowCharacter == Character.Boy)
                {
                    Message.Send<TTSSendMsg>(new TTSSendMsg("", "와우! 오늘 입으신 옷이 정말 멋져요!", playerInventoryModel.NowCharacter));
                }
                else
                {
                    Message.Send<TTSSendMsg>(new TTSSendMsg("", "어머! 오늘 입으신 옷이 정말 멋지네요!", playerInventoryModel.NowCharacter));
                }
            }
        }

        protected override void OnExit()
        {
            if (corSituationTime != null)
                StopCoroutine(corSituationTime);

            corSituationTime = null;

            RemoveMessage();
        }

        private void RemoveMessage()
        {
            Message.RemoveListener<RunMenuMsg>(RunMenu);
            Message.RemoveListener<SetCharacterDressMsg>(SetCharacterDress);
            Message.RemoveListener<SetCharacterAnimationMsg>(SetCharacterAnimation);
            Message.RemoveListener<SetSituationAniMsg>(SetSituationAni);
            Message.RemoveListener<CameraZoomMsg>(CameraZoom);
            Message.RemoveListener<CharacterTouchMsg>(CharacterTouch);
            Message.RemoveListener<WakeUpTTSMsg>(WakeUpTTS);
        }

        float deltaTime = 0.0f;

        void Update()
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        }

        void OnGUI()
        {
            int w = Screen.width, h = Screen.height;
            GUIStyle style = new GUIStyle();
            Rect rect = new Rect(0, 0, w, h * 2 / 100);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 2 / 100;
            style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;
            string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
            GUI.Label(rect, text, style);
        }
    }
}