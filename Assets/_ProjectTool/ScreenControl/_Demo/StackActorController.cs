using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _ProjectTool
{
    public class StackActorController<TActor> :
        IRegistrableController
        where TActor : MonoBehaviour, IActor
    {
        public string Name { get; }
        public GameObject Parent { get; }

        private readonly ActorFactory<TActor> _actorFactory;
        private IUserInputRestricted UserInputRestricted { get; }
        private ActorDefaultAnimation ActorDefaultAnimation { get; }

        private bool _isRunning;

        private bool _isStack;
        private List<string> _actorStack = new();
        public bool IsPlaying => _actorStack.Count != 0;

        public StackActorController(string name, GameObject parent, IAssetLoader assetLoader,
            IUserInputRestricted userInputRestricted, ActorDefaultAnimation actorDefaultAnimation)
        {
            Name = name;
            Parent = parent;
            UserInputRestricted = userInputRestricted;
            ActorDefaultAnimation = actorDefaultAnimation;
            _actorFactory = new ActorFactory<TActor>(assetLoader, parent.transform);
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

        private async UniTask<TActor> Register(string key, Action<TActor> setUpAction = null)
        {
            await _actorFactory.Register(key, key, setUpAction);
            _actorStack.Add(key);
            _actorFactory.TryResolve(key, out var actor);
            return actor;
        }

        private void DeRegister(string key)
        {
            _actorFactory.DeRegister(key);
            _actorStack.Remove(key);
        }
        
        public UniTask Open(string key, bool stack, bool useAnimation = true, Action<TActor> setUpAction = null)
        {
            return OpenAsync(key, stack, useAnimation, setUpAction);
        }

        private async UniTask OpenAsync(string key, bool stack, bool useAnimation = true,
            Action<TActor> setUpAction = null)
        {
            if (_isRunning) return;
            SetInteractive(false);
            if (_actorStack.Contains(key))
            {
                Debug.LogWarning($"StackActorController.OpenAsync.Error:{key}.Already playing!");
                SetInteractive(true);
                return;
            }

            var next = await Register(key, setUpAction);
            if (next == null)
            {
                Debug.LogWarning($"StackActorController.OpenAsync.Error:{key}.Register.Failed!");
                SetInteractive(true);
                return;
            }

            var taskList = new List<UniTask>();

            next.transform.SetAsLastSibling();
            taskList.Add(next.Open(ActorDefaultAnimation.DefaultOpenAnimation, useAnimation));

            if (_actorStack.Count > 1)
            {
                var lastKey = _actorStack[^1];
                _actorFactory.TryResolve(lastKey, out var current);
                taskList.Add(current.Close(ActorDefaultAnimation.DefaultCloseAnimation, useAnimation));
                await UniTask.WhenAll(taskList);
                if (!_isStack) DeRegister(lastKey);
            }
            else
            {
                await UniTask.WhenAll(taskList);
            }

            _isStack = stack;
            SetInteractive(true);
        }


        public UniTask Close(bool useAnimation = true)
        {
            var key = "";
            return CloseAsync(key, useAnimation);
        }

        public UniTask Close(string key, bool useAnimation = true)
        {
            return CloseAsync(key, useAnimation);
        }

        private async UniTask CloseAsync(string key, bool useAnimation = true)
        {
            if (_isRunning) return;
            if (!IsPlaying) return;
            SetInteractive(false);

            //Key指定がない+戻れるActorが存在すれば対象に加える
            var taskList = new List<UniTask>();

            var canPop = _actorStack.Count > 1;
            if (canPop)
            {
                var isPop = key == "" || key == _actorStack[^1];
                if (isPop) key = _actorStack[^1];

                var isSkip = !isPop && _actorStack.Contains(key);

                if (isPop || isSkip)
                {
                    _actorFactory.TryResolve(key, out var next);
                    taskList.Add(next.Open(ActorDefaultAnimation.DefaultOpenAnimation, useAnimation));
                }

                if (isSkip)
                {
                    var skipRange = _actorStack.IndexOf(key) + 1;
                    var removeList = _actorStack.Skip(skipRange).ToList();
                    removeList.RemoveAt(removeList.Count);
                    foreach (var remove in removeList)
                        DeRegister(remove);
                }
            }

            var currentKey = _actorStack.Last();
            _actorFactory.TryResolve(currentKey, out var current);
            taskList.Add(current.Close(ActorDefaultAnimation.DefaultCloseAnimation, useAnimation));

            await UniTask.WhenAll(taskList);
            DeRegister(currentKey);

            _isStack = IsPlaying;

            SetInteractive(true);
        }

        public void CloseImmediateAll()
        {
            //List操作だめ
            var cash = new List<string>(_actorStack);
            foreach (var key in cash)
                DeRegister(key);
            cash = null;
        }

        public void Dispose()
        {
            CloseImmediateAll();
        }
    }
}