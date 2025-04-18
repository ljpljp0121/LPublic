using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ChronosSystem), true)]
public class ChronosSystemEditor : ToolEditor
{
    private double lastRepaintTime;
    public ChronosSystem Target => (ChronosSystem)target;

    public override void OnInspectorGUI()
    {
        if (Application.isPlaying && 
            EditorApplication.timeSinceStartup - lastRepaintTime > 0.1)
        {
            Repaint();
            lastRepaintTime = EditorApplication.timeSinceStartup;
        }
        Header();
        base.OnInspectorGUI();
        ShowRuntimeInfomation();
    }

    private void Header()
    {
        Header2($"{Target.name}", true);
        LabelGrey($"{Target.GetType()}");
    }

    private void ShowRuntimeInfomation()
    {
        Label("运行时时间刻度组(TimeScaleGroups)", true, true, true);
        if (!Application.isPlaying)
        {
            MessageBox("请切换到运行时查看运行时信息");
            return;
        }

        Label($"当前有 {Target.TimeScales.Count} 个时间刻度组", false, true, true);
        Label($"<i>本地时间刻度组将不会显示在这里</i>", false, true, true);
        Space();
        foreach (var timeScale in Target.TimeScales.ToArray())
        {
            Label($"刻度组名称<b>{timeScale.Key}</b>", false, true, true);
            Label($"刻度组名称<i>{timeScale.Value.Name}</i>", false, true, true);
            Label($"刻度组事件刻度<i>{timeScale.Value.ScaleValue}</i>", false, true, true);
            Label($"刻度组订阅者数量: {timeScale.Value.LocalTimerCount}", false, true, true);
            Space();
            foreach (var timer in timeScale.Value.LocalTimerNames)
            {
                Label($"订阅者: {timer}", false, true, true);
            }
        }
    }
}