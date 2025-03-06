using System.Collections.Generic;
using cfg.Skill;
using UnityEngine;

public class SkillPlayer : MonoBehaviour, IComponent, IUpdatable, IRequire<AnimationComponent>
{
    private AnimationComponent animComp;

    public bool IsPlaying { get; private set; }

    private int currentFrameIndex;
    private float playerTotalTime;
    private int frameRate;
    private SkillClip skillClip;

    private Dictionary<int, AnimationClip> animClipDic = new();
    private Dictionary<int, AudioClip> audioClipDic = new();
    private Dictionary<int, GameObject> effectDic = new();

    public void SetDependency(AnimationComponent dependency) => animComp = dependency;

    public void Init()
    {

    }

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

    #region 驱动技能

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

        }
    }

    private void TickAnimation()
    {
        if (animComp == null)
        {
            LogSystem.Error("玩家没有挂载AnimationComponent，无法播放动画!!");
            return;
        }

        if (skillClip.AnimationEvent.TryGetValue(currentFrameIndex, out var animEvent))
        {
            if (!animClipDic.TryGetValue(animEvent.AnimationClip.GetHashCode(), out var animClip))
            {
                animClip = AssetSystem.LoadAsset<AnimationClip>(animEvent.AnimationClip);
                animClipDic[animEvent.GetHashCode()] = animClip;
            }
            animComp.PlaySingleAnimation(animClip, 1, true, animEvent.TransitionTime);

        }
    }

    private void TickAudio()
    {
        for (int i = 0; i < skillClip.AudioEvent.Count; i++)
        {
            SkillAudioEvent audioEvent = skillClip.AudioEvent[i];
            if (audioEvent != null && audioEvent.FrameIndex == currentFrameIndex)
            {
                
            }
        }
    }

    private void TickEffect()
    {

    }

    private void TickCollider()
    {

    }

    #endregion
}