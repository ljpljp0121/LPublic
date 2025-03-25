using System;
using UnityEngine;
using UnityEngine.Serialization;


public class Player : MonoBehaviour, IComponent
{
    public const int HpMax = 100;
    public const int MpMax = 100;
    public const int StaminaMax = 100;
    public const int ATK = 5;
    public const int Speed = 8;

    #region 组件初始化

    public void Init() { }

    public void UnInit() { }

    #endregion
}