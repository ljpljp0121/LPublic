using UnityEngine;

public class RunStartState : MoveStateBase
{
    public override void Enter()
    {
        animationCom.PlaySingleAnimation("Animation/XingJianYa/A06_RunC_A_Start_handProp.anim", OnAnimEnd);
    }

    public override void Update()
    {
        if (moveUnit.InputDir == Vector3.zero)
            moveUnit.ChangeState(MoveState.RunEnd);
    }

    private void OnAnimEnd()
    {
        moveUnit.ChangeState(MoveState.Run);
    }
}