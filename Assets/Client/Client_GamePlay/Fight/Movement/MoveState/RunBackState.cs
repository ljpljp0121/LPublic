using UnityEngine;

public class RunBackState : MoveStateBase
{
    public override void Enter()
    {
        movementCom.LockRotate = true;
        animationCom.PlaySingleAnimation("Animation/XingJianYa/A06_RunA_B_Turn.anim", OnAnimEnd);
    }

    public override void Update()
    {
        // if (movementCom.InputDir == Vector3.zero)
        // {
        //     movementCom.ChangeState(MoveState.RunEnd);
        //     movementCom.LockRotate = false;
        // }
    }

    private void OnAnimEnd()
    {
        movementCom.LockRotate = false;
        movementCom.ChangeState(MoveState.Run);
    }
}