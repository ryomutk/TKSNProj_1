using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine.AddressableAssets;
using System.Linq;

/// <summary>
/// 各種データを集積し、DataManagerに渡す人
/// </summary>
/// <typeparam name="T"></typeparam>
public static class DataManager
{
    public interface IAssetContainer
    {
        void Release();
        T FindByName<T>(string name, bool exact=false) where T : Object;
    }
    //IListをそのまま渡して改編やリリース漏れがおこることを防ぐためのもの。
    public class AssetsContainer : IAssetContainer
    {
        public AssetsContainer(IList<object> assets)
        {
            this._assets = assets;
            this.assets = (assets as List<object>).AsReadOnly();
        }
        public ReadOnlyCollection<object> assets { get; }
        IList<object> _assets { get; }
        public T FindByName<T>(string name, bool exact)
        where T : Object
        {
            foreach (T item in _assets)
            {
                if (!exact && item.name.Contains(name))
                {
                    return item;
                }
                else if (exact && item.name == name)
                {
                    return item;
                }
            }

#if DEBUG
            Debug.LogWarning("Object" + name + " not found");
#endif   
            return null;
        }
        public void Release()
        {
            DataManager.ReleaseDatas(assets);
        }
    }

    public class AssetsContainer<T> : IAssetContainer
    where T : Object
    {
        public AssetsContainer(IList<T> assets)
        {
            this._assets = assets;
            this.assets = (assets as List<T>).AsReadOnly();
        }
        public ReadOnlyCollection<T> assets { get; }
        IList<T> _assets { get; }
        public void Release()
        {
            DataManager.ReleaseDatas(assets);
        }

        public M FindByName<M>(string name, bool exact = false)
        where M : Object
        {
            if (typeof(M) == typeof(T))
            {
                return FindByName(name, exact) as M;
            }
#if DEBUG
            Debug.LogWarning("Object" + name + " not found");
#endif   
            return null;
        }

        public T FindByName(string name, bool exact = false)
        {
            if (!exact)
            {
                return _assets.First(x => x.name.Contains(name));
            }
            else
            {
                return _assets.First(x => x.name == name);
            }
        }
    }

    public static ITask<AssetsContainer<T>> LoadDatasAsync<T>(AssetLabelReference label,System.Action onCompleate = null)
    where T : Object
    {

        var loadTask = Addressables.LoadAssetsAsync<T>(label.labelString, (x) => { });

        var task = new SmallTask<AssetsContainer<T>>(
            () => loadTask.IsDone,
            () =>
            {
                return new AssetsContainer<T>(loadTask.Result);
            });

        return task;
    }

    /// <summary>
    /// object型でソースをロード！
    /// 非推奨。できるだけジェネリックのほうを使ってください
    /// </summary>
    /// <param name="label"></param>
    /// <returns></returns>
    public static ITask<AssetsContainer> LoadDatasAsync(AssetLabelReference label)
    {

        var loadTask = Addressables.LoadAssetsAsync<object>(label.labelString, (x) => { });

        var task = new SmallTask<AssetsContainer>(
            () => loadTask.IsDone,
            () =>
            {
                return new AssetsContainer(loadTask.Result);
            });

        return task;
    }

    public static ITask<T> LoadDataAsync<T>(AssetLabelReference label)
    where T : class
    {
        var loadTask = Addressables.LoadAssetAsync<T>(label.labelString);

        var task = new SmallTask<T>(
            () => loadTask.IsDone,
            () =>
            {
                return loadTask.Result;
            });


        return task;
    }

    public static ITask<object> LoadDataAsync(AssetLabelReference label)
    {
        var loadTask = Addressables.LoadAssetAsync<object>(label.labelString);

        var task = new SmallTask<object>(
            () => loadTask.IsDone,
            () =>
            {
                return loadTask.Result;
            });


        return task;
    }

    /*
        static IEnumerator FetchData(string label, SmallTask<DataIndexer> task)
        {
            var loadTask = Addressables.LoadAssetsAsync<ISalvageData>(label, (x) => { });

            yield return new WaitUntil(() => loadTask.IsDone);

            var result = new DataIndexer((List<ISalvageData>)loadTask.Result, true);

            task.result = result;
        }
    */
    static void ReleaseDatas(IList<object> datas)
    {
        /*
        foreach (var data in datas)
        {
            Addressables.Release<ISalvageData>(data);
        }
        */
        Addressables.Release<IList<object>>(datas);

        datas = null;
    }

    static void ReleaseDatas<T>(IList<T> datas)
    {
        Addressables.Release<IList<T>>(datas);

        datas = null;
    }

    static public void ReleaseData(object data)
    {
        Addressables.Release<object>(data);
        data = null;
    }
}