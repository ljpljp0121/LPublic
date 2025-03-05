using System;
using UnityEngine;
using UnityEngine.Serialization;


public class Player : MonoBehaviour, IComponent, IStateMachineOwner, IRequire<AnimationComponent>
{
    private StateMachine stateMachine;
    private PlayerState playerState;
    public AnimationComponent AniComponent;

    #region 组件初始化

    public void SetDependency(AnimationComponent dependency) => AniComponent = dependency;
    
    public void Init()
    {
        stateMachine = AssetSystem.GetOrNew<StateMachine>();
        stateMachine.Init(this);
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
}