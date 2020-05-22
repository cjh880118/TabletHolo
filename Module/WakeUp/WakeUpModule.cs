using JHchoi.UI.Event;
using Midiazen;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using JHchoi.Models;
using UnityEngine;

namespace JHchoi.Module
{
    public class WakeUpModule : IModule
    {
        [DllImport("libMkSpot", CallingConvention = CallingConvention.Cdecl)]
        static public extern IntPtr mkspot_create(int numParProc);

        [DllImport("libMkSpot.dll", CallingConvention = CallingConvention.Cdecl)]
        static public extern void mkspot_destroy(IntPtr pMkSpotObj);

        [DllImport("libMkSpot.dll", CallingConvention = CallingConvention.Cdecl)]
        static public extern int mkspot_loadconf(IntPtr pMkSpotObj, byte[] mkspot_conf);

        [DllImport("libMkSpot.dll", CallingConvention = CallingConvention.Cdecl)]
        static public extern int mkspot_addwave_inbytes(IntPtr pMkSpotObj, byte[] pData, int nDataSize);

        [DllImport("libMkSpot.dll", CallingConvention = CallingConvention.Cdecl)]
        static public extern void mkspot_resetwave(IntPtr pMkSpotObj, bool reset_pos = false);

        [DllImport("libMkSpot.dll", CallingConvention = CallingConvention.Cdecl)]
        static public extern int mkspot_getrelbeginpos(IntPtr pMkSpotObj);

        [DllImport("libMkSpot.dll", CallingConvention = CallingConvention.Cdecl)]
        static public extern int mkspot_getrelendpos(IntPtr pMkSpotObj);

        static IntPtr pMkSpotObj = IntPtr.Zero;
        // Start is called before the first frame update

        AudioSource _audioSource;
        bool isRecord;
        byte[] receiveBuffer = new byte[9999];
        byte[] sendBuffer = new byte[9999];
        int loadConfigTryCount = 0;
        Coroutine corRecordStart;

        protected override void OnLoadStart()
        {
            AddMessage();

            var sm = Model.First<SettingModel>();

            if (sm.UseWakeUp)
                StartCoroutine(LoadConfig());
            else
                this.gameObject.SetActive(false);

            SetResourceLoadComplete();
        }

        private void AddMessage()
        {
            Message.AddListener<WakeUpMsg>(WakeUp);
        }

        IEnumerator LoadConfig()
        {
            _audioSource = this.gameObject.GetComponent<AudioSource>();
            pMkSpotObj = mkspot_create(4);
            yield return StartCoroutine(CopyFileAsyncWindow("mkspot.conf"));
            yield return StartCoroutine(CopyFileAsyncWindow("mkspot_dict.txt"));
            yield return StartCoroutine(CopyFileAsyncWindow("mkspot_dnnrecog.txt"));
            yield return StartCoroutine(CopyFileAsyncWindow("mkspot_model.txt"));

            if (pMkSpotObj != IntPtr.Zero)
            {
                loadConfigTryCount++;
                string strConfPath = Application.persistentDataPath + "/Config/mkspot.conf";
                bool exist = File.Exists(strConfPath);
                string debug1 = string.Format("conf file:{0}...{1}\n", strConfPath, exist ? "exist" : "not exist");
                Debug.Log(debug1);
                Log.Instance.log(debug1);

                byte[] chConfPath = System.Text.Encoding.Default.GetBytes(strConfPath);
                int ret = mkspot_loadconf(pMkSpotObj, chConfPath);
                string debug = string.Format("wakeup model loading:{0}...{1}\n ret : {2}", strConfPath, ret >= 0 ? "succeeded" : "failed", ret);
                Debug.Log(debug);
                Log.Instance.log(debug);

                if (ret >= 0)
                    WakeUp(new WakeUpMsg(true));
                else
                {
                    Log.Instance.log(debug);
                    yield return new WaitForSeconds(3.0f);
                    if (loadConfigTryCount < 5)
                        StartCoroutine(LoadConfig());
                    else
                        Log.Instance.log("Load Config Fail");
                }
            }
        }

        private void WakeUp(WakeUpMsg msg)
        {
            if (msg.isWakeUp)
            {
                if (corRecordStart != null)
                {
                    StopCoroutine(corRecordStart);
                    corRecordStart = null;
                }

                isRecord = true;
                corRecordStart = StartCoroutine(RecordStart());
            }
            else
            {
                isRecord = false;
                mkspot_resetwave(pMkSpotObj);
                if (corRecordStart != null)
                {
                    StopCoroutine(corRecordStart);
                    corRecordStart = null;
                }
            }
        }

        IEnumerator CopyFileAsyncWindow(string name)
        {
            string fromPath = Application.streamingAssetsPath + "/Config/";
            string toPath = Application.persistentDataPath + "/Config/";

            string[] filesNamesToCopy = new string[] { name, name };

            if (!Directory.Exists(toPath))
            {
                Directory.CreateDirectory(toPath);
            }

            if (!File.Exists(toPath + name))
            {
                foreach (string fileName in filesNamesToCopy)
                {
                    Debug.Log("copying from " + fromPath + fileName + " to " + toPath);
                    WWW www = new WWW(fromPath + fileName);
                    yield return www;
                    Debug.Log("yield done");
                    File.WriteAllBytes(toPath + fileName, www.bytes);
                    Debug.Log("file copy done");
                }
            }
        }

        IEnumerator RecordStart()
        {
            _audioSource.clip = Microphone.Start(null, false, 100, 16000);
            int _lastSample = 0;

            while (isRecord)
            {
                yield return null;
                int pos = Microphone.GetPosition(null);
                int diff = pos - _lastSample;

                if (diff > 0)
                {
                    float[] samples = new float[diff * _audioSource.clip.channels];
                    _audioSource.clip.GetData(samples, _lastSample);
                    byte[] bytes = ConvertAudioClipDataToInt16ByteArray(samples);

                    try
                    {
                        if (diff >= 0)
                        {
                            int a = mkspot_addwave_inbytes(pMkSpotObj, bytes, bytes.Length);

                            if (a > 0)
                            {
                                Debug.Log("wake up");
                                AwakeWakeUp();
                                //웨이크업 발생
                            }
                            _lastSample = pos;
                        }
                    }
                    catch (Exception E)
                    {

                    }
                }
                else if (diff < 0)
                {
                    string TestLog = string.Format("diff : {0} pos : {1} _lastSample : {2}", diff, pos, _lastSample);
                    Debug.Log(TestLog);
                    StartCoroutine(ResetRecord());
                }
            }
        }

        IEnumerator ResetRecord()
        {
            WakeUp(new WakeUpMsg(false));
            yield return new WaitForSeconds(1.0f);
            WakeUp(new WakeUpMsg(true));
        }

        void AwakeWakeUp()
        {
            Message.Send<WakeUpTTSMsg>(new WakeUpTTSMsg());
            WakeUp(new WakeUpMsg(false));
            Message.Send<STTRecord>(new STTRecord());
        }

        public static byte[] ConvertAudioClipDataToInt16ByteArray(float[] data)
        {
            MemoryStream dataStream = new MemoryStream();

            int x = sizeof(Int16);

            Int16 maxValue = Int16.MaxValue;

            int i = 0;
            while (i < data.Length)
            {
                dataStream.Write(BitConverter.GetBytes(Convert.ToInt16(data[i] * maxValue)), 0, x);
                ++i;
            }
            byte[] bytes = dataStream.ToArray();

            // Validate converted bytes
            Debug.AssertFormat(data.Length * x == bytes.Length, "Unexpected float[] to Int16 to byte[] size: {0} == {1}", data.Length * x, bytes.Length);

            dataStream.Dispose();

            return bytes;
        }

        protected override void OnUnload()
        {
            RemoveMessage();
        }

        private void RemoveMessage()
        {
            Message.RemoveListener<WakeUpMsg>(WakeUp);
        }
    }
}
