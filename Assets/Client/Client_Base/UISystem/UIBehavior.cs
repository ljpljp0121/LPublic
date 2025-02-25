using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using cfg;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas), typeof(GraphicRaycaster))]
public abstract class UIBehavior : MonoBehaviour
{
    #region 静态管理系统

    private static Transform mRoot;
    private static RectTransform mRootRect;
    private static RectTransform uiRootRect;
    private static Camera uiCamera;
    private static Dictionary<string, UIBehavior> uiBehaviorDic = new Dictionary<string, UIBehavior>();
    private static Dictionary<string, GameObject> loadedUIPrefabDic = new Dictionary<string, GameObject>();
    private static Stack<UIBehavior> uiStack = new Stack<UIBehavior>();
    private static int globalSortingOrder = 1000;

    public static Camera UICamera => uiCamera != null ? uiCamera : (uiCamera = Root.Find("UICamera").GetComponent<Camera>());
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
    public static RectTransform RootRect => mRootRect != null ? mRootRect : (mRootRect = Root.GetComponent<RectTransform>());
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

    #endregion

    #region 实例属性

    private UIWnd wndInfo;
    private Canvas canvas;
    private GraphicRaycaster rayCaster;
    private float lastAccessTime;

    public Canvas Canvas => canvas != null ? canvas : canvas = GetComponent<Canvas>();
    public GraphicRaycaster RayCaster => rayCaster != null ? rayCaster : (rayCaster = GetComponent<GraphicRaycaster>());
    public int SortingOrder => Canvas.sortingOrder;
    public bool IsVisible { get; private set; }
    public bool IsActive => gameObject.activeInHierarchy;
    public bool IsShowing => IsShow(wndInfo.Name);

    #endregion

    #region 生命周期管理

    protected virtual void Awake()
    {
        Canvas.overrideSorting = true;
        Canvas.sortingOrder = wndInfo.Layer * 2000;
        rayCaster.enabled = false;
        AutoBindComponents();
    }

    protected virtual void OnDestroy()
    {
        if (uiBehaviorDic.ContainsKey(name))
            uiBehaviorDic.Remove(name);
    }

    #endregion

    #region 事件管理

    public static Func<GameObject, bool, Task> SetVisibleMethod;
    public static Action<UIWnd> OnBeforeLoadPrefab;
    public static Action<GameObject> OnLoadPrefab;

    #endregion

    #region 基础方法

    public Task SetVisible(bool value)
    {
        IsVisible = value;
        if (SetVisibleMethod != null && this.gameObject != null)
        {
            return SetVisibleMethod(this.gameObject, value);
        }
        return Task.CompletedTask;
    }

    #endregion

    #region UI调用管理

    protected virtual void OnShow(params object[] args) { }
    protected virtual Task OnShowAsync(params object[] args) => Task.CompletedTask;
    protected virtual Task OnPreShowAsync(params object[] args) => Task.CompletedTask;
    protected virtual Task OnPostShowAsync(params object[] args) => Task.CompletedTask;

    public virtual Animator GetAnim()
    {
        return this.transform.GetComponent<Animator>();
    }

    protected virtual void OnHide() { }

    #endregion

    #region 显示/隐藏实现

    protected async Task ShowImp(params object[] args)
    {
        try
        {
            transform.localPosition = new Vector3(-50000, -50000, 0);
            if (IsShowing)
            {
                try
                {
                    OnHide();
                }
                catch (Exception e)
                {
                    LogSystem.Error(e);
                }
                Debug.Log("UIBehavior ShowImp 1:" + transform.name);

                await SetVisible(false);
                gameObject.SetActive(false);
            }

            try
            {
                await OnPreShowAsync(args);
                OnShow(args);
                await OnShowAsync(args);
                await OnPostShowAsync(args);
            }
            catch (Exception e)
            {
                LogSystem.Error("show wnd failed 1!{0}", e);
            }

            if (!IsVisible)
            {
                await SetVisible(true);
            }

            transform.localPosition = Vector3.zero;
        }
        catch (Exception e)
        {
            Debug.LogError($"UI Show Error: {e}");
        }
    }

    public void HideImp()
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

    public static void HideUI(string uiName, bool force = false)
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

    public static void HideUI<T>(bool force = false) where T : UIBehavior
    {
        HideUI(typeof(T).Name, force);
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
            uiBehaviorDic.TryAdd(name, uiBehavior);
        }

        return uiBehavior;
    }

    public static T GetUIBehaviour<T>(GameObject uiGO = null) where T : UIBehavior
    {
        return (T)GetUIBehaviour(typeof(T).Name, uiGO);
    }

    public static bool IsShow(string name)
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

    private static void GetUIPrefabByUIBehaviourName(string uiName, Action<GameObject, UIWnd> callback)
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

    #region 自动绑定

    private void AutoBindComponents()
    {
        var fields = GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var field in fields)
        {
            var bindAttr = field.GetCustomAttribute<AutoBindAttribute>();
            if (bindAttr != null)
            {
                var component = FindComponent(field.FieldType, bindAttr.Path);
                if (component != null)
                {
                    field.SetValue(this, component);
                }
            }
        }
    }

    private Component FindComponent(Type type, string path)
    {
        var targetTransform = string.IsNullOrEmpty(path) ?
            transform.Find(type.Name) :
            transform.Find(path);

        return targetTransform?.GetComponent(type);
    }

    #endregion

    #region 层级管理    

    private void UpdateSortingOrder()
    {
        Canvas.sortingOrder = wndInfo.Layer * 2000 + GetLayerCount() * 100;
    }

    private int GetLayerCount()
    {
        int count = 0;
        foreach (var ui in uiBehaviorDic.Values)
        {
            if (ui.wndInfo.Layer == wndInfo.Layer &&
                ui.IsShowing &&
                ui != this)
            {
                count++;
            }
        }
        return count;
    }

    #endregion

    #region 额外方法

    public void SetActive(GameObject obj, bool bol)
    {
        if (obj != null && obj.activeSelf != bol)
            obj.SetActive(bol);
    }

    public T Fd<T>(string path, Transform tr = null) where T : Component
    {
        if (tr == null)
            tr = transform;
        var tran = tr.Find(path);
        if (tran == null)
            return null;
        return tran.GetComponent<T>();
    }

    #endregion
}