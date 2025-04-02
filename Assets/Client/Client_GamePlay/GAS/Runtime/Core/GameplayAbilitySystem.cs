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
            AbilitySystemComponents = new List<AbilitySystemComponent>();
            GameComponents = new List<GameComponent>();
            GASTimer.InitStartTimestamp();

            GasHost = new GameObject("GAS Host").AddComponent<GasHost>();
            GasHost.hideFlags = HideFlags.HideAndDontSave;
            Object.DontDestroyOnLoad(GasHost.gameObject);
            GasHost.gameObject.SetActive(true);
        }

        public List<AbilitySystemComponent> AbilitySystemComponents { get; }
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

        public void Register(AbilitySystemComponent abilitySystemComponent)
        {
            // if (!GasHost.enabled)
            // {
            //     Debug.LogWarning("[EX] GAS is paused, can't register new ASC!");
            //     return;
            // }

            if (AbilitySystemComponents.Contains(abilitySystemComponent)) return;
            AbilitySystemComponents.Add(abilitySystemComponent);
        }

        public bool Unregister(AbilitySystemComponent abilitySystemComponent)
        {
            // if (!GasHost.enabled)
            // {
            //     Debug.LogWarning("[EX] GAS is paused, can't unregister ASC!");
            //     return false;
            // }

            return AbilitySystemComponents.Remove(abilitySystemComponent);
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