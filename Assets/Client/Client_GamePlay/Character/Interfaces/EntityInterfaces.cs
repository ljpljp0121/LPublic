using cfg.Skill;
using UnityEngine;

/// <summary>
/// 组件接口
/// </summary>
public interface IComponent
{
    void Init();
}

public interface IUpdatable
{
    void OnUpdate();
}

public interface IFixedUpdatable
{
    void OnFixedUpdate();
}

public interface ILateUpdatable
{
    void OnLateUpdate();
}

/// <summary>
/// 依赖接口
/// </summary>
public interface IRequire<T> where T : class
{
    void SetDependency(T dependency);
}

#region 技能相关

public interface ISkillComponent : IComponent
{
}

public interface ISkillClipStart
{
    void OnSkillClipStart();
}

public interface ISkillClipEnd
{
    void OnSkillClipEnd();
}

public interface ISkillTick
{
    void OnTickSkill(int frameIndex);
}

public interface ISkillAttackTrigger
{
    void OnAttackTrigger(Collider collider);
}

public interface ISkillBeforeCustomEvent
{
    SkillCustomEvent BeforeSkillCustomEvent(SkillCustomEvent customEvent);
}

public interface ISkillBeforeAnimationEvent
{
    SkillCustomEvent BeforeSkillAnimationEvent(SkillAnimationEvent animationEvent);
}

public interface ISkillBeforeAudioEvent
{
    SkillAudioEvent BeforeSkillAudioEvent(SkillAudioEvent audioEvent);
}

public interface ISkillBeforeEffectEvent
{
    SkillEffectEvent BeforeSkillEffectEvent(SkillEffectEvent effectEvent);
}

public interface ISkillBeforeColliderEvent
{
    SkillColliderEvent BeforeSkillColliderEvent(SkillColliderEvent colliderEvent);
}

public interface ISkillAfterCustomEvent
{
    void AfterSkillCustomEvent(SkillCustomEvent customEvent);
}

public interface ISkillAfterAnimationEvent
{
    void AfterSkillAnimationEvent(SkillAnimationEvent animationEvent);
}

public interface ISkillAfterAudioEvent
{
    void AfterSkillAudioEvent(SkillAudioEvent audioEvent);
}

public interface ISkillAfterEffectEvent
{
    void AfterSkillEffectEvent(SkillEffectEvent effectEvent);
}

public interface ISkillAfterColliderEvent
{
    void AfterSkillColliderEvent(SkillColliderEvent colliderEvent);
}

public interface IRootMotion
{
    void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation);
}

#endregion