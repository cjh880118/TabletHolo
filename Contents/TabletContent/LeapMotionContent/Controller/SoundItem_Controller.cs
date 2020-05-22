using CellBig.UI.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundItem_Controller : MonoBehaviour
{
    bool isTouch;
    public void ObjectSelect()
    {
        if(!isTouch)
        {
            isTouch = true;
            Message.Send<VolumeChangeMsg>(new VolumeChangeMsg());
            StartCoroutine(TouchDelay());
        }
    }

    IEnumerator TouchDelay()
    {
        yield return new WaitForSeconds(0.3f);
        isTouch = false;
    }
}
