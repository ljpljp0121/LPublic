using System;
using DG.Tweening.Core.Easing;
using GAS;
using GAS.Runtime;
using UnityEngine;

public class MovementCom : GameComponent, IStateMachineOwner
{
    private AnimationCom animCom;
    private CharacterController characterController;
    private Player player;
    private StateMachine stateMachine;
    private AbilitySystemComponent asc;
    private MoveState curState = MoveState.Idle;


    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        stateMachine = AssetSystem.GetOrNew<StateMachine>();
        animCom = GetComponentInChildren<AnimationCom>();
        player = GetComponent<Player>();
        asc = GetComponent<AbilitySystemComponent>();
    }

    public override void Enable()
    {
        animCom.SetRootMotionAction(OnRootMotion);
    }

    public override void Disable()
    {
        animCom.ClearRootMotionAction();
    }

    public void ChangeState(MoveState moveState, bool reCurrstate = false)
    {
        switch (moveState)
        {
            case MoveState.Idle:
                stateMachine.ChangeState<IdleState>();
                break;
            case MoveState.Walk:
                stateMachine.ChangeState<WalkState>();
                break;
            case MoveState.RunStart:
                stateMachine.ChangeState<RunStartState>();
                break;
            case MoveState.Run:
                stateMachine.ChangeState<RunState>();
                break;
            case MoveState.RunEnd:
                stateMachine.ChangeState<RunEndState>();
                break;
            case MoveState.RunBack:
                stateMachine.ChangeState<RunBackState>();
                break;
            case MoveState.Lock:
                stateMachine.ChangeState<LockState>();
                break;
        }
    }

    public void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        animCom.transform.rotation *= deltaRotation;
        characterController.Move(deltaPosition);
    }

    public void Tick()
    {
    }
}