using JHchoi.Constants;
using JHchoi.Contents;
using JHchoi.UI.Event;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuItem_Controller : MonoBehaviour
{
    DateTime prevTime;
    DateTime startTime;
    Coroutine corTimeCheck;
    public Animator animator;
    public GameObject loading;
    float a = 0;
    public void ObjectSelect()
    {
        if (corTimeCheck == null)
        {
            Debug.Log("최초 터치");
            a = 0;
            startTime = DateTime.Now;
            prevTime = DateTime.Now;
            animator.SetTrigger("Over");
            loading.GetComponent<MeshRenderer>().material.SetFloat("_AlphaVal", 0.0f);
            corTimeCheck = StartCoroutine(CheckTimer());
        }

        if (prevTime.AddSeconds(0.01f) < DateTime.Now && corTimeCheck != null)
        {
            prevTime = DateTime.Now;
            StopCoroutine(corTimeCheck);
            corTimeCheck = StartCoroutine(CheckTimer());
        }

        if (corTimeCheck != null && startTime.AddSeconds(2.0f) < DateTime.Now)
        {
            Debug.Log("버튼 유지 완료");
            ResetButton();
            BluetoothData data = new BluetoothData();
            data.msg = "";
            data.dataType = SENDMSGTYPE.MENU;
            data.musicInfo = MUSICINFO.None;
            string dataMsg;

            if (this.gameObject.transform.parent.name == "ICON_HOME")
            {
                Debug.Log("메인");
                data.menu = Menu.Main;
                dataMsg = JsonUtility.ToJson(data);
                Message.Send<TabletMotionMsg>(new TabletMotionMsg(dataMsg));
                Message.Send<RunMenuMsg>(new RunMenuMsg(Menu.None));
            }
            else if (this.gameObject.transform.parent.name == "ICON_CLOCK")
            {
                data.menu = Menu.Watch;
                dataMsg = JsonUtility.ToJson(data);
                Message.Send<TabletMotionMsg>(new TabletMotionMsg(dataMsg));
                Message.Send<RunMenuMsg>(new RunMenuMsg(Menu.Watch));
            }
            else if (this.gameObject.transform.parent.name == "ICON_GAME")
            {
                data.dataType = SENDMSGTYPE.GAME;
                data.msg = GameType.RhythmGame.ToString(); ;
                dataMsg = JsonUtility.ToJson(data);
                Message.Send<TabletMotionMsg>(new TabletMotionMsg(dataMsg));
                Message.Send<RunMenuMsg>(new RunMenuMsg(Menu.Game));
            }
            else if (this.gameObject.transform.parent.name == "ICON_MUSIC")
            {
                data.menu = Menu.Music;
                dataMsg = JsonUtility.ToJson(data);
                Message.Send<TabletMotionMsg>(new TabletMotionMsg(dataMsg));
                Message.Send<RunMenuMsg>(new RunMenuMsg(Menu.Music));
            }
            else if (this.gameObject.transform.parent.name == "ICON_HOLOSTAR")
            {
                data.menu = Menu.HoloStar;
                dataMsg = JsonUtility.ToJson(data);
                Message.Send<TabletMotionMsg>(new TabletMotionMsg(dataMsg));
                Message.Send<RunMenuMsg>(new RunMenuMsg(Menu.HoloStar));
            }
            else if (this.gameObject.transform.parent.name == "ICON_OPTION")
            {
                data.menu = Menu.Option;
                dataMsg = JsonUtility.ToJson(data);
                Message.Send<TabletMotionMsg>(new TabletMotionMsg(dataMsg));
                Message.Send<RunMenuMsg>(new RunMenuMsg(Menu.Option));
            }
        }
    }

    
    IEnumerator CheckTimer( )
    {
        while (DateTime.Now < prevTime.AddSeconds(0.5f))
        {
            yield return null;
            TimeSpan a = DateTime.Now - startTime;
            //Debug.Log(a.TotalMilliseconds);
            double fillout = a.TotalMilliseconds / (2 * 1000);
            float vOut = Convert.ToSingle(fillout);
            loading.GetComponent<MeshRenderer>().material.SetFloat("_AlphaVal", vOut + 0.01f);
        }

        ResetButton();
    }

    public void ResetButton()
    {
        Debug.Log("터치 아웃");
        animator.SetTrigger("OverOut");
        if (corTimeCheck != null)
        {
            StopCoroutine(corTimeCheck);
            corTimeCheck = null;
        }
    }
}
