using CellBig.UI.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmGameButton_Controller : MonoBehaviour
{
    bool isTouch;
    public bool isRight;
    public GameObject over;
    public void OnInputKey()
    {
        if (!isTouch)
        {
            isTouch = true;
            Message.Send<RhythmGameBtnTouchMsg>(new RhythmGameBtnTouchMsg(isRight));
            over.SetActive(true);
            StartCoroutine(TouchDelay());
        }
    }

    IEnumerator TouchDelay()
    {
        yield return new WaitForSeconds(0.3f);
        isTouch = false;
        over.SetActive(false);
    }

    public void ResetButton()
    {
        isTouch = false;
        over.SetActive(false);
    }
}
