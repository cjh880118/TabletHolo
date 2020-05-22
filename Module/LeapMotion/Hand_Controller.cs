using JHchoi.Contents;
using JHchoi.UI.Event;
using Midiazen;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JHchoi.Models;

public class Hand_Controller : MonoBehaviour
{
    string prevMenuTag;
    string nowMenuTag;
    //float touchTime = 0;
    bool isTouch = false;
    RaycastHit[] raycastIndex;
    RaycastHit[] raycastMiddle;
    RaycastHit ray;
    public float distance = 10f;
    public GameObject palm;
    public GameObject indexEnd;
    public GameObject indexFront;
    public GameObject middleEnd;
    public GameObject middleFront;
    SettingModel settingModel;

    public float multiflier = 1f;
    

    private void Start()
    {
        settingModel = SettingModel.First<SettingModel>();
    }

    private void LateUpdate()
    {
        Vector3 pos = transform.position * multiflier;
        pos.z = transform.position.z;
        transform.position = pos;
        RayCheck(raycastIndex, indexFront);
    }

    void RayCheck(RaycastHit[] raycastHits, GameObject objectFront)//, GameObject objectEnd)
    {
        Vector3 dir = (objectFront.transform.position - (Vector3.forward * distance)) - objectFront.transform.position;
        raycastHits = Physics.RaycastAll(objectFront.transform.position, dir, distance + Vector3.Distance((objectFront.transform.position - (Vector3.forward * distance)), indexFront.transform.position));
        Debug.DrawRay(objectFront.transform.position, dir * (distance + Vector3.Distance((objectFront.transform.position - (Vector3.forward * distance)), objectFront.transform.position)), Color.red);
        foreach (RaycastHit hit in raycastHits)
        {
            if (hit.collider.tag == "MenuButton")
            {
                hit.collider.gameObject.GetComponent<MenuItem_Controller>().ObjectSelect();
            }
            else if (hit.collider.tag == "MusicButton")
            {
                hit.collider.gameObject.GetComponent<MusicItem_Controller>().ObjectSelect();
            }
            else if (hit.collider.tag == "OptionButton")
            {
                hit.collider.gameObject.GetComponent<OptionItem_Controller>().ObjectSelect();
            }
            else if (hit.collider.tag == "OptionSoundButton")
            {
                hit.collider.gameObject.GetComponent<OptionSoundItem_Controller>().ObjectSelect();
            }
            else if (hit.collider.tag == "RhythmButton" && !isTouch)
            {
                hit.collider.gameObject.GetComponent<RhythmGameButton_Controller>().OnInputKey();
            }
            else if (hit.collider.tag == "MenuSound")
            {
                hit.collider.gameObject.GetComponentInParent<SoundItem_Controller>().ObjectSelect();
            }

            else if (hit.collider.tag == "STTON")
            {
                Message.Send<STTRecord>(new STTRecord());
            }

            else if (hit.collider.tag == "Character" && !settingModel.isOpenMenu)
            {
                hit.collider.gameObject.GetComponentInParent<Character_Controller>().CharacterTouch();
            }
        }
    }

    //void RunMenu(string menu)
    //{
    //    if (menu == "WatchMenu")
    //    {
    //        IContent.RequestContentEnter<TabletWatchContent>();
    //    }
    //    else if (menu == "MusicMenu")
    //    {
    //        IContent.RequestContentEnter<TabletMusicContent>();
    //    }
    //}
}
