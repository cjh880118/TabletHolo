﻿using CellBig.Constants;
using CellBig.Contents;
using CellBig.UI.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weather_Controller : MonoBehaviour
{
    WeatherInfo weatherInfo;
    CellBig.Contents.LocationInfo locationInfo;

    private void Location(LocationMsg msg)
    {
        StartCoroutine(IGetWeather(msg.lat, msg.lon));
    }

    IEnumerator IGetWeather(string lat, string lon)
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
            //myWeather.SetWeatherDegree(weatherInfo.temp + "");
            StartCoroutine(IGetLocale(lat, lon));
        }
    }

    IEnumerator IGetLocale(string lat, string lon)
    {
        WWW www = new WWW("https://maps.googleapis.com/maps/api/geocode/json?address=" + lat + "," + lon + "&key=AIzaSyC55Xgl4YTGrnILi4jgqiXroEB4venvhzI&language=ko");
        yield return www;
        if (www.error != null)
        {

        }
        else if (www.isDone)
        {
            //Debug.Log(www.text);
            locationInfo = JsonUtility.FromJson<CellBig.Contents.LocationInfo>(www.text);
            try
            {
                string split = " ";
                string[] addr = locationInfo.results[0].formatted_address.Split(split.ToCharArray());
                Message.Send<AddressMsg>(new AddressMsg(addr[1], addr[2]));
                //myWeather.SetWeatherLocale(addr[1], addr[2]);
            }
            catch (System.Exception e)
            {
                //Debug.Log(e.ToString);
            }
        }
    }
}
