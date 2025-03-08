using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public static class CoreEngineExtension
{
    #region GameObjet

    public static bool IsNull(this GameObject obj)
    {
        return ReferenceEquals(obj, null);
    }

    #endregion

    #region 资源管理

    /// <summary>
    /// GameObject放入对象池
    /// </summary>
    public static void GameObjectPushPool(this GameObject go)
    {
        if (go.IsNull())
        {
            LogSystem.Error("将空物体放入对象池");
        }
        else
        {
            PoolSystem.PushGameObject(go);
        }
    }

    public static void GameObjectPushPool(this GameObject go, string poolName)
    {
        if (go.IsNull())
        {
            LogSystem.Error("将空物体放入对象池");
        }
        else
        {
            PoolSystem.PushGameObject(poolName, go);
        }
    }

    /// <summary>
    /// GameObject放入对象池
    /// </summary>
    public static void GameObjectPushPool(this Component com)
    {
        GameObjectPushPool(com.gameObject);
    }

    /// <summary>
    /// 普通类放进池子
    /// </summary>
    public static void ObjectPushPool(this object obj)
    {
        PoolSystem.PushObject(obj);
    }

    #endregion

    #region Mono

    public static void AddUpdate(this object obj, Action action)
    {
        MonoSystem.AddUpdate(action);
    }


    public static void RemoveUpdate(this object obj, Action action)
    {
        MonoSystem.RemoveUpdate(action);
    }

    public static void AddLateUpdate(this object obj, Action action)
    {
        MonoSystem.AddLateUpdate(action);
    }

    public static void RemoveLateUpdate(this object obj, Action action)
    {
        MonoSystem.RemoveLateUpdate(action);
    }


    public static void AddFixedUpdate(this object obj, Action action)
    {
        MonoSystem.AddFixedUpdate(action);
    }

    public static void RemoveFixedUpdate(this object obj, Action action)
    {
        MonoSystem.RemoveFixedUpdate(action);
    }

    public static Coroutine StartCoroutine(this object obj, IEnumerator routine)
    {
        return MonoSystem.BeginCoroutine(obj, routine);
    }

    public static void StopCoroutine(this object obj, Coroutine routine)
    {
        MonoSystem.EndCoroutine(obj, routine);
    }

    /// <summary>
    /// 关闭全部协程，注意只会关闭调用对象所属的协程
    /// </summary>
    /// <param name="obj"></param>
    public static void StopAllCoroutine(this object obj)
    {
        MonoSystem.StopAllCoroutine(obj);
    }

    #endregion

    #region Unity通用拓展

    public static void BroadcastMessage<T>(this GameObject obj, string message) where T : MonoBehaviour
    {
        T[] tList = obj.GetComponentsInChildren<T>(true);
        foreach (var t in tList)
        {
            var func = t.GetType().GetMethod(message,
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);
            if (func != null)
            {
                func.Invoke(t, null);
            }
        }
    }

    #endregion
}