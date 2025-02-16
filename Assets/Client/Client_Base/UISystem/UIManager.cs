using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using cfg;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class UIManager : SingletonMono<UIManager>
{
    private Transform uiRoot;
    private RectTransform uiCanvas;
    private Camera mainCamera;
    private Camera uiCamera;

    private readonly Dictionary<string, UIBase> loadedUIs = new();

    public Camera UICamera => uiCamera;

    protected override void Awake()
    {
        base.Awake();
        this.transform.SetParent(CoreEngineRoot.RootTransform);
        FindMainCamera();
        InitUIRoot();
        InitUICamera();
        InitUICanvas();
        DealInGameDebug();
        DealStartLoading();
    }

    #region 初始化

    private void FindMainCamera()
    {
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("UI"));
        }
    }

    private void InitUIRoot()
    {
        uiRoot = new GameObject("UIRoot").transform;
        uiRoot.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        DontDestroyOnLoad(uiRoot);
    }

    private void InitUICamera()
    {
        var cameraGo = new GameObject("UICamera");
        cameraGo.transform.SetParent(uiRoot);
        uiCamera = cameraGo.AddComponent<Camera>();
        uiCamera.orthographic = true;
        uiCamera.orthographicSize = 5;
        uiCamera.nearClipPlane = 0.0f;
        uiCamera.farClipPlane = 2000f;
        uiCamera.cullingMask = 1 << LayerMask.NameToLayer("UI");
        uiCamera.clearFlags = CameraClearFlags.Depth;
        uiCamera.depth = mainCamera != null ? mainCamera.depth + 1 : 1;
        uiCamera.GetUniversalAdditionalCameraData().renderType = CameraRenderType.Overlay;
        mainCamera.GetUniversalAdditionalCameraData().cameraStack.Add(uiCamera);
    }

    private void InitUICanvas()
    {
        uiCanvas = new GameObject("UICanvas").AddComponent<RectTransform>();
        uiCanvas.SetParent(uiRoot);
        uiCanvas.gameObject.layer = LayerMask.NameToLayer("UI");

        var rootCanvas = uiCanvas.gameObject.AddComponent<Canvas>();
        rootCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        rootCanvas.worldCamera = uiCamera;
        rootCanvas.sortingLayerID = SortingLayer.NameToID("UI");

        var canvasScaler = uiCanvas.gameObject.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920, 1080);
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        canvasScaler.matchWidthOrHeight = 0.5f;
    }

    private void DealInGameDebug()
    {
        IngameDebugConsole.DebugLogManager.Instance.transform.SetParent(uiRoot);

        var canvas = IngameDebugConsole.DebugLogManager.Instance.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = uiCamera;
        canvas.overrideSorting = true;
        canvas.sortingLayerID = SortingLayer.NameToID("UI");
        canvas.sortingOrder = 30002;

        var canvasScaler = IngameDebugConsole.DebugLogManager.Instance.GetComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920, 1080);
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        canvasScaler.matchWidthOrHeight = 1f;
    }

    private void DealStartLoading()
    {
        StartLoading.Instance.transform.SetParent(uiRoot);
        StartLoading.Instance.transform.GetChild(1).GetComponent<Canvas>().overrideSorting = false;
        Destroy(StartLoading.Instance.transform.GetChild(0).gameObject);
        var canvas = StartLoading.Instance.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = uiCamera;
        canvas.overrideSorting = true;
        canvas.sortingLayerID = SortingLayer.NameToID("UI");
        canvas.sortingOrder = 30001;
    }

    #endregion


    public void ShowUIByName(string name, params object[] args)
    {
        TaskUtil.Run(ShowUIByNameImp(name, args));
    }

    public void ShowUI<T>() where T : UIBase
    {
        TaskUtil.Run(ShowUIByNameImp(typeof(T).Name));
    }

    public void ShowUI<T>(params object[] args) where T : UIBase
    {
        TaskUtil.Run(ShowUIByNameImp(typeof(T).Name, args));
    }

    public Task ShowUIByNameAsync(string name, params object[] args)
    {
        return ShowUIByNameImp(name, args);
    }

    public Task ShowUIAsync<T>() where T : UIBase
    {
        return ShowUIByNameImp(typeof(T).Name);
    }

    public Task ShowUIAsync<T>(params object[] args) where T : UIBase
    {
        return ShowUIByNameImp(typeof(T).Name, args);
    }

    private Task ShowUIByNameImp(string name, params object[] args)
    {
        Debug.Log($"ShowUIByNameImp:{name}");
        var tcs = new TaskCompletionSource<bool>();

        void ShowUIBase(UIBase uiBase)
        {
            TaskUtil.Run(async () =>
            {
                await uiBase.Show(args);
                tcs.SetResult(true);
            });
        }

        UIBase uiBase = null;
        if (loadedUIs.TryGetValue(name, out uiBase))
        {
            ShowUIBase(uiBase);
        }
        else
        {
            LoadUI(name, (uibase) => { ShowUIBase(uibase); });
        }
        return tcs.Task;
    }

    public void HideUI(string uiName)
    {
        if (loadedUIs.TryGetValue(uiName, out var ui))
        {
            ui.Hide();

            // 如果配置为隐藏后销毁，则从字典中移除
            if (ui.WndInfo.DestroyOnHide)
            {
                loadedUIs.Remove(uiName);
            }
        }
    }

    public void DestroyUI(string uiName)
    {
        if (loadedUIs.TryGetValue(uiName, out var ui))
        {
            ui.Destroy();
            loadedUIs.Remove(uiName);
        }
    }

    private void LoadUI(string name, Action<UIBase> ret)
    {
        var wndInfo = TableSystem.Table.TbUIWnd.Get(name);
        if (wndInfo == null)
        {
            Debug.LogError($"UI 配置未找到: {name}");
            return;
        }
        // 动态加载 UI 预制体
        var uiPrefab = AssetSystem.LoadAsset<GameObject>($"Prefab/UI/{wndInfo.Path}");
        if (uiPrefab == null)
        {
            Debug.LogError($"UI 预制体加载失败: {wndInfo.Path}");
            return;
        }

        // 实例化 UI
        UIBase uiInstance = Instantiate(uiPrefab, uiCanvas).GetComponent<UIBase>();
        if (uiInstance == null)
        {
            Debug.LogError($"UI 组件未找到: {wndInfo.Name}");
            return;
        }

        uiInstance.Init();
        loadedUIs.Add(wndInfo.Name, uiInstance);
        if (ret != null)
        {
            ret(uiInstance);
        }
    }
}