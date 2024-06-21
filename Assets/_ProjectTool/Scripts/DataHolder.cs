using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _ProjectTool
{
    /// <summary>
    /// データを保持するクラス
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataHolder<T> where T : Object
    {
        public static DataHolder<T> Instance { get; } = new DataHolder<T>();
    
        private  readonly object _lockObject = new object();
        private  T _holderData;
        private  int? _loadId;
        private  IAssetLoader _assetLoader;

        private bool isAdded => _holderData != null;

        public  T Data
        {
            get
            {
                lock (_lockObject)
                {
                    if (_holderData == null) return null;
                    return _holderData;
                }
            }
        }
    
        public bool TryGetData(out T data)
        {
            lock (_lockObject)
            {
                data=_holderData;
                return isAdded;
            }
        }

        public void SetData(T data)
        {
            _holderData = data;
        }

        public async UniTask LoadDataAsync(IAssetLoader assetLoader, string key, Action<T> callBack = null)
        {
            _assetLoader = assetLoader;
            if (isAdded)
            {
                Release();
            }
        
            var task = assetLoader.LoadAssetAsync<T>(key);
            await task;

            if (!task.Result.IsSucceeded)
            {
                Debug.Log($"DataHolder.LoadDataAsync: Error: {task.Result.Exception}");
                return;
            }

            lock (_lockObject)
            {
                _loadId = task.Result.ID;
                _holderData = task.Result.Asset;
            }

            callBack?.Invoke(_holderData);
        }

        public void Release()
        {
            lock (_lockObject)
            {
                if (_loadId != null && _assetLoader != null)
                {
                    _assetLoader.Release(_loadId.Value);
                    _loadId = null;
                    _holderData = null;
                    return;
                }
                _holderData = null;
            }
        }
    }
}