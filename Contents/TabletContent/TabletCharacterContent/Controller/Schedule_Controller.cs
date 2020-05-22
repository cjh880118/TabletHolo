using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JHchoi.Constants;

public class Schedule_Controller : MonoBehaviour
{
    public GameObject[] ScheduleObj;

    public void SetObject(AnimationType animationType)
    {
        foreach (var o in ScheduleObj)
            o.SetActive(false);

        if (animationType == AnimationType.Vocal)
        {
            ScheduleObj[0].SetActive(true);
        }
        else if (animationType == AnimationType.Dance)
        {
            ScheduleObj[1].SetActive(true);
        }
        else if (animationType == AnimationType.Entertainment)
        {
            ScheduleObj[2].SetActive(true);
        }
        else if (animationType == AnimationType.Intelligence)
        {
            ScheduleObj[3].SetActive(true);
        }
        else if (animationType == AnimationType.Meal)
        {
            ScheduleObj[4].SetActive(true);
        }
    }
}
