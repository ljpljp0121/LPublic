using System;
using System.Collections.Generic;
using GAS.Runtime;
using UnityEngine;
using UnityEngine.Serialization;


public class Player : GameComponent
{
    public Role RoleConfig = new Role();
    private AbilitySystemComponent asc;
    private AnimationCom animCom;

    #region 组件初始化

    public void Awake()
    {
        PreLoadAssets();
        asc = GetComponent<AbilitySystemComponent>();
        animCom = GetComponent<AnimationCom>();
        asc.InitWithPreset(1);
    }

    public override void Enable()
    {
        EventSystem.RegisterEvent<EOnInputCommonAttack>(OnCommonAttack);
    }

    public override void Disable()
    {
        EventSystem.RemoveEvent<EOnInputCommonAttack>(OnCommonAttack);
    }

    private void OnCommonAttack(EOnInputCommonAttack obj)
    {
        var result = asc.TryActivateAbility(GAbilityLib.CommonAttack.Name);
        LogSystem.Log("技能施放结果：" + result);
    }

    private void PreLoadAssets() { }

    public void OnDestroy() { }
    public void Tick() { }

    #endregion
}