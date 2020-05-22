using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig.Constants;

namespace CellBig.Contents
{
    public class ScheduleStage_Controller : MonoBehaviour
    {
        public GameObject Vocal;
        public GameObject Dance;
        public GameObject Entertainment;
        public GameObject Intelligence;
        public GameObject Meal;
        public GameObject Rest;

        public void SetStage(Schedule schedule)
        {
            AllObjectActiveFalse();
            if (schedule == Schedule.Vocal)
                Vocal.SetActive(true);
            else if (schedule == Schedule.Dance)
                Dance.SetActive(true);
            else if (schedule == Schedule.Entertainment)
                Entertainment.SetActive(true);
            else if (schedule == Schedule.Intelligence)
                Intelligence.SetActive(true);
            else if (schedule == Schedule.Meal)
                Meal.SetActive(true);
            else if (schedule == Schedule.Rest)
                Rest.SetActive(true);
        }

        void AllObjectActiveFalse()
        {
            Meal.SetActive(false);
            Dance.SetActive(false);
            Intelligence.SetActive(false);
            Entertainment.SetActive(false);
            Rest.SetActive(false);
            Vocal.SetActive(false);
        }
    }
}