using CellBig.Constants;
using CellBig.UI.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig.Models;

namespace CellBig.Contents
{
    public class TabletWatchContent : IContent
    {
        SettingModel settingModel;
        PlayerInventoryModel playerInventoryModel;
        Coroutine corWeather;
        WeatherRequest_Controller weather_Controller;

        protected override void OnLoadStart()
        {
            settingModel = Model.First<SettingModel>();
            playerInventoryModel = Model.First<PlayerInventoryModel>();
            StartCoroutine(LoadInitialData());
        }

        IEnumerator LoadInitialData()
        {
            string path = "Object/Tablet/WeatherRequest_Controller";
            yield return StartCoroutine(ResourceLoader.Instance.Load<GameObject>(path,
               o =>
               {
                   var obj = Instantiate(o) as GameObject;
                   weather_Controller = obj.GetComponent<WeatherRequest_Controller>();
                   weather_Controller.InitWeatherController();
               }));

            SetLoadComplete();
        }

        protected override void OnEnter()
        {
            AddMessage();
            UI.IDialog.RequestDialogEnter<UI.TabletWatchDialog>();

            if (settingModel.UseLookingGless)
                UI.IDialog.RequestRotate<UI.TabletWatchDialog>();

            corWeather = StartCoroutine(WeatherInfo());
        }

        private void AddMessage()
        {
            Message.AddListener<LocationMsg>(Location);
        }
        
        IEnumerator WeatherInfo()
        {
            while (true)
            {
                weather_Controller.Location(PlayerPrefs.GetString("lat"), PlayerPrefs.GetString("lon"), playerInventoryModel.NowCharacter);
                yield return new WaitForSeconds(3000f);
            }
        }

        private void Location(LocationMsg msg)
        {
            weather_Controller.Location(msg.lat, msg.lon, playerInventoryModel.NowCharacter);
        }

        protected override void OnExit()
        {
            if (corWeather != null)
            {
                StopCoroutine(corWeather);
                corWeather = null;
            }

            RemoveMessage();
            DialogAllClose();
        }

        private void RemoveMessage()
        {
            Message.RemoveListener<LocationMsg>(Location);
        }
    }
}