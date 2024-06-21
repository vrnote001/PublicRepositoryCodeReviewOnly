using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace _ProjectTool
{
//Objectの生成と保持を行うクラス
public sealed class ActorFactory<TActor> : IActorFactory<TActor> where TActor : MonoBehaviour,IActor
{
    private readonly Dictionary<string, LoadActorData> _actorDataStack = new Dictionary<string, LoadActorData>();

    private sealed class LoadActorData
    {
        public LoadActorData(string identifier, TActor actor, int? loadId)
        {
            Identifier = identifier;
            Actor = actor;
            LoadId = loadId;
        }

        public string Identifier { get; }
        public TActor Actor { get; }
        public int? LoadId { get; }
    }

    private readonly object _lock=new();
    private Transform _actorParent;
    private IAssetLoader _assetLoader;

    public ActorFactory(IAssetLoader assetLoader, Transform parent)
    {
        _assetLoader = assetLoader;
        _actorParent = parent;
    }

    public bool Contain(TActor actor)
    {
        lock (_lock)
        {
            return _actorDataStack.Any(data => data.Value.Actor == actor);
        }
    }
    public bool Contain(string identifier)
    {
        lock (_lock)
        {
            return _actorDataStack.ContainsKey(identifier);
        }
    }

    /// <summary>
    /// コントロールするGameObjectのロードを行い登録します
    /// </summary>
    /// <param name="identifier">操作に使用する識別子</param>
    /// <param name="key">追加するGameObjectのPrefabKey</param>
    /// <param name="loadAction">ロード完了後のコールバック</param>
    /// <returns></returns>
    public UniTask Register(string identifier, string key, Action<TActor> loadAction = null)
    {
        lock (_lock)
        {
            return RegisterAsync(identifier, key, null, loadAction);
        }
    }

    /// <summary>
    /// コントロールするGameObjectを登録します
    /// リソースの破棄・解放は対象外になります
    /// </summary>
    /// <param name="identifier">操作に使用する識別子</param>
    /// <param name="target">追加するGameObject</param>
    /// <param name="loadAction">ロード完了後のコールバック</param>
    /// <returns></returns>
    public UniTask Register(string identifier, TActor target, Action<TActor> loadAction = null)
    {
        lock (_lock)
        {
            return RegisterAsync(identifier, null, target, loadAction);
        }
    }

    private async UniTask RegisterAsync(string identifier, string key, TActor target, Action<TActor> loadAction)
    {
        if (_actorDataStack.ContainsKey(identifier))
        {
            Debug.LogWarning(
                $"ActorFactory.RegisterAsync:Error:This identifier is already in use{identifier}");
            return;
        }

        var addActor = target;

        var isSceneActor = addActor != null;
        var isLoadActor = addActor == null && key != null;

        if (isSceneActor)
        {
            addActor.transform.SetParent(_actorParent, false);
            AddStack(identifier,addActor,null);
        }

        if (isLoadActor)
        {
            var load = _assetLoader.LoadAssetAsync<GameObject>(key);
            await load;
            if (!load.Result.IsSucceeded)
            {
                Debug.LogWarning($"ActorFactory.RegisterAsync:Error:{load.Result.Exception}");
                return;
            }

            var spawn = UnityEngine.Object.Instantiate(load.Result.Asset, _actorParent, false);
            addActor = spawn.GetOrAddComponent<TActor>();
            AddStack(identifier, addActor, load.Result.ID);
        }
        
        if(addActor==null)return;
        await addActor.Initialize();
        
        loadAction?.Invoke(addActor);
    }

    private void AddStack(string identifier, TActor actor, int? loadId)
    {
        lock (_lock)
        {
            if (_actorDataStack.ContainsKey(identifier))
            {
                Debug.LogWarning(
                    $"SpatialPanelController.RegisterAsync:Error:This identifier is already in use{identifier}");
                return;
            }
            _actorDataStack.Add(identifier, new LoadActorData(identifier, actor, loadId));
        }
    }

    /// <summary>
    /// 登録済みのGameObjectを破棄・解放します
    /// </summary>
    /// <param name="identifier"></param>
    public void DeRegister(string identifier)
    {
        lock (_lock)
        {
            if(!_actorDataStack.ContainsKey(identifier)) return;
            
            var data = _actorDataStack[identifier];
            
            data.Actor.Dispose();
            
            if(data.Actor!=null)
                 UnityEngine.Object.Destroy(data.Actor);
                 
            if (data.LoadId != null)
                _assetLoader.Release(data.LoadId.Value);

            _actorDataStack.Remove(identifier);
        }
    }

    public bool TryResolve(string identifier,out TActor result)
    {
        lock (_lock)
        {
            if (_actorDataStack.ContainsKey(identifier))
            {
                result = _actorDataStack[identifier].Actor;
                return true;
            }
        }
        result = null;
        return false;
    }

    public void Dispose()
    {
        var stack = _actorDataStack.Keys.ToList();//一度リストにキャッシュする
        foreach (var target in stack)
        {
            DeRegister(target);
        }
    }
}


public interface IActorFactory<TActor> where TActor : MonoBehaviour , IActor
{
    public UniTask Register(string identifier, string key, Action<TActor> loadAction = null);

    public UniTask Register(string identifier, TActor target, Action<TActor> loadAction = null);

    public void DeRegister(string identifier);
}
}
