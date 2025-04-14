using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.RunTime
{
    public class GameComponent : MonoBehaviour, IGameComponent
    {
        //public GameTagAggregator GameplayTagAggregator { get; private set; }

        private void OnEnable()
        {
            GameComponentSystem.Instance.Register(this);
            Enable();
        }

        private void OnDisable()
        {
            GameComponentSystem.Instance.UnRegister(this);
            Disable();
        }

        public virtual void Enable() { }
        public virtual void Disable() { }
        public virtual void Tick() { }
    }
}

