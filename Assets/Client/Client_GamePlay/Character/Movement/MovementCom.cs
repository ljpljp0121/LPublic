using System;
using DG.Tweening.Core.Easing;
using GAS.Runtime;
using UnityEngine;

public class MovementCom : MonoBehaviour, IComponent, IStateMachineOwner, IRequire<AnimationCom>,
    IRequire<AbilitySystemComponent>,
    IRequire<Player>
{
    private AnimationCom animCom;
    private CharacterController characterController;
    private AbilitySystemComponent asc;
    private Player player;
    private StateMachine stateMachine;
    private MoveState curState = MoveState.Idle;

    public void SetDependency(AnimationCom dependency) => animCom = dependency;
    public void SetDependency(AbilitySystemComponent dependency) => asc = dependency;
    public void SetDependency(Player dependency) => player = dependency;

    public void Init()
    {
        characterController = GetComponent<CharacterController>();
        animCom.SetRootMotionAction(OnRootMotion);
        stateMachine = AssetSystem.GetOrNew<StateMachine>();
    }

    public void UnInit()
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
}