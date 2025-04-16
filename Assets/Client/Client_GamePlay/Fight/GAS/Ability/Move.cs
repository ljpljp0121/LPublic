using System.Text.RegularExpressions;
using GAS.Runtime;
using UnityEngine;

public class Move : AbstractAbility<AAMove>
{
    public Move(AAMove abilityAsset) : base(abilityAsset) { }

    public override AbilitySpec CreateSpec(AbilitySystemComponent owner) => new MoveSpec(this, owner);
}

public class MoveSpec : AbilitySpec, IStateMachineOwner
{
    public Vector3 InputDir { get; private set; }
    public AnimationCom AnimCom => animCom;

    private readonly Player player;
    private readonly AnimationCom animCom;
    private readonly CharacterController characterController;
    private readonly NoneUpdateStateMachine stateMachine;
    private readonly Camera mainCamera;
    private MoveState curState = MoveState.Idle;

    public MoveSpec(AbstractAbility ability, AbilitySystemComponent owner) : base(ability, owner)
    {
        player = owner.GetComponent<Player>();
        animCom = owner.GetComponentInChildren<AnimationCom>();
        characterController = owner.GetComponent<CharacterController>();
        stateMachine = AssetSystem.GetOrNew<NoneUpdateStateMachine>();
        stateMachine.Init(this);
        mainCamera = Camera.main;
    }

    public override void ActivateAbility(params object[] args)
    {
        EventSystem.RegisterEvent<EOnInputMove>(OnMove);
        ChangeState(MoveState.Idle);
    }

    public override void CancelAbility()
    {
        EventSystem.RemoveEvent<EOnInputMove>(OnMove);
    }

    public override void EndAbility()
    {
        CancelAbility();
    }


    protected override void AbilityTick()
    {
        stateMachine.Tick();
        Rotate(InputDir);
    }

    private void OnMove(EOnInputMove obj)
    {
        InputDir = new Vector3(obj.MoveDir.x, 0, obj.MoveDir.y);
    }

    public void Rotate(Vector3 input, float rotateSpeed = 0)
    {
        if (input == Vector3.zero) return;
        if (rotateSpeed == 0) rotateSpeed = player.RoleConfig.RotateSpeed;

        if (mainCamera != null)
        {
            float y = mainCamera.transform.rotation.eulerAngles.y;
            Vector3 moveDir = Quaternion.Euler(0, y, 0) * input;
            animCom.ModelTransform.rotation = Quaternion.Slerp(animCom.ModelTransform.rotation,
                Quaternion.LookRotation(moveDir), Time.deltaTime * rotateSpeed);
        }
    }

    public void ChangeState(MoveState moveState, bool reCurrstate = false)
    {
        curState = moveState;
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
        }
    }
}