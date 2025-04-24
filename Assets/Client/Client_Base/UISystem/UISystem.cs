using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using cfg.UI;
using DG.Tweening;
using UITool;
using UnityEngine;

public interface IVisibleHandler
{
    /// <summary>
    /// UI显示在界面上的时候触发
    /// </summary>
    /// <param name="isOpen">如果为True,表示主动Show的时候,如果为False,表示其他UI关闭使它显示在最上层</param>
    void OnVisible(bool isOpen);
    /// <summary>
    /// UI关闭的时候触发
    /// </summary>
    /// <param name="isClose">如果为True,表示主动Hide的时候,如果为False,表示其他UI打开使它不显示在最上层</param>
    void OnDisVisible(bool isClose);
}

public class UISystem : SingletonMono<UISystem>, IUIStorage
{
    private const int LAYER_BASE_INTERVAL = 2000;
    private const int LAYER_ORDER_STEP = 100;

    #region 组件对象

    private Transform uiRoot;
    private RectTransform windowRoot;
    private Camera uiCamera;
    private CanvasGroup black;

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

    public CanvasGroup Black
    {
        get
        {
            if (black == null)
            {
                var blackGo = (GameObject)GameObject.Instantiate(Resources.Load("Black"), WindowRoot);
                black = blackGo.GetComponent<CanvasGroup>();
                black.GetComponent<RectTransform>().SetFullRect();
            }
            return black;
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
    //各个层级最上层的UI
    private readonly Dictionary<UIWindowLayer, UIBehavior> topWindows = new Dictionary<UIWindowLayer, UIBehavior>();

    #region 生命周期管理

    protected override void Awake()
    {
        base.Awake();
        uiRoot = GetComponent<Transform>();
        uiCamera = uiRoot.Find("UICamera").GetComponent<Camera>();
        windowRoot = uiRoot.Find("WindowRoot").GetComponent<RectTransform>();
        Init();
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

    public static void ShowUIByName(string uiName, params object[] args)
    {
        Instance.ShowUIByNameImp(uiName, args).Run();
    }

    public static void ShowUI<T>() where T : UIBehavior
    {
        Instance.ShowUIByNameImp(typeof(T).Name).Run();
    }

    public static void ShowUI<T>(params object[] args) where T : UIBehavior
    {
        Instance.ShowUIByNameImp(typeof(T).Name, args).Run();
    }

    public static Task ShowUIByNameAsync(string uiName, params object[] args)
    {
        return Instance.ShowUIByNameImp(uiName, args);
    }

    public static Task ShowUIAsync<T>() where T : UIBehavior
    {
        return Instance.ShowUIByNameImp(typeof(T).Name);
    }

    public static Task ShowUIAsync<T>(params object[] args) where T : UIBehavior
    {
        return Instance.ShowUIByNameImp(typeof(T).Name, args);
    }

    public static void HideUIByName(string uiName)
    {
        Instance.HideUIByNameImp(uiName);
    }

    public static void HideUI<T>() where T : UIBehavior
    {
        Instance.HideUIByNameImp(typeof(T).Name);
    }

    #endregion

    #region UI调用内部接口

    protected Task ShowUIByNameImp(string uiName, params object[] args)
    {
        Debug.Log($"ShowUIByNameImp:{uiName}");
        var tcs = new TaskCompletionSource<bool>();

        //UI显示流程
        void ShowUIBehavior(UIBehavior uiBase)
        {
            TaskUtil.Run(async () =>
            {
                await uiBase.ShowImp(args); //UIBehavior自定义流程
                OnUIShown?.Invoke(uiBase, args);
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

    #region 拓展事件

    /// <summary>
    /// UIShow之后
    /// </summary>
    public static event Action<UIBehavior, object[]> OnUIShown;
    /// <summary>
    /// UIHide事件
    /// </summary>
    public static event Action<UIBehavior> OnUIHide;
    /// <summary>
    /// 设置可见性
    /// </summary>
    public static Func<GameObject, bool, Task> SetVisibleFunc;

    #endregion

    #region 拓展

    private void Init()
    {
        OnUIShown += PostShowUI;
        OnUIHide += PreUIHide;
        SetVisibleFunc += SetVisibleImp;
    }

    private void PostShowUI(UIBehavior uiBehavior, object[] args)
    {
        Debug.Log($"PostShowUI: {uiBehavior.name}");
        SetTopWindowOnShow(uiBehavior);
        UpdateSortingLayer(uiBehavior, true);
    }

    private void PreUIHide(UIBehavior uiBehavior)
    {
        Debug.Log($"UISystem HideUI : {uiBehavior.name}");
        DealTopWindowOnHide(uiBehavior);
    }

    private void SetTopWindowOnShow(UIBehavior uiBehavior)
    {
        SetTopWindow(uiBehavior);
        if (uiBehavior is IVisibleHandler)
        {
            (uiBehavior as IVisibleHandler).OnVisible(true);
        }
        if (uiBehavior.WndInfo.Layer == 1)
        {
            List<UIBehavior> all = GetAllBehavior();
            for (int i = all.Count - 1; i >= 0; i--)
            {
                UIBehavior bhvr = all[i];
                if (bhvr != uiBehavior && bhvr.WndInfo.Layer == uiBehavior.WndInfo.Layer && bhvr.IsVisible)
                {
                    if (bhvr is IVisibleHandler)
                        (bhvr as IVisibleHandler).OnDisVisible(false);
                }
            }
        }
    }

    private void DealTopWindowOnHide(UIBehavior uiBehavior)
    {
        if (uiBehavior is IVisibleHandler)
            (uiBehavior as IVisibleHandler).OnDisVisible(true);

    }

    private void SetTopWindow(UIBehavior uiBehavior)
    {
        UIWindowLayer layer = (UIWindowLayer)uiBehavior.WndInfo.Layer;
        if (topWindows.ContainsKey(layer))
            topWindows[layer] = uiBehavior;
        else
            topWindows.Add(layer, uiBehavior);
    }

    private List<UIBehavior> GetAllBehavior()
    {
        var list = new List<UIBehavior>(uiBehaviorDic.Values);
        var keys = new List<string>(uiBehaviorDic.Keys);
        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (list[i] == null)
            {
                list.RemoveAt(i);
                var t = keys[i];
                uiBehaviorDic.Remove(t);
                Debug.LogWarning($"UIBehavior name : {t} is NULL !!!");
            }
        }
        list.Sort(UIBehaviorSortFunc);
        return list;
    }

    private int UIBehaviorSortFunc(UIBehavior x, UIBehavior y)
    {
        return x.SortingOrder - y.SortingOrder;
    }

    private async Task SetVisibleImp(GameObject obj, bool visible)
    {
        try
        {
            Debug.Log($"UISystem SetVisible: {obj.name}- {visible}");
            var bhvr = obj.GetComponent<UIBehavior>();
            Animator anim = obj.GetComponent<UIBehavior>().GetAnim();
            if (visible)
            {
                obj.gameObject.SetActive(true);
                obj.transform.localPosition = Vector3.zero;
                if (anim != null)
                {
                    anim.SetTrigger("open");
                }
            }
            else
            {
                if (anim != null)
                {
                    anim.SetTrigger("close");
                    black.gameObject.SetActive(true);

                    var hide = obj.TryAddComponent<UIHide>();
                    var timer = anim.GetAnimDuration("out", 1f);
                    await hide.Hide(timer);
                    obj.transform.localPosition = new Vector3(-50000, -50000, 0);
                    black.gameObject.SetActive(false);
                }
                else
                {
                    obj.transform.localPosition = new Vector3(-50000, -50000, 0);
                }
            }

            Debug.Log($"UISystem SetVisible localPosition :{obj.transform.localPosition}");
        }
        catch (Exception e)
        {
            Debug.LogError($"{e.Message}\n{e.StackTrace}");
        }
    }

    public bool IsTop(UIBehavior uiBehavior)
    {
        UIWindowLayer layer = (UIWindowLayer)uiBehavior.WndInfo.Layer;
        if (topWindows.TryGetValue(layer, out var topWindow))
        {
            return topWindow.Equals(uiBehavior);
        }
        return false;
    }

    #endregion
}

public enum UIWindowLayer
{
    Normal = 1,
    PopUp = 2,
    Tip = 3,
    Marquee = 4,
    Common = 5,
    Loading = 6,
}

public class UIHide : MonoBehaviour
{
    private Tween tw;
    private TaskCompletionSource<bool> tcs;
    public Task Hide(float delayTimer = 1f)
    {
        tcs = new TaskCompletionSource<bool>();
        tw = DOVirtual.DelayedCall(delayTimer, DelayDo);
        return tcs.Task;
    }

    public virtual void UIHideFinish()
    {
        if (tw != null)
        {
            tw.Kill();
            tw = null;
            tcs.SetResult(true);
        }
    }

    private void DelayDo()
    {
        if (tw != null)
        {
            tcs.SetResult(true);
            tw = null;
        }
    }
}