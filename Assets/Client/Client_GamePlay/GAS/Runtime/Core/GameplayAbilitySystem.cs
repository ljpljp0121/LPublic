using System.Collections.Generic;
using GAS.General;
using GAS.Runtime;
using UnityEngine;

namespace GAS
{
    public class GameplayAbilitySystem
    {
        private static GameplayAbilitySystem _gas;

        private GameplayAbilitySystem()
        {
            AbilitySystemComponents = new List<SkillSystemComponent>();
            GameComponents = new List<GameComponent>();
            SkillTimer.InitStartTimestamp();

            GasHost = new GameObject("GAS Host").AddComponent<GasHost>();
            GasHost.hideFlags = HideFlags.HideAndDontSave;
            Object.DontDestroyOnLoad(GasHost.gameObject);
            GasHost.gameObject.SetActive(true);
        }

        public List<SkillSystemComponent> AbilitySystemComponents { get; }
        public List<GameComponent> GameComponents { get; }

        private GasHost GasHost { get; }

        public static GameplayAbilitySystem GAS
        {
            get
            {
                _gas ??= new GameplayAbilitySystem();
                return _gas;
            }
        }

        public bool IsPaused => !GasHost.enabled;

        public void Register(SkillSystemComponent skillSystemComponent)
        {
            if (AbilitySystemComponents.Contains(skillSystemComponent)) return;
            AbilitySystemComponents.Add(skillSystemComponent);
        }

        public bool Unregister(SkillSystemComponent skillSystemComponent)
        {
            return AbilitySystemComponents.Remove(skillSystemComponent);
        }

        public void Register(GameComponent gameComponent)
        {
            if (GameComponents.Contains(gameComponent)) return;
            GameComponents.Add(gameComponent);
        }

        public bool Unregister(GameComponent gameComponent)
        {
            return GameComponents.Remove(gameComponent);
        }

        public void Pause()
        {
            GasHost.enabled = false;
        }

        public void Unpause()
        {
            GasHost.enabled = true;
        }

        public void ClearComponents()
        {
            foreach (var t in AbilitySystemComponents)
                t.Dispose();

            AbilitySystemComponents.Clear();

            foreach (var t in GameComponents)
                t.Disable();
            GameComponents.Clear();
        }

    }
}