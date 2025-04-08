using GAS.Editor;
using GAS.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR

public class GetTagTaskInspector : OngoingTaskInspector<GetTagTask>
{
    [Delayed]
    [OnValueChanged("OnGameplayTagChanged")]
    public GameplayTag Tag;
    public override void Init(OngoingAbilityTask task)
    {
        base.Init(task);

    }

    void OnGameplayTagChanged()
    {
        _task.SetGameplayTag(Tag);
        Save();
    }
}

#endif