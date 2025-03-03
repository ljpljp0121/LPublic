using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Mono系统，通过添加事件让所有的游戏更新都在一个Update下进行，方便控制时序
/// </summary>
public class MonoSystem : MonoBehaviour
{
    private static MonoSystem instance;
    private List<Action> updateActionList = new();
    private List<Action> lateUpdateActionList = new();
    private List<Action> fixedUpdateActionList = new();
    
    private readonly ConcurrentDictionary<object, List<Coroutine>> coroutineDic = new();
    private static readonly ObjectPoolModule PoolModule = new();

    public static void Init()
    {
        instance = CoreEngineRoot.RootTransform.GetComponent<MonoSystem>();
        if (instance == null)
        {
            instance = CoreEngineRoot.RootTransform.gameObject.AddComponent<MonoSystem>();
        }
        instance.updateActionList = new List<Action>();
        instance.lateUpdateActionList = new List<Action>();
        instance.fixedUpdateActionList = new List<Action>();
    }

    #region 生命周期函数

    public static void AddUpdate(Action action)
    {
        if (!instance.updateActionList.Contains(action))
            instance.updateActionList.Add(action);
    }

    public static void RemoveUpdate(Action action)
    {
        instance.updateActionList.Remove(action);
    }

    public static void AddLateUpdate(Action action)
    {
        if (!instance.lateUpdateActionList.Contains(action))
            instance.lateUpdateActionList.Add(action);
    }

    public static void RemoveLateUpdate(Action action)
    {
        instance.lateUpdateActionList.Remove(action);
    }

    public static void AddFixedUpdate(Action action)
    {
        if (!instance.fixedUpdateActionList.Contains(action))
            instance.fixedUpdateActionList.Add(action);
    }

    public static void RemoveFixedUpdate(Action action)
    {
        instance.fixedUpdateActionList.Remove(action);
    }

    private void Update()
    {
        foreach (var action in updateActionList)
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception e)
            {
                LogSystem.Error($"Update Error: {e}");
            }
        }
    }

    private void LateUpdate()
    {
        foreach (var action in lateUpdateActionList)
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception e)
            {
                LogSystem.Error($"Update Error: {e}");
            }
        }
    }

    private void FixedUpdate()
    {
        foreach (var action in fixedUpdateActionList)
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception e)
            {
                LogSystem.Error($"Update Error: {e}");
            }
        }
    }

    #endregion

    #region 协程

    public static Coroutine BeginCoroutine(IEnumerator coroutine)
    {
        return instance.StartCoroutine(coroutine);
    }

    public static Coroutine BeginCoroutine(object obj, IEnumerator coroutine)
    {
        List<Coroutine> corList = instance.coroutineDic.GetOrAdd(obj, _ =>
            PoolModule.GetObject<List<Coroutine>>() ?? new List<Coroutine>()
        );
        Coroutine wrapperCor = null;

        IEnumerator Wrapper()
        {
            yield return instance.StartCoroutine(coroutine);
            corList.Remove(wrapperCor); // 通过闭包移除协程实例
            if (corList.Count == 0)
                instance.coroutineDic.TryRemove(obj, out _);
        }

        wrapperCor = instance.StartCoroutine(Wrapper());
        corList.Add(wrapperCor);

        return wrapperCor;
    }

    /// <summary>
    /// 停止一个协程序并基于某个对象
    /// </summary>
    public static void EndCoroutine(object obj, Coroutine routine)
    {
        if (instance.coroutineDic.TryRemove(obj, out var list))
        {
            foreach (var cor in list) instance.StopCoroutine(cor);
            list.Clear();
            PoolModule.PushObject(list);
        }
    }

    /// <summary>
    /// 停止一个协程序
    /// </summary>
    public static void EndCoroutine(Coroutine routine)
    {
        instance.StopCoroutine(routine);
    }

    /// <summary>
    /// 停止某个对象的全部协程
    /// </summary>
    public static void StopAllCoroutine(object obj)
    {
        if (instance.coroutineDic.Remove(obj, out List<Coroutine> _coroutineList))
        {
            for (int i = 0; i < _coroutineList.Count; i++)
            {
                instance.StopCoroutine(_coroutineList[i]);
            }
            _coroutineList.Clear();
            PoolModule.PushObject(_coroutineList);
        }
    }

    /// <summary>
    /// 整个系统全部协程都会停止
    /// </summary>
    public static void StopAllCoroutine()
    {
        // 全部数据都会无效
        foreach (List<Coroutine> _item in instance.coroutineDic.Values)
        {
            _item.Clear();
            PoolModule.PushObject(_item);
        }
        instance.coroutineDic.Clear();
        instance.StopAllCoroutines();
    }

    #endregion
}