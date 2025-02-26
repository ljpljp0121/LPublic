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
    private UISystem uiSystem;

    public UIWnd WndInfo { get => wndInfo; set => wndInfo = value; }
    public Canvas Canvas => canvas != null ? canvas : canvas = GetComponent<Canvas>();
    public GraphicRaycaster RayCaster => rayCaster != null ? rayCaster : (rayCaster = GetComponent<GraphicRaycaster>());
    public int SortingOrder => Canvas.sortingOrder;

    #endregion

    #region 生命周期管理

    protected virtual void Awake()
    {
        uiSystem = UISystem.Instance;
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

    #region 显示/隐藏实现

    public async Task ShowImp(params object[] args)
    {
        try
        {
            transform.localPosition = new Vector3(-50000, -50000, 0);
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