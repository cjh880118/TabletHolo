﻿using JHchoi.UI.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JHchoi.UI
{
    public class RhythmGameResultDialog : IDialog
    {
        public Text txtBad;
        public Text txtNormal;
        public Text txtGood;
        public Text txtPerfect;
        public Text txtCombo;
        public Text txtScore;

        protected override void OnEnter()
        {
            AddMessage();
        }

        private void AddMessage()
        {
            Message.AddListener<RhythmGameResultMsg>(RhythmGameResult);
        }

        private void RhythmGameResult(RhythmGameResultMsg msg)
        {
            txtBad.text = msg.bad.ToString();
            txtNormal.text = msg.normal.ToString();
            txtGood.text = msg.good.ToString();
            txtPerfect.text = msg.perfect.ToString();
            txtCombo.text = msg.combo.ToString();
            txtScore.tag = msg.score.ToString();
        }

        protected override void OnExit()
        {
            RemoveMessage();
        }

        private void RemoveMessage()
        {
            Message.RemoveListener<RhythmGameResultMsg>(RhythmGameResult);
        }
    }
}
