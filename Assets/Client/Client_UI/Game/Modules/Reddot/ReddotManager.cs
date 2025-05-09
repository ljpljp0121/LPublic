using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ReddotManager
{
    [InitOnLoad]
    static void Init()
    {
        //定义红点关系

    }

    //红点关系字典,因为红点只可能有一个父节点,所以Key为子节点,Value为父节点
    public static Dictionary<EReddotId, EReddotId> Parents = new();
    //红点刷新字典,按EReddotSource来刷新旗下的所有红点
    public static Dictionary<EReddotSource, List<EReddotId>> Refreshes = new();
    public static Dictionary<EReddotId, Func<int, int>> CalReddotCountFunc = new();

    /// <summary>
    /// 添加红点刷新关系。将id添加到source的刷新列表中。
    /// 红点刷新都是按照EReddotSource来刷新的。
    /// </summary>
    static void AddRefresh(EReddotId id, EReddotSource source)
    {
        if (!Refreshes.TryGetValue(source, out var list))
        {
            list = new List<EReddotId>();
            Refreshes[source] = list;
        }
        list.Add(id);
    }

    public static int GetReddot(EReddotId id, int param)
    {
        int count = 0;
        Func<int, int> func;
        if (CalReddotCountFunc.TryGetValue(id, out func))
        {
            count += func(param);
        }
        foreach (var parent in Parents)
        {
            if (parent.Value == id)
            {
                count += GetReddot(parent.Key, param);
            }
        }
        return count;
    }

    public static bool IsFather(EReddotId son, EReddotId father)
    {
        if (Parents.TryGetValue(son, out var p))
        {
            if (p == father)
                return true;
            else
                return IsFather(p, father);
        }

        return false;
    }
}

public class Reddot : MonoBehaviour
{
    public GameObject RedDotObj;
    public EReddotId Id;
    public TMP_Text NumText;
    public int ReddotParam;
    public int CountLimit;

    private int count;

    void OnEnable()
    {
        EventSystem.RegisterEvent<E_RefreshReddot>(OnRefreshReddot);
    }

    void OnDisable()
    {
        EventSystem.RemoveEvent<E_RefreshReddot>(OnRefreshReddot);
    }

    private int GetCount()
    {
        return count + ReddotManager.GetReddot(Id, ReddotParam);
    }

    private void Refresh()
    {
        int num = GetCount();
        if (num == 0)
        {
            if (RedDotObj != null) RedDotObj.SetActive(false);
            if (NumText != null) NumText.gameObject.SetActive(false);
        }
        else
        {
            if (RedDotObj != null) RedDotObj.SetActive(true);
            if (NumText != null)
            {
                if (CountLimit > 1 && count > CountLimit)
                    NumText.text = CountLimit.ToString() + "+";
                else
                    NumText.text = count.ToString();
                NumText.gameObject.SetActive(true);
            }
        }
    }

    private void OnRefreshReddot(E_RefreshReddot evt)
    {
        if (ReddotManager.Refreshes.TryGetValue((EReddotSource)evt.Source, out var ids))
        {
            if (ids.Contains(Id))
                Refresh();
            else
            {
                foreach (var id in ids)
                {
                    if (ReddotManager.IsFather(id, Id))
                    {
                        Refresh();
                        break;
                    }
                }
            }
        }
    }

    public void SetId(int id)
    {
        Id = (EReddotId)id;
        Refresh();
    }

    public void SetCount(int count)
    {
        this.count = count;
        Refresh();
    }

    public void SetParam(int param)
    {
        this.ReddotParam = param;
        Refresh();
    }
}

public class E_RefreshReddot : BaseEvent
{
    public int Source;

    public E_RefreshReddot(int source)
    {
        Source = source;
    }

    public E_RefreshReddot(EReddotSource source)
    {
        Source = (int)source;
    }
}