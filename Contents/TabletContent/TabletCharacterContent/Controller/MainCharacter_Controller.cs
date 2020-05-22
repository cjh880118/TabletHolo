using CellBig;
using CellBig.Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacter_Controller : MonoBehaviour
{
    GameObject character;
    Character_Controller nowCharacter_Controller;

    IEnumerator LoadCharacter(Character name, int dressNum)
    {
        string path = "Object/Character/" + name.ToString();
        yield return StartCoroutine(ResourceLoader.Instance.Load<GameObject>(path,
           o =>
           {
               character = Instantiate(o) as GameObject;
               character.transform.parent = this.gameObject.transform;
               character.transform.position = new Vector3(0, 0, 0);
               character.SetActive(true);
               nowCharacter_Controller = character.GetComponent<Character_Controller>();
               nowCharacter_Controller.SetDress(dressNum);

               //SetCharacterAnimation(new SetCharacterAnimationMsg(AnimationType.Idel1, false));
               //dicCharacter.Add(Character.Boy, inGameObject.transform.GetChild(0).gameObject);
               //dicCharacter.Add(Character.Girl, inGameObject.transform.GetChild(1).gameObject);
               //foreach (var obj in dicCharacter)
               //{
               //    obj.Value.SetActive(false);
               //}

           }));
    }
}
