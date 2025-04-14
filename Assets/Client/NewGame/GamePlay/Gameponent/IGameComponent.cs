using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.RunTime
{
    public interface IGameComponent
    {
        public void Enable();
        public void Disable();
        public void Tick();
    }
}

