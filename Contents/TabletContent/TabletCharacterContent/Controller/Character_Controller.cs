using CellBig.Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig.UI.Event;
using System;
using Random = UnityEngine.Random;

public class Character_Controller : MonoBehaviour
{
    public GameObject Effect;
    public Schedule_Controller Schedule;
    public RuntimeAnimatorController NewAniController;

    //public RuntimeAnimatorController AloneGame;
    //public RuntimeAnimatorController MotionDance;
    //public RuntimeAnimatorController Face;
    //public RuntimeAnimatorController Idle;
    public Animator animator;

    public GameObject[] dress;
    public GameObject etc;

    int touchNum = (int)AnimationType.Touch0;
    bool isTouch = false;
    Coroutine corTouchTimer;
    Coroutine corAnimation;

    public void SetCharacter(GameObject effect, Schedule_Controller schedule)
    {
        animator.runtimeAnimatorController = NewAniController;
        Effect = effect;
        Schedule = schedule;
    }

    public void SetDress(int dressNum)
    {
        etc.SetActive(false);

        foreach (var o in dress)
        {
            o.SetActive(false);
        }
        if (dressNum < 5)
            etc.SetActive(true);

        dress[dressNum].SetActive(true);
    }

    public void CharacterTouch()
    {
        //if (animator.runtimeAnimatorController == Idle && !isTouch)
        //{
        AnimatorStateInfo animatorInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (!isTouch && animatorInfo.IsTag("Idle"))
        {
            isTouch = true;
            AnimationType ani = (AnimationType)Enum.Parse(typeof(AnimationType), touchNum.ToString());
            animator.SetTrigger(ani.ToString());
            Message.Send<CharacterTouchMsg>(new CharacterTouchMsg(ani));

            if (corTouchTimer != null)
            {
                StopCoroutine(TouchTimer());
                corTouchTimer = null;
            }

            touchNum++;
            if (touchNum > (int)AnimationType.Touch4)
                touchNum = (int)AnimationType.Touch0;

            corTouchTimer = StartCoroutine(TouchTimer());
        }
    }

    IEnumerator TouchTimer()
    {
        yield return new WaitForSeconds(1.5f);
        isTouch = false;
    }

    public void SetAniMation(int aniNum, bool isBluetoothCommand)//, Character character)
    {
        if (corAnimation != null)
        {
            StopCoroutine(corAnimation);
            corAnimation = null;
        }

        if (aniNum >= 0 && aniNum < 3)
        {
           // animator.runtimeAnimatorController = Idle;

            if (!isBluetoothCommand)
            {
                if (corAnimation != null)
                {
                    StopCoroutine(corAnimation);
                    corAnimation = null;
                }

                if (this.gameObject.activeSelf)
                    corAnimation = StartCoroutine(RandomIdleAni());

                animator.SetBool("Idle", true);
                animator.SetInteger("AnimationNum", aniNum);
            }
        }
        else if (aniNum >= 3 && aniNum < 9)
        {
            //3 ~ 8
            //Effect.SetActive(false);
            //animator.runtimeAnimatorController = AloneGame;
            animator.SetBool("Idle", false);
            animator.SetTrigger(((AnimationType)aniNum).ToString());
        }
        else if (aniNum >= 9 && aniNum < 15)
        {
            //Effect.SetActive(false);
            //animator.runtimeAnimatorController = Face;
        }
        else if (aniNum >= 15)
        {
            //Effect.SetActive(false);
            animator.SetBool("Idle", false);
            Debug.Log(((AnimationType)aniNum).ToString());
            animator.SetTrigger(((AnimationType)aniNum).ToString());

            if (!isBluetoothCommand)
            {
                if (corAnimation != null)
                    StopCoroutine(corAnimation);

                corAnimation = StartCoroutine(RandomMotionAni(aniNum));
            }
        }

        Debug.Log("aniNum : " + aniNum);
        Schedule.SetObject((AnimationType)aniNum);
    }

    IEnumerator RandomIdleAni()
    {
        float rndSec = Random.Range(5, 10);
        yield return new WaitForSeconds(rndSec);
        int aniNum = Random.Range((int)AnimationType.Idel1, (int)AnimationType.Idel3 + 1);
        Message.Send<SetCharacterAnimationMsg>(new SetCharacterAnimationMsg((AnimationType)aniNum, false));
    }

    IEnumerator RandomMotionAni(int num)
    {
        yield return new WaitForSeconds(3f);

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            yield return null;
        }

        Debug.Log("재생끝");

        int rndDance = Random.Range((int)AnimationType.Motion1, (int)AnimationType.Motion7 + 1);

        while (num == rndDance)
        {
            rndDance = Random.Range((int)AnimationType.Motion1, (int)AnimationType.Motion7 + 1);
            Debug.Log("rndDance : " + rndDance);
        }

        Message.Send<SetCharacterAnimationMsg>(new SetCharacterAnimationMsg((AnimationType)rndDance, false));
    }

    public void EffectOff(bool isOnEffect)
    {
        if (isOnEffect)
            Effect.SetActive(true);
        else
            Effect.SetActive(false);
    }
}
