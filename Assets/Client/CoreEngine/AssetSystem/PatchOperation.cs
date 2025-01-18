using YooAsset;

public class PatchOperation : GameAsyncOperation, IStateMachineOwner
{
    private enum ESteps
    {
        None,
        Update,
        Done,
    }

    private readonly StateMachine stateMachine;
    private readonly string packageName;
    private ESteps steps = ESteps.Done;

    public PatchOperation(string packageName, EPlayMode playMode)
    {
        this.packageName = packageName;
        EventSystem.RegisterEvent<UserTryInitialize>(OnUserTryInitialize);
        EventSystem.RegisterEvent<UserBeginDownloadWebFiles>(OnUserBeginDownloadWebFiles);
        EventSystem.RegisterEvent<UserTryUpdatePackageVersion>(OnUserTryUpdatePackageVersion);
        EventSystem.RegisterEvent<UserTryUpdatePatchManifest>(OnUserTryUpdatePatchManifest);
        EventSystem.RegisterEvent<UserTryDownloadWebFiles>(OnUserTryDownloadWebFiles);

        stateMachine = new StateMachine();
        stateMachine.Init(this, true);

        //添加共享数据
        stateMachine.AddShareData("PackageName", packageName);
        stateMachine.AddShareData("PlayMode", playMode);
    }


    protected override void OnStart()
    {
        steps = ESteps.Update;
        stateMachine.ChangeState<InitializePackage>();
    }

    protected override void OnUpdate()
    {
        if (steps == ESteps.None || steps == ESteps.Done)
            return;
        if (steps == ESteps.Update)
            stateMachine.Update();
    }

    protected override void OnAbort()
    {
    }

    public void SetFinish()
    {
        steps = ESteps.Done;
        Status = EOperationStatus.Succeed;
        EventSystem.RemoveEvent<UserTryInitialize>();
        EventSystem.RemoveEvent<UserBeginDownloadWebFiles>();
        EventSystem.RemoveEvent<UserTryUpdatePackageVersion>();
        EventSystem.RemoveEvent<UserTryUpdatePatchManifest>();
        EventSystem.RemoveEvent<UserTryDownloadWebFiles>();
        LogSystem.Log($"Package {packageName} patch done !");
    }

    #region 事件相关

    /// <summary>
    /// 用户尝试再次初始化资源包
    /// </summary>
    private void OnUserTryInitialize(UserTryInitialize arg)
    {
        stateMachine.ChangeState<InitializePackage>();
    }

    /// <summary>
    /// 用户开始下载网络文件
    /// </summary>
    private void OnUserBeginDownloadWebFiles(UserBeginDownloadWebFiles obj)
    {
        stateMachine.ChangeState<DownloadPackageFiles>();
    }

    /// <summary>
    /// 用户尝试再次更新静态版本
    /// </summary>
    private void OnUserTryUpdatePackageVersion(UserTryUpdatePackageVersion obj)
    {
        stateMachine.ChangeState<UpdatePackageVersion>();
    }

    /// <summary>
    /// 用户尝试再次更新补丁清单
    /// </summary>
    private void OnUserTryUpdatePatchManifest(UserTryUpdatePatchManifest obj)
    {
        stateMachine.ChangeState<UpdatePackageManifest>();
    }

    /// <summary>
    /// 用户尝试再次下载网络文件
    /// </summary>
    private void OnUserTryDownloadWebFiles(UserTryDownloadWebFiles obj)
    {
        stateMachine.ChangeState<CreatePackageDownloader>();
    }

    #endregion
}