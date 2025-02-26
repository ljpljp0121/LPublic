using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using cfg;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class UISystem : SingletonMono<UISystem>
{
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

    private Dictionary<string, UIBehavior> uiBehaviorDic = new Dictionary<string, UIBehavior>();
    private Dictionary<string, GameObject> loadedUIPrefabDic = new Dictionary<string, GameObject>();

    public Func<GameObject, bool, Task> SetVisibleMethod;
    public Action<UIWnd> OnBeforeLoadPrefab;
    public Action<GameObject> OnLoadPrefab;

    #region 生命周期管理

    protected override void Awake()
    {
        base.Awake();
        uiRoot = GetComponent<Transform>();
        uiCamera = uiRoot.Find("UICamera").GetComponent<Camera>();
        windowRoot = uiRoot.Find("WindowRoot").GetComponent<RectTransform>();
    }

    #endregion

    #region UI开启关闭

    public void ShowUIByName(string name, params object[] args)
    {
        ShowUIByNameImp(name, args).Run();
    }

    public void ShowUI<T>() where T : UIBehavior
    {
        ShowUIByNameImp(typeof(T).Name).Run();
    }

    public void ShowUI<T>(params object[] args) where T : UIBehavior
    {
        ShowUIByNameImp(typeof(T).Name, args).Run();
    }

    public Task ShowUIByNameAsync(string name, params object[] args)
    {
        return ShowUIByNameImp(name, args);
    }

    public Task ShowUIAsync<T>() where T : UIBehavior
    {
        return ShowUIByNameImp(typeof(T).Name);
    }

    public Task ShowUIAsync<T>(params object[] args) where T : UIBehavior
    {
        return ShowUIByNameImp(typeof(T).Name, args);
    }

    public void HideUI(string uiName, bool force = false)
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

    public void HideUI<T>(bool force = false) where T : UIBehavior
    {
        HideUI(typeof(T).Name, force);
    }

    public UIBehavior GetUIBehaviour(string name, GameObject uiGo = null)
    {
        UIBehavior uiBehavior = null;
        if (uiBehaviorDic.TryGetValue(name, out uiBehavior))
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
                LogSystem.Error("error ：GetUIBehaviour cant find " + name + " UI");
            }
        }

        if (uiBehavior != null)
        {
            uiBehaviorDic.TryAdd(name, uiBehavior);
        }

        return uiBehavior;
    }

    public T GetUIBehaviour<T>(GameObject uiGO = null) where T : UIBehavior
    {
        return (T)GetUIBehaviour(typeof(T).Name, uiGO);
    }

    public bool IsShow(string name)
    {
        if (uiBehaviorDic.TryGetValue(name, out var uiBehaviour))
        {
            return uiBehaviour.IsVisible;
        }
        else
        {
            return false;
        }
    }


    #endregion

    #region 私有方法

    protected Task ShowUIByNameImp(string name, params object[] args)
    {
        Debug.Log($"ShowUIByNameImp:{name}");
        var tcs = new TaskCompletionSource<bool>();

        void ShowUIBehavior(UIBehavior uiBase)
        {
            TaskUtil.Run(async () =>
            {
                await uiBase.ShowImp(args);
                tcs.SetResult(true);
            });
        }

        if (uiBehaviorDic.TryGetValue(name, out var uiBehavior))
        {
            ShowUIBehavior(uiBehavior);
        }
        else
        {
            LoadUI(name, ShowUIBehavior);
        }
        return tcs.Task;
    }

    private void LoadUI(string name, Action<UIBehavior> ret)
    {
        GetUIPrefabByUIBehaviourName(name, (prefab, wndInfo) =>
        {
            if (prefab == null)
            {
                LogSystem.Error("! ! ! Can't find in UI prefab. " + name);
                return;
            }
            UIBehavior uiBehavior = GetUIBehaviour(name, prefab);
            if (uiBehavior == null)
            {
                LogSystem.Error("! ! ! Can't UIBehavior find in UI prefab. " + name);
                return;
            }
            if (wndInfo == null)
            {
                wndInfo = TableSystem.GetVOData<TbUIWnd>().Get(name);
                if (wndInfo == null)
                {
                    LogSystem.Error("! ! ! Can't find in UI config. " + name);
                    return;
                }
            }
            uiBehavior.wndInfo = wndInfo;
            if (ret != null)
                ret(uiBehavior);
        });
    }

    private void GetUIPrefabByUIBehaviourName(string uiName, Action<GameObject, UIWnd> callback)
    {
        GameObject returnValue;
        if (loadedUIPrefabDic.TryGetValue(uiName, out returnValue))
        {
            callback(returnValue, null);
            return;
        }
        UIWnd wndInfo = TableSystem.GetVOData<TbUIWnd>().Get(uiName);
        if (wndInfo == null)
        {
            LogSystem.Log($"! ! ! Can't find in UI config. {uiName}");
            callback(null, null);
            return;
        }
        string prefabDir = wndInfo.Path;
        if (!string.IsNullOrEmpty(prefabDir))
        {
            OnBeforeLoadPrefab?.Invoke(wndInfo);
            var uiPrefab = AssetSystem.LoadAsset<GameObject>($"Prefab/UI/{prefabDir}");
            if (uiPrefab == null)
            {
                LogSystem.Error("! ! ! Can't find UI Prefab ! ! ! prefabDir：" + prefabDir);
                callback(null, null);
                return;
            }
            if (loadedUIPrefabDic.ContainsKey(uiName))
                return;
            OnLoadPrefab?.Invoke(uiPrefab);
            GameObject go = GameObject.Instantiate(uiPrefab, UIRoot.transform) as GameObject;
            go.transform.Reset();
            loadedUIPrefabDic.Add(uiName, go);
            LogSystem.Log("-----Add Prefab:" + uiName);
            callback(go, wndInfo);
            return;
        }
        LogSystem.Error("  error : GetUIPrefabByUIBehaviourName  cant find uibehaviour:" + uiName + " prefabDir:" +
                        prefabDir);
        callback(null, null);
    }


    #endregion
}