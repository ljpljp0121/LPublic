using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum UpdatePriority
{
    Critical, //每帧执行
    High, //每帧执行
    Medium, //3帧执行
    Low  //5帧执行
}

/// <summary>
/// Mono系统，通过添加事件让所有的游戏更新都在一个Update下进行，方便控制时序
/// </summary>
public class MonoSystem : MonoBehaviour
{
    private static MonoSystem instance;

    private readonly List<Action>[] updateActionLists = new List<Action>[4];
    private readonly List<Action>[] lateUpdateActionLists = new List<Action>[4];
    private readonly List<Action>[] fixedUpdateActionLists = new List<Action>[4];
    private int frameCount;
    private int fixedFrameCount;
    private readonly bool[] shouldExecuteUpdate = new bool[4];
    private readonly bool[] shouldExecuteLateUpdate = new bool[4];
    private readonly bool[] shouldExecuteFixedUpdate = new bool[4];

    private readonly ConcurrentDictionary<object, List<Coroutine>> coroutineDic = new();
    private static readonly ObjectPoolModule PoolModule = new();

    public static void Init()
    {
        instance = CoreEngineRoot.RootTransform.GetComponent<MonoSystem>();
        if (instance == null)
        {
            instance = CoreEngineRoot.RootTransform.gameObject.AddComponent<MonoSystem>();
        }
        for (int i = 0; i < 4; i++)
        {
            instance.updateActionLists[i] = new List<Action>();
            instance.lateUpdateActionLists[i] = new List<Action>();
            instance.fixedUpdateActionLists[i] = new List<Action>();
        }
    }

    #region 生命周期函数

    public static void AddUpdate(Action action, UpdatePriority priority = UpdatePriority.High)
    {
        instance.updateActionLists[(int)priority].Add(action);
    }

    public static void RemoveUpdate(Action action)
    {
        foreach (var list in instance.updateActionLists)
        {
            if (list.Remove(action))
                break;
        }
    }

    public static void AddLateUpdate(Action action, UpdatePriority priority = UpdatePriority.High)
    {
        instance.lateUpdateActionLists[(int)priority].Add(action);
    }

    public static void RemoveLateUpdate(Action action)
    {
        foreach (var list in instance.lateUpdateActionLists)
        {
            if (list.Remove(action))
                break;
        }
    }

    public static void AddFixedUpdate(Action action, UpdatePriority priority = UpdatePriority.High)
    {
        instance.fixedUpdateActionLists[(int)priority].Add(action);
    }

    public static void RemoveFixedUpdate(Action action)
    {
        foreach (var list in instance.fixedUpdateActionLists)
        {
            if (list.Remove(action))
                break;
        }

    }

    private void Update()
    {
        frameCount = Time.frameCount;
        shouldExecuteUpdate[(int)UpdatePriority.Critical] = true;
        shouldExecuteUpdate[(int)UpdatePriority.High] = true;
        shouldExecuteUpdate[(int)UpdatePriority.Medium] = frameCount % 3 == 0;
        shouldExecuteUpdate[(int)UpdatePriority.Low] = frameCount % 5 == 0;
        ExecuteActions(updateActionLists, shouldExecuteUpdate);
    }

    private void LateUpdate()
    {
        shouldExecuteLateUpdate[(int)UpdatePriority.Critical] = true;
        shouldExecuteLateUpdate[(int)UpdatePriority.High] = true;
        shouldExecuteLateUpdate[(int)UpdatePriority.Medium] = frameCount % 3 == 0;
        shouldExecuteLateUpdate[(int)UpdatePriority.Low] = frameCount % 5 == 0;
        ExecuteActions(lateUpdateActionLists, shouldExecuteLateUpdate);
    }

    private void FixedUpdate()
    {
        fixedFrameCount++;
        shouldExecuteFixedUpdate[(int)UpdatePriority.Critical] = true;
        shouldExecuteFixedUpdate[(int)UpdatePriority.High] = true;
        shouldExecuteFixedUpdate[(int)UpdatePriority.Medium] = fixedFrameCount % 3 == 0;
        shouldExecuteFixedUpdate[(int)UpdatePriority.Low] = fixedFrameCount % 5 == 0;
        ExecuteActions(fixedUpdateActionLists, shouldExecuteFixedUpdate);
    }

    private void ExecuteActions(List<Action>[] actionLists, bool[] shouldExecute)
    {
        for (int i = 0; i < actionLists.Length; i++)
        {
            if (!shouldExecute[i]) continue;
            var actions = actionLists[i];
            for (int j = 0; j < actions.Count; j++)
            {
                try { actions[j]?.Invoke(); }
                catch (Exception e) { Debug.LogError(e); }
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