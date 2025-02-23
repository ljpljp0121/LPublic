using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class StartPanel : UIBase
{
    [SerializeField] private Button StartBtn;
    [SerializeField] private Button ContinueBtn;
    [SerializeField] private Button QuitBtn;

    protected override void OnShow(params object[] args)
    {
        StartBtn.SetButton(OnStartBtnClick);
        ContinueBtn.SetButton(OnContinueBtnClick);
        QuitBtn.SetButton(OnQuitBtnClick);
        StartLoading.Close();
    }

    protected override void OnHide()
    {
        base.OnHide();
    }

    private void OnStartBtnClick()
    {
    }

    private void OnContinueBtnClick()
    {
        //进入存档界面
    }

    private void OnQuitBtnClick()
    {
        Application.Quit();
    }
}