using JHchoi;
using JHchoi.UI.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Situation_Controller : MonoBehaviour
{
    // Use this for initialization

    bool isAniStart = false;
    GameObject situation;

    public void SetSituationAni(DateTime dateTime)
    {
        if (isAniStart)
            return;

        if (dateTime.ToString("HHmmss") == "070000" || Input.GetKeyDown(KeyCode.Alpha1))
        {
            isAniStart = true;
            StartCoroutine(LoadSituation("Yoga"));
        }
        else if (dateTime.ToString("HHmmss") == "090000" || Input.GetKeyDown(KeyCode.Alpha2))
        {
            isAniStart = true;
            StartCoroutine(LoadSituation("Working"));
        }
        else if (dateTime.ToString("HHmmss") == "100000" || Input.GetKeyDown(KeyCode.Alpha3))
        {
            isAniStart = true;
            StartCoroutine(LoadSituation("Meeting"));
        }
        else if (dateTime.ToString("HHmmss") == "110000" || Input.GetKeyDown(KeyCode.Alpha4))
        {
            isAniStart = true;
            StartCoroutine(LoadSituation("Reading"));
        }
        else if (dateTime.ToString("HHmmss") == "130000" || Input.GetKeyDown(KeyCode.Alpha5))
        {
            isAniStart = true;
            StartCoroutine(LoadSituation("Hotdog"));
        }
        else if (dateTime.ToString("HHmmss") == "140000" || Input.GetKeyDown(KeyCode.Alpha6))
        {
            isAniStart = true;
            StartCoroutine(LoadSituation("Sleeping"));
        }
        else if (dateTime.ToString("HHmmss") == "150000" || Input.GetKeyDown(KeyCode.Alpha7))
        {
            isAniStart = true;
            StartCoroutine(LoadSituation("Coffee"));
        }
        else if (dateTime.ToString("HHmmss") == "160000" || Input.GetKeyDown(KeyCode.Alpha8))
        {
            isAniStart = true;
            StartCoroutine(LoadSituation("Walking"));
        }
        else if (dateTime.ToString("HHmmss") == "170000" || Input.GetKeyDown(KeyCode.Alpha9))
        {
            isAniStart = true;
            StartCoroutine(LoadSituation("Danceing"));
        }
        else if (dateTime.ToString("HHmmss") == "180000" || Input.GetKeyDown(KeyCode.Alpha0))
        {
            isAniStart = true;
            StartCoroutine(LoadSituation("Singing"));
        }
        if (dateTime.ToString("HHmmss") == "190000" || Input.GetKeyDown(KeyCode.Q))
        {
            isAniStart = true;
            StartCoroutine(LoadSituation("Running"));
        }
    }

    IEnumerator LoadSituation(string name)
    {
        string path = "Object/Situation/" + name;
        yield return StartCoroutine(ResourceLoader.Instance.Load<GameObject>(path,
           o =>
           {
               situation = Instantiate(o) as GameObject;
               situation.transform.parent = this.gameObject.transform;
               situation.transform.position = new Vector3(0, 0, 0);
               Message.Send<SetSituationAniMsg>(new SetSituationAniMsg(true));
               StartCoroutine(SituationEnd());
           }));
    }

    IEnumerator SituationEnd()
    {
        SoundManager.Instance.PlaySound((int) SoundType.TimeAnimation);
        yield return new WaitForSeconds(3);
        Destroy(situation);
        Resources.UnloadUnusedAssets();
        Message.Send<SetSituationAniMsg>(new SetSituationAniMsg(false));
        isAniStart = false;
    }
}
