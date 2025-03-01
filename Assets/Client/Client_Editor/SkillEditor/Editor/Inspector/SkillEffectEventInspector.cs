using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 特效片段Inspector窗口类
/// </summary>
public class SkillEffectEventInspector : SkillEventDataInspectorBase<EffectTrackItem, EffectTrack>
{
    private IntegerField effectDurationField;

    public override void OnDraw()
    {
        //预制体
        ObjectField effectAssetField = new ObjectField("特效预制体");
        effectAssetField.objectType = typeof(GameObject);
        effectAssetField.value = trackItem.EffectEvent.EffectPrefab;
        effectAssetField.RegisterValueChangedCallback(EffectAssetFieldValueChanged);
        root.Add(effectAssetField);

        //坐标
        Vector3Field posField = new Vector3Field("位置坐标");
        posField.value = trackItem.EffectEvent.Position;
        posField.RegisterValueChangedCallback(PosFieldValueChanged);
        root.Add(posField);

        //旋转
        Vector3Field rotationField = new Vector3Field("位置旋转");
        rotationField.value = trackItem.EffectEvent.Rotation;
        rotationField.RegisterValueChangedCallback(RotationFieldValueChanged);
        root.Add(rotationField);

        //缩放
        Vector3Field scaleField = new Vector3Field("缩放大小");
        scaleField.value = trackItem.EffectEvent.Scale;
        scaleField.RegisterValueChangedCallback(ScaleFielddValueChanged);
        root.Add(scaleField);

        //自动销毁
        Toggle autoDestroyToggle = new Toggle("自动销毁");
        autoDestroyToggle.value = trackItem.EffectEvent.AutoDestroy;
        autoDestroyToggle.RegisterValueChangedCallback(AutoDestroyToggleValueChanged);
        root.Add(autoDestroyToggle);

        //持续时间
        effectDurationField = new IntegerField("持续帧数");
        effectDurationField.value = trackItem.EffectEvent.Duration;
        effectDurationField.RegisterCallback<FocusInEvent>(EffectDurationFieldFocusIn);
        effectDurationField.RegisterCallback<FocusOutEvent>(EffectDurationFieldFocusOut);
        root.Add(effectDurationField);

        //时间计算按钮
        Button calTimeBtn = new Button(CalculateEffectDuration);
        calTimeBtn.text = "重新计时";
        root.Add(calTimeBtn);

        //应用模型Transform属性按钮
        Button applyModelTransformBtn = new Button(ApplyModelTransform);
        applyModelTransformBtn.text = "应用模型Transform";
        root.Add(applyModelTransformBtn);

        //设置持续帧数至选中帧
        Button setFrameBtn = new Button(SetEffectFrameBtnClick);
        setFrameBtn.text = "设置持续帧数至选中帧";
        root.Add(setFrameBtn);
    }

    #region Inspector可交互元素回调

    /// <summary>
    /// 重新计算时间
    /// </summary>
    private void CalculateEffectDuration()
    {
        ParticleSystem[] particleSystems = trackItem.EffectEvent.EffectPrefab
                                            .GetComponentsInChildren<ParticleSystem>();
        int max = -1;
        for (int i = 0; i < particleSystems.Length; i++)
        {
            if (particleSystems[i].main.duration > max)
            {
                max = (int)(particleSystems[i].main.duration * SkillEditorWindow.Instance.SkillClip.FrameRate);
            }
        }
        trackItem.EffectEvent.Duration = max;
        effectDurationField.value = max;
        trackItem.ResetView();
    }

    /// <summary>
    /// 应用模型Transform属性
    /// </summary>
    private void ApplyModelTransform()
    {
        trackItem.ApplyModelTransform();
        SkillEditorInspector.Instance.Show();
    }

    /// <summary>
    /// 设置持续帧数至选中帧按钮点击
    /// </summary>
    private void SetEffectFrameBtnClick()
    {
        EffectDurationFieldFocusIn(null);
        effectDurationField.value = SkillEditorWindow.Instance.CurrentSelectFrameIndex - trackItem.FrameIndex;
        EffectDurationFieldFocusOut(null);
    }

    /// <summary>
    /// 在Inspector窗口中修改特效资源时
    /// 能够同时在数据层面修改特效资源
    /// </summary>
    /// <param name="evt"></param>
    private void EffectAssetFieldValueChanged(ChangeEvent<UnityEngine.Object> evt)
    {
        trackItem.EffectEvent.EffectPrefab = evt.newValue as GameObject;
        //重新计时
        CalculateEffectDuration();
        SkillEditorWindow.Instance.TickSkill();
    }

    /// <summary>
    /// 在Inspector窗口中修改位置坐标时
    /// 能够同时在数据层面修改位置坐标
    /// </summary>
    /// <param name="evt"></param>
    private void PosFieldValueChanged(ChangeEvent<UnityEngine.Vector3> evt)
    {
        trackItem.EffectEvent.Position = evt.newValue;
        trackItem.ResetView();
        SkillEditorWindow.Instance.TickSkill();
    }

    /// <summary>
    /// 在Inspector窗口中修改旋转坐标时
    /// 能够同时在数据层面修改旋转坐标
    /// </summary>
    /// <param name="evt"></param>
    private void RotationFieldValueChanged(ChangeEvent<UnityEngine.Vector3> evt)
    {
        trackItem.EffectEvent.Rotation = evt.newValue;
        trackItem.ResetView();
        SkillEditorWindow.Instance.TickSkill();
    }

    /// <summary>
    /// 在Inspector窗口中修改缩放大小时
    /// 能够同时在数据层面修改缩放大小
    /// </summary>
    /// <param name="evt"></param>
    private void ScaleFielddValueChanged(ChangeEvent<UnityEngine.Vector3> evt)
    {
        trackItem.EffectEvent.Scale = evt.newValue;
        trackItem.ResetView();
        SkillEditorWindow.Instance.TickSkill();
    }

    /// <summary>
    /// 是否自动销毁
    /// </summary>
    private void AutoDestroyToggleValueChanged(ChangeEvent<bool> evt)
    {
        trackItem.EffectEvent.AutoDestroy = evt.newValue;
    }

    float oldDuration;
    /// <summary>
    /// 点击Inspector窗口，持续时间时调用
    /// </summary>
    private void EffectDurationFieldFocusIn(FocusInEvent evt)
    {
        oldDuration = effectDurationField.value;
    }

    /// <summary>
    /// 在Inspector窗口中配置持续时间
    /// 点击回车进行保存
    /// </summary>
    private void EffectDurationFieldFocusOut(FocusOutEvent evt)
    {
        if (!Mathf.Approximately(effectDurationField.value, oldDuration))
        {
            trackItem.EffectEvent.Duration = effectDurationField.value;
            trackItem.ResetView();
            SkillEditorWindow.Instance.TickSkill();
        }
    }

    #endregion
}

