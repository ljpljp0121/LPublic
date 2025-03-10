using cfg.Skill;
using JKFrame;
using log4net.Util;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.TextCore.Text;

/// <summary>
/// 技能播放器
/// </summary>
public class SkillPlayer1 : SerializedMonoBehaviour
{
    private Animation_Controller animation_Controller;

    private bool isPlaying = false;
    /// <summary>
    /// 当前是否播放
    /// </summary>
    public bool IsPlaying { get => IsPlaying; }

    private SkillClip skillClip;
    private int currentFrameIndex;
    private float playerTotalTime;
    private int frameRote;

    private Transform modelTransform;
    public Transform ModelTransform { get => modelTransform; }
    public LayerMask attackDetectionLayer;
    public ICharacter owner;
    public void Init(ICharacter owner, Animation_Controller animation_Controller, Transform modelTransform)
    {
        this.owner = owner;
        this.animation_Controller = animation_Controller;
        this.modelTransform = modelTransform;
        foreach (WeaponController item in weaponDic.Values)
        {
            item.Init(attackDetectionLayer, OnWeaponDetection);
        }
    }

    #region 武器

    [SerializeField] private ParentConstraint mainWeaponParentConstraint;
    [SerializeField] private Dictionary<string, WeaponController> weaponDic = new Dictionary<string, WeaponController>();

    public ParentConstraint MainWeaponParentConstraint { get => mainWeaponParentConstraint; }
    public Dictionary<string, WeaponController> WeaponDic { get => weaponDic; }

    /// <summary>
    /// 设置主武器在哪只手持有
    /// </summary>
    /// <param name="isLeft"></param>
    public void SetMainWeaponHand(bool isLeft)
    {
        if (mainWeaponParentConstraint == null) return;
        ConstraintSource left = mainWeaponParentConstraint.GetSource(0);
        ConstraintSource right = mainWeaponParentConstraint.GetSource(1);
        left.weight = isLeft ? 1 : 0;
        right.weight = isLeft ? 0 : 1;
        mainWeaponParentConstraint.SetSource(0, left);
        mainWeaponParentConstraint.SetSource(1, right);
    }

    /// <summary>
    /// 武器攻击检测到时触发
    /// </summary>
    private void OnWeaponDetection(IHitTarget hitTarget, AttackData attackData)
    {
        skillBehaviour.OnAttackDetection(hitTarget, attackData);
    }

    #endregion

    private SkillBehaviourBase skillBehaviour;

    /// <summary>
    /// 开始播放技能配置
    /// 当角色开始播放技能时,应当第一个调用它来获取播放技能时必要的配置
    /// </summary>
    /// <param name="skillBehaviour"></param>
    public void StartPlaySkillBehaviour(SkillBehaviourBase skillBehaviour)
    {
        this.skillBehaviour = skillBehaviour;
    }

    /// <summary>
    /// 播放技能片段
    /// </summary>
    /// <param name="skillClip">技能片段</param>
    /// <param name="skillEndAction">技能结束事件</param>
    /// <param name="onWeaponDetection">武器击中事件</param>
    /// <param name="rootMotionAction">根运动事件</param>
    public void PlaySkillClip(SkillClip skillClip)
    {
        this.skillClip = skillClip;
        currentFrameIndex = -1;
        frameRote = skillClip.FrameRote;
        playerTotalTime = 0;
        isPlaying = true;
    }

    /// <summary>
    /// 技能结束后清空所有事件
    /// </summary>
    private void Clean()
    {
        this.skillClip = null;
    }

    private void Update()
    {
        if (isPlaying)
        {
            playerTotalTime += Time.deltaTime;
            //根据总时间判断当前是第几帧
            int targetFrameIndex = (int)(playerTotalTime * frameRote);
            //防止一帧延迟过大，追帧
            while (currentFrameIndex < targetFrameIndex)
            {
                //驱动一次技能
                TickSkill();
            }
            //如果到达最后一帧，技能结束
            if (targetFrameIndex >= skillClip.FrameCount)
            {
                isPlaying = false;
                skillBehaviour.OnSkillClipEnd();
                Clean();
            }
        }
    }

    #region 驱动技能

    /// <summary>
    /// 驱动技能
    /// 根据当前的帧索引去采样技能轨道的所有轨道并呈现出来
    /// </summary>
    private void TickSkill()
    {
        currentFrameIndex++;
        skillBehaviour.OnTickSkill(currentFrameIndex);
        TickCustomEvent();
        TickAnimationEvent();
        TickAudioEvent();
        TickEffectEvent();
        TickAttackDetectionEvent();
    }

    /// <summary>
    /// 驱动自定义事件
    /// </summary>
    private void TickCustomEvent()
    {
        if (skillClip.SkillCustomEventData.FrameData.TryGetValue(currentFrameIndex, out SkillCustomEvent customEvent))
        {
            customEvent = skillBehaviour.BeforeSkillCustomEvent(customEvent);
            if (customEvent != null)
            {
                skillBehaviour.AfterSkillCustomEvent(customEvent);
            }
        }
    }

    /// <summary>
    /// 驱动动画
    /// </summary>
    private void TickAnimationEvent()
    {
        if (animation_Controller != null && skillClip.SkillAnimationData.FrameData.TryGetValue(currentFrameIndex, out SkillAnimationEvent animationEvent))
        {
            animationEvent = skillBehaviour.BeforeSkillAnimationEvent(animationEvent);
            if (animationEvent != null)
            {
                animation_Controller.PlaySingleAniamtion(animationEvent.AnimationClip, 1, true, animationEvent.TransitionTime);

                if (animationEvent.ApplyRootMotion)
                {
                    animation_Controller.SetRootMotionAction(skillBehaviour.OnRootMotion);
                }
                else
                {
                    animation_Controller.ClearRootMotionAction();
                }
                skillBehaviour.AfterSkillAnimationEvent(animationEvent);
            }
        }
    }

    /// <summary>
    /// 驱动音效
    /// </summary>
    private void TickAudioEvent()
    {
        for (int i = 0; i < skillClip.SkillAudioData.FrameData.Count; i++)
        {
            SkillAudioEvent audioEvent = skillClip.SkillAudioData.FrameData[i];
            audioEvent = skillBehaviour.BeforeSkillAudioEvent(audioEvent);
            if (audioEvent != null)
            {
                if (audioEvent.AudioClip != null && audioEvent.FrameIndex == currentFrameIndex)
                {
                    AudioSystem.PlayOneShot(audioEvent.AudioClip, transform.position, false, audioEvent.Volume);
                }
                skillBehaviour.AfterSkillAudioEvent(audioEvent);
            }
        }
    }

    /// <summary>
    /// 驱动特效
    /// </summary>
    private void TickEffectEvent()
    {
        for (int i = 0; i < skillClip.SkillEffectData.FrameData.Count; i++)
        {
            SkillEffectEvent effectEvent = skillClip.SkillEffectData.FrameData[i];
            effectEvent = skillBehaviour.BeforeSkillEffectEvent(effectEvent);
            if (effectEvent != null)
            {
                if (effectEvent.EffectPrefab != null && effectEvent.FrameIndex == currentFrameIndex)
                {
                    GameObject effectObj = PoolSystem.GetGameObject(effectEvent.EffectPrefab.name);
                    if (effectObj == null)
                    {
                        effectObj = GameObject.Instantiate(effectEvent.EffectPrefab);
                        effectObj.name = effectEvent.EffectPrefab.name;
                    }
                    effectObj.transform.position = modelTransform.TransformPoint(effectEvent.Position);
                    effectObj.transform.rotation = Quaternion.Euler(modelTransform.eulerAngles + effectEvent.Rotation);
                    effectObj.transform.localScale = effectEvent.Scale;
                    if (effectEvent.AutoDestroy)
                    {
                        StartCoroutine(AutoDestroyEffectObj((float)effectEvent.Duration / skillClip.FrameRote, effectObj));
                    }
                }
                skillBehaviour.AfterSkillEffectEvent(effectEvent);
            }
        }
    }

    /// <summary>
    /// 驱动伤害检测
    /// </summary>
    private void TickAttackDetectionEvent()
    {
#if UNITY_EDITOR
        if (drawAttackDetectionGizmos) currentAttackDetectionList.Clear();
#endif
        for (int i = 0; i < skillClip.SkillAttackDetectionData.FrameData.Count; i++)
        {
            SkillAttackDetectionEvent detectionEvent = skillClip.SkillAttackDetectionData.FrameData[i];
            detectionEvent = skillBehaviour.BeforeSkillAttackDetectionEvent(detectionEvent);
            if (detectionEvent != null)
            {
                AttackDetectionType type = detectionEvent.GetAttackDetectionType();
                //武器关注第一帧和结束帧
                if (type == AttackDetectionType.Weapon)
                {
                    if (detectionEvent.FrameIndex == currentFrameIndex)
                    {
                        //驱动武器开启
                        WeaponAttackDetection weaponDetection = (WeaponAttackDetection)detectionEvent.AttackDetectionData;
                        if (weaponDic.TryGetValue(weaponDetection.weaponName, out WeaponController weapon))
                        {
                            AttackData attackData = new AttackData()
                            {
                                detectionEvent = detectionEvent,
                                source = owner,
                                attackValue = owner.GetAttackValue(detectionEvent),
                            };
                            weapon.StartDetection(attackData);
                        }
                        else Debug.Log("没有指定的武器");
                    }
                    if (currentFrameIndex == detectionEvent.FrameIndex + detectionEvent.DurationFrame)
                    {
                        //驱动武器关闭
                        WeaponAttackDetection weaponDetection = (WeaponAttackDetection)detectionEvent.AttackDetectionData;
                        if (weaponDic.TryGetValue(weaponDetection.weaponName, out WeaponController weapon))
                        {
                            weapon.StopDetection();
                        }
                        else Debug.Log("没有指定的武器");
                    }
                }
                else
                {
                    //当前帧在范围内
                    if (currentFrameIndex >= detectionEvent.FrameIndex
                        && currentFrameIndex <= detectionEvent.FrameIndex + detectionEvent.DurationFrame)
                    {
                        Collider[] colliders = SkillAttackDetectionTool.ShapeDetection(modelTransform, detectionEvent.AttackDetectionData, type, attackDetectionLayer);
                        if (colliders == null || colliders.Length == 0) break;
                        for (int ii = 0; ii < colliders.Length; ii++)
                        {
                            if (colliders[ii] != null)
                            {
                                IHitTarget hitTarget = colliders[ii].GetComponentInChildren<IHitTarget>();
                                if (hitTarget != null)
                                {
                                    Vector3 pos = ((ShapeAttackDetection)detectionEvent.AttackDetectionData).Position;
                                    AttackData attackData = new AttackData()
                                    {
                                        detectionEvent = detectionEvent,
                                        source = owner,
                                        attackValue = owner.GetAttackValue(detectionEvent),
                                        hitPoint = colliders[ii].ClosestPoint(modelTransform.TransformPoint(pos)),
                                    };
                                    skillBehaviour.OnAttackDetection(hitTarget, attackData);
                                }
                            }
                        }
                    }
                }
#if UNITY_EDITOR
                if (drawAttackDetectionGizmos)
                {
                    if (currentFrameIndex >= detectionEvent.FrameIndex
                        && currentFrameIndex <= detectionEvent.FrameIndex + detectionEvent.DurationFrame)
                    {
                        currentAttackDetectionList.Add(detectionEvent);
                    }
                }
#endif
                skillBehaviour.AfterSkillAttackDetectionEvent(detectionEvent);
            }

        }
    }

    private IEnumerator AutoDestroyEffectObj(float time, GameObject obj)
    {
        yield return new WaitForSeconds(time);
        obj.GameObjectPushPool();
    }

    #endregion

    #region Editor
#if UNITY_EDITOR

    [SerializeField] private bool drawAttackDetectionGizmos;
    private List<SkillAttackDetectionEvent> currentAttackDetectionList = new List<SkillAttackDetectionEvent>();

    private void OnDrawGizmos()
    {
        if (drawAttackDetectionGizmos && currentAttackDetectionList.Count != 0)
        {
            for (int i = 0; i < currentAttackDetectionList.Count; i++)
            {
                SkillGizmosTool.DraweDetection(currentAttackDetectionList[i], this);
            }
        }
    }

    internal void Init(ICharacter owner, object animation_Controller, object modelTransform)
    {
        throw new NotImplementedException();
    }

#endif
    #endregion
}

