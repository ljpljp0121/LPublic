using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using HybridCLR;
using UnityEngine;
using YooAsset;

/// <summary>
/// 核心框架根节点
/// 所有系统的初始化都在这里
/// </summary>
[DefaultExecutionOrder(-100)]
public class CoreEngineRoot : MonoBehaviour
{
    /// <summary>
    /// 资源系统运行模式
    /// </summary>
    public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;

    public string ResourcePackageName = "ResourcePackage";
    public string DllPackageName = "DllPackage";

    private static CoreEngineRoot Instance { get; set; }
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
        TableSystem.Init();
        AudioSystem.Init();
        yield return AssetSystem.InitDll(DllPackageName, PlayMode);
        yield return AssetSystem.InitResource(ResourcePackageName, PlayMode);
        SceneSystem.LoadSceneAsync("GameScene");
    }
}