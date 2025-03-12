using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;

public class Launcher : MonoBehaviour
{
    public static Launcher Instance;
    public GameObject[] ShowLoginImage;
    public AudioSource Audio;

    private bool launchEnd;
    public UnityEvent launchEndAction;

    void Awake()
    {
        Instance = this;

        PlayLoginAnim();
    }

    private void PlayLoginAnim()
    {
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.2f);
        if (ShowLoginImage.Length < 3)
            PlayLoginVoice();
        for (int i = 0; i < ShowLoginImage.Length; i++)
        {
            if (ShowLoginImage[i] != null && ShowLoginImage[i].TryGetComponent<VideoPlayer>(out var video))
            {
                DoFit(video.transform);
            }

            TweenCallback callback = null;
            if (ShowLoginImage.Length >= 3)
            {
                if (i == 0)
                {
                    callback = PlayLoginVoice;
                }
            }
            if (i == ShowLoginImage.Length - 1)
            {
                callback = LoginAniComplete;
            }
            DealSplash(seq, ShowLoginImage[i], callback);
        }
        seq.Play();
        LogSystem.Log("PlayLoginAnim");
    }

    private void DealSplash(Sequence seq, GameObject go, TweenCallback callback)
    {
        if (go.GetComponent<Image>() != null)
        {
            Image img = go.GetComponent<Image>();
            img.SetNativeSize();
            seq.AppendInterval(0.1f);
            seq.Append(img.DOFade(1, 1.1f));
            seq.Append(img.DOFade(0, 1.1f));
        }
        else
        {
            var rimg = go.GetComponent<RawImage>();
            var video = go.GetComponent<VideoPlayer>();
            video.Prepare();
            video.prepareCompleted += (v) =>
            {
                video.Pause();
                video.frame = 0;
            };
            seq.AppendCallback(() =>
            {
                go.SetActive(true);
            });
            seq.Append(rimg.DOFade(1, 1.1f));
            seq.AppendCallback(() => video.Play());
            seq.AppendInterval((float)video.clip.length);
            seq.Append(rimg.DOFade(0, 1.1f));
            seq.AppendCallback(() => go.SetActive(false));
        }
        if (callback != null)
        {
            seq.AppendCallback(callback);
        }
    }

    private void LoginAniComplete()
    {
        LogSystem.Log("LoginAniComplete");
        launchEnd = true;
        launchEndAction.Invoke();
    }

    private void PlayLoginVoice()
    {

    }

    #region 界面适配

    /// <summary>超过该值，宽度适配</summary>
    public Vector2 WidthMaxSize = new Vector2(1920f, 1080f);
    /// <summary>超过该值，高度适配</summary>
    public Vector2 HeightMaxSize = new Vector2(1920f, 1080f);
    public Vector3 selfVec = new Vector3(1f, 1f, 1f);
    private void DoFit(Transform tran)
    {
        Debug.Log($"分辨率{Screen.currentResolution.width},{Screen.currentResolution.height}");
        Debug.Log($"宽高{Screen.width},{Screen.height}");
        // 获取设备的宽高比例
        var deviceRatio = (float)Screen.width / Screen.height;
        // 宽度适配比例 
        var WidthMaxSizeRatio = WidthMaxSize.x / WidthMaxSize.y;
        // 高度适配比例
        var HeightMaxSizeRatio = HeightMaxSize.x / HeightMaxSize.y;

        //需要高度适配
        if (deviceRatio > HeightMaxSizeRatio)
        {
            var sx = Screen.width / HeightMaxSize.x;
            var sy = Screen.height / HeightMaxSize.y;
            var ratio = Mathf.Max(sx, sy);
            if (ratio > 1) tran.localScale = selfVec * ratio;
        }
        //需要宽度适配
        else if (deviceRatio < WidthMaxSizeRatio)
        {
            var sx = Screen.width / WidthMaxSize.x;
            var sy = Screen.height / WidthMaxSize.y;
            var ratio = Mathf.Max(sx, sy);
            if (ratio > 1) tran.localScale = selfVec * ratio;
        }
        else tran.localScale = selfVec;
    }

    #endregion
}
