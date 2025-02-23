public class E_OnShowLoadingUI : BaseEvent
{
    public bool IsShow;
    public float Progress;
    public string Tip;

    public E_OnShowLoadingUI(bool isShow, float progress, string tip)
    {
        IsShow = isShow;
        Progress = progress;
        Tip = tip;
    }
}

public class E_OnCloseLoadingUI : BaseEvent
{
}