using System.Collections;
using System.Collections.Generic;
using cfg.Skill;
using UnityEngine;

[RequireComponent(typeof(AnimationCom))]
public class SkillPlayerCom : MonoBehaviour, IComponent, IUpdatable, IRequire<AnimationCom>,
    IRequire<IEnumerable<WeaponController>>, IRequire<SkillBehaviorCom>
{
    #region 变量

    public bool IsPlaying { get; private set; }
    public Transform ModelTransform => animComp.ModelTransform;

    private int currentFrameIndex;
    private float playerTotalTime;
    private int frameRate;
    private SkillClip skillClip;

    #endregion

    #region 组件

    private SkillBehaviorCom skillBehaviorCom;
    private AnimationCom animComp;
    private readonly Dictionary<string, WeaponController> weaponDic = new Dictionary<string, WeaponController>();
    public Dictionary<string, WeaponController> WeaponDic => weaponDic;
    public LayerMask ColliderLayer;
    public void SetDependency(AnimationCom dependency) => animComp = dependency;

    public void SetDependency(SkillBehaviorCom dependency) => skillBehaviorCom = dependency;

    public void SetDependency(IEnumerable<WeaponController> dependency)
    {
        foreach (var weaponController in dependency)
        {
            weaponDic.Add(weaponController.WeaponName, weaponController);
        }
    }

    public void Init() { }

    public void OnUpdate()
    {
        if (!IsPlaying) return;
        playerTotalTime += Time.deltaTime;
        int targetFrameIndex = (int)(playerTotalTime * frameRate);
        while (currentFrameIndex < targetFrameIndex)
        {
            TickSkill();
        }
        if (targetFrameIndex >= skillClip.FrameCount)
        {
            IsPlaying = false;
            Clear();
        }
    }

    #endregion

    /// <summary>
    /// 播放技能
    /// </summary>
    public void PlaySkillClip(int clipId)
    {
        SkillClip clip = TableSystem.GetVOData<TbSkillClip>().Get(clipId);
        if (clip == null)
        {
            LogSystem.Error($"对象{gameObject.name}的技能配置表中没有ID为{clipId}的技能!!");
            return;
        }
        PlaySkillClip(clip);
    }

    public void PlaySkillClip(SkillClip clip)
    {
        this.skillClip = clip;
        currentFrameIndex = -1;
        frameRate = skillClip.FrameRate;
        playerTotalTime = 0;
        IsPlaying = true;
    }

    private void Clear()
    {
        this.skillClip = null;
    }

    #region 播放技能

    private void TickSkill()
    {
        currentFrameIndex++;
        TickCustomEvent();
        TickAnimation();
        TickAudio();
        TickEffect();
        TickCollider();
    }

    private void TickCustomEvent()
    {
        if (skillClip.CustomEvent.TryGetValue(currentFrameIndex, out SkillCustomEvent customEvent))
        {
            customEvent = skillBehaviorCom.BeforeSkillCustomEvent(customEvent);
            if (customEvent != null)
                skillBehaviorCom.AfterSkillCustomEvent(customEvent);
        }
    }

    private void TickAnimation()
    {
        if (animComp == null)
        {
            LogSystem.Error($"对象{gameObject.name}身上没有挂载AnimationComponent!!");
            return;
        }

        if (skillClip.AnimationEvent.TryGetValue(currentFrameIndex, out var animEvent))
        {
            animEvent = skillBehaviorCom.BeforeSkillAnimationEvent(animEvent);
            AnimationClip animClip = PoolSystem.GetObject<AnimationClip>(animEvent.AnimationClip);
            if (animClip == null)
            {
                animClip = AssetSystem.LoadAsset<AnimationClip>(animEvent.AnimationClip);
            }
            if (animClip == null)
            {
                LogSystem.Error($"对象{gameObject.name}的技能配置表中的动画事件{animEvent.AnimationClip}找不到对应的动画!!");
                return;
            }
            PoolSystem.PushObject(animClip, animEvent.AnimationClip);
            animComp.PlaySingleAnimation(animClip, 1, true, animEvent.TransitionTime);
            animComp.SetRootMotionAction(skillBehaviorCom.OnRootMotion);
            skillBehaviorCom.AfterSkillAnimationEvent(animEvent);
        }
    }

    private void TickAudio()
    {
        for (int i = 0; i < skillClip.AudioEvent.Count; i++)
        {
            SkillAudioEvent audioEvent = skillClip.AudioEvent[i];
            if (audioEvent != null && audioEvent.FrameIndex == currentFrameIndex)
            {
                audioEvent = skillBehaviorCom.BeforeSkillAudioEvent(audioEvent);
                if (audioEvent == null) return;
                AudioClip audioClip = PoolSystem.GetObject<AudioClip>(audioEvent.AudioClip);
                if (audioClip == null)
                {
                    audioClip = AssetSystem.LoadAsset<AudioClip>(audioEvent.AudioClip);
                }
                if (audioClip == null)
                {
                    LogSystem.Error($"对象{gameObject.name}的技能配置表中的音频事件{audioEvent.AudioClip}找不到对应的音频!!");
                    return;
                }
                PoolSystem.PushObject(audioClip, audioEvent.AudioClip);
                AudioSystem.PlayBgAudio(audioClip);
                skillBehaviorCom.AfterSkillAudioEvent(audioEvent);
            }
        }
    }

    private void TickEffect()
    {
        for (int i = 0; i < skillClip.EffectEvent.Count; i++)
        {
            SkillEffectEvent effectEvent = skillClip.EffectEvent[i];
            if (effectEvent.EffectPrefab != null && effectEvent.FrameIndex == currentFrameIndex)
            {
                effectEvent = skillBehaviorCom.BeforeSkillEffectEvent(effectEvent);
                if (effectEvent == null) return;
                GameObject effectObj = PoolSystem.GetGameObject(effectEvent.EffectPrefab);
                if (effectObj == null)
                {
                    var prefab = AssetSystem.LoadAsset<GameObject>(effectEvent.EffectPrefab);
                    if (prefab != null)
                        effectObj = Instantiate(prefab);
                }
                if (effectObj == null)
                {
                    LogSystem.Error($"对象{gameObject.name}的技能配置表中的特效事件{effectEvent.EffectPrefab}找不到对应的特效!!");
                    return;
                }
                effectObj.transform.position = ModelTransform.TransformPoint(effectEvent.Position);
                effectObj.transform.rotation = Quaternion.Euler(ModelTransform.eulerAngles + effectEvent.Rotation);
                effectObj.transform.localScale = effectEvent.Scale;
                LogSystem.Log($"特效{effectEvent.EffectPrefab}创建成功");
                if (effectEvent.AutoDestroy)
                {
                    MonoSystem.BeginCoroutine(AutoDestroyEffectObj((float)effectEvent.Duration / skillClip.FrameRate,
                        effectObj, effectEvent.EffectPrefab));
                }
                skillBehaviorCom.AfterSkillEffectEvent(effectEvent);
            }
        }
    }

    private IEnumerator AutoDestroyEffectObj(float time, GameObject obj, string keyName)
    {
        yield return new WaitForSeconds(time);
        obj.GameObjectPushPool(keyName);
    }

    private void TickCollider()
    {
#if UNITY_EDITOR
        if (IsDrawCollider) curColliderList.Clear();
#endif
        for (var i = 0; i < skillClip.ColliderEvent.Count; i++)
        {
            var colliderEvent = skillClip.ColliderEvent[i];
            if (colliderEvent == null)
            {
                return;
            }
            if (colliderEvent.FrameIndex == currentFrameIndex)
                colliderEvent = skillBehaviorCom.BeforeSkillColliderEvent(colliderEvent);

            if (colliderEvent == null) return;
            if (colliderEvent.ColliderData is WeaponCollider weaponCollider)
            {
                if (colliderEvent.FrameIndex == currentFrameIndex)
                {
                    if (weaponDic.TryGetValue(weaponCollider.WeaponName, out var weaponController))
                    {
                        weaponController.StartDetection();
                        LogSystem.Log("开启武器检测");
                    }
                    else LogSystem.Error("没有找到对应名称武器");
                }
                else if (colliderEvent.FrameIndex + colliderEvent.DurationFrame == currentFrameIndex)
                {
                    if (weaponDic.TryGetValue(weaponCollider.WeaponName, out var weaponController))
                    {
                        weaponController.EndDetection();
                        LogSystem.Log("关闭武器检测");
                    }
                    else LogSystem.Error("没有找到对应名称武器");
                }
            }
            else
            {
                if (currentFrameIndex >= colliderEvent.FrameIndex
                    && currentFrameIndex <= colliderEvent.FrameIndex + colliderEvent.DurationFrame)
                {
                    Collider[] colliders = SkillColliderTool.ShapeDetection(ModelTransform,
                        colliderEvent.ColliderData, ColliderLayer);
                    if (colliders == null || colliders.Length == 0) break;
                    for (int ii = 0; ii < colliders.Length; ii++)
                    {
                        if (colliders[ii] != null) { }
                    }
                    LogSystem.Log($"检测到{colliders.Length}个碰撞体");
                }
            }
#if UNITY_EDITOR
            if (IsDrawCollider)
            {
                if (currentFrameIndex >= colliderEvent.FrameIndex
                    && currentFrameIndex <= colliderEvent.FrameIndex + colliderEvent.DurationFrame)
                {
                    curColliderList.Add(colliderEvent);
                }
            }
#endif
            if (colliderEvent.FrameIndex + colliderEvent.DurationFrame == currentFrameIndex)
                skillBehaviorCom.AfterSkillColliderEvent(colliderEvent);
        }
    }

    #endregion

    #region Editor

#if UNITY_EDITOR
    [SerializeField] private bool IsDrawCollider;
    private readonly List<SkillColliderEvent> curColliderList = new List<SkillColliderEvent>();

    private void OnDrawGizmos()
    {
        if (IsDrawCollider && curColliderList.Count != 0)
        {
            for (int i = 0; i < curColliderList.Count; i++)
            {
                SkillGizmosTool.DrawCollider(curColliderList[i], this);
            }
        }
    }

#endif

    #endregion
}