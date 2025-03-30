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
               stateMachine.ChangeState<>()
                break;
            case MoveState.Walk:
                break;
            case MoveState.RunStart:
                break;
            case MoveState.Run:
                break;
            case MoveState.RunEnd:
                break;
            case MoveState.RunBack:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(moveState), moveState, null);
        }
    }
    public void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        animCom.transform.rotation *= deltaRotation;
        characterController.Move(deltaPosition);
    }
}