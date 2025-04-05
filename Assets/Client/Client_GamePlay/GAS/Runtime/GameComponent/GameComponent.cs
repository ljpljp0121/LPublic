using System.Collections;
using UnityEngine;

namespace GAS.Runtime
{
    /// <summary>
    /// 组件接口
    /// </summary>
    public interface IGameComponent
    {
        void Tick();
    }

    public abstract class GameComponent : MonoBehaviour, IGameComponent
    {
        private void OnEnable()
        {
            GameplayAbilitySystem.GAS.Register(this);
            Enable();
            StartCoroutine(DelayCall());
        }

        public IEnumerator DelayCall()
        {
            yield return CoroutineTool.WaitForEndOfFrame();
            AfterEnable();
        }

        private void OnDisable()
        {
            GameplayAbilitySystem.GAS.Unregister(this);
            Disable();
        }

        public virtual void Tick() { }
        public virtual void Enable() { }
        public virtual void AfterEnable() { }
        public virtual void Disable() { }
    }
}