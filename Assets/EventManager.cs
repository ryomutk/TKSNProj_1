using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using System.Collections;

public class EventManager : Singleton<EventManager>
{
    [SerializeField] SerializableDictionary<EventName, AssetLabelReference> eventLabelTable = new SerializableDictionary<EventName, AssetLabelReference>();
    Dictionary<EventName, IEvent> eventTable = new Dictionary<EventName, IEvent>();

    public ITask Notice(EventName name)
    {
        if (eventTable.TryGetValue(name, out var eve))
        {
            return eve.Notice();
        }

#if DEBUG
        Debug.LogWarning("no one is listening" + name);
#endif

        return SmallTask.nullTask;
    }

    public ITask Notice<T>(EventName name, T arg)
    where T : IEventArg
    {
        if (eventTable.TryGetValue(name, out var eve))
        {
            var teve = eve as IEvent<T>;
            if (teve == null)
            {
                Debug.LogError(name + " is not event of arg " + typeof(T) + "!" + "(" + eve.GetType() + ")");
            }
            
            return teve.Notice(arg);
        }

#if DEBUG
        Debug.LogWarning("no one is listening" + name);
#endif

        return SmallTask.nullTask;
    }

    public ITask Register(IEventListener listener, EventName eventName)
    {
        if (eventTable.TryGetValue(eventName, out IEvent ev))
        {
            ev.Register(listener);
            return SmallTask.nullTask;
        }

        var task = new SmallTask();

        StartCoroutine(RegisterRoutine(task, eventName, listener));

        return task;
    }

    public ITask Register<T>(IEventListener<T> listener, EventName eventName)
    where T : IEventArg
    {
        try
        {
            if (eventTable.TryGetValue(eventName, out IEvent ev))
            {
                var tev = ev as IEvent<T>;
                tev.Register(listener);
                return SmallTask.nullTask;
            }

            var task = new SmallTask();

            StartCoroutine(RegisterRoutine(task, eventName, listener));

            return task;
        }
        catch (System.NullReferenceException)
        {
            var ev = eventTable[eventName];

            throw new System.Exception(ev + " is not Event of type " + typeof(T));
        }
    }


    public bool Disregister<T>(IEventListener<T> listener, EventName name)
    where T : IEventArg
    {
        var res = eventTable.TryGetValue(name, out IEvent ev);

        if (res)
        {
            var eve = ev as IEvent<T>;
            var result = eve.DisRegister(listener);
            if (eve.listenerCount == 0)
            {
                ReleaseEvent(name);
            }

            return result;
        }

#if DEBUG
        Debug.LogWarning("event not registered");
#endif

        return false;
    }

    public bool Disregister(IEventListener listener, EventName name)
    {
        eventTable.TryGetValue(name, out var eve);
        if (eve != null)
        {
            var result = eve.DisRegister(listener);
            if (eve.listenerCount == 0)
            {
                ReleaseEvent(name);
            }

            return result;
        }

#if DEBUG
        Debug.LogWarning("event is null");
#endif

        return false;
    }

    void ReleaseEvent(EventName name)
    {
        var eve = eventTable[name];
        DataManager.ReleaseData(eve);
        eventTable.Remove(name);
    }


    IEnumerator RegisterRoutine(SmallTask task, EventName name, IEventListener listener)
    {
        yield return StartCoroutine(LoadEvent(name));
        var eve = eventTable[name] as IEvent;
        eve.Register(listener);
        task.compleated = true;
    }

    IEnumerator RegisterRoutine<T>(SmallTask task, EventName name, IEventListener<T> listener)
    where T : IEventArg
    {
        yield return StartCoroutine(LoadEvent(name));

        var eve = eventTable[name] as IEvent<T>;
        eve.Register(listener);
        task.compleated = true;
    }

    List<EventName> nowLoading = new List<EventName>();

    IEnumerator LoadEvent(EventName name)
    {
        if (eventTable.ContainsKey(name))
        {
            yield break;
        }

        //すでにロードが始まっている場合
        if (nowLoading.Contains(name))
        {
            //NowLoadingがなくなるまでやる
            yield return new WaitWhile(() => nowLoading.Contains(name));
            yield break;
        }
        else
        {
            nowLoading.Add(name);
        }

        var label = eventLabelTable[name];

        var loadtask = DataManager.LoadDataAsync(label);
        yield return new WaitUntil(() => loadtask.compleated);

        if (eventTable.ContainsKey(name))
        {
            //同時に呼ばれてしまった場合。
            DataManager.ReleaseData(loadtask.result);
        }
        else
        {
            eventTable[name] = loadtask.result as IEvent;
        }
        nowLoading.Remove(name);
    }
}