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
        asc.AttrSet<AS_Fight>().InitHP(100);
        asc.AttrSet<AS_Fight>().InitATK(10);
    }

    public override void Enable()
    {
        EventSystem.RegisterEvent<EOnInputCommonAttack>(OnCommonAttack);
        asc.AttrSet<AS_Fight>().HP.RegisterPreBaseValueChange(OnHpChangePre);
        asc.AttrSet<AS_Fight>().HP.RegisterPostBaseValueChange(OnHpChangePost);
    }

    public override void Disable()
    {
        EventSystem.RemoveEvent<EOnInputCommonAttack>(OnCommonAttack);
        asc.AttrSet<AS_Fight>().HP.UnregisterPreBaseValueChange(OnHpChangePre);
        asc.AttrSet<AS_Fight>().HP.UnregisterPostBaseValueChange(OnHpChangePost);
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

    #region 事件

    private void OnHpChangePost(AttributeBase arg1, float arg2, float arg3)
    {
    }

    private float OnHpChangePre(AttributeBase attr, float newValue)
    {
        return 0f;
    }

    #endregion
}