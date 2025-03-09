using System.Collections;
using System.Collections.Generic;
using System.Linq;
using cfg.Skill;
using UnityEngine;

public class SkillBehaviorCom : MonoBehaviour, IComponent, IRequire<IEnumerable<ISkillComponent>>,
    IRequire<AnimationCom>
{
    private List<ISkillComponent> skillComponents;
    private CharacterController characterController;
    private AnimationCom animCom;

    private bool canRotate = false;

    public void SetDependency(AnimationCom dependency) => animCom = dependency;

    public void SetDependency(IEnumerable<ISkillComponent> dependency) =>
        skillComponents = dependency.ToList();

    public void Init()
    {
        characterController = GetComponent<CharacterController>();
    }

    #region 技能播放事件方法，用于在技能播放中操作技能

    /// <summary>
    /// 技能片段开始时调用
    /// </summary>
    public void OnSkillClipStart() { }

    /// <summary>
    /// 技能片段结束时会调用
    /// </summary>
    public void OnSkillClipEnd() { }

    /// <summary>
    /// 驱动技能时调用,技能运行时每一帧都会调用
    /// </summary>
    public void OnTickSkill(int frameIndex) { }

    /// <summary>
    /// 当攻击范围检测命中时调用
    /// </summary>
    public void OnAttackTrigger(Collider collider) { }

    /// <summary>
    /// 在自定义事件调用前调用
    /// 自定义事件可能会在调用时因为一些原因发生变化
    /// </summary>
    public SkillCustomEvent BeforeSkillCustomEvent(SkillCustomEvent customEvent)
    {
        LogSystem.Log("BeforeSkillCustomEvent");
        return customEvent;
    }

    /// <summary>
    /// 在动画事件调用前调用
    /// 动画事件可能会在调用时因为一些原因发生变化
    /// </summary>
    public SkillAnimationEvent BeforeSkillAnimationEvent(SkillAnimationEvent animationEvent)
    {
        LogSystem.Log("BeforeSkillAnimationEvent");
        return animationEvent;
    }

    /// <summary>
    /// 在音频事件调用前调用
    /// 音频事件可能会在调用时因为一些原因发生变化
    /// </summary>
    public SkillAudioEvent BeforeSkillAudioEvent(SkillAudioEvent audioEvent)
    {
        LogSystem.Log("BeforeSkillAudioEvent");
        return audioEvent;
    }

    /// <summary>
    /// 在范围检测事件调用前调用
    /// 范围检测事件可能会在调用时因为一些原因发生变化
    /// </summary>
    public SkillColliderEvent BeforeSkillColliderEvent(SkillColliderEvent colliderEvent)
    {
        LogSystem.Log("BeforeSkillColliderEvent");
        return colliderEvent;
    }

    /// <summary>
    /// 在特效事件调用前调用
    /// 特效事件可能会在调用时因为一些原因发生变化
    /// </summary>
    public SkillEffectEvent BeforeSkillEffectEvent(SkillEffectEvent effectEvent)
    {
        LogSystem.Log("BeforeSkillEffectEvent");
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
    }

    /// <summary>
    /// 在动画事件调用后调用
    /// 动画事件可能会在调用时因为一些原因发生变化
    /// </summary>
    public void AfterSkillAnimationEvent(SkillAnimationEvent animationEvent)
    {
        LogSystem.Log("AfterSkillAnimationEvent");
    }

    /// <summary>
    /// 在音频事件调用后调用
    /// 音频事件可能会在调用时因为一些原因发生变化
    /// </summary>
    public void AfterSkillAudioEvent(SkillAudioEvent audioEvent)
    {
        LogSystem.Log("AfterSkillAudioEvent");
    }

    /// <summary>
    /// 在范围检测事件调用后调用
    /// 范围检测事件可能会在调用时因为一些原因发生变化
    /// </summary>
    public void AfterSkillColliderEvent(SkillColliderEvent colliderEvent)
    {
        LogSystem.Log("AfterSkillColliderEvent");
    }

    /// <summary>
    /// 在特效事件调用后调用
    /// 特效事件可能会在调用时因为一些原因发生变化
    /// </summary>
    public virtual void AfterSkillEffectEvent(SkillEffectEvent effectEvent)
    {
        LogSystem.Log("AfterSkillEffectEvent");
    }

    public void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        animCom.transform.rotation *= deltaRotation;
        characterController.Move(deltaPosition);
    }

    #endregion
}