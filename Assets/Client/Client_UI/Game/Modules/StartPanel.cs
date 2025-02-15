using System.Threading.Tasks;

public class StartPanel : UIBase
{
    protected override void OnShow(params object[] args)
    {
        StartLoading.Close();
        base.OnShow(args);
    }
}