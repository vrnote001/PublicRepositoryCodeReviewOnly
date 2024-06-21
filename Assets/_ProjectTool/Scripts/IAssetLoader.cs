using System;
using System.Threading.Tasks;
using Object = UnityEngine.Object;

namespace _ProjectTool
{
    public interface IAssetLoader
    {
        public Task<AssetLoadResult<T>> LoadAssetAsync<T>(string key, Action<float> onProgressUpdate = null) where T : Object;
        public void Release(int id);
    }
    public class AssetLoadResult<T>
    {
        /// <summary>
        /// リリース時に必要になるID
        /// </summary>
        public int ID { get; }
    
        /// <summary>
        /// 取得したアセット
        /// </summary>
        public T Asset { get; }
    
        /// <summary>
        /// 成功?
        /// </summary>
        public bool IsSucceeded { get; }
    
        /// <summary>
        /// エラーの内容
        /// </summary>
        public Exception Exception { get; }

        private AssetLoadResult(int id ,T asset, bool isSucceeded, Exception exception)
        {
            ID = id;
            Asset = asset;
            IsSucceeded = isSucceeded;
            Exception = exception;
        }

        public static AssetLoadResult<T> Success(int id ,T asset)
        {
            // Debug.Log($"AssetLoadResult<T>{id}:{asset}");
            return new AssetLoadResult<T>( id , asset, true, null);
        
        }

        public static AssetLoadResult<T> Failure(Exception exception)
        {
            return new AssetLoadResult<T>(default,default, false, exception);
        }
    }
}