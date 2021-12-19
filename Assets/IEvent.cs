
public interface IEvent
{
    ITask Notice(IEventArg arg);
    int listenerCount{get;}
    void Register(IEventListener listener);
    bool DisRegister(IEventListener listener);
}


public interface IEvent<T>:IEvent
{
    void Register(IEventListener<T> listener);
    bool DisRegister(IEventListener<T> listener);
    ITask Notice(T arg);
}