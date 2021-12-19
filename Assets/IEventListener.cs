public interface IEventListener
{
    ITask OnNotice();
}

/// <summary>
/// 何かしらの処理をしたらtrueを返す。
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IEventListener<T>
{
    ITask OnNotice(T arg);
}