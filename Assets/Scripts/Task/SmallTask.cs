using System;
using UnityEngine;

public class SmallTask<T> : ITask<T>
where T : class
{
    public bool compleated { get { return conditionGetter(); } }
    Func<bool> conditionGetter;
    public T result { get { return resultGetter(); } }

    Func<T> resultGetter;


    /// <summary>
    /// 
    /// </summary>
    /// <param name="conditionGetter">readyになる条件。デフォルトはresult != null</param>
    public SmallTask(Func<bool> conditionGetter, Func<T> resultGetter)
    {
        this.conditionGetter = conditionGetter;
        this.resultGetter = resultGetter;
    }
}

public class SmallTask : ITask
{
    static SmallTask _nullTask = new SmallTask();
    public static SmallTask nullTask
    {
        get
        {
            if (!_nullTask.compleated)
            {
                _nullTask.compleated = true;
            }
            return _nullTask;
        }
    }
    bool _ready;
    public virtual bool compleated
    {
        get
        {
            return _ready;
        }
        set
        {
            _ready = value;
        }
    }
    public SmallTask()
    {
        compleated = false;
    }
}