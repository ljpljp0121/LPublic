using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

[Serializable]
public class SkillEditorPlayer : MonoBehaviour
{
    public Transform ModelTransform { get; set; }

    [NonSerialized, OdinSerialize] [DictionaryDrawerSettings(KeyLabel = "武器ID", ValueLabel = "预制体")]
    private Dictionary<string, GameObject> weaponDic = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> WeaponDic => weaponDic;
}