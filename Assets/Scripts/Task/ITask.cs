public interface ITask
{
    bool compleated{get;}
}

public interface ITask<T>:ITask
{
    T result{get;}
}