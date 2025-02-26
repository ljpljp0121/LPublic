using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using cfg;
using UnityEngine;

public class UISystem : SingletonMono<UISystem>, IUIStorage
{
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
                windowRoot = uiRoot.Find("WindowRoot").GetComponent<RectTransform>();
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

    public void RegisterUIDic(string key, UIBehavior uiBehavior)
    {
        uiBehaviorDic[key] = uiBehavior;
    }

    public bool TryGetFromUIDic(string key, out UIBehavior uiBehavior)
    {
        return uiBehaviorDic.TryGetValue(key, out uiBehavior);
    }

    public void UnRegisterUIDic(string key)
    {
        bool removed = uiBehaviorDic.Remove(key);
        if (!removed)
        {
            LogSystem.Warning($"Key {key} not found in UIBehaviorDic");
        }
    }

    public void RegisterPrefabDic(string key, GameObject prefab)
    {
        loadedUIPrefabDic[key] = prefab;
    }

    public bool TryGetFromPrefabDic(string key, out GameObject prefab)
    {
        return loadedUIPrefabDic.TryGetValue(key, out prefab);
    }

    public void UnRegisterPrefabDic(string key)
    {
        bool removed = loadedUIPrefabDic.Remove(key);
        if (!removed)
        {
            LogSystem.Warning($"Key {key} not found in LoadedUIPrefabDic");
        }
    }

    public void PushUIStack(UIBehavior uiBehavior)
    {
        uiBehaviorStack.Push(uiBehavior);
    }

    public UIBehavior PopUIStack()
    {
        if (uiBehaviorStack.Count != 0)
            return uiBehaviorStack.Pop();
        LogSystem.Error("UIBehaviorStack Is Empty!!!");
        return null;
    }

    public UIBehavior PeekUIStack()
    {
        if (uiBehaviorStack.Count != 0)
            return uiBehaviorStack.Peek();
        LogSystem.Error("UIBehaviorStack Is Empty!!!");
        return null;
    }

    #endregion

    #region UI开启关闭

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

    #region 私有方法

    protected Task ShowUIByNameImp(string uiName, params object[] args)
    {
        Debug.Log($"ShowUIByNameImp:{uiName}");
        var tcs = new TaskCompletionSource<bool>();

        void ShowUIBehavior(UIBehavior uiBase)
        {
            TaskUtil.Run(async () =>
            {
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

        if (loadedUIPrefabDic.ContainsKey(uiName))
            return (loadedUIPrefabDic[uiName], wndInfo);

        var uiPrefab = AssetSystem.LoadAsset<GameObject>($"Prefab/UI/{prefabDir}");
        if (uiPrefab == null)
        {
            LogSystem.Error("! ! ! Can't find UI Prefab ! ! ! prefabDir：" + prefabDir);
            return (null, null);
        }
        GameObject go = GameObject.Instantiate(uiPrefab, WindowRoot.transform) as GameObject;
        go.transform.Reset();
        loadedUIPrefabDic.Add(uiName, go);
        LogSystem.Log("-----Add Prefab:" + uiName);
        return (go, wndInfo);
    }

    private UIBehavior GetUIBehaviour(string uiName, GameObject uiGo = null)
    {
        UIBehavior uiBehavior = null;
        if (uiBehaviorDic.TryGetValue(uiName, out uiBehavior))
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


}