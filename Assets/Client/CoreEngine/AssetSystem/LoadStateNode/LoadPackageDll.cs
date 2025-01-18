using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YooAsset;

public class LoadPackageDll : StateBase
{
    public override void Enter()
    {
        EventSystem.DispatchEvent<PatchStatesChange>(new PatchStatesChange("补充元数据!"));
        MonoSystem.BeginCoroutine(LoadDll());
    }

    private IEnumerator LoadDll()
    {
        if (!TryGetShareData("PackageName", out string packageName))
        {
            LogSystem.Error("设置的资源包名称不存在");
        }
        var package = YooAssets.GetPackage(packageName);
        var assets = new List<string>()
        {
            "Client_Logic.dll",
            "Client_GamePlay.dll",
            "Client_UI.dll",
        }.Concat(CoreEngineRoot.AOTMetaAssemblyFiles);
        foreach (var asset in assets)
        {
            RawFileHandle handle = package.LoadRawFileAsync($"Assets/Bundle/Dll/{asset}");
            yield return handle;
            byte[] fileData = handle.GetRawFileData();
            CoreEngineRoot.assetDataDic[asset] = fileData;
            Debug.Log($"{asset} size: {fileData.Length}");
        }
        stateMachine.ChangeState<ClearPackageCache>();
    }
}