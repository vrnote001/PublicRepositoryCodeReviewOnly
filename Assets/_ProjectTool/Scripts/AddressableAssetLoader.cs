using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace _ProjectTool
{
    public class AddressableAssetLoader : IAssetLoader
    {
        private readonly Dictionary<int, AsyncOperationHandle> _assetCash = new Dictionary<int, AsyncOperationHandle>();
        private int _currentID;

        public void Release(int id)
        {
            if (_assetCash.ContainsKey(id))
            {
                Debug.Log($"AddressableAssetLoader:Release:{id}:{_assetCash[id].Result}");
                Addressables.Release(_assetCash[id]);
                _assetCash.Remove(id);
                return;
            }
            Debug.Log($"AddressableAssetLoader:Release.Nothing:{id}");
        }
    
        private void ErrorHandle(AsyncOperationHandle handle, Exception e)
        {
        }

        public async Task<AssetLoadResult<T>> LoadAssetAsync<T>(string key, Action<float> onProgressUpdate = null)
            where T : Object
        {
            var progressTracker = new ProgressTracker<float>(onProgressUpdate);

            var handle = Addressables.LoadAssetAsync<T>(key);
            ResourceManager.ExceptionHandler = ErrorHandle;

            while (!handle.IsDone)
            {
                progressTracker.Report(handle.PercentComplete);
                await Task.Delay(1);
            }

            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Failed || handle.Status == AsyncOperationStatus.None)
            {
                Debug.Log(handle.OperationException);
                return AssetLoadResult<T>.Failure(handle.OperationException);
            }
        
            var id =Interlocked.Increment(ref _currentID);
            _assetCash.Add(id ,handle);
     
            Debug.Log($"AddressableAssetLoader.Success :{id}:{handle.Result}");
            return AssetLoadResult<T>.Success(id, handle.Result);
        }
    
        //現在Addressableではtry catchにより例外処理が取得できない謎状況にあります。そのため上記の仮のハンドリングを実装しています
        // try
        // {
        //      var handle = Addressables.LoadAssetAsync<T>(key);
        //    
        //      while (!handle.IsDone)
        //      {
        //          progressTracker.Report(handle.PercentComplete);
        //          await Task.Delay(1);
        //      }
        //      
        //      await handle.Task;
        //     currentID++;
        //     AddCash(handle);
        //     return AssetLoadResult<T>.Success(currentID,handle.Result);
        // }
        // catch (System.Exception exception)
        // {
        //     if (exception is InvalidKeyException)
        //     {
        //         Debug.Log("アセットの取得に使われたキーが見当たりません");
        //     }
        //
        //     Debug.Log("アセットのロード処理のエラー");
        //
        //     return AssetLoadResult<T>.Failure(exception);
        // }
    

        // 進捗を報告するためのProgressTrackerクラス
        private class ProgressTracker<T> : IProgress<T>
        {
            private readonly Action<T> _onProgressUpdate;

            //コンストラクタで登録
            public ProgressTracker(Action<T> onProgressUpdate)
            {
                this._onProgressUpdate = onProgressUpdate;
            }

            public void Report(T value)
            {
                _onProgressUpdate?.Invoke(value);
            }
        }
    }
}
