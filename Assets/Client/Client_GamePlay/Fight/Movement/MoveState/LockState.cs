using GAS.Runtime;
using UnityEngine;

public class LockState : MoveStateBase
{
    public override void Update()
    {
        base.Update();
        if (!movementCom.ASC.HasTag(GTagLib.Event_BlockMove))
        {
            if (movementCom.InputDir != Vector3.zero)
                movementCom.ChangeState(MoveState.RunStart);
            else if(!movementCom.ASC.HasTag(GTagLib.Event_UseAbility))
                movementCom.ChangeState(MoveState.Idle);
        }
    }
}