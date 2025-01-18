using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class BuildMenu
{
    [MenuItem("Project/Build/Client")]
    public static void BuildClient()
    {
        Debug.Log("开始构建客户端");
        HybridCLR.Editor.SettingsUtil.Enable = true;
    }
}