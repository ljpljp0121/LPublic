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
        }

        private void OnDisable()
        {
            GameplayAbilitySystem.GAS.Unregister(this);
            Disable();
        }

        public virtual void Tick() { }
        public virtual void Enable() { }
        public virtual void Disable() { }
    }
}