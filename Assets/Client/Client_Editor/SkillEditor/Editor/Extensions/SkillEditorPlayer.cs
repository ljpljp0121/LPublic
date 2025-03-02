using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

[Serializable]
public class SkillEditorPlayer : SerializedMonoBehaviour
{
    public Transform ModelTransform { get; set; }

    [NonSerialized, OdinSerialize] [DictionaryDrawerSettings(KeyLabel = "武器ID", ValueLabel = "预制体")]
    private Dictionary<string, Collider> weaponDic = new ();
    public Dictionary<string, Collider> WeaponDic => weaponDic;
}