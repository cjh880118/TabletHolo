using CellBig.UI.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CellBig.UI
{
    public class TabletInfoDialog : IDialog
    {
        public GameObject objText;
        public Text txtInfo;
        Coroutine msgTime;

        protected override void OnEnter()
        {
            objText.SetActive(false);
            AddMessage();
        }

        private void AddMessage()
        {
            Message.AddListener<SetInfoPositionMsg>(SetInfoPosition);
            Message.AddListener<InfoMsg>(Info);
        }

        private void SetInfoPosition(SetInfoPositionMsg msg)
        {
            if (msg.isTablet)
                return;

            //루킹 글라스인가?
            if (msg.isLooking)
            {
                objText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 550f);
            }
            else
            {

                if (msg.isSingle)//pc버전 단면
                {
                    objText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 350f);
                }
                else
                {
                    objText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 145);
                    objText.GetComponent<RectTransform>().localScale = new Vector3(0.5f, 0.5f, 0.5f);
                }
            }

        }

        private void Info(InfoMsg msg)
        {
            if (msgTime != null)
                StopCoroutine(msgTime);

            objText.SetActive(true);
            txtInfo.text = msg.msg;
            msgTime = StartCoroutine(InfoClose());
        }

        IEnumerator InfoClose()
        {
            yield return new WaitForSeconds(1.5f);
            objText.SetActive(false);
        }


        protected override void OnExit()
        {
            if (msgTime != null)
            {
                StopCoroutine(msgTime);
                msgTime = null;
            }

            RemoveMessage();
        }

        private void RemoveMessage()
        {
            Message.RemoveListener<InfoMsg>(Info);
        }
    }
}