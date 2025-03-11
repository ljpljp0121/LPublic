using System.Collections.Generic;
using System.Linq;
using cfg.Skill;
using UnityEngine;

[RequireComponent(typeof(AnimationCom))]
public class SkillBehaviorCom : MonoBehaviour, IComponent, IRequire<IEnumerable<ISkillComponent>>,
    IRequire<AnimationCom>
{
    private IEnumerable<ISkillComponent> skillComponents;
    private CharacterController characterController;
    private AnimationCom animCom;

    private bool canRotate = false;

    public void SetDependency(AnimationCom dependency) => animCom = dependency;

    public void SetDependency(IEnumerable<ISkillComponent> dependency) =>
        skillComponents = dependency.OrderByDescending(skillCom => skillCom.Order);

    public void Init()
    {
        characterController = GetComponent<CharacterController>();
    }

    #region 技能播放事件方法，用于在技能播放中操作技能

    /// <summary>
    /// 技能片段开始时调用
    /// </summary>
    public void OnSkillClipStart()
    {
        foreach (ISkillComponent component in skillComponents)
        {
            if (component is ISkillClipStart c)
            {
                c.OnSkillClipStart();
            }
        }
    }

    /// <summary>
    /// 技能片段结束时会调用
    /// </summary>
    public void OnSkillClipEnd()
    {
        foreach (ISkillComponent component in skillComponents)
        {
            if (component is ISkillClipEnd c)
            {
                c.OnSkillClipEnd();
            }
        }
    }

    /// <summary>
    /// 驱动技能时调用,技能运行时每一帧都会调用
    /// </summary>
    public void OnTickSkill(int frameIndex)
    {
        foreach (ISkillComponent component in skillComponents)
        {
            if (component is ISkillTick c)
            {
                c.OnTickSkill(frameIndex);
            }
        }
    }

    /// <summary>
    /// 当攻击范围检测命中时调用
    /// </summary>
    public void OnAttackTrigger(Collider collider)
    {
        foreach (ISkillComponent component in skillComponents)
        {
            if (component is ISkillAttackTrigger c)
            {
                c.OnAttackTrigger(collider);
            }
        }
    }

    /// <summary>
    /// 在自定义事件调用前调用
    /// 自定义事件可能会在调用时因为一些原因发生变化
    /// </summary>
    public SkillCustomEvent BeforeSkillCustomEvent(SkillCustomEvent customEvent)
    {
        LogSystem.Log("BeforeSkillCustomEvent");
        foreach (ISkillComponent component in skillComponents)
        {
            if (component is ISkillBeforeCustomEvent c)
            {
                c.BeforeSkillCustomEvent(customEvent);
            }
        }
        return customEvent;
    }

    /// <summary>
    /// 在动画事件调用前调用
    /// 动画事件可能会在调用时因为一些原因发生变化
    /// </summary>
    public SkillAnimationEvent BeforeSkillAnimationEvent(SkillAnimationEvent animationEvent)
    {
        LogSystem.Log("BeforeSkillAnimationEvent");
        foreach (ISkillComponent component in skillComponents)
        {
            if (component is ISkillBeforeAnimationEvent c)
            {
                c.BeforeSkillAnimationEvent(animationEvent);
            }
        }
        return animationEvent;
    }

    /// <summary>
    /// 在音频事件调用前调用
    /// 音频事件可能会在调用时因为一些原因发生变化
    /// </summary>
    public SkillAudioEvent BeforeSkillAudioEvent(SkillAudioEvent audioEvent)
    {
        LogSystem.Log("BeforeSkillAudioEvent");
        foreach (ISkillComponent component in skillComponents)
        {
            if (component is ISkillBeforeAudioEvent c)
            {
                c.BeforeSkillAudioEvent(audioEvent);
            }
        }
        return audioEvent;
    }

    /// <summary>
    /// 在范围检测事件调用前调用
    /// 范围检测事件可能会在调用时因为一些原因发生变化
    /// </summary>
    public SkillColliderEvent BeforeSkillColliderEvent(SkillColliderEvent colliderEvent)
    {
        LogSystem.Log("BeforeSkillColliderEvent");
        foreach (ISkillComponent component in skillComponents)
        {
            if (component is ISkillBeforeColliderEvent c)
            {
                c.BeforeSkillColliderEvent(colliderEvent);
            }
        }
        return colliderEvent;
    }

    /// <summary>
    /// 在特效事件调用前调用
    /// 特效事件可能会在调用时因为一些原因发生变化
    /// </summary>
    public SkillEffectEvent BeforeSkillEffectEvent(SkillEffectEvent effectEvent)
    {
        LogSystem.Log("BeforeSkillEffectEvent");
        foreach (ISkillComponent component in skillComponents)
        {
            if (component is ISkillBeforeEffectEvent c)
            {
                c.BeforeSkillEffectEvent(effectEvent);
            }
        }
        return effectEvent;
    }

    /// <summary>
    /// 在自定义事件调用后调用
    /// 自定义事件可能会在调用时因为一些原因发生变化
    /// </summary>
    /// <param name="customEvent">调用后的自定义事件</param>
    public void AfterSkillCustomEvent(SkillCustomEvent customEvent)
    {
        LogSystem.Log("AfterSkillCustomEvent");
        if (customEvent.EventType == SkillEventType.CanSkillRelease) { }
        else if (customEvent.EventType == SkillEventType.CanRotate)
        {
            canRotate = true;
        }
        else if (customEvent.EventType == SkillEventType.CanNotRotate)
        {
            canRotate = false;
        }

        foreach (ISkillComponent component in skillComponents)
        {
            if (component is ISkillAfterCustomEvent c)
            {
                c.AfterSkillCustomEvent(customEvent);
            }
        }
    }

    /// <summary>
    /// 在动画事件调用后调用
    /// 动画事件可能会在调用时因为一些原因发生变化
    /// </summary>
    public void AfterSkillAnimationEvent(SkillAnimationEvent animationEvent)
    {
        LogSystem.Log("AfterSkillAnimationEvent");
        foreach (ISkillComponent component in skillComponents)
        {
            if (component is ISkillAfterAnimationEvent c)
            {
                c.AfterSkillAnimationEvent(animationEvent);
            }
        }
    }

    /// <summary>
    /// 在音频事件调用后调用
    /// 音频事件可能会在调用时因为一些原因发生变化
    /// </summary>
    public void AfterSkillAudioEvent(SkillAudioEvent audioEvent)
    {
        LogSystem.Log("AfterSkillAudioEvent");
        foreach (ISkillComponent component in skillComponents)
        {
            if (component is ISkillAfterAudioEvent c)
            {
                c.AfterSkillAudioEvent(audioEvent);
            }
        }
    }

    /// <summary>
    /// 在范围检测事件调用后调用
    /// 范围检测事件可能会在调用时因为一些原因发生变化
    /// </summary>
    public void AfterSkillColliderEvent(SkillColliderEvent colliderEvent)
    {
        LogSystem.Log("AfterSkillColliderEvent");
        foreach (ISkillComponent component in skillComponents)
        {
            if (component is ISkillAfterColliderEvent c)
            {
                c.AfterSkillColliderEvent(colliderEvent);
            }
        }
    }

    /// <summary>
    /// 在特效事件调用后调用
    /// 特效事件可能会在调用时因为一些原因发生变化
    /// </summary>
    public virtual void AfterSkillEffectEvent(SkillEffectEvent effectEvent)
    {
        LogSystem.Log("AfterSkillEffectEvent");
        foreach (ISkillComponent component in skillComponents)
        {
            if (component is ISkillAfterEffectEvent c)
            {
                c.AfterSkillEffectEvent(effectEvent);
            }
        }
    }

    public void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        animCom.transform.rotation *= deltaRotation;
        characterController.Move(deltaPosition);
        foreach (ISkillComponent component in skillComponents)
        {
            if (component is IRootMotion c)
            {
                c.OnRootMotion(deltaPosition, deltaRotation);
            }
        }
    }

    #endregion
}