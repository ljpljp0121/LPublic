using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class Player : MonoBehaviour, IComponent
{
    public const int HpMax = 100;
    public const int MpMax = 100;
    public const int StaminaMax = 100;
    public const int ATK = 5;
    public const int Speed = 8;


    public Dictionary<string, AnimationClip> animDict = new Dictionary<string, AnimationClip>();

    #region 组件初始化

    public void Init()
    {
        PreLoadAssets();
    }

    private void PreLoadAssets() { }

    public void UnInit() { }

    #endregion
}