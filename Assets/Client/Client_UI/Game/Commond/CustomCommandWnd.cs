
using DG.Tweening;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class CustomCommandWnd : MonoBehaviour
{
    void Start()
    {
        Button prefab = this.transform.ClearChildrenExceptFirst<Button>();
        MethodInfo[] infos = this.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);

        for (int i = 0; i < infos.Length; i++)
        {
            var info = infos[i];
            foreach (object attribute in info.GetCustomAttributes(false))
            {
                if (attribute.GetType() == typeof(CommandAttribute))
                {
                    try
                    {
                        CommandAttribute attr = attribute as CommandAttribute;
                        var btn = Instantiate(prefab, this.transform);
                        SetCheckFlag(btn, attr);
                        SetButtonName(btn, attr);
                        btn.SetButton(() =>
                        {
                            info.Invoke(this, null);
                            SetCheckFlag(btn, attr);
                        });
                        btn.gameObject.SetActive(true);
                        break;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
            }
        }
    }

    private void SetButtonName(Button btn, CommandAttribute attr)
    {
        try
        {
            if (attr.BtnName.EndsWith("()"))
            {
                var methodInfo = this.GetType().GetMethod(attr.BtnName.Replace("()", ""));
                if (methodInfo != null)
                {
                    btn.GetComponentInChildren<Text>().text = (string)methodInfo.Invoke(this, null);
                }
                else
                {
                    btn.GetComponentInChildren<Text>().text = attr.BtnName;
                }
            }
            else
            {
                btn.GetComponentInChildren<Text>().text = attr.BtnName;
            }
            if (attr.BtnNameRefresh > 0)
            {
                DOVirtual.DelayedCall(attr.BtnNameRefresh, () =>
                {
                    SetButtonName(btn, attr);
                });
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    private void SetCheckFlag(Button btn, CommandAttribute attr)
    {
        try
        {
            var img = btn.GetComponent<Image>();
            if (img != null)
            {
                bool btnActive = false;
                if (string.IsNullOrEmpty(attr.checkFlagFunc) == false)
                {
                    var methodInfo = this.GetType().GetMethod(attr.checkFlagFunc);
                    btnActive = (bool)methodInfo.Invoke(this, null);
                }
                img.color = btnActive ? Color.yellow : Color.white;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }


    #region 指令区域

    

    #endregion
}

[AttributeUsage(AttributeTargets.Method)]
public class CommandAttribute : Attribute
{
    public string BtnName = "未命名";
    public string checkFlagFunc = "";
    public float BtnNameRefresh = 0;

    public CommandAttribute(string name)
    {
        this.BtnName = name;
    }

    public CommandAttribute(string name, string checkFlagFunc)
    {
        this.BtnName = name;
        this.checkFlagFunc = checkFlagFunc;
    }
    public CommandAttribute(string name, float refresh)
    {
        this.BtnName = name;
        this.BtnNameRefresh = refresh;
    }
}