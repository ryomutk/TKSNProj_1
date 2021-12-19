using System.Collections.Generic;
using UnityEngine;

public abstract class ScriptableEventBase<T> :ScriptableObject,IEvent<T>
where T:class,IEventArg
{
    protected List<IEventListener<T>> eventListeners = new List<IEventListener<T>>();
    public ITask Notice(IEventArg arg)
    {
        return Notice(arg as T);
    }
    public int listenerCount { get{return eventListeners.Count;} }
    public void Register(IEventListener<T> listener)
    {
        eventListeners.Add(listener);
    }
    public bool DisRegister(IEventListener<T> listener)
    {
        return eventListeners.Remove(listener);
    }
    public void Register(IEventListener listener)
    {
        eventListeners.Add(listener as IEventListener<T>);
    }
    public bool DisRegister(IEventListener listener)
    {
        return eventListeners.Remove(listener as IEventListener<T>);
    }
    public abstract ITask Notice(T arg);
}