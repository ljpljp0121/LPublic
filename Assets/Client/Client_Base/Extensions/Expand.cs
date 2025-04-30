

using System;
using UnityEngine;

public static class Expand
{

    public static void Reset(this Transform ts)
    {
        ts.localPosition = Vector3.zero;
        ts.localRotation = Quaternion.identity;
        ts.localScale = Vector3.one;
    }

    public static T GetComponent<T>(this GameObject go, bool creat) where T : Component
    {
        T t = go.GetComponent<T>();
        if (t == null && creat)
        {
            t = go.AddComponent<T>();
        }
        return t;
    }

    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        T t = gameObject.GetComponent<T>();
        if (t == null)
        {
            t = gameObject.AddComponent<T>();
        }
        return t;
    }

    public static Component GetOrAddComponent(this GameObject gameObject, Type type)
    {
        Component component = gameObject.GetComponent(type);
        if (component == null)
        {
            component = gameObject.AddComponent(type);
        }

        return component;
    }
}