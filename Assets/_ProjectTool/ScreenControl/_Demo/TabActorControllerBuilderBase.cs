using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _ProjectTool
{
    public class TabActorControllerBuilderBase<TActor> : MonoBehaviour where TActor : MonoBehaviour, IActor
    {
        public string controllerName;
        public ActorDefaultAnimation actorDefaultAnimation;
        private TabActorController<TActor> _controller;
        private IAssetLoader _assetLoader;
        private IUserInputRestricted _inputRestricted;

        public TabControllerGroupBase<TActor> tabControllerGroup;

        private void Awake()
        {
            _assetLoader = ServiceLocator.Resolve<IAssetLoader>();
            _inputRestricted = ServiceLocator.Resolve<IUserInputRestricted>();
            
            _controller = new TabActorController<TActor>(controllerName, gameObject, _assetLoader, _inputRestricted,
                actorDefaultAnimation);
            ControllerRepository<TabActorController<TActor>>.Register(_controller);

            if (tabControllerGroup != null)
                tabControllerGroup.Resister(_controller);
        }

        private void OnDestroy()
        {
            if (_controller == null) return;
            if (tabControllerGroup != null) tabControllerGroup.UnResister(_controller);
            _controller.Dispose();
            ControllerRepository<TabActorController<TActor>>.DeRegister(_controller);
        }
    }
}


