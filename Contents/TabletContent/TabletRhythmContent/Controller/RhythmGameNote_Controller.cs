﻿using CellBig.Constants;
using CellBig.UI.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmGameNote_Controller : MonoBehaviour
{
    // Start is called before the first frame update
    bool isRight;
    int index;
    Coroutine corMove;
    public RhythmNote rhythmNote;
    public int noteCount;
    public GameObject[] note;
    public ParticleSystem particle;

    public void MoveStart(Vector3 target, bool isRight, int index, int noteCount)
    {
        //this.gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
        particle.gameObject.SetActive(false);
        rhythmNote = RhythmNote.None;
        this.isRight = isRight;
        this.index = index;
        this.noteCount = noteCount;
        foreach (var o in note)
        {
            o.SetActive(false);
        }

        corMove = StartCoroutine(MoveNote(target));
    }

    IEnumerator MoveNote(Vector3 target)
    {
        note[0].SetActive(true);

        while (true)
        {
            yield return null;

            //(1.8  0)
            float distance = Vector3.Distance(this.gameObject.transform.position, target);

            if (distance > 1.3)
            {
                note[0].SetActive(true);
                rhythmNote = RhythmNote.None;
            }
            else if (distance <= 1.3 & distance > 1)
            {
                note[0].SetActive(false);
                note[1].SetActive(true);
                rhythmNote = RhythmNote.Bad;
            }
            else if (distance <= 1 & distance > 0.7)
            {
                note[1].SetActive(false);
                note[2].SetActive(true);
                rhythmNote = RhythmNote.Normal;
            }
            else if (distance <= 0.7 & distance > 0.3)
            {
                note[1].SetActive(false);
                note[2].SetActive(true);
                rhythmNote = RhythmNote.Good;
            }
            else if (distance <= 0.3 & distance > 0)
            {
                note[2].SetActive(false);
                note[3].SetActive(true);
                rhythmNote = RhythmNote.Perfect;
            }
            else
            {
                note[0].SetActive(true);
                rhythmNote = RhythmNote.Bad;
                NoteDelete();
            }

            this.gameObject.transform.position = Vector3.MoveTowards(this.gameObject.transform.position, target, 0.7f * Time.deltaTime);
        }
    }

    public void NoteDelete()
    {
        Message.Send<RhythmGameNoteDeleteMsg>(new RhythmGameNoteDeleteMsg(rhythmNote, index, this.gameObject, isRight, true));
    }

    public void TouchEffect()
    {
        this.gameObject.GetComponent<Animator>().SetTrigger("Touch");
        particle.gameObject.SetActive(true);
    }
}
