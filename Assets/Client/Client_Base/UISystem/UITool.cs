
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

namespace UITool
{
    public static class UITool
    {
        public static void SetFullRect(this RectTransform rectTrans)
        {
            rectTrans.anchorMin = Vector2.zero;
            rectTrans.anchorMax = Vector2.one;
            rectTrans.sizeDelta = Vector2.zero;
            rectTrans.anchoredPosition3D = Vector3.zero;
        }

        public static float GetAnimDuration(this Animator animator, string animName, float defaultTime = 0.1f)
        {
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            for (int i = 0; i < clips.Length; i++)
            {
                var clipName = clips[i].name.ToLower();
                if (clipName.IndexOf(animName) > 0)
                {
                    return clips[i].length;
                }
            }
            return defaultTime;
        }

    }
}

