﻿//#define LOAD_FROM_ASSETBUNDLE

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using CellBig;
using CellBig.Common;
using System.IO;


namespace CellBig
{
    public class TableManager : MonoSingleton<TableManager>
    {
        bool _alreadyLoading = false;
        bool _loadComplete = false;

        readonly Dictionary<System.Type, object> _tables = new Dictionary<System.Type, object>();

        public bool IsComplete { get { return _loadComplete; } }

        public T GetTableClass<T>() where T : class
        {
            object table;

            if (_tables.TryGetValue(typeof(T), out table))
                return (T)table;

            Debug.LogErrorFormat("{0} is null", typeof(T).Name);
            return null;
        }

        public IEnumerator Load()
        {
            if (_loadComplete)
                yield break;

            if (_alreadyLoading)
            {
                while (!_loadComplete)
                    yield return null;

                yield break;
            }

            _alreadyLoading = true;
            _loadComplete = false;

            while (!Log.Instance.IsInitializeComplete)
                yield return new WaitForSeconds(0.2f);

            yield return ResourceLoader.Instance.Load<BT_Sound>("Tables/Table/BT_Sound", o =>
            {
                _tables.Add(Type.GetType("BT_Sound"), o);
            });

            //상점 테이블 로드
            yield return ResourceLoader.Instance.Load<Buff_Table>("Tables/Table/Buff_Table", o =>
            {
                _tables.Add(Type.GetType("Buff_Table"), o);
            });

            yield return ResourceLoader.Instance.Load<Cash_Table>("Tables/Table/Cash_Table", o =>
            {
                _tables.Add(Type.GetType("Cash_Table"), o);
            });

            yield return ResourceLoader.Instance.Load<Character_Table>("Tables/Table/Character_Table", o =>
            {
                _tables.Add(Type.GetType("Character_Table"), o);
            });

            yield return ResourceLoader.Instance.Load<Skill_Table>("Tables/Table/Skill_Table", o =>
            {
                _tables.Add(Type.GetType("Skill_Table"), o);
            });

            yield return ResourceLoader.Instance.Load<Skin_Table>("Tables/Table/Skin_Table", o =>
            {
                _tables.Add(Type.GetType("Skin_Table"), o);
            });



            //yield return AssetBundleLoader.Instance.Load<Level_Table>("Table/Level_Table", "Level_Table", o =>
            //{
            //    _tables.Add(Type.GetType("Level_Table"), o);
            //});

            _alreadyLoading = false;
            _loadComplete = true;
        }

        public void Clear()
        {
            //ResourceLoader.Instance.Unload("Tables/Table/BT_Sound");

            _tables.Clear();
            Debug.Log("Clear Tables. - " + GetInstanceID());
        }

        protected override void Release()
        {
            Clear();
        }
    }
}
