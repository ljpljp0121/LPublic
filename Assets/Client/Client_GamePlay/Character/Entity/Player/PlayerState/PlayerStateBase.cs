public class PlayerStateBase : StateBase
{
    protected Player player;
    
    public override void Init(IStateMachineOwner owner)
    {
        base.Init(owner);
        player = (Player)owner;
    }
}