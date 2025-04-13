public class MoveStateBase : StateBase
{
    protected AnimationCom animationCom;
    protected MoveSpec moveUnit;

    public override void Init(IStateMachineOwner owner)
    {
        base.Init(owner);
        moveUnit = (MoveSpec)owner;
        animationCom = moveUnit.AnimCom;
    }
}