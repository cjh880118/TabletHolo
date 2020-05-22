using JHchoi.Constants;
using JHchoi.Contents;
using JHchoi.UI.Event;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicItem_Controller : MonoBehaviour
{
    DateTime prevTime;
    DateTime startTime;
    Coroutine corTimeCheck;
    public Animator animator;
    public GameObject loading;

    public void ObjectSelect()
    {
        //this.gameObject.transform.localScale = new Vector3(20, 20, 20);
        //this.gameObject.transform.DORotate(new Vector3(0, 180f, 0), 3);
        //this.gameObject.GetComponent<MeshRenderer>().materials[0].color = Color.red;
        //this.gameObject.GetComponent<MeshRenderer>().materials[1].color = Color.red;

        if (corTimeCheck == null)
        {
            startTime = DateTime.Now;
            prevTime = DateTime.Now;
            animator.SetTrigger("Over");
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

            if (this.gameObject.transform.parent.name == "ICON_NEXT")
            {
                Message.Send<MusicRequestMsg>(new MusicRequestMsg(MUSICINFO.MUSIC_NEXT, ""));
            }
            else if (this.gameObject.transform.parent.name == "ICON_STOP")
            {
                Message.Send<MusicRequestMsg>(new MusicRequestMsg(MUSICINFO.MUSIC_PAUSE, ""));
            }
            else if (this.gameObject.transform.parent.name == "ICON_PLAY")
            {
                Message.Send<MusicRequestMsg>(new MusicRequestMsg(MUSICINFO.MUSIC_PLAY, ""));
            }
            else if (this.gameObject.transform.parent.name == "ICON_BACKWARDS")
            {
                Message.Send<MusicRequestMsg>(new MusicRequestMsg(MUSICINFO.MUSIC_PREV, ""));
            }
        }
    }

    IEnumerator CheckTimer()
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
        animator.SetTrigger("OverOut");
        if (corTimeCheck != null)
        {
            StopCoroutine(corTimeCheck);
            corTimeCheck = null;
        }
    }
}
