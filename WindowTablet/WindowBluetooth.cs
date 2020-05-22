using System.Collections;
using UnityEngine;
using System.IO.Ports;
using System;
using System.Text;
using JHchoi.Constants;
using JHchoi.UI.Event;
using JHchoi.Contents;
using System.Threading;
using System.Collections.Generic;

public class WindowBluetooth : MonoBehaviour
{
    private static WindowBluetooth instance;
    private static GameObject contain;

    private SerialPort serialPort;
    //public bool IsSendPossible = true;
    //public List<string> ListMsg = new List<string>();

    public static WindowBluetooth GetInstance()
    {
        if (!instance)
        {
            contain = new GameObject();
            contain.name = "WindowBluetooth";
            instance = contain.AddComponent(typeof(WindowBluetooth)) as WindowBluetooth;
        }
        return instance;
    }
    

    //포트 열기
    public void OpenPort(string port, int baud, Parity parity, int bits, StopBits stopBits)
    {
        try
        {
            Debug.Log("블루투스 포트 오픈 포트 번호 : " + port);
            serialPort = new SerialPort(port, baud, parity, bits, stopBits);
            serialPort.ReadTimeout = 1000;
            serialPort.Handshake = Handshake.RequestToSend;
            serialPort.DtrEnable = true;
            serialPort.RtsEnable = true;
            serialPort.Open();
            serialPort.Encoding = Encoding.UTF8;
            StartCoroutine(ReceiveMsg());
            Debug.Log("Open Port");
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    //Data수신 스레드
    IEnumerator ReceiveMsg()
    {
        while (true)
        {
            int count = serialPort.BytesToRead;
            if (count > 0)
            {
                try
                {
                    string msg = serialPort.ReadExisting();
                    Debug.Log(msg);
                    Message.Send<TabletReceiveMsg>(new TabletReceiveMsg(msg));
                }
                catch (Exception e)
                {
                    Debug.Log(e.ToString());
                }
            }
            yield return null;
        }
    }

    //Data송신
    public void SendBluetoothMsg(string msg, SENDMSGTYPE type, MUSICINFO musicInfo = MUSICINFO.None, Menu menu = Menu.None)
    {
        BluetoothData data = new BluetoothData
        {
            msg = msg,
            dataType = type,
            musicInfo = musicInfo,
            menu = menu
        };

        string dataMsg = JsonUtility.ToJson(data);

        try
        {
            serialPort.Write(dataMsg);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    //IEnumerator SendDelay(string dataMsg)
    //{
    //    yield return null;
    //    try
    //    {
    //        serialPort.Write(dataMsg);
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.Log(e.ToString());
    //    }
    //}


    //포트 닫기
    public void ClosePort()
    {
        serialPort.Close();
    }

    private void OnDestroy()
    {
        WindowBluetooth.GetInstance().SendBluetoothMsg("false", SENDMSGTYPE.CONNECTION);
    }

}
