using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.RunTime
{
    public class GameComponentSystem
    {
        private static GameComponentSystem instance;
        public static GameComponentSystem Instance => instance ??= new GameComponentSystem();


        private GameComponentSystem()
        {
            GameComponents = new List<GameComponent>();
            GameTimer.InitStartTimestamp();

            GameHost = new GameObject("GameHost").AddComponent<GameHost>();
            GameHost.hideFlags = HideFlags.HideAndDontSave;
            Object.DontDestroyOnLoad(GameHost.gameObject);
            GameHost.gameObject.SetActive(true);
        }

        public List<GameComponent> GameComponents { get; }
        private GameHost GameHost { get; }

        public bool IsPaused => !GameHost.enabled;

        public void Register(GameComponent gameComponent)
        {
            if (GameComponents.Contains(gameComponent)) return;
            GameComponents.Add(gameComponent);
        }

        public bool UnRegister(GameComponent gameComponent)
        {
            return GameComponents.Remove(gameComponent);
        }

        public void Pause()
        {
            GameHost.enabled = false;
        }

        public void UnPause()
        {
            GameHost.enabled = true;
        }

        public void ClearComponents()
        {
            foreach (var t in GameComponents)
            {
                t.Disable();
            }
            GameComponents.Clear();
        }
    }
}

