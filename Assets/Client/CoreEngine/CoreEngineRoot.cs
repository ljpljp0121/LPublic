using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 核心框架根节点
/// 所有系统的初始化都在这里
/// </summary>
[DefaultExecutionOrder(-100)]
public class CoreEngineRoot : MonoBehaviour
{
    private static CoreEngineRoot Instance { get; set; }
    public static Transform RootTransform { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        RootTransform = transform;
        DontDestroyOnLoad(gameObject);
        Init();
    }

    private void Init()
    {
        InitSystems();
    }

    /// <summary>
    /// 初始化各个系统
    /// </summary>
    private void InitSystems()
    {
        PoolSystem.Init();
        EventSystem.Init();
        MonoSystem.Init();
        TableSystem.Init();
        AudioSystem.Init();
    }
    
}