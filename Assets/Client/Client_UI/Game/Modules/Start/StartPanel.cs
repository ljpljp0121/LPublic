using System.Threading.Tasks;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class StartPanel : UIBehavior
{
    [SerializeField] private Button StartBtn;
    [SerializeField] private Button ContinueBtn;
    [SerializeField] private Button QuitBtn;

    protected override void OnShow(params object[] args)
    {
        StartBtn.SetButton(OnStartBtnClick);
        ContinueBtn.SetButton(OnContinueBtnClick);
        QuitBtn.SetButton(OnQuitBtnClick);
    }

    private void OnStartBtnClick()
    {
        Debug.Log("开始游戏");
        UISystem.HideUI<StartPanel>();

        CinemachineFreeLook _camera = GameObject.Find("CharacterCamera").GetComponent<CinemachineFreeLook>();
        GameObject go = AssetSystem.LoadAsset<GameObject>("Prefab/Role/星见雅");
        GameObject obj = Instantiate(go);
        _camera.Follow = obj.transform;
        _camera.LookAt = obj.transform;
        MouseManager.SetMouseLocked(true);
    }

    private void OnContinueBtnClick()
    {
        //进入存档界面
        Debug.Log("继续游戏");
    }

    private void OnQuitBtnClick()
    {
        Application.Quit();
    }
}