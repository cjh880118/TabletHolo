using CellBig;
using CellBig.Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HolostartCharacter_Controller : MonoBehaviour
{
    Animator nowCharacterAnimator;
    GameObject character;
    Coroutine corCharacterRotate;

    public void SetCharacter(Character nowCharacter)
    {
        nowCharacterAnimator = null;
        StartCoroutine(LoadCharacterData(nowCharacter.ToString()));
    }

    IEnumerator LoadCharacterData(string name)
    {
        string path = "Object/Face/" + name;
        yield return StartCoroutine(ResourceLoader.Instance.Load<GameObject>(path,
           o =>
           {
               character = Instantiate(o) as GameObject;
               character.transform.parent = this.gameObject.transform;
               character.transform.position = new Vector3(0, 0, 0);
               nowCharacterAnimator = character.GetComponent<Animator>();
           }));
    }

    IEnumerator CharacterRotate()
    {
        while (character.gameObject.activeSelf)
        {
            character.transform.DORotate(new Vector3(0, -15, 1), 3);
            yield return new WaitForSeconds(3.0f);
            character.transform.DORotate(new Vector3(0, 15, 1), 3);
            yield return new WaitForSeconds(3.0f);
        }
    }

    public void SetAniMation(int aniNum, bool isBluetoothCommand)
    {
        nowCharacterAnimator.SetInteger("AnimationNum", aniNum);
    }

    public void ZoomInOut(bool isZoom)
    {
        if (nowCharacterAnimator == null)
            return;

        if (!isZoom)
        {
            nowCharacterAnimator.SetBool("Zoom", false);
            nowCharacterAnimator.SetTrigger("ZoomOut");

            if (corCharacterRotate != null)
            {
                StopCoroutine(corCharacterRotate);
                corCharacterRotate = null;
            }

            DOTween.PauseAll();
            character.transform.rotation = Quaternion.identity;
        }
        else
        {
            nowCharacterAnimator.SetBool("Zoom", true);
            nowCharacterAnimator.SetTrigger("ZoomIn");

         
            if(corCharacterRotate == null)
                corCharacterRotate = StartCoroutine(CharacterRotate());
        }

    }

    public void CharacterDestory()
    {
        if (corCharacterRotate != null)
            StopCoroutine(corCharacterRotate);

        Destroy(character);
        Resources.UnloadUnusedAssets();
    }
}
