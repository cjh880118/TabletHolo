using CellBig.Constants;
using CellBig.UI.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionSoundItem_Controller : MonoBehaviour
{
    DateTime prevTime;
    DateTime startTime;
    Coroutine corTimeCheck;
    public Animator animator;
    public GameObject SoundController;

    private void Start()
    {
        AddMessage();
    }

    private void AddMessage()
    {
        Message.AddListener<SetVolumeValueMsg>(SetVolumeValue);
    }

    private void SetVolumeValue(SetVolumeValueMsg msg)
    {
        for (int i = 0; i < SoundController.transform.childCount; i++)
        {
            SoundController.transform.GetChild(i).gameObject.SetActive(false);
        }
        if (msg.volume > 0 && msg.volume < 0.1)
        {
            for (int i = 0; i < SoundController.transform.childCount - 9; i++)
            {
                SoundController.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
        else if (msg.volume >= 0.1 && msg.volume < 0.2)
        {
            for (int i = 0; i < SoundController.transform.childCount - 8; i++)
            {
                SoundController.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
        else if (msg.volume >= 0.2 && msg.volume < 0.3)
        {
            for (int i = 0; i < SoundController.transform.childCount - 7; i++)
            {
                SoundController.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
        else if (msg.volume >= 0.3 && msg.volume < 0.4)
        {
            for (int i = 0; i < SoundController.transform.childCount - 6; i++)
            {
                SoundController.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
        else if (msg.volume >= 0.4 && msg.volume < 0.5)
        {
            for (int i = 0; i < SoundController.transform.childCount - 5; i++)
            {
                SoundController.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
        else if (msg.volume >= 0.5 && msg.volume < 0.6)
        {
            for (int i = 0; i < SoundController.transform.childCount - 4; i++)
            {
                SoundController.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
        else if (msg.volume >= 0.6 && msg.volume < 0.7)
        {
            for (int i = 0; i < SoundController.transform.childCount - 3; i++)
            {
                SoundController.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
        else if (msg.volume >= 0.7 && msg.volume < 0.8)
        {
            for (int i = 0; i < SoundController.transform.childCount - 2; i++)
            {
                SoundController.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
        else if (msg.volume >= 0.8 && msg.volume < 0.9)
        {
            for (int i = 0; i < SoundController.transform.childCount - 1; i++)
            {
                SoundController.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < SoundController.transform.childCount; i++)
            {
                SoundController.transform.GetChild(i).gameObject.SetActive(true);
            }

        }
    }

    public void ObjectSelect()
    {
        if (corTimeCheck == null)
        {
            animator.SetTrigger("Over");
            SoundController.SetActive(true);
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

        //if (corTimeCheck != null && startTime.AddSeconds(3.0f) < DateTime.Now)
        //{
        //    Debug.Log("버튼 유지 완료");
        //    ResetButton();

        // //   Message.Send<InfoMsg>(new InfoMsg("설정완료 되었습니다."));
        //}
    }

    IEnumerator CheckTimer()
    {
        while (DateTime.Now < prevTime.AddSeconds(0.05f))
        {
            yield return null;
            Message.Send<VolumeChangeMsg>(new VolumeChangeMsg());
            Debug.Log("볼륨 고고");
        }

        ResetButton();
    }

    public void ResetButton()
    {
        animator.SetTrigger("OverOut");
        SoundController.SetActive(false);
        if (corTimeCheck != null)
        {
            StopCoroutine(corTimeCheck);
            corTimeCheck = null;
        }
    }

    public void SetVolumeController(float volume)
    {

    }

    private void OnDestroy()
    {
        RemoveMessage();
    }

    private void RemoveMessage()
    {
        Message.RemoveListener<SetVolumeValueMsg>(SetVolumeValue);
    }
}
