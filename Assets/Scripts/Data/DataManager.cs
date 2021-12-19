using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine.AddressableAssets;

/// <summary>
/// 各種データを集積し、DataManagerに渡す人
/// </summary>
/// <typeparam name="T"></typeparam>
public static class DataManager
{
    //IListをそのまま渡して改編やリリース漏れがおこることを防ぐためのもの。
    public class AssetsContainer
    {
        public AssetsContainer(IList<object> assets)
        {
            this._assets = assets;
            this.assets = (assets as List<object>).AsReadOnly();
        }
        public ReadOnlyCollection<object> assets{get;}
        IList<object> _assets { get; }
        public void Release()
        {
            DataManager.ReleaseDatas(assets);
        }
    }

    public class AssetsContainer<T>
    {
        public AssetsContainer(IList<T> assets)
        {
            this._assets = assets;
            this.assets = (assets as List<T>).AsReadOnly();
        }
        public ReadOnlyCollection<T> assets{get;}
        IList<T> _assets { get; }
        public void Release()
        {
            DataManager.ReleaseDatas(assets);
        }
    }

    public static ITask<AssetsContainer<T>> LoadDatasAsync<T>(AssetLabelReference label)
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