using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public unsafe struct testCMkSpot
{
    public IntPtr _vtable;
}


public class WakeUp : MonoBehaviour
{

    private testCMkSpot abc = new testCMkSpot();
         
    [DllImport("libMkSpot")]
    static extern IntPtr CMkSpot();

    [DllImport("libMkSpot", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern int loadConfModel([MarshalAs(UnmanagedType.LPStr)]string path);

    [DllImport("libMkSpot", CharSet = CharSet.Unicode, ExactSpelling = true)]
    static extern void addWave(IntPtr pData, int nDataSize);

    [DllImport("libMkSpot", CharSet = CharSet.Unicode, ExactSpelling = true)]
    static extern void resetWave(bool resetCumulativePos);

    // Start is called before the first frame update
    void Start()
    {
        abc._vtable = CMkSpot();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
