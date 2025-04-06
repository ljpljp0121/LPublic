using UnityEngine;

public class RunEndState : MoveStateBase
{
    public override void Enter()
    {
        animationCom.PlaySingleAnimation("Animation/XingJianYa/A06_RunC_Stop_LeftFront.anim", OnAnimEnd);
    }

    public override void Update()
    {
        if(movementCom.InputDir != Vector3.zero)
            movementCom.ChangeState(MoveState.RunStart);
    }

    private void OnAnimEnd()
    {
        movementCom.ChangeState(MoveState.Idle);
    }
}