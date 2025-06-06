using UnityEngine;

public class Loading_Main : UIBehavior
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
            //while (!UISystem.Instance.IsShow<Loading_Main>())
            //    await TaskUtil.Return();
            //UISystem.Instance.GetUIBase<Loading_Main>().Hide();
        });
    }
}