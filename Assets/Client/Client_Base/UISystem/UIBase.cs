using System.Threading.Tasks;
using cfg;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas), typeof(GraphicRaycaster))]
public abstract class UIBase : MonoBehaviour
{
    public enum UIState
    {
        PreInit,
        Init,
        Showing,
        Visible,
        Hiding,
        Hidden,
        Destroying
    }

    private UIWnd wndInfo;
    private Canvas canvas;
    private UIState curState = UIState.PreInit;
    private int baseOrder;

    public bool IsVisible => curState == UIState.Visible;
    public int Layer => wndInfo.Layer;

    protected virtual void Awake()
    {
        wndInfo = TableSystem.GetVOData<TbUIWnd>().Get(this.GetType().Name);
        canvas = gameObject.GetComponent<Canvas>();

    }

    public void Init()
    {

        gameObject.SetActive(false);

        canvas = gameObject.GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
        }
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = UISystem.Instance.UICamera;
        canvas.overrideSorting = true;
        canvas.sortingOrder = wndInfo.Layer;

        if (gameObject.GetComponent<GraphicRaycaster>() == null)
        {
            gameObject.AddComponent<GraphicRaycaster>();
        }
    }

    public async Task Show(params object[] args)
    {
        if (IsVisible) return;

        gameObject.SetActive(true);
        transform.SetAsLastSibling();
        // UpdateSortingOrder();
        OnShow(args);
        await OnShowAsync(args);
    }

    private void UpdateSortingOrder(int order)
    {
    }

    public void Hide()
    {
        if (!IsVisible) return;

        if (UISystem.Instance.activeUIDic.ContainsKey(wndInfo.Name))
        {
            UISystem.Instance.activeUIDic.Remove(wndInfo.Name);
        }

        OnHide();
        gameObject.SetActive(false);

        // 如果配置为隐藏后销毁，则销毁 UI
        if (wndInfo.DestroyOnHide)
        {
            Destroy();
        }
    }

    public void Destroy()
    {
        OnDestroy();
        Destroy(gameObject);
    }

    protected virtual void OnShow(params object[] args)
    {
    }

    // 子类实现：UI 显示时的逻辑
    protected virtual Task OnShowAsync(params object[] args) => Task.CompletedTask;

    // 子类实现：UI 隐藏时的逻辑
    protected virtual void OnHide()
    {
    }

    // 子类实现：UI 销毁时的逻辑
    protected virtual void OnDestroy()
    {
    }
}