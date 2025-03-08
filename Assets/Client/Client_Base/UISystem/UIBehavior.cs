using System;
using System.Reflection;
using System.Threading.Tasks;
using cfg.UI;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas), typeof(GraphicRaycaster))]
public abstract class UIBehavior : MonoBehaviour
{
    #region 实例属性

    private UIWnd wndInfo;
    private Canvas canvas;
    private GraphicRaycaster rayCaster;
    private UISystem uiSystem;

    public UIWnd WndInfo
    {
        get => wndInfo;
        set => wndInfo = value;
    }
    public Canvas Canvas => canvas != null ? canvas : canvas = GetComponent<Canvas>();
    public GraphicRaycaster RayCaster => rayCaster != null ? rayCaster : (rayCaster = GetComponent<GraphicRaycaster>());
    public int SortingOrder => Canvas.sortingOrder;

    #endregion

    #region 生命周期管理

    protected virtual void Awake()
    {
        uiSystem = UISystem.Instance;
        Canvas.overrideSorting = true;
        AutoBindComponents();
    }

    #endregion

    #region UI调用管理

    protected virtual void OnShow(params object[] args)
    {
    }

    protected virtual Task OnShowAsync(params object[] args) => Task.CompletedTask;
    protected virtual Task OnPreShowAsync(params object[] args) => Task.CompletedTask;
    protected virtual Task OnPostShowAsync(params object[] args) => Task.CompletedTask;

    public virtual Animator GetAnim()
    {
        return this.transform.GetComponent<Animator>();
    }

    protected virtual void OnPreHide()
    {

    }

    protected virtual void OnPostHide()
    {

    }

    #endregion

    #region 显示/隐藏实现

    /// <summary>
    /// 显示接口，uiSystem调用
    /// </summary>
    public async Task ShowImp(params object[] args)
    {
        try
        {
            transform.localPosition = new Vector3(-50000, -50000, 0);
            try
            {
                await OnPreShowAsync(args);
                gameObject.SetActive(true);
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

    /// <summary>
    /// 关闭接口，uiSystem调用
    /// </summary>
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
        if (uiSystem.TryGetFromPrefabDic(uiName, out var prefab))
        {
            Destroy(prefab);
            uiSystem.TryRemovePrefabDic(uiName);
            LogSystem.Log("-----Destroy Prefab:" + uiName);
        }
        else
        {
            LogSystem.Log("=====DestroyUI Can't find uibehaviourName:" + uiName);
        }
        uiSystem.TryRemoveUIDic(uiName);
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
        var targetTransform = string.IsNullOrEmpty(path) ? transform.Find(type.Name) : transform.Find(path);

        return targetTransform?.GetComponent(type);
    }

    #endregion

    #region 显示关闭其他UI

    private void ShowUI<T>(params object[] args) where T : UIBehavior
    {
        uiSystem.ShowUI<T>(args);
    }

    private void HideUI<T>() where T : UIBehavior
    {
        uiSystem.HideUI<T>();
    }

    #endregion
}