using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Mono系统，通过添加事件让所有的游戏更新都在一个Update下进行，方便控制时序
/// </summary>
public class MonoSystem : MonoBehaviour
{
    private static MonoSystem instance;
    private Action updateEvent;
    private Action lateUpdateEvent;
    private Action fixedUpdateEvent;

    public static void Init()
    {
        instance = CoreEngineRoot.RootTransform.GetComponent<MonoSystem>();
        if (instance == null)
        {
            instance = CoreEngineRoot.RootTransform.gameObject.AddComponent<MonoSystem>();
        }
        instance.updateEvent = null;
        instance.lateUpdateEvent = null;
        instance.fixedUpdateEvent = null;
    }

    #region 生命周期函数

    public static void AddUpdate(Action action)
    {
        instance.updateEvent += action;
    }

    public static void RemoveUpdate(Action action)
    {
        instance.updateEvent -= action;
    }

    public static void AddLateUpdate(Action action)
    {
        instance.lateUpdateEvent += action;
    }

    public static void RemoveLateUpdate(Action action)
    {
        instance.lateUpdateEvent -= action;
    }

    public static void AddFixedUpdate(Action action)
    {
        instance.fixedUpdateEvent += action;
    }

    public static void RemoveFixedUpdate(Action action)
    {
        instance.fixedUpdateEvent -= action;
    }

    private void Update()
    {
        updateEvent?.Invoke();
    }

    private void LateUpdate()
    {
        lateUpdateEvent?.Invoke();
    }

    private void FixedUpdate()
    {
        fixedUpdateEvent?.Invoke();
    }

    #endregion

    #region 协程

    private Dictionary<object, List<Coroutine>> coroutineDic = new Dictionary<object, List<Coroutine>>();
    private static ObjectPoolModule poolModule = new ObjectPoolModule();

    public static Coroutine BeginCoroutine(IEnumerator coroutine)
    {
        return instance.StartCoroutine(coroutine);
    }

    public static Coroutine BeginCoroutine(object obj, IEnumerator coroutine)
    {
        Coroutine _coroutine = instance.StartCoroutine(coroutine);
        if (!instance.coroutineDic.TryGetValue(obj, out List<Coroutine> _coroutineList))
        {
            _coroutineList = poolModule.GetObject<List<Coroutine>>();
            if (_coroutineList == null) _coroutineList = new List<Coroutine>();
            instance.coroutineDic.Add(obj, _coroutineList);
        }
        _coroutineList.Add(_coroutine);
        return _coroutine;
    }

    /// <summary>
    /// 停止一个协程序并基于某个对象
    /// </summary>
    public static void EndCoroutine(object obj, Coroutine routine)
    {
        if (instance.coroutineDic.TryGetValue(obj, out List<Coroutine> _coroutineList))
        {
            instance.StopCoroutine(routine);
            _coroutineList.Remove(routine);
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
            poolModule.PushObject(_coroutineList);
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
            poolModule.PushObject(_item);
        }
        instance.coroutineDic.Clear();
        instance.StopAllCoroutines();
    }

    #endregion
}