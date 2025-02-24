using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using cfg;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas), typeof(GraphicRaycaster))]
public abstract class UIBehavior : MonoBehaviour
{
    private static Transform mRoot;
    private static RectTransform mRootRect;
    private static RectTransform uiRootRect;
    private static Camera uiCamera;

    public static Camera UICamera
    {
        get
        {
            if (uiCamera == null)
            {
                uiCamera = Root.Find("UICamera").GetComponent<Camera>();
            }
            return uiCamera;
        }
    }
    public static Transform Root
    {
        get
        {
            if (mRoot == null)
            {
                GameObject goRoot = GameObject.Find("UIRoot");
                mRoot = (goRoot == null ? new GameObject("UIRoot") : goRoot).transform;
                DontDestroyOnLoad(mRoot.gameObject);
                mRootRect = mRoot.GetComponent<RectTransform>();
            }
            return mRoot;
        }
    }
    public static RectTransform RootRect => mRootRect;
    public static RectTransform UIRoot
    {
        get
        {
            if (uiRootRect == null)
            {
                var tr = Root.Find("UICanvas");
                if (tr != null)
                {
                    uiRootRect = tr.GetComponent<RectTransform>();
                }
                else
                {
                    var uiRootGo = new GameObject("UICanvas", typeof(RectTransform));
                    uiRootGo.transform.SetParent(Root.transform);
                    uiRootRect = uiRootGo.GetComponent<RectTransform>();
                    uiRootRect.localPosition = Vector3.zero;
                    uiRootRect.localScale = Vector3.one;
                    uiRootRect.anchorMin = Vector2.zero;
                    uiRootRect.anchorMax = Vector2.one;
                    uiRootRect.sizeDelta = Vector2.zero;
                    uiRootRect.anchoredPosition3D = Vector3.zero;
                }
            }
            return uiRootRect;
        }
    }

    private UIWnd wndInfo;
    private Canvas canvas;
    private int baseOrder;
    public int Layer => wndInfo.Layer;

    protected async Task Show(params object[] args)
    {
        try
        {
            gameObject.SetActive(true);
            await OnPreShowAsync(args);
            OnShow(args);
            await OnShowAsync(args);
            await OnPostShowAsync(args);
        }
        catch (Exception e)
        {
            Debug.LogError($"UI Show Error: {e}");
        }
    }

    public void Hide()
    {
        try
        {
            LogSystem.Log("Hide:" + GetType());
            gameObject.SetActive(false);
            if (wndInfo.DestroyOnHide)
            {
                DestroyImp();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"UI Hide Error: {e}");
        }
    }

    private void DestroyImp()
    {
        string uiName = wndInfo.Name;
        if (loadedUIPrefabDic.ContainsKey(uiName))
        {
            Destroy(loadedUIPrefabDic[uiName]);
            loadedUIPrefabDic.Remove(uiName);
            LogSystem.Log("-----Destroy Prefab:" + uiName);
        }
        else
        {
            LogSystem.Log("=====DestroyUI Can't find uibehaviourName:" + uiName);
        }
        uiBehaviorDic.Remove(uiName);
        Destroy(gameObject);
    }

    protected virtual void OnShow(params object[] args)
    {
    }

    protected virtual Task OnShowAsync(params object[] args) => Task.CompletedTask;
    protected virtual Task OnPreShowAsync(params object[] args) => Task.CompletedTask;
    protected virtual Task OnPostShowAsync(params object[] args) => Task.CompletedTask;

    // 子类实现：UI 隐藏时的逻辑
    protected virtual void OnHide()
    {
    }

    #region UI资源管理

    public static Action<UIWnd> OnBeforeLoadPrefab;
    public static Action<GameObject> OnLoadPrefab;

    private static Dictionary<string, UIBehavior> uiBehaviorDic = new Dictionary<string, UIBehavior>();
    private static Dictionary<string, GameObject> loadedUIPrefabDic = new Dictionary<string, GameObject>();

    public static void ShowUIByName(string name, params object[] args)
    {
        TaskUtil.Run(ShowUIByNameImp(name, args));
    }

    public static void ShowUI<T>() where T : UIBehavior
    {
        TaskUtil.Run(ShowUIByNameImp(typeof(T).Name));
    }

    public static void ShowUI<T>(params object[] args) where T : UIBehavior
    {
        TaskUtil.Run(ShowUIByNameImp(typeof(T).Name, args));
    }

    public static Task ShowUIByNameAsync(string name, params object[] args)
    {
        return ShowUIByNameImp(name, args);
    }

    public static Task ShowUIAsync<T>() where T : UIBehavior
    {
        return ShowUIByNameImp(typeof(T).Name);
    }

    public static Task ShowUIAsync<T>(params object[] args) where T : UIBehavior
    {
        return ShowUIByNameImp(typeof(T).Name, args);
    }

    protected static Task ShowUIByNameImp(string name, params object[] args)
    {
        Debug.Log($"ShowUIByNameImp:{name}");
        var tcs = new TaskCompletionSource<bool>();

        void ShowUIBehavior(UIBehavior uiBase)
        {
            TaskUtil.Run(async () =>
            {
                await uiBase.Show(args);
                tcs.SetResult(true);
            });
        }

        UIBehavior uiBehavior = null;
        if (uiBehaviorDic.TryGetValue(name, out uiBehavior))
        {
            ShowUIBehavior(uiBehavior);
        }
        else
        {
            LoadUI(name, ShowUIBehavior);
        }
        return tcs.Task;
    }

    public static void HideUI(string uiName, bool force = false)
    {
        UIBehavior uiBehavior = null;
        if (uiBehaviorDic.TryGetValue(uiName, out uiBehavior))
        {
            uiBehavior.Hide();
        }
        else
        {
            LogSystem.Warning($"HideUI {uiName} not found!");
        }
    }

    public static void HideUI<T>(bool force = false) where T : UIBehavior
    {
        HideUI(typeof(T).Name, force);
    }

    private static void LoadUI(string name, Action<UIBehavior> ret)
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

    public static void GetUIPrefabByUIBehaviourName(string uiName, Action<GameObject, UIWnd> callback)
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

    public static UIBehavior GetUIBehaviour(string name, GameObject uiGo = null)
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
            if (!uiBehaviorDic.ContainsKey(name))
            {
                uiBehaviorDic.Add(name, uiBehavior);
            }
        }

        return uiBehavior;
    }

    public static T GetUIBehaviour<T>(GameObject uiGO = null) where T : UIBehavior
    {
        return (T)GetUIBehaviour(typeof(T).Name, uiGO);
    }

    #endregion
}