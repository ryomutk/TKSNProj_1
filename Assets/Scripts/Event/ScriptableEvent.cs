using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu]
public class ScriptableEvent<T> : ScriptableEventBase<T>
where T:class,IEventArg
{
    /// <summary>
    /// Notice event to listeners
    /// </summary>
    /// <returns>true count</returns>
    public override ITask Notice(T arg)
    {
        List<ITask> tasks = null;
        for (int i = 0; i < eventListeners.Count; i++)
        {
            var task = eventListeners[i].OnNotice(arg);
            //待つ必要のあるものだけ待つ
            if (!task.compleated)
            {
                if (tasks != null)
                {
                    tasks = new List<ITask>();
                }

                tasks.Add(task);
            }
        }

        if (tasks != null)
        {
            return SmallTask.nullTask;
        }
        else
        {
            return new TaskBase(() =>
            {
                for (int i = 0; i < tasks.Count; i++)
                {
                    if (!tasks[i].compleated)
                    {
                        return false;
                    }
                }

                return true;
            });
        }
    }

    void OnDisable()
    {
        eventListeners.Clear();
    }

}
/*
public class SalvageEvent<T> : SalvageEvent, ISalvageData, IEvent<T>
where T : SalvageEventArg
{
    public override int listeners { get { return base.listeners + registrations.Count; } }
    [ShowInInspector, ReadOnly] List<IEventListener<T>> registrations = new List<IEventListener<T>>();
    public void Register(IEventListener<T> fukidashi)
    {
        registrations.Add(fukidashi);
    }

    public bool DisRegister(IEventListener<T> fukidashi)
    {
        return registrations.Remove(fukidashi);
    }

    /// <summary>
    /// りすなーさんたちにargをnotice!
    /// </summary>
    /// <param name="arg">あーぎゅめんと</param>
    /// <returns>true count,なんかしたらtrueを返す予定</returns>
    public ITask Notice(T arg)
    {
        var task = base.Notice();
        List<ITask> tasks = null;
        for (int i = 0; i < registrations.Count; i++)
        {
            var noticeTask = registrations[i].OnNotice(arg);

            if (!noticeTask.compleated)
            {
                if (tasks == null)
                {
                    tasks = new List<ITask>();
                }
                tasks.Add(noticeTask);
            }
        }
        
        //実質的なTaskが一つもなければListも生成しない新設設計
        if (tasks == null)
        {
            return SmallTask.nullTask;
        }
        else
        {
            return new TaskBase(() =>
            {
                for(int i = 0;i<tasks.Count;i++)
                {
                    if(!tasks[i].compleated)
                    {
                        return false;
                    }
                }

                return true;
            });
        }
    }

    //事故防止のために使えなくしちゃう
    public override ITask Notice()
    {
        throw new System.Exception("This is Generic Event. Use Notice<T> otherwise");
    }

    void OnDisable()
    {
        Debug.Log("Cleared");
        registrations.Clear();
    }

}
*/