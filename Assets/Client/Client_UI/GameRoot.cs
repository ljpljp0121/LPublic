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

        LogSystem.Log("InitBaseOnLoad.Init");
        InitBaseOnLoad.Init();

        LogSystem.Log("InitBaseOnLoad.Init");
        InitLogicOnLoad.Init();

        LogSystem.Log("InitGamePlayOnLoad.Init");
        InitGamePlayOnLoad.Init();

        LogSystem.Log("InitUIOnLoad.Init");
        InitUIOnLoad.Init();

        UISystem.Instance.ShowUI<StartPanel>();
    }
}