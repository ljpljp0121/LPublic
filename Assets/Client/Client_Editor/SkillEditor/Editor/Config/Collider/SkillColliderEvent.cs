using System;
using UnityEngine;

[Serializable]
public class SkillColliderEvent : SkillFrameEventBase
{
#if UNITY_EDITOR
    public string TrackName = "攻击检测轨道";
#endif
    public int FrameIndex; //事件开始帧
    public int DurationFrame = 5; //攻击检测持续时间
    public SkillColliderBase SkillColliderData; //攻击检测类型

    public SkillColliderType GetAttackDetectionType()
    {
        if (SkillColliderData == null) return SkillColliderType.None;
        if (SkillColliderData is WeaponCollider) return SkillColliderType.Weapon;
        if (SkillColliderData is BoxSkillCollider) return SkillColliderType.Box;
        if (SkillColliderData is SphereSkillCollider) return SkillColliderType.Sphere;
        if (SkillColliderData is FanSkillCollider) return SkillColliderType.Fan;
        return SkillColliderType.None;
    }

#if UNITY_EDITOR
    public SkillColliderType SkillColliderType
    {
        get => GetAttackDetectionType();
        set
        {
            if (value != SkillColliderType)
            {
                switch (value)
                {
                    case SkillColliderType.None:
                        SkillColliderData = null;
                        break;
                    case SkillColliderType.Weapon:
                        SkillColliderData = new WeaponCollider();
                        break;
                    case SkillColliderType.Box:
                        SkillColliderData = new BoxSkillCollider();
                        break;
                    case SkillColliderType.Sphere:
                        SkillColliderData = new SphereSkillCollider();
                        break;
                    case SkillColliderType.Fan:
                        SkillColliderData = new FanSkillCollider();
                        break;
                }
            }
        }
    }
#endif
}

#region 攻击检测类型

/// <summary>
/// 攻击检测类型
/// </summary>
public enum SkillColliderType
{
    None,
    Weapon,
    Box,
    Sphere,
    Fan
}

/// <summary>
/// 攻击检测基类
/// </summary>
public abstract class SkillColliderBase
{
}

/// <summary>
/// 武器攻击检测类
/// </summary>
public class WeaponCollider : SkillColliderBase
{
    public string weaponName;
}

public abstract class ShapeSkillCollider : SkillColliderBase
{
    public Vector3 Position;
}

/// <summary>
/// 盒型攻击检测类
/// </summary>
public class BoxSkillCollider : ShapeSkillCollider
{
    public Vector3 Rotation;
    public Vector3 Scale = Vector3.one;
}

/// <summary>
/// 球形攻击检测类
/// </summary>
public class SphereSkillCollider : ShapeSkillCollider
{
    public float Radius = 1;
}

/// <summary>
/// 扇形攻击检测类
/// </summary>
public class FanSkillCollider : ShapeSkillCollider
{
    public Vector3 Rotation;
    public float InsideRadius = 1;
    public float OutsideRadius = 3;
    public float Height = 0.5f;
    public float Angle = 90;
}

#endregion