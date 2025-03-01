using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 攻击检测片段Inspector窗口类
/// </summary>
public class
    SkillAttackDetectionEventInspector : SkillEventDataInspectorBase<AttackDetectionTrackItem, AttackDetectionTrack>
{
    public override void OnDraw()
    {
        DrawDetection();
        // DrawHitConfig();
    }

    #region Inspector窗口检测部分

    private List<string> detectionChoiceList;

    IntegerField durationFrameField;
    Vector3Field shapeDetectionPosField;
    Vector3Field boxDetectionRotField;
    Vector3Field boxDetectionScaleField;
    FloatField sphereRadiusField;

    private void DrawDetection()
    {
        durationFrameField = new IntegerField("持续帧数");
        durationFrameField.value = trackItem.ColliderEvent.DurationFrame;
        durationFrameField.RegisterCallback<FocusInEvent>(DetectionDurationFrameFieldFocusIn);
        durationFrameField.RegisterCallback<FocusOutEvent>(DetectionDurationFrameFieldFocusOut);
        root.Add(durationFrameField);

        detectionChoiceList = new List<string>(Enum.GetNames(typeof(SkillColliderType)));
        DropdownField detectionDropDownField = new DropdownField("检测类型", detectionChoiceList,
            (int)trackItem.ColliderEvent.SkillColliderType);
        detectionDropDownField.RegisterValueChangedCallback(OnDetectionDropDownFieldValueChanged);
        root.Add(detectionDropDownField);
        //根据检测类型进行绘制
        switch (trackItem.ColliderEvent.SkillColliderType)
        {
            case SkillColliderType.Weapon:
                WeaponCollider weaponDetectionData = (WeaponCollider)trackItem.ColliderEvent.SkillColliderData;
                DropdownField weaponDetectionDropDownField = new DropdownField("武器选择");
                if (SkillEditorWindow.Instance.PreviewCharacterObj != null)
                {
                    SkillEditorPlayer skillPlayer =
                        SkillEditorWindow.Instance.PreviewCharacterObj.GetComponent<SkillEditorPlayer>();
                    weaponDetectionDropDownField.choices = skillPlayer.WeaponDic.Keys.ToList();
                }

                if (!string.IsNullOrEmpty(weaponDetectionData.weaponName))
                {
                    weaponDetectionDropDownField.value = weaponDetectionData.weaponName;
                }

                weaponDetectionDropDownField.RegisterValueChangedCallback(WeaponDetectionDropDownFieldValueChanged);

                root.Add(weaponDetectionDropDownField);
                break;
            case SkillColliderType.Box:
                BoxSkillCollider boxDetectionData =
                    (BoxSkillCollider)trackItem.ColliderEvent.SkillColliderData;
                shapeDetectionPosField = new Vector3Field("坐标");
                boxDetectionRotField = new Vector3Field("旋转");
                boxDetectionScaleField = new Vector3Field("缩放");

                shapeDetectionPosField.value = boxDetectionData.Position;
                boxDetectionRotField.value = boxDetectionData.Rotation;
                boxDetectionScaleField.value = boxDetectionData.Scale;

                shapeDetectionPosField.RegisterValueChangedCallback(ShapeDetectionPosFieldValueChanged);
                boxDetectionRotField.RegisterValueChangedCallback(BoxDetectionRotFieldValueChanged);
                boxDetectionScaleField.RegisterValueChangedCallback(BoxDetectionScaleFieldValueChanged);

                root.Add(shapeDetectionPosField);
                root.Add(boxDetectionRotField);
                root.Add(boxDetectionScaleField);
                break;
            case SkillColliderType.Sphere:
                SphereSkillCollider sphereDetectionData =
                    (SphereSkillCollider)trackItem.ColliderEvent.SkillColliderData;
                shapeDetectionPosField = new Vector3Field("坐标");
                sphereRadiusField = new FloatField("半径");

                shapeDetectionPosField.value = sphereDetectionData.Position;
                sphereRadiusField.value = sphereDetectionData.Radius;

                shapeDetectionPosField.RegisterValueChangedCallback(ShapeDetectionPosFieldValueChanged);
                sphereRadiusField.RegisterValueChangedCallback(SphereRadiusFieldValueChanged);

                root.Add(shapeDetectionPosField);
                root.Add(sphereRadiusField);
                break;
            case SkillColliderType.Fan:
                FanSkillCollider fanAttackDetection =
                    (FanSkillCollider)trackItem.ColliderEvent.SkillColliderData;
                shapeDetectionPosField = new Vector3Field("坐标");
                Vector3Field fanDetectionRotField = new Vector3Field("旋转");
                FloatField fanInsideRadiusField = new FloatField("内半径");
                FloatField fanOutsideRadiusField = new FloatField("外半径");
                FloatField fanHeightField = new FloatField("高度");
                FloatField fanAngleField = new FloatField("角度");

                shapeDetectionPosField.value = fanAttackDetection.Position;
                fanDetectionRotField.value = fanAttackDetection.Rotation;
                fanInsideRadiusField.value = fanAttackDetection.InsideRadius;
                fanOutsideRadiusField.value = fanAttackDetection.OutsideRadius;
                fanHeightField.value = fanAttackDetection.Height;
                fanAngleField.value = fanAttackDetection.Angle;

                shapeDetectionPosField.RegisterValueChangedCallback(ShapeDetectionPosFieldValueChanged);
                fanDetectionRotField.RegisterValueChangedCallback(FanDetectionRotFieldValueChanged);
                fanInsideRadiusField.RegisterValueChangedCallback(FanInsideRadiusFieldValueChanged);
                fanOutsideRadiusField.RegisterValueChangedCallback(FanOutsideRadiusFieldValueChanged);
                fanHeightField.RegisterValueChangedCallback(FanHeightFieldValueChanged);
                fanAngleField.RegisterValueChangedCallback(FanAngleFieldValueChanged);

                root.Add(shapeDetectionPosField);
                root.Add(fanDetectionRotField);
                root.Add(fanInsideRadiusField);
                root.Add(fanOutsideRadiusField);
                root.Add(fanHeightField);
                root.Add(fanAngleField);
                break;
        }

        //设置持续帧数至选中帧
        Button setFrameBtn = new Button(SetDetectionFrameBtnClick);
        setFrameBtn.text = "设置持续帧数至选中帧";
        root.Add(setFrameBtn);
    }

    #region 通用事件

    float oldDetectionDuration;

    private void DetectionDurationFrameFieldFocusIn(FocusInEvent evt)
    {
        oldDetectionDuration = durationFrameField.value;
    }

    private void DetectionDurationFrameFieldFocusOut(FocusOutEvent evt)
    {
        if (!Mathf.Approximately(durationFrameField.value, oldDetectionDuration))
        {
            trackItem.ColliderEvent.DurationFrame = durationFrameField.value;
            SkillEditorInspector.Instance.Show();
            trackItem.ResetView();
        }
    }

    private void OnDetectionDropDownFieldValueChanged(ChangeEvent<string> evt)
    {
        trackItem.ColliderEvent.SkillColliderType = (SkillColliderType)detectionChoiceList.IndexOf(evt.newValue);
        SkillEditorInspector.Instance.Show();
    }

    private void SetDetectionFrameBtnClick()
    {
        DetectionDurationFrameFieldFocusIn(null);
        durationFrameField.value = SkillEditorWindow.Instance.CurrentSelectFrameIndex - trackItem.FrameIndex;
        DetectionDurationFrameFieldFocusOut(null);
    }

    private void ShapeDetectionPosFieldValueChanged(ChangeEvent<Vector3> evt)
    {
        ShapeSkillCollider shapeAttackDetection = (ShapeSkillCollider)trackItem.ColliderEvent.SkillColliderData;
        shapeAttackDetection.Position = evt.newValue;
    }

    #endregion

    #region 武器事件

    private void WeaponDetectionDropDownFieldValueChanged(ChangeEvent<string> evt)
    {
        WeaponCollider weaponAttackDetection =
            (WeaponCollider)trackItem.ColliderEvent.SkillColliderData;
        weaponAttackDetection.weaponName = evt.newValue;
    }

    #endregion

    #region 盒型事件

    private void BoxDetectionRotFieldValueChanged(ChangeEvent<Vector3> evt)
    {
        BoxSkillCollider boxAttackDetection = (BoxSkillCollider)trackItem.ColliderEvent.SkillColliderData;
        boxAttackDetection.Rotation = evt.newValue;
    }

    private void BoxDetectionScaleFieldValueChanged(ChangeEvent<Vector3> evt)
    {
        BoxSkillCollider boxAttackDetection = (BoxSkillCollider)trackItem.ColliderEvent.SkillColliderData;
        boxAttackDetection.Scale = evt.newValue;
    }

    #endregion

    #region 球形事件

    private void SphereRadiusFieldValueChanged(ChangeEvent<float> evt)
    {
        SphereSkillCollider sphereAttackDetection =
            (SphereSkillCollider)trackItem.ColliderEvent.SkillColliderData;
        sphereAttackDetection.Radius = evt.newValue;
    }

    #endregion

    #region 扇形事件

    private void FanDetectionRotFieldValueChanged(ChangeEvent<Vector3> evt)
    {
        FanSkillCollider fanAttackDetection = (FanSkillCollider)trackItem.ColliderEvent.SkillColliderData;
        fanAttackDetection.Rotation = evt.newValue;
    }

    private void FanInsideRadiusFieldValueChanged(ChangeEvent<float> evt)
    {
        FanSkillCollider fanAttackDetection = (FanSkillCollider)trackItem.ColliderEvent.SkillColliderData;
        fanAttackDetection.InsideRadius = evt.newValue;
        if (fanAttackDetection.OutsideRadius < fanAttackDetection.InsideRadius)
        {
            fanAttackDetection.OutsideRadius = fanAttackDetection.InsideRadius;
            SkillEditorInspector.Instance.Show();
        }
    }

    private void FanOutsideRadiusFieldValueChanged(ChangeEvent<float> evt)
    {
        FanSkillCollider fanAttackDetection = (FanSkillCollider)trackItem.ColliderEvent.SkillColliderData;
        fanAttackDetection.OutsideRadius = evt.newValue;
        if (fanAttackDetection.OutsideRadius < fanAttackDetection.InsideRadius)
        {
            fanAttackDetection.InsideRadius = fanAttackDetection.OutsideRadius;
            SkillEditorInspector.Instance.Show();
        }
    }

    private void FanHeightFieldValueChanged(ChangeEvent<float> evt)
    {
        FanSkillCollider fanAttackDetection = (FanSkillCollider)trackItem.ColliderEvent.SkillColliderData;
        fanAttackDetection.Height = evt.newValue;
        if (fanAttackDetection.Height < 0)
        {
            fanAttackDetection.Height = 0f;
            SkillEditorInspector.Instance.Show();
        }
    }

    private void FanAngleFieldValueChanged(ChangeEvent<float> evt)
    {
        FanSkillCollider fanAttackDetection = (FanSkillCollider)trackItem.ColliderEvent.SkillColliderData;
        fanAttackDetection.Angle = evt.newValue;
        if (fanAttackDetection.Angle > 360 || fanAttackDetection.Angle < 0)
        {
            fanAttackDetection.Angle = Mathf.Clamp(evt.newValue, 0, 360);
            SkillEditorInspector.Instance.Show();
        }
    }

    #endregion

    #endregion

    //TODO Inspector命中部分
    // #region Inspector命中部分
    //
    // private void DrawHitConfig()
    // {
    //     root.Add(new Label());
    //     FloatField attackMultiplyField = new FloatField("攻击力系数");
    //     attackMultiplyField.value = trackItem.AttackDetectionEvent.AttackHitConfig.AttackMultiply;
    //     attackMultiplyField.RegisterValueChangedCallback(OnAttackMultiplyFieldValueChanged);
    //     root.Add(attackMultiplyField);
    //
    //     Vector3Field repelStrengthField = new Vector3Field("击退强度");
    //     repelStrengthField.value = trackItem.AttackDetectionEvent.AttackHitConfig.RepelStrength;
    //     repelStrengthField.RegisterValueChangedCallback(OnRepelStrengthFieldValueChanged);
    //     root.Add(repelStrengthField);
    //
    //     FloatField repelTimeField = new FloatField("击退时间");
    //     repelTimeField.value = trackItem.AttackDetectionEvent.AttackHitConfig.RepelTime;
    //     repelTimeField.RegisterValueChangedCallback(OnRepelTimeFieldValueChanged);
    //     root.Add(repelTimeField);
    //
    //     ObjectField hitEffectPrefabField = new ObjectField("击中特效预制体");
    //     hitEffectPrefabField.objectType = typeof(GameObject);
    //     hitEffectPrefabField.value = trackItem.AttackDetectionEvent.AttackHitConfig.HitEffectPrefab;
    //     hitEffectPrefabField.RegisterValueChangedCallback(OnHitEffectPrefabFieldValueChanged);
    //     root.Add(hitEffectPrefabField);
    //
    //     ObjectField hitAudioClipField = new ObjectField("击中音效");
    //     hitAudioClipField.objectType = typeof(AudioClip);
    //     hitAudioClipField.value = trackItem.AttackDetectionEvent.AttackHitConfig.HitAudioClip;
    //     hitAudioClipField.RegisterValueChangedCallback(HitAudioClipFieldValueChanged);
    //     root.Add(hitAudioClipField);
    // }
    //
    // private void OnAttackMultiplyFieldValueChanged(ChangeEvent<float> evt)
    // {
    //     trackItem.AttackDetectionEvent.AttackHitConfig.AttackMultiply = evt.newValue;
    // }
    //
    // private void OnRepelTimeFieldValueChanged(ChangeEvent<float> evt)
    // {
    //     trackItem.AttackDetectionEvent.AttackHitConfig.RepelTime = evt.newValue;
    // }
    //
    // private void OnRepelStrengthFieldValueChanged(ChangeEvent<Vector3> evt)
    // {
    //     trackItem.AttackDetectionEvent.AttackHitConfig.RepelStrength = evt.newValue;
    // }
    //
    // private void OnHitEffectPrefabFieldValueChanged(ChangeEvent<UnityEngine.Object> evt)
    // {
    //     trackItem.AttackDetectionEvent.AttackHitConfig.HitEffectPrefab = (GameObject)evt.newValue;
    // }
    //
    // private void HitAudioClipFieldValueChanged(ChangeEvent<UnityEngine.Object> evt)
    // {
    //     trackItem.AttackDetectionEvent.AttackHitConfig.HitAudioClip = (AudioClip)evt.newValue;
    // }
    //
    // #endregion
}