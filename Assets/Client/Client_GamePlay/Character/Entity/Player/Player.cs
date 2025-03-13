using System;
using UnityEngine;
using UnityEngine.Serialization;


public class Player : MonoBehaviour, IComponent, IStateMachineOwner, IUpdatable, IRequire<AnimationCom>, IRequire<SkillPlayerCom>
{
    private StateMachine stateMachine;
    private PlayerState playerState;
    private AnimationCom aniCom;
    private SkillPlayerCom skillPlayerCom;

    #region 组件初始化

    public void SetDependency(AnimationCom dependency) => aniCom = dependency;

    public void SetDependency(SkillPlayerCom dependency) => skillPlayerCom = dependency;

    public void Init()
    {
        stateMachine = AssetSystem.GetOrNew<StateMachine>();
        stateMachine.Init(this);
    }

    public void UnInit()
    {
        stateMachine = null;
    }

    #endregion

    public void ChangeState(PlayerState playerState, bool reCurState = false)
    {
        this.playerState = playerState;
        switch (playerState)
        {
            case PlayerState.Idle:
                stateMachine.ChangeState<PlayerIdleState>(reCurState);
                break;
            case PlayerState.Move:
                stateMachine.ChangeState<PlayerMoveState>(reCurState);
                break;
        }
    }

    public void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            skillPlayerCom.PlaySkillClip(1001);
        }
    }
}