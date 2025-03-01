using System;

/// <summary>
/// 自定义事件帧事件
/// </summary>
[Serializable]
public class SkillCustomEvent : SkillFrameEventBase
{
    public SkillEventType EventType;
    public string CustomEventName;
}

public enum SkillEventType
{
    Custom,                 //自定义事件
    CanSkillRelease,        //可打断技能事件
    CanRotate,              //可技能时旋转事件
    CanNotRotate,           //不可技能时旋转事件
    AddBuff,                //添加Buff事件
}