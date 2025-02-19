using System.Threading.Tasks;
using UnityEngine;

public class StartPanel : UIBase
{
    protected override void OnShow(params object[] args)
    {
        StartLoading.Close();
    }
}