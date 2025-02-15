using System;
using System.Collections.Generic;
using System.Diagnostics;

public class TimeSystem : Singleton<TimeSystem>
{
    private DictionaryEx<int, TimerData> timers = new DictionaryEx<int, TimerData>();
    private List<Action> updateList = new List<Action>();
    private List<Action> lateUpdateList = new List<Action>();
    private List<Action> fixedUpdateList = new List<Action>();

    public TimeSystem()
    {
        stw.Start();
    }

    private float realtimeSinceStartup = 0f;
    private float realDeltaTime = 0f;

    private readonly Stopwatch stw = new Stopwatch();

    public void Update()
    {
        realDeltaTime = (float)stw.ElapsedMilliseconds / 1000 - realtimeSinceStartup;
        realtimeSinceStartup = (float)stw.ElapsedMilliseconds / 1000;

        if (updateList.Count > 0)
        {
            for (int i = 0; i < updateList.Count; i++)
            {
                Action fun = updateList[i];
                try
                {
                    fun();
                }
                catch (Exception e)
                {
                    LogSystem.Error($"run timer exception {e}");
                }
            }
        }

        if (timers.Count > 0)
        {
            timers.Iterator((id, cbd) =>
            {
                if (cbd.Interval == 0 || realtimeSinceStartup - cbd.LastTime >= cbd.Interval)
                {
                    try
                    {
                        cbd.Fun(cbd.Obj);
                    }
                    catch (Exception e)
                    {
                        LogSystem.Error($"run timer exception {e}");
                    }

                    cbd.CurRepeat++;
                    if (cbd.LifeTime != 0 && realtimeSinceStartup - cbd.StartTime >= cbd.LifeTime)
                    {
                        timers.Remove(cbd.Id);
                    }
                    else if (cbd.Repeat != 0 && cbd.CurRepeat >= cbd.Repeat)
                    {
                        timers.Remove(cbd.Id);
                    }
                    else
                    {
                        cbd.LastTime = realtimeSinceStartup;
                    }
                }
            });
        }
    }

    public void LateUpdate()
    {
        if (lateUpdateList.Count > 0)
        {
            for (int i = 0; i < lateUpdateList.Count; i++)
            {
                Action fun = lateUpdateList[i];
                try
                {
                    fun();
                }
                catch (Exception e)
                {
                    LogSystem.Error($"run timer exception {e}");
                }
            }
        }
    }

    public void FixedUpdate()
    {
        if (fixedUpdateList.Count > 0)
        {
            for (int i = 0; i < fixedUpdateList.Count; i++)
            {
                Action fun = fixedUpdateList[i];
                try
                {
                    fun();
                }
                catch (Exception e)
                {
                    LogSystem.Error($"run timer exception {e}");
                }
            }
        }
    }

    class TimerData
    {
        private static int id = 1;
        public readonly int Id;

        public Action<object> Fun;
        public float Interval = 0; //延时秒（0，则每帧都刷，但不会立即执行，而是等到下一帧）或间隔多少
        public uint Repeat = 0; //执行次数（0，则无限循环）

        public float LastTime = 0; //上一次执行的时间点
        public uint CurRepeat = 0; //当前已经执行的次数

        public float StartTime = 0; //开始执行时间
        public float LifeTime = 0; //生命周期
        public object Obj;

        public TimerData()
        {
            Id = id++;
        }
    }

    public void AddUpdate(Action fun)
    {
        if (updateList.Contains(fun))
            return;

        updateList.Add(fun);
    }

    public void AddLateUpdate(Action fun)
    {
        if (lateUpdateList.Contains(fun))
            return;

        lateUpdateList.Add(fun);
    }

    public void AddFixedUpdate(Action fun)
    {
        if (fixedUpdateList.Contains(fun))
            return;

        fixedUpdateList.Add(fun);
    }

    public void RemoveUpdate(Action fun)
    {
        updateList.Remove(fun);
    }

    public void RemoveLateUpdate(Action fun)
    {
        lateUpdateList.Remove(fun);
    }

    public void RemoveFixedUpdate(Action fun)
    {
        fixedUpdateList.Remove(fun);
    }

    public int AddTimer(Action<object> fun, object obj = null, float interval = 0, uint repeat = 1, float lifetime = 0)
    {
        TimerData cbd = new TimerData();
        cbd.Fun = fun;
        cbd.Interval = interval;
        cbd.Repeat = repeat;
        cbd.StartTime = realtimeSinceStartup;
        cbd.LifeTime = lifetime;
        cbd.Obj = obj;

        cbd.LastTime = realtimeSinceStartup;
        timers.Add(cbd.Id, cbd);

        return cbd.Id;
    }

    public int CreateDelay(float delay, Action callback)
    {
        TimerData cbd = new TimerData();

        void fun(object obj)
        {
            callback?.Invoke();
        }

        cbd.Fun = fun;
        cbd.Interval = delay;
        cbd.Repeat = 1;
        cbd.StartTime = realtimeSinceStartup;
        cbd.LifeTime = 0;
        cbd.Obj = null;

        cbd.LastTime = realtimeSinceStartup;
        timers.Add(cbd.Id, cbd);

        return cbd.Id;
    }

    public void RemoveTimer(int id)
    {
        if (timers.ContainsKey(id))
        {
            timers.Remove(id);
        }
    }
}