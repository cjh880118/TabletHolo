﻿using JHchoi.Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JHchoi.UI.Event
{

    public class RhythmGameEndMsg : Message
    {
        public bool isPause;
        public RhythmGameEndMsg(bool isPause)
        {
            this.isPause = isPause;
        }
    }

    //class LeapMotionMenuMoveMsg : Message
    //{
    //    public Menu menu;
    //    public LeapMotionMenuMoveMsg(Menu menu)
    //    {
    //        this.menu = menu;
    //    }
    //}
}