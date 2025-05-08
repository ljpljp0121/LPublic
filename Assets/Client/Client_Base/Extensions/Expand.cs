

using System;
using UnityEngine;
using Object = UnityEngine.Object;

public static class Expand
{
    public static void ThrowException(this string reason)
    {
        throw new System.Exception("ET stop running because " + reason);
    }

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

    public static Transform FindNode(this Transform t, params string[] nodes)
    {
        var curt = t;
        for (int i = 0; i < nodes.Length; i++)
        {
            string node = nodes[i];
            curt = curt.Find(node);
            if (curt == null) $"The node '{node}' is not found".ThrowException();
        }
        return curt;
    }

    public static T FindComponent<T>(this Transform t, params string[] nodes) where T : Component
    {
        var child = t.FindNode(nodes);
        if (child.TryGetComponent<T>(out var component)) return component;
        return child.gameObject.AddComponent<T>();
    }

    public static T ClearChildrenExceptFirst<T>(this Transform t) where T : Component
    {
        var children = new Transform[t.childCount];
        for (var i = 0; i < t.childCount; i++)
            children[i] = t.GetChild(i);

        for (var i = 1; i < children.Length; i++)
        {
            children[i].gameObject.SetActive(false);
            Object.Destroy(children[i].gameObject);
        }

        var child = children[0];
        child.gameObject.SetActive(false);
        return child.FindComponent<T>();
    }
}