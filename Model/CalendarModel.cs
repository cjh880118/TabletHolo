using JHchoi.Contents;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JHchoi.Models
{
    public class CalendarModel : Model
    {
        GameModel _owner;

        public void Setup(GameModel owner)
        {
            _owner = owner;
            alarmEvents = new List<AlarmEvent>();
        }

        List<AlarmEvent> alarmEvents;
        public List<AlarmEvent> AlarmEvents { get => alarmEvents; set => alarmEvents = value; }

        public int GetTodayEvent()
        {
            int i = 0;
            foreach (var o in alarmEvents)
            {
                DateTime dtime = new DateTime(o.tick);
                if (dtime.ToString("yyyy-MM-dd") == DateTime.Now.ToString("yyyy-MM-dd")){
                    i++;
                }
            }

            return i;
        }

        public int GetTomorrowEvent()
        {
            int i = 0;
            foreach (var o in alarmEvents)
            {
                DateTime dtime = new DateTime(o.tick);
                if (dtime.ToString("yyyy-MM-dd") == DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"))
                {
                    i++;
                }
            }

            return i;
        }
    }
}