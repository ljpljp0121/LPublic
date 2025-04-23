using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using cfg.UI;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

public class UITool : OdinEditorWindow
{
    private static IEnumerable UIChoices = new ValueDropdownList<string>();

    public UITool()
    {
        SetUINameChoices();
        SetTypeChoices();
    }

    [InfoBox("<size=50>请注意,由于ShowUI会涉及到如资源系统,UI系统,所以一定要启动游戏并且初始化完成后才能够正常使用该工具", InfoMessageType.Error,VisibleIf = "IsPlay")]
    [LabelText("UI名称")]
    [ValueDropdown("UIChoices", HideChildProperties = true)]
    public string UIName;

    [BoxGroup("参数配置")]
    [ListDrawerSettings(ShowPaging = true, NumberOfItemsPerPage = 5)]
    public List<DynamicParameter> Parameters = new List<DynamicParameter>();

    [Button("调用UI", ButtonSizes.Large)]
    private void ShowUI()
    {
        object[] args = Parameters.Select(p => p.Value).ToArray();
        UISystem.ShowUIByName(UIName, args);
    }

    private static void SetUINameChoices()
    {
        var tbWndList = TableSystem.GetVOData<TbUIWnd>().DataMap;

        var uiNameChoices = new ValueDropdownList<string>();
        foreach (string uiName in tbWndList.Keys)
        {
            uiNameChoices.Add(uiName, uiName);
        }
        UIChoices = uiNameChoices;
    }

    private static void SetTypeChoices()
    {
        var types = new ValueDropdownList<Type>();
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            try
            {
                if (IsSystemAssembly(assembly)) continue;

                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsClass && !type.IsAbstract && !IsSystemType(type))
                    {
                        types.Add(type);
                    }
                }
            }
            catch (ReflectionTypeLoadException)
            {
            }
        }
        var temp = new ValueDropdownList<Type>()
        {
            typeof(int),
            typeof(float),
            typeof(string),
            typeof(bool),
            typeof(Vector2),
            typeof(Vector3),
            typeof(Vector2Int),
        };
        types.AddRange(temp);

        DynamicParameter.TypeChoices = types;

    }

    private static bool IsSystemAssembly(Assembly assembly)
    {
        return !assembly.FullName.StartsWith("Client");
    }

    private static bool IsSystemType(Type type)
    {
        return type.Namespace?.Split('.').Length > 1;
    }

    bool IsPlay()
    {
        return Application.isPlaying;
    }
}

[Serializable]
public class DynamicParameter
{
    [LabelText("参数类型")]
    [ValueDropdown("TypeChoices")]
    [OnValueChanged("OnTypeChanged")]
    [ShowInInspector]
    public Type SelectedType = typeof(int);

    [LabelText("参数值")]
    [ShowInInspector]
    public object Value
    {
        get => value;
        set => this.value = ConvertValue(value);
    }

    private object value;


    public static IEnumerable TypeChoices = new ValueDropdownList<Type>
    {
        //typeof(int),
        //typeof(float),
        //typeof(string),
        //typeof(bool),
        //typeof(Vector2),
        //typeof(Vector3),
        //typeof(Vector2Int),
        //typeof(object),
    };

    private void OnTypeChanged()
    {
        value = Activator.CreateInstance(SelectedType);
    }



    private object ConvertValue(object input)
    {
        try
        {
            return Convert.ChangeType(input, SelectedType);
        }
        catch
        {
            return value; // 转换失败时保留原值
        }
    }
}