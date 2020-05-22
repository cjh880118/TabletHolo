using CellBig.UI.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CellBig.UI
{
    public class AvatarStatus_Controller : MonoBehaviour
    {
        public Text txtVocal;
        public Text txtDance;
        public Text txtEntertainment;
        public Text txtIntelligence;
        public Text txtPotential;
        public Text txtCoin;

        public Image imgVocal;
        public Image imgDance;
        public Image imgEntertainment;
        public Image imgIntelligence;

        public void AddMessage()
        {
            Message.AddListener<AvatarStatusMsg>(AvatarStatus);
        }

        private void AvatarStatus(AvatarStatusMsg msg)
        {
            txtVocal.text = msg.playerStatus.vocal.ToString();
            txtDance.text = msg.playerStatus.dance.ToString();
            txtEntertainment.text = msg.playerStatus.entertainment.ToString();
            txtIntelligence.text = msg.playerStatus.intelligence.ToString();
            txtPotential.text = msg.playerStatus.potential.ToString();
            txtCoin.text = msg.playerStatus.coin.ToString();

            imgVocal.fillAmount = msg.playerStatus.vocalGage * 0.01f;
            imgDance.fillAmount = msg.playerStatus.danceGage * 0.01f;
            imgEntertainment.fillAmount = msg.playerStatus.entertainmentGage * 0.01f;
            imgIntelligence.fillAmount = msg.playerStatus.intelligenceGage * 0.01f;
        }

        public void RemoveMessage()
        {
            Message.RemoveListener<AvatarStatusMsg>(AvatarStatus);
        }
    }
}