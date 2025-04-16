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
    private CharacterController characterController;
    private Dictionary<string, string> comboMap = new Dictionary<string, string>();

    private string curComboName = GAbilityLib.CommonAttack1.Name;
    private string nextCombo = GAbilityLib.CommonAttack2.Name;

    #region 组件初始化
    
    public void Awake()
    {
        PreLoadAssets();
        asc = GetComponent<AbilitySystemComponent>();
        animCom = GetComponentInChildren<AnimationCom>();
        characterController = GetComponent<CharacterController>();
        asc.InitWithPreset(1);
        asc.AttrSet<AS_Fight>().InitHP(100);
        asc.AttrSet<AS_Fight>().InitATK(10);
    }

    public override void Enable()
    {
        EventSystem.RegisterEvent<EOnInputCommonAttack>(OnCommonAttack);
        EventSystem.RegisterEvent<EOnInputMove>(OnMove);
        animCom.SetRootMotionAction(OnRootMotion);
        asc.AttrSet<AS_Fight>().HP.RegisterPreBaseValueChange(OnHpChangePre);
        asc.AttrSet<AS_Fight>().HP.RegisterPostBaseValueChange(OnHpChangePost);
        comboMap.Add(GAbilityLib.CommonAttack1.Name, GAbilityLib.CommonAttack2.Name);
        comboMap.Add(GAbilityLib.CommonAttack2.Name, GAbilityLib.CommonAttack3.Name);
        comboMap.Add(GAbilityLib.CommonAttack3.Name, GAbilityLib.CommonAttack4.Name);
        comboMap.Add(GAbilityLib.CommonAttack4.Name, GAbilityLib.CommonAttack5.Name);
        comboMap.Add(GAbilityLib.CommonAttack5.Name, GAbilityLib.CommonAttack1.Name);
    }

    public override void Disable()
    {
        EventSystem.RemoveEvent<EOnInputCommonAttack>(OnCommonAttack);
        EventSystem.RemoveEvent<EOnInputMove>(OnMove);
        animCom.ClearRootMotionAction();
        asc.AttrSet<AS_Fight>().HP.UnregisterPreBaseValueChange(OnHpChangePre);
        asc.AttrSet<AS_Fight>().HP.UnregisterPostBaseValueChange(OnHpChangePost);
    }

    private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        animCom.transform.rotation *= deltaRotation;
        characterController.Move(deltaPosition);
        SendEvent<EOnInputMove>();
    }
    
    public void SendEvent<T>() where T : BaseEvent
    {
        EventSystem.RegisterEvent<T>(OnTick);
    }

    private void OnTick<T>(T obj) where T : BaseEvent
    {
        
    }

    private void OnCommonAttack(EOnInputCommonAttack obj)
    {
        bool result = false;
        if (asc.HasTag(GTagLib.Effect_Common_ComboWindow))
        {
            result = asc.TryActivateAbility(nextCombo);
            if (result)
            {
                curComboName = nextCombo;
                nextCombo = comboMap[curComboName];
            }
        }
        else
        {
            result = asc.TryActivateAbility(GAbilityLib.CommonAttack1.Name);
            if (result)
            {
                curComboName = GAbilityLib.CommonAttack1.Name;
                nextCombo = GAbilityLib.CommonAttack2.Name;
            }
        }
        if (asc.HasTag(GTagLib.Effect_Common_CancelWindow))
        {
            result = asc.TryActivateAbility(GAbilityLib.CommonAttack1.Name);
            if (result)
            {
                curComboName = GAbilityLib.CommonAttack1.Name;
                nextCombo = GAbilityLib.CommonAttack2.Name;
            }
        }

        LogSystem.Log($"技能名称:{curComboName} 下一个技能:{nextCombo} 技能施放结果:{result}");
    }

    private void OnMove(EOnInputMove obj)
    {
        if (asc.HasTag(GTagLib.Effect_Common_CancelWindow))
        {
            asc.TryActivateAbility(GAbilityLib.Move.Name);
        }
    }

    private void PreLoadAssets() { }

    public void OnDestroy() { }

    public override void Tick()
    {
        if (!asc.HasTag(GTagLib.State))
        {
            asc.TryActivateAbility(GAbilityLib.Move.Name);
        }
    }

    #endregion

    #region 事件

    private void OnHpChangePost(AttributeBase arg1, float arg2, float arg3) { }

    private float OnHpChangePre(AttributeBase attr, float newValue)
    {
        return 0f;
    }

    #endregion
}