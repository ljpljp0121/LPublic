using System.Collections;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using YooAsset;

/// <summary>
/// �浵ϵͳ����
/// </summary>
public enum SaveSystemType
{
    Binary,
    Json
}

public class Bootstrap : MonoBehaviour
{
    public SaveSystemType SaveSystemType = SaveSystemType.Binary;

    [Title("Assets Setting")]
    public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;
    public string ResourcePackageName = "ResourcePackage";
    public string DllPackageName = "DllPackage";
    public string Version = "v1.0.0";


    public static Bootstrap Instance { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Debug.Log($"��Դϵͳ����ģʽ��{PlayMode}");
        Instance = this;
        DontDestroyOnLoad(gameObject);
        BootTaskUtil.Run(async () =>
        {
            await ReadyAsset();
        });
    }

    private async Task ReadyAsset()
    {
        YooAssets.Initialize();
        await StartLoadUtils.InitDll(DllPackageName, PlayMode);
        await StartLoadUtils.InitResource(ResourcePackageName, PlayMode);
        var package = YooAssets.TryGetPackage(ResourcePackageName);
        YooAssets.SetDefaultPackage(package);
    }
}