using UnityEngine;

public class RunState : MoveStateBase
{
    public override void Enter()
    {
        animationCom.PlaySingleAnimation("Animation/XingJianYa/A06_RunA_B_Loop.anim");
    }

    public override void Update()
    {
        if (moveUnit.InputDir == Vector3.zero)
            moveUnit.ChangeState(MoveState.RunEnd);
    }
}