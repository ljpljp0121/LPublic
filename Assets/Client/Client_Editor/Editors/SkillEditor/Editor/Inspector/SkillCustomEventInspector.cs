using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 自定义事件片段Inspector窗口类
/// </summary>
public class SkillCustomEventInspector : SkillEventDataInspectorBase<CustomEventTrackItem, CustomEventTrack>
{
    private List<string> eventTypeChoiceList;

    public override void OnDraw()
    {
        eventTypeChoiceList = new List<string>(Enum.GetNames(typeof(SkillEventType)));
        DropdownField eventTypeDropDownField = new DropdownField("事件类型", eventTypeChoiceList,
            (int)trackItem.CustomEvent.EventType);
        eventTypeDropDownField.RegisterValueChangedCallback(OnEventTypeDropDownFieldValueChanged);
        root.Add(eventTypeDropDownField);
        //根据检测类型进行绘制
        switch (trackItem.CustomEvent.EventType)
        {
            case SkillEventType.Custom:
                //名称
                TextField nameField = new TextField("事件名称");
                nameField.value = trackItem.CustomEvent.CustomEventName;
                nameField.RegisterValueChangedCallback(NameFieldValueChanged);
                root.Add(nameField);
                break;
            case SkillEventType.CanSkillRelease:
                break;
        }

        //TODO 添加参数
        //参数
        // IntegerField intArgField = new IntegerField("Int参数");
        // intArgField.value = trackItem.CustomEvent.IntArg;
        // intArgField.RegisterValueChangedCallback(IntArgFieldValueChanged);
        //
        // FloatField floatArgField = new FloatField("Float参数");
        // floatArgField.value = trackItem.CustomEvent.FloatArg;
        // floatArgField.RegisterValueChangedCallback(FloatArgFieldValueChanged);
        //
        // TextField stringArgField = new TextField("String参数");
        // stringArgField.value = trackItem.CustomEvent.StringArg;
        // stringArgField.RegisterValueChangedCallback(StringArgFieldValueChanged);
        //
        // ObjectField objectArgField = new ObjectField("Object参数");
        // objectArgField.objectType = typeof(UnityEngine.Object);
        //
        // objectArgField.value = trackItem.CustomEvent.ObjectArg;
        // objectArgField.RegisterValueChangedCallback(ObjectArgFieldValueChanged);

        //删除

        Button deleteBtn = new Button(DeleteCustomEventTrackItemButtonClick);
        deleteBtn.text = "删除";
        deleteBtn.style.backgroundColor = new Color(1, 0, 0, 0.5f);

        //TODO 添加参数
        // root.Add(intArgField);
        // root.Add(floatArgField);
        // root.Add(stringArgField);
        // root.Add(objectArgField);
        root.Add(deleteBtn);
    }

    private void OnEventTypeDropDownFieldValueChanged(ChangeEvent<string> evt)
    {
        trackItem.CustomEvent.EventType = (SkillEventType)eventTypeChoiceList.IndexOf(evt.newValue);
        if (trackItem.CustomEvent.EventType != SkillEventType.Custom)
        {
            trackItem.CustomEvent.CustomEventName = "";
        }
        SkillEditorInspector.Instance.Show();
    }

    //TODO 添加参数
    // private void IntArgFieldValueChanged(ChangeEvent<int> evt)
    // {
    //     trackItem.CustomEvent.IntArg = evt.newValue;
    // }
    //
    // private void FloatArgFieldValueChanged(ChangeEvent<float> evt)
    // {
    //     trackItem.CustomEvent.FloatArg = evt.newValue;
    // }
    //
    // private void StringArgFieldValueChanged(ChangeEvent<string> evt)
    // {
    //     trackItem.CustomEvent.StringArg = evt.newValue;
    // }
    //
    // private void ObjectArgFieldValueChanged(ChangeEvent<UnityEngine.Object> evt)
    // {
    //     trackItem.CustomEvent.ObjectArg = evt.newValue;
    // }

    private void NameFieldValueChanged(ChangeEvent<string> evt)
    {
        trackItem.CustomEvent.CustomEventName = evt.newValue;
    }

    private void DeleteCustomEventTrackItemButtonClick()
    {
        track.DeleteTrackItem(itemFrameIndex);
        Selection.activeObject = null;
    }
}