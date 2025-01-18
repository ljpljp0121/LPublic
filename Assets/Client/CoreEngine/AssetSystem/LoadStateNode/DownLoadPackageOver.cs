public class DownLoadPackageOver : StateBase
{
    public override void Enter()
    {
        stateMachine.ChangeState<LoadPackageDll>();
    }
}