using JHchoi.Constants;
using JHchoi.Models;
using JHchoi.UI.Event;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu_Controller : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject musicMenu;
    public GameObject optionMenu;
    public GameObject BtnPlay;
    public GameObject BtnPause;
    GameObject LeapMotionModule;
    SettingModel settingModel;

    public void InitMenu(GameObject LeapMotion)
    {
        settingModel = Model.First<SettingModel>();
        LeapMotionModule = LeapMotion;
        AddMessage();
    }

    private void AddMessage()
    {
        Message.AddListener<SetMusicPlayButtonMsg>(SetMusicPlayButton);
    }

    private void SetMusicPlayButton(SetMusicPlayButtonMsg msg)
    {
        if (msg.isPlay)
        {
            BtnPlay.SetActive(false);
            BtnPause.SetActive(true);
        }
        else
        {
            BtnPlay.SetActive(true);
            BtnPause.SetActive(false);
        }
    }

    public void SetMenu(Menu menu, bool isBluetoothConnect = false, bool isSMS = false, bool isSchedule = false, bool isMute = false, float volume = 0, bool isMusicPlay = false)
    {
        mainMenu.SetActive(false);
        musicMenu.SetActive(false);
        optionMenu.SetActive(false);
        settingModel.isOpenMenu = false;

        if (menu == Menu.Music)
        {
            settingModel.isOpenMenu = true;
            musicMenu.SetActive(true);
        }
        else if (menu == Menu.Option)
        {
            settingModel.isOpenMenu = true;
            optionMenu.SetActive(true);
            optionMenu.GetComponent<OptionMenu_Controller>().SetIcon(isBluetoothConnect, isSMS, isSchedule, isMute, volume);
        }
        else if (menu == Menu.Main)
        {
            settingModel.isOpenMenu = true;
            mainMenu.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        RemoveMessage();
    }

    private void RemoveMessage()
    {
        Message.RemoveListener<SetMusicPlayButtonMsg>(SetMusicPlayButton);
    }
}