public interface IEventListener
{
    ITask OnNotice(IEventArg arg);
}


public interface IEventListener<T>:IEventListener
{
    ITask OnNotice(T arg);
}