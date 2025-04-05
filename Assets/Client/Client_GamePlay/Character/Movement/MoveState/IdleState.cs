using UnityEngine;

public class IdleState : MoveStateBase
{
    public override void Enter()
    {
        base.Enter();
        animationCom.PlaySingleAnimation("Animation/XingJianYa/A01_Idle.anim");
    }

    public override void Update()
    {
        if (movementCom.InputDir != Vector3.zero)
        {
            movementCom.ChangeState(MoveState.RunStart);
        }
    }
    
}