using System;
using System.Collections.Generic;
using GAS.Runtime;
using UnityEngine;
using UnityEngine.Serialization;


public class Player : GameComponent
{
    public Dictionary<string, AnimationClip> animDict = new Dictionary<string, AnimationClip>();

    #region 组件初始化

    public void Start()
    {
        PreLoadAssets();
    }

    private void PreLoadAssets() { }

    public void OnDestroy() { }
    public void Tick()
    {

    }

    #endregion
}