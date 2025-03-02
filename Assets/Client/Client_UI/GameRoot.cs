using UnityEngine;

[DefaultExecutionOrder(-100)]
public class GameRoot : MonoBehaviour
{
    private void Awake()
    {
        DealInitOnLoad();
    }

    private void DealInitOnLoad()
    {
        LogSystem.Log("InitEngineOnLoad.Init");
        InitEngineOnLoad.Init();
        StartLoading.SetProgress("InitEngineOnLoad.Init", 20);

        LogSystem.Log("InitBaseOnLoad.Init");
        InitBaseOnLoad.Init();
        StartLoading.SetProgress("InitBaseOnLoad.Init", 40);

        LogSystem.Log("InitBaseOnLoad.Init");
        InitLogicOnLoad.Init();
        StartLoading.SetProgress("InitLogicOnLoad.Init", 60);

        LogSystem.Log("InitGamePlayOnLoad.Init");
        InitGamePlayOnLoad.Init();
        StartLoading.SetProgress("InitGamePlayOnLoad.Init", 80);

        LogSystem.Log("InitUIOnLoad.Init");
        InitUIOnLoad.Init();
        StartLoading.SetProgress("InitUIOnLoad.Init", 100);

        UISystem.Instance.ShowUI<StartPanel>();
    }
}