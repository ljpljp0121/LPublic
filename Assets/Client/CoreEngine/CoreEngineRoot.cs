using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using HybridCLR;
using UnityEngine;
using YooAsset;

/// <summary>
/// 存档系统类型
/// </summary>
public enum SaveSystemType
{
    Binary,
    Json
}

/// <summary>
/// 核心框架根节点
/// 所有系统的初始化都在这里
/// </summary>
[DefaultExecutionOrder(-200)]
public class CoreEngineRoot : MonoBehaviour
{
    /// <summary>
    /// 资源系统运行模式
    /// </summary>
    public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;
    public SaveSystemType SaveSystemType = SaveSystemType.Binary;

    public string ResourcePackageName = "ResourcePackage";
    public string DllPackageName = "DllPackage";
    public static string Version = "v1.0.0";

    public static CoreEngineRoot Instance { get; set; }
    public static Transform RootTransform { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Debug.Log($"资源系统运行模式：{PlayMode}");
        Instance = this;
        RootTransform = transform;
        DontDestroyOnLoad(gameObject);
        StartCoroutine(InitSystems());
    }

    /// <summary>
    /// 初始化各个系统
    /// </summary>
    private IEnumerator InitSystems()
    {
        PoolSystem.Init();
        EventSystem.Init();
        MonoSystem.Init();
        AudioSystem.Init();
        TimeSystem.Init();
        var go = Resources.Load<GameObject>("StartLoading");
        GameObject.Instantiate(go, GameObject.Find("UIRoot").transform);
        yield return AssetSystem.Init(DllPackageName, ResourcePackageName, PlayMode);
        GameObject gameRoot = AssetSystem.LoadAsset<GameObject>("Prefab/GameRoot");
        Instantiate(gameRoot, RootTransform);
    }
}