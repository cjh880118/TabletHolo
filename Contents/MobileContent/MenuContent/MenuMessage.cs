using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JHchoi.Constants;

namespace JHchoi.UI.Event
{
    public class RunMenuMsg : Message
    {
        public Menu menu;
        public RunMenuMsg(Menu menu)
        {
            this.menu = menu;
        }
    }

    public class RunGameMsg : Message
    {
        public GameType menu;
        public RunGameMsg(GameType menu)
        {
            this.menu = menu;
        }
    }

    public class GameSelectCloseMsg : Message
    {

    }
}