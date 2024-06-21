using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _ProjectTool
{
    public class TabActorController<TActor> :
        IRegistrableController,//外部から取得するControllerRepositoryへ登録可能にする
        IActorFactory<TActor>//生成管理する(内部で委譲している)
        where TActor : MonoBehaviour, IActor
    {
        public string Name { get; }
        public GameObject Parent { get; }
        private readonly ActorFactory<TActor> _actorFactory;//生成管理の処理を委譲
        private IUserInputRestricted UserInputRestricted { get; }//ユーザーの入力制御処理を委譲する
        private ActorDefaultAnimation ActorDefaultAnimation { get; }//生成したものに渡す基底Animation

        private bool _isRunning;
        private string _currentIdentifier = "";
        public bool IsPlaying => _currentIdentifier != "";

        private readonly TabActorEventNotifier<TActor> _actorEventNotifier;//イベント処理を委譲する
        
        public event Action<TabActorEventNotifier<TActor>.TabActorEventInfo> OnOpen//ユーザーアクセスを容易にする
        {
            add => _actorEventNotifier.OnOpen += value;
            remove => _actorEventNotifier.OnOpen -= value;
        }

        public event Action<TabActorEventNotifier<TActor>.TabActorEventInfo> OnClose
        {
            add => _actorEventNotifier.OnClose += value;
            remove => _actorEventNotifier.OnClose -= value;
        }

        public event Action<TabActorEventNotifier<TActor>.TabActorEventInfo> OnCloseImmediate
        {
            add => _actorEventNotifier.OnCloseImmediate += value;
            remove => _actorEventNotifier.OnCloseImmediate -= value;
        }

        public TabActorController(string name, GameObject parent, IAssetLoader assetLoader,
            IUserInputRestricted userInputRestricted, ActorDefaultAnimation actorDefaultAnimation)
        {
        
            Name = name;
            Parent = parent;
            UserInputRestricted = userInputRestricted;
            ActorDefaultAnimation = actorDefaultAnimation;
            _actorFactory = new ActorFactory<TActor>(assetLoader, parent.transform);
            _actorEventNotifier = new TabActorEventNotifier<TActor>();
        }

        public void Dispose()
        {
            if (!_isRunning)
                _actorFactory.Dispose();
        }

        //遷移中のユーザー操作を制御する
        private void SetInteractive(bool active)
        {

            if (active)
            {
                _isRunning = false;
                UserInputRestricted.UnBlock();
                return;
            }

            _isRunning = true;
            UserInputRestricted.Block();
        }

        public UniTask Open(string identifier, bool useAnimation = true, Action<TActor> setUpAction = null)
        {
            return OpenAsync(identifier, useAnimation, setUpAction);
        }

        //一番手前に移動を追記する
        private async UniTask OpenAsync(string identifier, bool useAnimation = true, Action<TActor> setUpAction = null)
        {
            if (_isRunning) return;
            SetInteractive(false);
            if (!_actorFactory.TryResolve(identifier, out var next))
            {
                Debug.Log($"TabActorController.Open.Error:identifier:{identifier}.NotFound......");
                return;
            }

            next.transform.SetAsLastSibling();
            setUpAction?.Invoke(next);

            var taskList = new List<UniTask>();

            if (IsPlaying)
            {
                _actorFactory.TryResolve(_currentIdentifier, out var current);
                taskList.Add(current.Close(ActorDefaultAnimation.DefaultCloseAnimation, useAnimation));
            }

            taskList.Add(next.Open(ActorDefaultAnimation.DefaultOpenAnimation, useAnimation));
            await UniTask.WhenAll(taskList);
            if (IsPlaying)
                _actorEventNotifier.NotifyClose(this,_currentIdentifier);

            _currentIdentifier = identifier;
            _actorEventNotifier.NotifyOpen(this,_currentIdentifier);
            SetInteractive(true);
        }

        public UniTask Close(bool useAnimation = true)
        {
            return CloseAsync(useAnimation);
        }

        private async UniTask CloseAsync(bool useAnimation = true)
        {
            if (_isRunning) return;
            SetInteractive(false);
            if (IsPlaying)
            {
                _actorFactory.TryResolve(_currentIdentifier, out var current);
                await current.Close(ActorDefaultAnimation.DefaultCloseAnimation, useAnimation);
                _actorEventNotifier.NotifyClose(this,_currentIdentifier);
                _currentIdentifier = "";
            }

            SetInteractive(true);
        }

        public void CloseImmediateAll()
        {
            if (_isRunning || !IsPlaying) return;
            _actorFactory.TryResolve(_currentIdentifier, out var current);
            current.CloseImmediate();
            _actorEventNotifier.NotifyCloseImmediate(this,_currentIdentifier);
            _currentIdentifier = "";
        }
        
        //処理を委譲
        public UniTask Register(string identifier, string key, Action<TActor> loadAction = null)
        {
            return _actorFactory.Register(identifier, key, loadAction);
        }

        public UniTask Register(string identifier, TActor target, Action<TActor> loadAction = null)
        {
            return _actorFactory.Register(identifier, target, loadAction);
        }

        public void DeRegister(string identifier)
        {
            _actorFactory.DeRegister(identifier);
        }
    }
}
