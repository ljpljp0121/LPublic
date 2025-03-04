using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using cfg.UI;
using UnityEngine;

public class UISystem : SingletonMono<UISystem>, IUIStorage
{
    private const int LAYER_BASE_INTERVAL = 2000;
    private const int LAYER_ORDER_STEP = 100;

    #region 组件对象

    private Transform uiRoot;
    private RectTransform windowRoot;
    private Camera uiCamera;

    public Transform UIRoot
    {
        get
        {
            if (uiRoot == null)
            {
                uiRoot = this.transform;
            }
            return uiRoot;
        }
    }
    public Camera UICamera => uiCamera != null ? uiCamera : (uiCamera = uiRoot.Find("UICamera").GetComponent<Camera>());

    public RectTransform WindowRoot
    {
        get
        {
            if (windowRoot == null)
            {
                windowRoot = UIRoot.Find("WindowRoot").GetComponent<RectTransform>();
                windowRoot.localPosition = Vector3.zero;
                windowRoot.localScale = Vector3.one;
                windowRoot.anchorMin = Vector2.zero;
                windowRoot.anchorMax = Vector2.one;
                windowRoot.sizeDelta = Vector2.zero;
                windowRoot.anchoredPosition3D = Vector3.zero;
            }
            return windowRoot;
        }
    }

    #endregion

    ////UI实例缓存字典,存储已经实例化的UI
    private readonly Dictionary<string, UIBehavior> uiBehaviorDic = new Dictionary<string, UIBehavior>();
    //预制体缓存字典,存储已经加载的UI预制体
    private readonly Dictionary<string, GameObject> loadedUIPrefabDic = new Dictionary<string, GameObject>();
    //UI实例栈,存储已经打开的UI
    private readonly Stack<UIBehavior> uiBehaviorStack = new Stack<UIBehavior>();
    //窗口排序值记录字典
    private readonly Dictionary<int, SortedSet<int>> layerOrders = new Dictionary<int, SortedSet<int>>();

    #region 生命周期管理

    protected override void Awake()
    {
        base.Awake();
        uiRoot = GetComponent<Transform>();
        uiCamera = uiRoot.Find("UICamera").GetComponent<Camera>();
        windowRoot = uiRoot.Find("WindowRoot").GetComponent<RectTransform>();
    }

    #endregion

    #region 数据修改接口

    public void ChangeOrAddUIDic(string uiName, UIBehavior uiBehavior)
    {
        uiBehaviorDic[uiName] = uiBehavior;
    }

    public bool TryGetFromUIDic(string uiName, out UIBehavior uiBehavior)
    {
        return uiBehaviorDic.TryGetValue(uiName, out uiBehavior);
    }

    public void TryRemoveUIDic(string uiName)
    {
        bool removed = uiBehaviorDic.Remove(uiName);
        if (!removed)
        {
            LogSystem.Warning($"Key {uiName} not found in UIBehaviorDic");
        }
    }

    public void ChangeOrAddPrefabDic(string uiName, GameObject prefab)
    {
        loadedUIPrefabDic[uiName] = prefab;
    }

    public bool TryGetFromPrefabDic(string uiName, out GameObject prefab)
    {
        return loadedUIPrefabDic.TryGetValue(uiName, out prefab);
    }

    public void TryRemovePrefabDic(string uiName)
    {
        bool removed = loadedUIPrefabDic.Remove(uiName);
        if (!removed)
        {
            LogSystem.Warning($"Key {uiName} not found in LoadedUIPrefabDic");
        }
    }

    public void PushUIStack(UIBehavior uiBehavior)
    {
        uiBehaviorStack.Push(uiBehavior);
    }

    public UIBehavior TryPopUIStack()
    {
        if (uiBehaviorStack.Count != 0)
            return uiBehaviorStack.Pop();
        LogSystem.Error("UIBehaviorStack Is Empty!!!");
        return null;
    }

    public UIBehavior TryPeekUIStack()
    {
        if (uiBehaviorStack.Count != 0)
            return uiBehaviorStack.Peek();
        LogSystem.Error("UIBehaviorStack Is Empty!!!");
        return null;
    }

    #endregion

    #region UI调用对外接口

    public void ShowUIByName(string uiName, params object[] args)
    {
        ShowUIByNameImp(uiName, args).Run();
    }

    public void ShowUI<T>() where T : UIBehavior
    {
        ShowUIByNameImp(typeof(T).Name).Run();
    }

    public void ShowUI<T>(params object[] args) where T : UIBehavior
    {
        ShowUIByNameImp(typeof(T).Name, args).Run();
    }

    public Task ShowUIByNameAsync(string uiName, params object[] args)
    {
        return ShowUIByNameImp(uiName, args);
    }

    public Task ShowUIAsync<T>() where T : UIBehavior
    {
        return ShowUIByNameImp(typeof(T).Name);
    }

    public Task ShowUIAsync<T>(params object[] args) where T : UIBehavior
    {
        return ShowUIByNameImp(typeof(T).Name, args);
    }

    public void HideUIByName(string uiName)
    {
        HideUIByNameImp(uiName);
    }

    public void HideUI<T>() where T : UIBehavior
    {
        HideUIByNameImp(typeof(T).Name);
    }

    #endregion

    #region UI调用内部接口

    protected Task ShowUIByNameImp(string uiName, params object[] args)
    {
        Debug.Log($"ShowUIByNameImp:{uiName}");
        var tcs = new TaskCompletionSource<bool>();

        void ShowUIBehavior(UIBehavior uiBase)
        {
            TaskUtil.Run(async () =>
            {
                UpdateSortingLayer(uiBase, true);
                await uiBase.ShowImp(args);
                tcs.SetResult(true);
            });
        }

        if (uiBehaviorDic.TryGetValue(uiName, out var uiBehavior))
        {
            ShowUIBehavior(uiBehavior);
        }
        else
        {
            LoadUI(uiName, ShowUIBehavior);
        }
        return tcs.Task;
    }

    protected void HideUIByNameImp(string uiName)
    {
        if (uiBehaviorDic.TryGetValue(uiName, out var uiBehavior))
        {
            UpdateSortingLayer(uiBehavior, false);
            uiBehavior.HideImp();
        }
        else
        {
            LogSystem.Warning($"HideUI {uiName} not found!");
        }
    }

    private void LoadUI(string uiName, Action<UIBehavior> ret)
    {
        try
        {
            var (prefab, wndInfo) = GetUIPrefabAndConfig(uiName);

            if (prefab == null)
            {
                LogSystem.Error("! ! ! Can't find in UI prefab. " + uiName);
                return;
            }
            UIBehavior uiBehavior = GetUIBehaviour(uiName, prefab);
            if (uiBehavior == null)
            {
                LogSystem.Error("! ! ! Can't UIBehavior find in UI prefab. " + uiName);
                return;
            }
            if (wndInfo == null)
            {
                wndInfo = TableSystem.GetVOData<TbUIWnd>().Get(uiName);
                if (wndInfo == null)
                {
                    LogSystem.Error("! ! ! Can't find in UI config. " + uiName);
                    return;
                }
            }
            uiBehavior.WndInfo = wndInfo;

            ret?.Invoke(uiBehavior);
        }
        catch (Exception e)
        {
            LogSystem.Error(e.Message);
        }
    }

    private (GameObject prefab, UIWnd wndInfo) GetUIPrefabAndConfig(string uiName)
    {
        if (loadedUIPrefabDic.TryGetValue(uiName, out var cachePrefab))
        {
            return (cachePrefab, null);
        }

        UIWnd wndInfo = TableSystem.GetVOData<TbUIWnd>().Get(uiName);
        if (wndInfo == null)
        {
            LogSystem.Error($"! ! ! Can't find in UI config. {uiName}");
            return (null, null);
        }
        string prefabDir = wndInfo.Path;
        if (string.IsNullOrEmpty(prefabDir))
        {
            LogSystem.Error("  error : GetUIPrefabByUIBehaviourName  cant find uibehaviour:" + uiName + " prefabDir:" +
                            prefabDir);
            return (null, null);
        }

        if (loadedUIPrefabDic.TryGetValue(uiName, out var value))
            return (value, wndInfo);

        var uiPrefab = AssetSystem.LoadAsset<GameObject>($"Prefab/UI/{prefabDir}");
        if (uiPrefab == null)
        {
            LogSystem.Error("! ! ! Can't find UI Prefab ! ! ! prefabDir：" + prefabDir);
            return (null, null);
        }
        GameObject go = GameObject.Instantiate(uiPrefab, WindowRoot.transform);
        go.transform.Reset();
        loadedUIPrefabDic.Add(uiName, go);
        LogSystem.Log("-----Add Prefab:" + uiName);
        return (go, wndInfo);
    }

    private UIBehavior GetUIBehaviour(string uiName, GameObject uiGo = null)
    {
        if (uiBehaviorDic.TryGetValue(uiName, out var uiBehavior))
        {
            return uiBehavior;
        }

        if (uiGo)
        {
            uiBehavior = uiGo.GetComponent<UIBehavior>(true);
            if (uiBehavior == null)
            {
                var monos = uiGo.GetComponentsInChildren<UIBehavior>(true);
                if (monos.Length > 0)
                    uiBehavior = monos[0];
            }
            if (uiBehavior == null)
            {
                LogSystem.Error("error ：GetUIBehaviour cant find " + uiName + " UI");
            }
        }

        if (uiBehavior != null)
        {
            uiBehaviorDic.TryAdd(uiName, uiBehavior);
        }

        return uiBehavior;
    }

    private T GetUIBehaviour<T>(GameObject uiGO = null) where T : UIBehavior
    {
        return (T)GetUIBehaviour(typeof(T).Name, uiGO);
    }

    #endregion

    #region 层级管理

    private void UpdateSortingLayer(UIBehavior uiBehavior, bool isShowing)
    {
        if (uiBehavior.WndInfo == null)
        {
            LogSystem.Error("! ! ! Can't find in UI config. " + uiBehavior.name);
            return;
        }
        int layer = uiBehavior.WndInfo.Layer;
        if (layer < 1 || layer > 5)
        {
            LogSystem.Error($"Invalid UI layer: {layer}");
            layer = Mathf.Clamp(layer, 1, 5);
        }

        if (!layerOrders.TryGetValue(layer, out var orders))
        {
            orders = new SortedSet<int>();
            layerOrders[layer] = orders;
        }

        if (isShowing)
        {
            int baseOrder = layer * LAYER_BASE_INTERVAL;
            int newOrder = orders.Count > 0 ? orders.Max + LAYER_ORDER_STEP : baseOrder;

            orders.Add(newOrder);
            uiBehavior.Canvas.sortingOrder = newOrder;
        }
        else
        {
            orders.Remove(uiBehavior.Canvas.sortingOrder);
        }
    }

    #endregion
}