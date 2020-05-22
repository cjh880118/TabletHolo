using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CellBig.Contents
{
    public class RhythmGameContent : IContent
    {
        protected override void OnEnter()
        {
            AddMessage();
        }

        private void AddMessage()
        {
        }

        protected override void OnExit()
        {
            DialogAllClose();
            RemoveMessage();
        }

        private void RemoveMessage()
        {
        }
    }
}
