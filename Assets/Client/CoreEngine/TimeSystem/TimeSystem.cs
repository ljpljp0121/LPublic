using System;
using System.Collections.Generic;
using System.Diagnostics;

public static class TimeSystem
{
    private static readonly DictionaryEx<int, TimerData> Timers = new DictionaryEx<int, TimerData>();
    private static readonly List<Action> UpdateList = new List<Action>();
    private static readonly List<Action> LateUpdateList = new List<Action>();
    private static readonly List<Action> FixedUpdateList = new List<Action>();

    static TimeSystem()
    {
        stw.Start();
    }

    public static void Init()
    {
        MonoSystem.AddUpdate(Update, UpdatePriority.Critical);
        MonoSystem.AddLateUpdate(LateUpdate, UpdatePriority.Critical);
        MonoSystem.AddFixedUpdate(FixedUpdate, UpdatePriority.Critical);
    }

    private static float realtimeSinceStartup = 0f;
    private static float realDeltaTime = 0f;

    private static readonly Stopwatch stw = new Stopwatch();

    public static void Update()
    {
        realDeltaTime = (float)stw.ElapsedMilliseconds / 1000 - realtimeSinceStartup;
        realtimeSinceStartup = (float)stw.ElapsedMilliseconds / 1000;

        if (UpdateList.Count > 0)
        {
            for (int i = 0; i < UpdateList.Count; i++)
            {
                Action fun = UpdateList[i];
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

        if (Timers.Count > 0)
        {
            Timers.Iterator((id, cbd) =>
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
                        Timers.Remove(cbd.Id);
                    }
                    else if (cbd.Repeat != 0 && cbd.CurRepeat >= cbd.Repeat)
                    {
                        Timers.Remove(cbd.Id);
                    }
                    else
                    {
                        cbd.LastTime = realtimeSinceStartup;
                    }
                }
            });
        }
    }

    public static void LateUpdate()
    {
        if (LateUpdateList.Count > 0)
        {
            for (int i = 0; i < LateUpdateList.Count; i++)
            {
                Action fun = LateUpdateList[i];
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

    public static void FixedUpdate()
    {
        if (FixedUpdateList.Count > 0)
        {
            for (int i = 0; i < FixedUpdateList.Count; i++)
            {
                Action fun = FixedUpdateList[i];
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

    public static void AddUpdate(Action fun)
    {
        if (UpdateList.Contains(fun))
            return;

        UpdateList.Add(fun);
    }

    public static void AddLateUpdate(Action fun)
    {
        if (LateUpdateList.Contains(fun))
            return;

        LateUpdateList.Add(fun);
    }

    public static void AddFixedUpdate(Action fun)
    {
        if (FixedUpdateList.Contains(fun))
            return;

        FixedUpdateList.Add(fun);
    }

    public static void RemoveUpdate(Action fun)
    {
        UpdateList.Remove(fun);
    }

    public static void RemoveLateUpdate(Action fun)
    {
        LateUpdateList.Remove(fun);
    }

    public static void RemoveFixedUpdate(Action fun)
    {
        FixedUpdateList.Remove(fun);
    }

    public static int AddTimer(Action<object> fun, object obj = null, float interval = 0, uint repeat = 1, float lifetime = 0)
    {
        TimerData cbd = new TimerData();
        cbd.Fun = fun;
        cbd.Interval = interval;
        cbd.Repeat = repeat;
        cbd.StartTime = realtimeSinceStartup;
        cbd.LifeTime = lifetime;
        cbd.Obj = obj;

        cbd.LastTime = realtimeSinceStartup;
        Timers.Add(cbd.Id, cbd);

        return cbd.Id;
    }

    public static int CreateDelay(float delay, Action callback)
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
        Timers.Add(cbd.Id, cbd);

        return cbd.Id;
    }

    public static void RemoveTimer(int id)
    {
        if (Timers.ContainsKey(id))
        {
            Timers.Remove(id);
        }
    }
}