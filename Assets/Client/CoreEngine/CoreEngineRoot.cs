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
    public static List<string> AOTMetaAssemblyFiles { get; } = new List<string>()
    {
        "mscorlib.dll",
        "System.dll",
        "System.Core.dll",
    };
    public static Dictionary<string, byte[]> assetDataDic = new Dictionary<string, byte[]>();

    /// <summary>
    /// 资源系统运行模式
    /// </summary>
    public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;

    public string PackageName = "DefaultPackage";
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
        yield return AssetSystem.Init(PackageName, PlayMode);
        StartGame();
        SceneSystem.LoadSceneAsync("GameScene");
    }

    private static Assembly client_Logic;
    private static Assembly client_GamePlay;
    private static Assembly client_UI;

    private void StartGame()
    {
        //LoadMetadataForAOTAssemblies();
// #if !UNITY_EDITOR
        Assembly.Load(ReadBytesFromStreamingAssets("Client_Logic.dll"));
        Assembly.Load(ReadBytesFromStreamingAssets("Client_GamePlay.dll"));
        Assembly.Load(ReadBytesFromStreamingAssets("Client_UI.dll"));
// #else
//         client_Logic = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "Client_Logic");
//         client_GamePlay = System.AppDomain.CurrentDomain.GetAssemblies()
//             .First(a => a.GetName().Name == "Client_GamePlay");
//         client_UI = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "Client_UI");
// #endif
    }

    /// <summary>
    /// 为aot assembly加载原始metadata， 这个代码放aot或者热更新都行。
    /// 一旦加载后，如果AOT泛型函数对应native实现不存在，则自动替换为解释模式执行
    /// </summary>
    private static void LoadMetadataForAOTAssemblies()
    {
        HomologousImageMode mode = HomologousImageMode.SuperSet;
        foreach (var aotDllName in AOTMetaAssemblyFiles)
        {
            byte[] dllBytes = ReadBytesFromStreamingAssets(aotDllName);
            // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
            LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);
            Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. mode:{mode} ret:{err}");
        }
    }

    public static byte[] ReadBytesFromStreamingAssets(string dllName)
    {
        return assetDataDic[dllName];
    }
}