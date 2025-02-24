

using UnityEngine;

public static class Expand
{
    /// <summary>
    /// 初始化Transform（Position、Rotation、Scale初始化）
    /// </summary>
    /// <param name="ts"></param>
    public static void Reset(this Transform ts)
    {
        ts.localPosition = Vector3.zero;
        ts.localRotation = Quaternion.identity;
        ts.localScale = Vector3.one;
    }
    
    /// <summary>
    /// 获取Component
    /// </summary>
    /// <param name="go"></param>
    /// <param name="creat">如果不存在是否新创建</param>
    /// <returns></returns>
    public static T GetComponent<T>(this GameObject go, bool creat) where T : Component
    {
        T t = go.GetComponent<T>();
        if (t == null && creat)
        {
            t = go.AddComponent<T>();
        }
        return t;
    }
}