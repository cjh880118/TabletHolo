using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig.Constants;


namespace CellBig.UI.Event
{
    public class WatchCharacterMsg : Message
    {
        public Character character;
        public WatchCharacterMsg(Character character)
        {
            this.character = character;
        }
    }

    public class LocationMsg : Message
    {
        public string lat;
        public string lon;
        public LocationMsg(string lat, string lon)
        {
            this.lat = lat;
            this.lon = lon;
        }
    }

    public class WeatherMsg : Message
    {
        public Weathers weathers;
        public string temperature;
        public WeatherMsg(Weathers weathers, string temperature)
        {
            this.weathers = weathers;
            this.temperature = temperature;
        }
    }

    public class AddressMsg: Message
    {
        public string addr1;
        public string addr2;
        public AddressMsg(string addr1, string addr2)
        {
            this.addr1 = addr1;
            this.addr2 = addr2;
        }
    }
}