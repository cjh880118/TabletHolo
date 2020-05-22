using JHchoi.Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JHchoi.Models;

namespace Android
{
    public class AndroidTrasferMgr : MonoBehaviour
    {
        private static AndroidTrasferMgr _instance;
        public AndroidJavaClass AJC;
        public AndroidJavaObject AJO;
        public AndroidJavaClass javaClass = null;
        public AndroidJavaObject javaClassInstance = null;

        public static AndroidTrasferMgr Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = GameObject.FindObjectOfType<AndroidTrasferMgr>();
                    if (!_instance)
                    {
                        GameObject container = new GameObject();
                        container.name = "AndroidTrasferMgr";
                        _instance = container.AddComponent(typeof(AndroidTrasferMgr)) as AndroidTrasferMgr;
                    }
                    _instance.AJC = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    _instance.AJO = _instance.AJC.GetStatic<AndroidJavaObject>("currentActivity");
                }

                return _instance;
            }
        }

        public void GetCalendar()
        {
#if (UNITY_ANDROID)
            _instance.AJO.Call("GetCalendar");
#endif
        }

        public void Vibe()
        {
#if (UNITY_ANDROID)
            _instance.AJO.Call("Vibe");
#endif
        }

        public void GetLocation()
        {
#if UNITY_ANDROID
            _instance.AJO.Call("GetLocation");
#endif
        }

        public void ShowToast(string msg)
        {
#if UNITY_ANDROID
            _instance.AJO.Call("ShowToast", msg);
#endif
        }

        //>> 2018-06-07 최진호 블루투스 통신 

        /********************************************
         * 2018-06-07 최진호
         * msg : 블루투스로 전송할 메세지
         * 블루투스를 통해 연결된 단말기로 msg 전송
         *******************************************/

        public void BluetoothSendMsg(string msg, SENDMSGTYPE type)
        {
            msg = (int)type + "|" + msg;
            _instance.AJO.Call("SendMsg", msg);
        }


        //블루투스 기기 검색
        public void SearchDevice()
        {
            _instance.AJO.Call("SearchDevice");
        }

        //블루투스 기기 리스트 
        public void BluetoothList()
        {
            _instance.AJO.Call("BluetoothList");
        }

        //블루투스 키고 끄기
        public void BluetoothTurnOn(bool on)
        {
            _instance.AJO.Call("TurnOnBluetooth", on);
        }

        /***********************************************
        * 2018-06-07 최진호
        * device : 블루투스 통신할 디바이스
        * 블루투스를 통신을 할 디바이스를 선택하여 전송
        ************************************************/
        public void SelectDevice(string device)
        {
            _instance.AJO.Call("SelectDevice", device);
        }

        //블루투스 사용
        public void EnableBluetooth()
        {
            _instance.AJO.Call("EnableBlueTooth");
        }


        //블루투스 ON OFF 체크
        public void IsBluetoothOn()
        {
            _instance.AJO.Call("IsBluetoothOn");
        }

        //블루투스 자동 연결
        public void AutoConnect()
        {
            _instance.AJO.Call("AutoConnect");
        }

        //STT 호출
        public void STTOpen()
        {
            _instance.AJO.Call("STTStart");
        }

        //<<================================ 블루투스
    }
}