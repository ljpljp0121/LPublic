using GAS.Runtime;

public class LockState : MoveStateBase
{
    public override void Enter()
    {

    }

    public override void Update()
    {
        if (!movementCom.ASC.HasTag(GTagLib.Event_BlockMove))
            movementCom.ChangeState(MoveState.Idle);
    }

    public override void Exit()
    {

    }
}