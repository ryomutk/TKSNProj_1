using System;
using UnityEngine;

public class TaskBase : ITask
{
    public bool compleated
    {
        get {return endChecker();}
        set {Debug.LogWarning("This is not small task");}
    }

    Func<bool> endChecker;

    public TaskBase(Func<bool> endChecker)
    {
        this.endChecker = endChecker;
    }
}