using System;
using DG.Tweening.Core.Easing;
using GAS;
using GAS.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

public class MovementCom : GameComponent, IStateMachineOwner
{
    public AnimationCom AnimCom { get; private set; }
    public Vector3 InputDir { get; private set; }
    private CharacterController characterController;
    private Player player;
    private NoneUpdateStateMachine stateMachine;
    private AbilitySystemComponent asc;
    private MoveState curState = MoveState.Idle;

    private Camera mainCamera;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        stateMachine = AssetSystem.GetOrNew<NoneUpdateStateMachine>();
        AnimCom = GetComponentInChildren<AnimationCom>();
        player = GetComponent<Player>();
        asc = GetComponent<AbilitySystemComponent>();
        stateMachine.Init(this);
        mainCamera = Camera.main;
    }

    public override void Enable()
    {
        AnimCom.SetRootMotionAction(OnRootMotion);
        EventSystem.RegisterEvent<EOnInputMove>(OnMove);
    }
    
    public override void AfterEnable()
    {
        ChangeState(MoveState.Idle);
    }

    public override void Disable()
    {
        AnimCom.ClearRootMotionAction();
        EventSystem.RemoveEvent<EOnInputMove>(OnMove);
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
        AnimCom.transform.rotation *= deltaRotation;
        characterController.Move(deltaPosition);
    }

    public void Rotate(Vector3 input, float rotateSpeed = 0)
    {
        if (rotateSpeed == 0) rotateSpeed = player.RoleConfig.RotateSpeed;

        if (mainCamera != null)
        {
            float y = mainCamera.transform.rotation.eulerAngles.y;
            Vector3 moveDir = Quaternion.Euler(0, y, 0) * input;
            AnimCom.ModelTransform.rotation = Quaternion.Slerp(AnimCom.ModelTransform.rotation,
                Quaternion.LookRotation(moveDir), Time.deltaTime * rotateSpeed);
        }
    }

    public override void Tick()
    {
        stateMachine.Tick();
        if (InputDir != Vector3.zero)
            Rotate(InputDir);
    }

    private void OnMove(EOnInputMove obj)
    {
        if (asc.HasTag(GTagLib.Event_BlockMove))
        {
            InputDir = Vector3.zero;
            return;
        }
        InputDir = new Vector3(obj.MoveDir.x, 0, obj.MoveDir.y);
    }
}