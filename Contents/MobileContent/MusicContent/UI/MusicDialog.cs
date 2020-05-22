using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Android;
using JHchoi.Constants;

namespace JHchoi.UI
{
    public class MusicDialog : IDialog
    {
        public Button btnMusicList;

        protected override void OnLoad()
        {
            btnMusicList.onClick.AddListener(() => AndroidTrasferMgr.Instance.BluetoothSendMsg("MUSICINFO.MISICLISTREQUEST", SENDMSGTYPE.MUSIC));
        }
    }
}
