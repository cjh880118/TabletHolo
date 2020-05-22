using CellBig.Constants;
using CellBig.Contents;
using CellBig.UI.Event;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionItem_Controller : MonoBehaviour
{
    DateTime prevTime;
    DateTime startTime;
    Coroutine corTimeCheck;

    public Material[] matMain;
    public Material[] matOver;
    public Sprite[] sprOver;
    public Material[] matLine;

    public Animator animator;
    public GameObject loading;

    public void ObjectSelect()
    {
        if (corTimeCheck == null)
        {
            animator.SetTrigger("Over");
            startTime = DateTime.Now;
            prevTime = DateTime.Now;
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

            //if (this.gameObject.name == "Option_Bluetooth")
            //{
            //    Message.Send<TabletOptionMsg>(new TabletOptionMsg(OptionSet.Bluetooth));
            //}
            if (this.gameObject.transform.parent.name == "ICON_SNS")
            {
                Message.Send<TabletOptionMsg>(new TabletOptionMsg(OptionSet.SMS));
            }
            else if (this.gameObject.transform.parent.name == "ICON_SCHEDULE")
            {
                Message.Send<TabletOptionMsg>(new TabletOptionMsg(OptionSet.Schedule));
            }

            Message.Send<InfoMsg>(new InfoMsg("설정완료 되었습니다."));
        }
    }

    IEnumerator CheckTimer()
    {
        while (DateTime.Now < prevTime.AddSeconds(0.5f))
        {
            yield return null;
            TimeSpan a = DateTime.Now - startTime;
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

    public void SetColor(bool isOn)
    {
        if (isOn)
        {
            this.gameObject.GetComponent<MeshRenderer>().material = matMain[1];
            this.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().material = matOver[1];
            this.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = sprOver[1];
            this.gameObject.transform.GetChild(1).GetComponent<MeshRenderer>().material = matLine[1];
        }
        else
        {
            this.gameObject.GetComponent<MeshRenderer>().material = matMain[0];
            this.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().material = matOver[0];
            this.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = sprOver[0];
            this.gameObject.transform.GetChild(1).GetComponent<MeshRenderer>().material = matLine[0];
        }
    }
}
