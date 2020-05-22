using JHchoi.Constants;
using JHchoi.Contents;
using JHchoi.UI.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Midiazen;

public class WeatherRequest_Controller : MonoBehaviour
{
    WeatherInfo weatherInfo;
    JHchoi.Contents.LocationInfo locationInfo;

    public void InitWeatherController()
    {
        AddMessage();
    }

    private void AddMessage()
    {
        Message.AddListener<WeatherRequestMsg>(WeatherRequest);
    }

    private void WeatherRequest(WeatherRequestMsg msg)
    {
        Location(PlayerPrefs.GetString("lat"), PlayerPrefs.GetString("lon"), msg.character, true);
    }

    public void Location(string lat, string lon, Character character, bool isTTSRequtest = false)
    {
        StartCoroutine(IGetWeather(lat, lon, character, isTTSRequtest));
    }

    IEnumerator IGetWeather(string lat, string lon, Character character, bool isTTSRequtest)
    {
        PlayerPrefs.SetString("lat", lat);
        PlayerPrefs.SetString("lon", lon);

        WWW www = new WWW("http://api.openweathermap.org/data/2.5/weather?lat=" + lat + "&lon=" + lon + "&appid=ce4995f5d2e026bf6ac967963210ceb1&units=metric");
        yield return www;
        if (www.error != null)
        {

        }
        else if (www.isDone)
        {
            weatherInfo = JsonUtility.FromJson<WeatherInfo>(www.text);
            Weathers tempWeathers = Weathers.Clean;
            Debug.Log(weatherInfo);
            foreach (WeatherInfo.Weather weather in weatherInfo.weather)
            {
                if (weather.id == 800)
                {   //clear                    
                    //myWeather.SetWeatherInfo(Weathers.맑음);
                    tempWeathers = Weathers.Clean;
                    break;
                }
                else if (weather.id > 800 && weather.id > 805)
                { //clouds
                  //myWeather.SetWeatherInfo(Weathers.흐림);
                    tempWeathers = Weathers.Cloud;
                    break;
                }
                else if (weather.id > 599 && weather.id < 623)
                { //snowy
                  //myWeather.SetWeatherInfo(Weathers.눈);
                    tempWeathers = Weathers.Snow;
                    break;
                }
                else if (weather.id > 499 && weather.id < 532)
                { //rainy
                  //myWeather.SetWeatherInfo(Weathers.비);
                    tempWeathers = Weathers.Rain;
                    break;
                }
                else if (weather.id > 299 && weather.id < 322)
                { //drizzle
                  //myWeather.SetWeatherInfo(Weathers.비);
                    tempWeathers = Weathers.Rain;
                    break;
                }
                else if (weather.id > 199 && weather.id < 233)
                { //thunder
                  //myWeather.SetWeatherInfo(Weathers.비);
                    tempWeathers = Weathers.Rain;
                    break;
                }
                else if (weather.id > 700 && weather.id < 782)
                {//etcs
                 //myWeather.SetWeatherInfo(Weathers.흐림)
                    tempWeathers = Weathers.Cloud;
                    break;
                }
            }

            Message.Send<WeatherMsg>(new WeatherMsg(tempWeathers, weatherInfo.main.temp.ToString()));
            StartCoroutine(IGetLocale(lat, lon, tempWeathers, weatherInfo.main.temp.ToString(), character, isTTSRequtest));
        }
    }

    IEnumerator IGetLocale(string lat, string lon, Weathers weathers, string temp, Character character, bool isTTsRequest)
    {
        string addr1 = "";
        string addr2 = "";
        WWW www = new WWW("https://maps.googleapis.com/maps/api/geocode/json?address=" + lat + "," + lon + "&key=AIzaSyC55Xgl4YTGrnILi4jgqiXroEB4venvhzI&language=ko");
        yield return www;
        if (www.error != null)
        {

        }
        else if (www.isDone)
        {
            //Debug.Log(www.text);
            locationInfo = JsonUtility.FromJson<JHchoi.Contents.LocationInfo>(www.text);
            try
            {
                string split = " ";
                string[] addr = locationInfo.results[0].formatted_address.Split(split.ToCharArray());
                addr1 = addr[1];
                addr2 = addr[2];
                Message.Send<AddressMsg>(new AddressMsg(addr[1], addr[2]));
            }
            catch (System.Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        if (isTTsRequest)
        {
            string weather = "";
            if (weathers == Weathers.Clean)
            {
                weather = "맑음";
            }
            else if (weathers == Weathers.Cloud)
            {
                weather = "흐림";
            }
            else if (weathers == Weathers.Rain)
            {
                weather = "비";
            }
            else if (weathers == Weathers.Snow)
            {
                weather = "눈";
            }

            string temper;
            Mathf.Abs((int)float.Parse(temp));
            if ((int)float.Parse(temp) >= 0)
            {
                temper = Mathf.Abs((int)float.Parse(temp)).ToString();
            }
            else
            {
                temper = "영하" + Mathf.Abs((int)float.Parse(temp)).ToString();
            }


            string msg = string.Format("현재 {0} {1}의 날씨는 {2}이고 현재 온도는 {3}도 입니다.", addr1, addr2, weather, temper);
            Message.Send<TTSSendMsg>(new TTSSendMsg("", msg, character));
        }
    }

    private void OnDestroy()
    {
        RemoveMessage();
    }

    private void RemoveMessage()
    {
        Message.RemoveListener<WeatherRequestMsg>(WeatherRequest);
    }
}
