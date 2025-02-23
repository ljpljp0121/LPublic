using UnityEngine;

public class Loading_Main : UIBase
{
    [InitOnLoad]
    private static void Init()
    {
        EventSystem.RegisterEvent<E_OnShowLoadingUI>(OnShowLoadingUI);
        EventSystem.RegisterEvent<E_OnCloseLoadingUI>(OnCloseLoadingUI);
    }

    private static void OnShowLoadingUI(E_OnShowLoadingUI obj)
    {
    }

    private static void OnCloseLoadingUI(E_OnCloseLoadingUI obj)
    {
        Debug.Log("E_CloseLoadingUI");
        TaskUtil.Run(async () =>
        {
            while (!UIManager.Instance.IsShow<Loading_Main>())
                await TaskUtil.Return();
            UIManager.Instance.GetUIBase<Loading_Main>().Hide();
        });
    }
}