public class MoveStateBase : StateBase
{
    protected AnimationCom animationCom;
    protected MovementCom movementCom;

    public override void Init(IStateMachineOwner owner)
    {
        base.Init(owner);
        movementCom = (MovementCom)owner;
        animationCom = movementCom.AnimCom;
    }
}