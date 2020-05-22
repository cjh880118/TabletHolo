using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CellBig.Contents
{
    public class CharacterContent : IContent
    {
        static string TAG = "CharacterContent :: ";

        #region Contents Load
        protected override void OnLoadStart()
        {
            Debug.Log(TAG + "OnLoadStart");
            StartCoroutine(LoadInitialData());
        }

        IEnumerator LoadInitialData()
        {
            string path = "Object/Character/GirlRttObject";
            yield return StartCoroutine(ResourceLoader.Instance.Load<GameObject>(path,
               o =>
               {
                   var inGameObject = Instantiate(o) as GameObject;
               }));

            path = "Object/Character/BoyRttObject";
            yield return StartCoroutine(ResourceLoader.Instance.Load<GameObject>(path,
               o =>
               {
                   var inGameObject = Instantiate(o) as GameObject;
                   inGameObject.transform.position = new Vector3(4, 0, 0);

               }));
            SetLoadComplete();
        }
        #endregion

        protected override void OnEnter()
        {
            AddMessage();
        }

        private void AddMessage()
        {
        }

        protected override void OnExit()
        {
            RemoveMessage();
        }

        private void RemoveMessage()
        {
        }
    }
}
