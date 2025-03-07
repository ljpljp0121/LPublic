using System.Collections;
using System.Collections.Generic;
using cfg.Skill;
using UnityEngine;

public class SkillPlayerCom : MonoBehaviour, IComponent, IUpdatable, IRequire<AnimationCom>, IRequire<IEnumerable<WeaponController>>
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

    private AnimationCom animComp;
    private Dictionary<string, WeaponController> weaponDic = new Dictionary<string, WeaponController>();
    public void SetDependency(AnimationCom dependency) => animComp = dependency;
    public void SetDependency(IEnumerable<WeaponController> dependency)
    {
        foreach (var weaponController in dependency)
        {
            
        }
    }

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

    #endregion

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
            AnimationClip animClip = PoolSystem.GetObject<AnimationClip>(animEvent.AnimationClip);
            if (animClip == null)
            {
                animClip = AssetSystem.LoadAsset<AnimationClip>(animEvent.AnimationClip);
                PoolSystem.PushObject(animClip, animEvent.AnimationClip);
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
                AudioClip audioClip = PoolSystem.GetObject<AudioClip>(audioEvent.AudioClip);
                if (audioClip == null)
                {
                    audioClip = AssetSystem.LoadAsset<AudioClip>(audioEvent.AudioClip);
                    PoolSystem.PushObject(audioClip, audioEvent.AudioClip);
                }
                AudioSystem.PlayOneShot(audioClip, transform.position);
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
                GameObject effectObj = PoolSystem.GetGameObject(effectEvent.EffectPrefab);
                if (effectObj == null)
                {
                    var prefab = AssetSystem.LoadAsset<GameObject>(effectEvent.EffectPrefab);
                    effectObj = Instantiate(prefab);
                    effectObj.name = effectEvent.EffectPrefab.Substring(effectEvent.EffectPrefab.LastIndexOf('/') + 1);
                }
                effectObj.transform.position = ModelTransform.TransformPoint(
                    new Vector3(effectEvent.Position.X, effectEvent.Position.Y, effectEvent.Position.Z));
                effectObj.transform.rotation = Quaternion.Euler(ModelTransform.eulerAngles + new Vector3(
                    effectEvent.Rotation.X,
                    effectEvent.Rotation.Y, effectEvent.Rotation.Z));
                effectObj.transform.localScale =
                    new Vector3(effectEvent.Scale.X, effectEvent.Scale.Y, effectEvent.Scale.Z);
                if (effectEvent.AutoDestroy)
                {
                    MonoSystem.BeginCoroutine(AutoDestroyEffectObj((float)effectEvent.Duration / skillClip.FrameRate,
                        effectObj));
                }
            }
        }
    }

    private IEnumerator AutoDestroyEffectObj(float time, GameObject obj)
    {
        yield return new WaitForSeconds(time);
        obj.GameObjectPushPool();
    }

    private void TickCollider()
    {
    }

    #endregion

#if UNITY_EDITOR

    [SerializeField] private bool IsDrawCollider;
    private List<SkillColliderEvent> curColliderList = new List<SkillColliderEvent>();

    private void OnDrawGizmos()
    {
        if (IsDrawCollider && curColliderList.Count != 0)
        {
            for (int i = 0; i < curColliderList.Count; i++)
            {
            }
        }
    }

#endif
}