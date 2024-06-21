using System;
using System.Collections;
using System.Collections.Generic;
using _ProjectTool;
using UnityEngine;

namespace _ProjectTool
{
    public class StackActorControllerBuilderBase<TActor> : MonoBehaviour where TActor : MonoBehaviour, IActor
    {
        public string controllerName;
        public ActorDefaultAnimation actorDefaultAnimation;
        private StackActorController<TActor> _controller;
        private IAssetLoader _assetLoader;
        private IUserInputRestricted _inputRestricted;

        private void Awake()
        {
            _assetLoader = ServiceLocator.Resolve<IAssetLoader>();
            _inputRestricted=ServiceLocator.Resolve<IUserInputRestricted>();
            _controller = new StackActorController<TActor>(controllerName, gameObject, _assetLoader, _inputRestricted,actorDefaultAnimation);
            ControllerRepository<StackActorController<TActor>>.Register(_controller);
        }

        private void OnDestroy()
        {
            if(_controller==null)return;
            _controller.Dispose();
            ControllerRepository<StackActorController<TActor>>.DeRegister(_controller);
        }
    }
}
