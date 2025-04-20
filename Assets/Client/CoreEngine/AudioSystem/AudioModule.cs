using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class AudioModule : MonoBehaviour
{
    private static GameObjectPoolModule poolModule;

    [SerializeField, LabelText("背景音乐播放器")]
    private AudioSource BgAudioSource;
    [SerializeField, LabelText("音效播放器预制体")]
    private GameObject EffectAudioPlayPrefab;
    [SerializeField, LabelText("音效对象池预设数量")] 
    private int EffectAudioPoolNum = 20;
    // 场景中生效的所有特效音乐播放器
    private List<AudioSource> audioPlayList;

    #region 音量、播放控制

    [SerializeField, Range(0, 1), OnValueChanged("UpdateAllAudioPlay")]
    private float globalVolume;
    public float GlobalVolume
    {
        get => globalVolume;
        set
        {
            if (Mathf.Approximately(globalVolume, value)) return;
            globalVolume = value;
            UpdateAllAudioPlay();
        }
    }

    [SerializeField] [Range(0, 1)] [OnValueChanged("UpdateBGAudioPlay")]
    private float bgVolume;
    public float BGVolume
    {
        get => bgVolume;
        set
        {
            if (Mathf.Approximately(bgVolume, value)) return;
            bgVolume = value;
            UpdateBGAudioPlay();
        }
    }

    [SerializeField] [Range(0, 1)] [OnValueChanged("UpdateEffectAudioPlay")]
    private float effectVolume;
    public float EffectVolume
    {
        get => effectVolume;
        set
        {
            if (Mathf.Approximately(effectVolume, value)) return;
            effectVolume = value;
            UpdateEffectAudioPlay();
        }
    }

    [SerializeField] [OnValueChanged("UpdateMute")]
    private bool isMute = false;
    public bool IsMute
    {
        get => isMute;
        set
        {
            if (isMute == value) return;
            isMute = value;
            UpdateMute();
        }
    }

    [SerializeField] [OnValueChanged("UpdateLoop")]
    private bool isLoop = true;
    public bool IsLoop
    {
        get => isLoop;
        set
        {
            if (isLoop == value) return;
            isLoop = value;
            UpdateLoop();
        }
    }

    [SerializeField] [OnValueChanged("UpdatePause")]
    private bool isPause = false;
    public bool IsPause
    {
        get => isPause;
        set
        {
            if (isPause == value) return;
            isPause = value;
            UpdatePause();
        }
    }

    /// <summary>
    /// 更新全部播放器类型
    /// </summary>
    private void UpdateAllAudioPlay()
    {
        UpdateBGAudioPlay();
        UpdateEffectAudioPlay();
    }

    /// <summary>
    /// 更新背景音乐
    /// </summary>
    private void UpdateBGAudioPlay()
    {
        BgAudioSource.volume = bgVolume * globalVolume;
    }

    /// <summary>
    /// 更新特效音乐播放器
    /// </summary>
    private void UpdateEffectAudioPlay()
    {
        if (audioPlayList == null) return;
        // 倒序遍历
        for (int i = audioPlayList.Count - 1; i >= 0; i--)
        {
            if (audioPlayList[i] != null)
            {
                SetEffectAudioPlay(audioPlayList[i]);
            }
            else
            {
                audioPlayList.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// 设置特效音乐播放器
    /// </summary>
    private void SetEffectAudioPlay(AudioSource audioPlay, float spatial = -1)
    {
        audioPlay.mute = isMute;
        audioPlay.volume = effectVolume * globalVolume;
        if (spatial != -1)
        {
            audioPlay.spatialBlend = spatial;
        }
        if (isPause)
        {
            audioPlay.Pause();
        }
        else
        {
            audioPlay.UnPause();
        }
    }

    /// <summary>
    /// 更新全局音乐静音情况
    /// </summary>
    private void UpdateMute()
    {
        BgAudioSource.mute = isMute;
        UpdateEffectAudioPlay();
    }

    /// <summary>
    /// 更新背景音乐循环
    /// </summary>
    private void UpdateLoop()
    {
        BgAudioSource.loop = isLoop;
    }

    /// <summary>
    /// 更新背景音乐暂停
    /// </summary>
    private void UpdatePause()
    {
        if (isPause)
        {
            BgAudioSource.Pause();
        }
        else
        {
            BgAudioSource.UnPause();
        }
    }

    #endregion

    public void Init()
    {
        Transform poolRoot = new GameObject("AudioPoolRoot").transform;
        poolRoot.SetParent(transform);
        poolModule = new GameObjectPoolModule();
        poolModule.Init(poolRoot);
        poolModule.InitObjectPool(EffectAudioPlayPrefab, -1, EffectAudioPoolNum);
        audioPlayList = new List<AudioSource>(EffectAudioPoolNum);
        audioPlayRoot = new GameObject("AudioPlayRoot").transform;
        audioPlayRoot.SetParent(transform);
        UpdateAllAudioPlay();
    }

    #region 背景音乐

    private static Coroutine fadeCoroutine;

    public void PlayBgAudio(AudioClip clip, bool loop = true, float volume = -1, float fadeOutTime = 0,
        float fadeInTime = 0)
    {
        IsLoop = loop;
        if (!Mathf.Approximately(volume, -1))
        {
            BGVolume = volume;
        }
        fadeCoroutine = StartCoroutine(DoVolumeFade(clip, fadeOutTime, fadeInTime));
    }

    private IEnumerator DoVolumeFade(AudioClip clip, float fadeOutTime, float fadeInTime)
    {
        float currTime = 0;
        if (fadeOutTime <= 0) fadeOutTime = 0.0001f;
        if (fadeInTime <= 0) fadeInTime = 0.0001f;

        // 降低音量，也就是淡出
        while (currTime < fadeOutTime)
        {
            yield return CoroutineTool.WaitForFrames();
            if (!isPause) currTime += Time.deltaTime;
            float ratio = Mathf.Lerp(1, 0, currTime / fadeOutTime);
            BgAudioSource.volume = bgVolume * globalVolume * ratio;
        }

        BgAudioSource.clip = clip;
        BgAudioSource.Play();
        LogSystem.Log($"开始播放背景音乐: {clip.name}");
        currTime = 0;
        // 提高音量，也就是淡入
        while (currTime < fadeInTime)
        {
            yield return CoroutineTool.WaitForFrames();
            if (!isPause) currTime += Time.deltaTime;
            float ratio = Mathf.InverseLerp(0, 1, currTime / fadeInTime);
            BgAudioSource.volume = bgVolume * globalVolume * ratio;
        }
        fadeCoroutine = null;
    }

    private static Coroutine bgWithClipsCoroutine;

    /// <summary>
    /// 使用音效数组播放背景音乐，自动循环
    /// </summary>
    public void PlayBgAudioWithClips(AudioClip[] clips, float volume = -1, float fadeOutTime = 0, float fadeInTime = 0)
    {
        bgWithClipsCoroutine = MonoSystem.BeginCoroutine(DoPlayBgAudioWithClips(clips, volume));
    }

    private IEnumerator DoPlayBgAudioWithClips(AudioClip[] clips, float volume = -1, float fadeOutTime = 0,
        float fadeInTime = 0)
    {
        if (!Mathf.Approximately(volume, -1)) BGVolume = volume;
        int currIndex = 0;
        while (true)
        {
            AudioClip clip = clips[currIndex];
            fadeCoroutine = StartCoroutine(DoVolumeFade(clip, fadeOutTime, fadeInTime));
            float time = clip.length;
            // 时间只要还好，一直检测
            while (time > 0)
            {
                yield return CoroutineTool.WaitForFrames();
                if (!isPause) time -= Time.deltaTime;
            }
            // 到达这里说明倒计时结束，修改索引号，继续外侧While循环
            currIndex++;
            if (currIndex >= clips.Length) currIndex = 0;
        }
    }

    public void StopBgAudio()
    {
        if (bgWithClipsCoroutine != null) MonoSystem.EndCoroutine(bgWithClipsCoroutine);
        if (fadeCoroutine != null) MonoSystem.EndCoroutine(fadeCoroutine);
        BgAudioSource.Stop();
        BgAudioSource.clip = null;
    }

    public void PauseBgAudio()
    {
        IsPause = true;
    }

    public void UnPauseBgAudio()
    {
        IsPause = false;
    }

    #endregion

    #region 特效音乐

    private Transform audioPlayRoot;

    /// <summary>
    /// 获取音乐播放器
    /// </summary>
    /// <returns></returns>
    private AudioSource GetAudioPlay(bool is3D = true)
    {
        // 从对象池中获取播放器
        GameObject audioPlay = poolModule.GetObject("AudioPlay", audioPlayRoot);
        if (audioPlay.IsNull())
        {
            audioPlay = GameObject.Instantiate(EffectAudioPlayPrefab, audioPlayRoot);
            audioPlay.name = EffectAudioPlayPrefab.name;
        }
        AudioSource audioSource = audioPlay.GetComponent<AudioSource>();
        SetEffectAudioPlay(audioSource, is3D ? 1f : 0f);
        audioPlayList.Add(audioSource);
        return audioSource;
    }

    /// <summary>
    /// 回收播放器
    /// </summary>
    private void RecycleAudioPlay(AudioSource audioSource, AudioClip clip, Action callBak)
    {
        StartCoroutine(DoRecycleAudioPlay(audioSource, clip, callBak));
    }

    private IEnumerator DoRecycleAudioPlay(AudioSource audioSource, AudioClip clip,
        Action callBak)
    {
        // 延迟 Clip的长度（秒）
        yield return CoroutineTool.WaitForSeconds(clip.length);
        // 放回池子
        if (audioSource != null)
        {
            audioPlayList.Remove(audioSource);
            poolModule.PushObject(audioSource.gameObject);
            callBak?.Invoke();
        }
    }

    public void PlayOneShot(AudioClip clip, Component component = null,
        float volumeScale = 1, bool is3d = true, Action callBack = null)
    {
        // 初始化音乐播放器
        AudioSource audioSource = GetAudioPlay(is3d);
        if (component == null) audioSource.transform.SetParent(null);
        else
        {
            audioSource.transform.SetParent(component.transform);
            audioSource.transform.localPosition = Vector3.zero;
            // 宿主销毁时，释放父物体
            component.OnDestroy(OnOwnerDestroy, audioSource);
        }
        // 播放一次音效
        audioSource.PlayOneShot(clip, volumeScale);
        LogSystem.Log($"播放音效：{clip.name}");
        // 播放器回收以及回调函数
        callBack += () => PlayOverRemoveOwnerDestroyAction(component);
        RecycleAudioPlay(audioSource, clip, callBack);
    }

    // 宿主销毁时，提前回收
    private void OnOwnerDestroy(GameObject go, AudioSource audioSource)
    {
        audioSource.transform.SetParent(audioPlayRoot);
    }

    // 播放结束时移除宿主销毁Action
    private void PlayOverRemoveOwnerDestroyAction(Component owner)
    {
        if (owner != null) owner.RemoveOnDestroy<AudioSource>(OnOwnerDestroy);
    }

    public void PlayOneShot(AudioClip clip, Vector3 position, float volumeScale = 1,
        bool is3d = true, Action callBack = null)
    {
        // 初始化音乐播放器
        AudioSource audioSource = GetAudioPlay(is3d);
        audioSource.transform.position = position;

        // 播放一次音效
        audioSource.PlayOneShot(clip, volumeScale);
        LogSystem.Log($"播放音效：{clip.name}");
        // 播放器回收以及回调函数
        RecycleAudioPlay(audioSource, clip, callBack);
    }

    #endregion
}