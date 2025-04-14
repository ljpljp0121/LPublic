using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.RunTime
{
    public class GameHost : MonoBehaviour
    {
        private GameComponentSystem gcs => GameComponentSystem.Instance;

        private void Update()
        {
            GameTimer.UpdateCurrentFrameCount();
            var snapshot = gcs.GameComponents.ToArray();
            foreach (var gameComponent in snapshot)
            {
                gameComponent.Tick();
            }       
        }

        private void OnDestroy()
        {
            gcs.ClearComponents();
        }
    }

}

