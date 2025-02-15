using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using HybridCLR;
using UnityEngine;
using YooAsset;

public class LoadPackageDll : StateBase
{
    public List<string> AOTMetaAssemblyFiles { get; } = new List<string>()
    {
        "mscorlib.dll",
        "System.dll",
        "System.Core.dll",
    };

    public List<string> HotUpdateAssets { get; } = new List<string>()
    {
        "Client_Logic.dll",
        "Client_GamePlay.dll",
        "Client_UI.dll",
    };

    public Dictionary<string, byte[]> assetDataDic = new Dictionary<string, byte[]>();

    private int progress = 70;

    public override void Enter()
    {
        StartLoading.SetProgress("加载热更DLL", progress);
        MonoSystem.BeginCoroutine(LoadDll());
    }

    private IEnumerator LoadDll()
    {
        if (!TryGetShareData("PackageName", out string packageName))
        {
            LogSystem.Error("设置的资源包名称不存在");
            yield break;
        }

        ResourcePackage package = YooAssets.GetPackage(packageName);

        yield return LoadAssets(package, AOTMetaAssemblyFiles, "AOTDll", true);
        yield return LoadAssets(package, HotUpdateAssets, "HotUpdateDll");

        LoadHotUpdateAssemblies();
        stateMachine.ChangeState<ClearPackageCache>();
    }

    private IEnumerator LoadAssets(ResourcePackage package, List<string> assets, string pathName,
        bool isAOTMetadata = false)
    {
        foreach (var asset in assets)
        {
            RawFileHandle handle = package.LoadRawFileAsync($"Assets/Bundle/{pathName}/{asset}");
            yield return handle;
            if (handle.Status != EOperationStatus.Succeed)
            {
                LogSystem.Error($"加载失败: {asset}, 错误: {handle.LastError}");
                continue;
            }

            byte[] fileData = handle.GetRawFileData();
            assetDataDic[asset] = fileData;
            Debug.Log($"{asset} 加载成功，大小: {fileData.Length}");
            StartLoading.SetProgress($"加载热更DLL: {asset}", progress++);
            // 如果是 AOT 元数据，需要加载到 HybridCLR 运行时
            if (isAOTMetadata)
            {
                LoadAOTMetadata(asset, fileData);
            }

            handle.Release(); // 释放资源句柄
        }
    }

    private void LoadAOTMetadata(string assetName, byte[] dllData)
    {
        try
        {
            // 使用 HybridCLR 的 API 加载 AOT 元数据
            HomologousImageMode mode = HomologousImageMode.SuperSet;
            RuntimeApi.LoadMetadataForAOTAssembly(dllData, mode);
            Debug.Log($"AOT 元数据加载成功: {assetName}");
            StartLoading.SetProgress($"加载AOT: {assetName}", progress++);
        }
        catch (Exception e)
        {
            Debug.LogError($"AOT 元数据加载失败: {assetName}, 错误: {e}");
        }
    }

    private void LoadHotUpdateAssemblies()
    {
        foreach (var asset in HotUpdateAssets)
        {
            if (assetDataDic.TryGetValue(asset, out byte[] dllData))
            {
                try
                {
                    System.Reflection.Assembly.Load(dllData);
                    LogSystem.Log($"热更新 DLL 加载成功: {asset}");
                }
                catch (Exception e)
                {
                    LogSystem.Log($"热更新 DLL 加载失败: {asset}, 错误: {e}");
                }
            }
        }
    }
}