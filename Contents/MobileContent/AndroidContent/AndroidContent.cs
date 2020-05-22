using Android;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JHchoi.UI.Event;

namespace JHchoi.Contents
{
    public class AndroidContent : IContent
    {
        protected override void OnEnter()
        {
            //AndroidTrasferMgr.Instance.ShowToast("로딩 완료");
        }

        protected override void OnExit()
        {
        }

        public void ReceiveLocation(string msg)
        {
            if (string.IsNullOrEmpty(msg))
            {
                Debug.Log("unknown location");
                return;
            }
            string split = ",";
            string[] loc = msg.Split(split.ToCharArray());
            Debug.Log("location lat : " + loc[0] + ", lon : " + loc[1]);
            Message.Send<LocationMsg>(new LocationMsg(loc[0], loc[1]));
        }
    }
}