public class UpdaterDone : StateBase
{
    private PatchOperation owner;

    public override void Init(IStateMachineOwner owner)
    {
        this.owner = owner as PatchOperation;
    }

    public override void Enter()
    {
        owner.SetFinish();
        StartLoading.Close();
    }
}