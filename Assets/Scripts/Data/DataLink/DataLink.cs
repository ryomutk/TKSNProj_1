using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;

//ラベルや名前などのデータロードに必要な情報のまとめ
public abstract class DataLink<T> : ScriptableObject
{
    [SerializeField] SerializableDictionary<DataType, AssetLabelReference> label;
    [SerializeField] SerializableDictionary<DataType, string> defaults;
    [SerializeField] SerializableDictionary<T, SerializableDictionary<DataType, string>> nameTables;

    public ITask<DataManager.AssetsContainer<M>> LoadData<M>(DataType type)
    where M : Object
    {
        return DataManager.LoadDatasAsync<M>(label[type]);
    }

    public M FindData<M>(DataType type, T dataLabel, DataManager.IAssetContainer container)
    where M : Object
    {
        if (nameTables.TryGetItem(dataLabel, out var dataSet))
        {
            if (dataSet.TryGetItem(type, out var assetName))
            {
                return container.FindByName<M>(assetName);
            }
        }
        else if(defaults.TryGetItem(type,out var name))
        {
            return container.FindByName<M>(name);
        }

        #if DEBUG
        Debug.LogError("could not find item nor default for"+dataLabel);
        #endif
        return null;
    }
}