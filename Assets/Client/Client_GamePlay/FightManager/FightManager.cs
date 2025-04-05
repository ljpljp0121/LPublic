public class FightManager : Singleton<FightManager>, IStateMachineOwner
{
    private FightState curState = FightState.None;
    private StateMachine stateMachine;

    public void BeginFight()
    {
        stateMachine = AssetSystem.GetOrNew<StateMachine>();
        stateMachine.Init(this);
        // stateMachine.ChangeState<>()
    }
}

public enum FightState
{
    None,
    Init,
    LoadingScene,
    LoadingEntities,
    BattleActive,
    Paused,
    BattleVictory,
    BattleFailed,
    UnLoading,
}