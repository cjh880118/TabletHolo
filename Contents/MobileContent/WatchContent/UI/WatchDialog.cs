﻿using JHchoi.UI.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using JHchoi.Constants;

namespace JHchoi.UI
{
    public class WatchDialog : IDialog
    {
        public Weather_Controller weather_Controller;
        public Text txtHour;
        public Text txtMinute;
        public Text txtPm;
        public Text txtDay;
        public RawImage imgCharacter;

        protected override void OnEnter()
        {
            StartCoroutine(Time());
            //string path = "UIImage/RenderTexture/BoyRtt";
            //imgCharacter.texture = Resources.Load(path) as Texture;
            AddMessage();
        }

        IEnumerator Time()
        {
            while (true)
            {
                txtHour.text = DateTime.Now.ToString("hh");
                txtMinute.text = DateTime.Now.ToString("mm");
                txtPm.text = DateTime.Now.ToString("tt");
                CultureInfo cultures = CultureInfo.CreateSpecificCulture("ko-KR");
                txtDay.text = DateTime.Now.ToString("MM월 dd일 ddd요일", cultures);
                yield return null;
            }
        }

        private void AddMessage()
        {
            Message.AddListener<WeatherMsg>(Weather);
            Message.AddListener<AddressMsg>(Address);
            
        }

        private void Weather(WeatherMsg msg)
        {
            weather_Controller.InitWeatehr(msg.weathers, msg.temperature);
        }

        private void Address(AddressMsg msg)
        {
            weather_Controller.InitLocation(msg.addr1, msg.addr2);
        }

        protected override void OnExit()
        {
            RemoveMessage();
        }

        private void RemoveMessage()
        {
            Message.RemoveListener<WeatherMsg>(Weather);
            Message.RemoveListener<AddressMsg>(Address);
        }
    }
}
