using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionMenu_Controller : MonoBehaviour
{
    public OptionItem_Controller Bluetooth;
    public OptionItem_Controller SMS;
    public OptionItem_Controller Schedule;
    public OptionSoundItem_Controller Sound;

    public void SetIcon(bool isBluetooth, bool isSMS, bool isSchedule, bool isMute, float volume)
    {
        Bluetooth.SetColor(isBluetooth);
        SMS.SetColor(isSMS);
        Schedule.SetColor(isSchedule);
        //Sound.SetVolumeController(volume);
    }
}
