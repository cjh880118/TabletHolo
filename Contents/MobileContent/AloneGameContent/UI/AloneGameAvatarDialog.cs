using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JHchoi.UI
{
    public class AloneGameAvatarDialog : IDialog
    {
        public RawImage ImgCharacter;
        public AvatarStatus_Controller avatarStatus_Controller;

        protected override void OnEnter()
        {
            avatarStatus_Controller.AddMessage();
        }

        protected override void OnExit()
        {
            avatarStatus_Controller.RemoveMessage();
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}