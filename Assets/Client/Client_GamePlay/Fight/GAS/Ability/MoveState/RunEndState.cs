using UnityEngine;

public class RunEndState : MoveStateBase
{
    public override void Enter()
    {
        animationCom.PlaySingleAnimation("Animation/XingJianYa/A06_RunC_Stop_LeftFront.anim", OnAnimEnd);
    }

    public override void Update()
    {
        if(moveUnit.InputDir != Vector3.zero)
            moveUnit.ChangeState(MoveState.RunStart);
    }

    private void OnAnimEnd()
    {
        moveUnit.ChangeState(MoveState.Idle);
    }
}