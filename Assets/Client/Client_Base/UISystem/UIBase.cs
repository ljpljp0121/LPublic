using System.Threading.Tasks;
using cfg;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIBase : MonoBehaviour
{
    public UIWnd WndInfo;
    private Canvas canvas;
    private int baseOrder;

    public bool IsVisible { get; private set; } = false;
    public int Layer => WndInfo.Layer;

    public void Init()
    {
        WndInfo = TableSystem.Table.TbUIWnd.Get(this.GetType().Name);
        gameObject.SetActive(false);

        canvas = gameObject.GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
        }
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = UIManager.Instance.UICamera;
        canvas.overrideSorting = true;
        canvas.sortingOrder = WndInfo.Layer;

        if (gameObject.GetComponent<GraphicRaycaster>() == null)
        {
            gameObject.AddComponent<GraphicRaycaster>();
        }
    }

    public async Task Show(params object[] args)
    {
        if (IsVisible) return;

        IsVisible = true;
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

        IsVisible = false;
        OnHide();
        gameObject.SetActive(false);

        // 如果配置为隐藏后销毁，则销毁 UI
        if (WndInfo.DestroyOnHide)
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