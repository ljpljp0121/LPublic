using System.Threading.Tasks;
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
        UISystem.Instance.HideUI<StartPanel>();
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