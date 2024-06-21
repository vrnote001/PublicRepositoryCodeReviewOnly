using System.Collections;
using System.Collections.Generic;
using _ProjectTool;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _ProjectTool
{
    public class ActorInjectorBase<TController, TActor> : MonoBehaviour
        where TController : class,
        IRegistrableController,//取得可能なコントローラー
        IActorFactory<TActor>//事前に生成登録が必要なコントローラー
        where TActor : MonoBehaviour,IActor
    {
        [Header("表示に使うIdentifierとGameObjectを紐付けて登録")] [SerializeField]
        private List<InjectActorTarget> injectActorList;

        public bool injectOnStart;

        private void Start()
        {
            if (injectOnStart)
                Inject();
        }

        public UniTask Inject()
        {
            return InjectInternal();
        }

        private async UniTask InjectInternal()
        {
            var controller =
                ControllerRepository<TController>.MyLayer(gameObject);

            if (controller == null) return;

            var taskList = new List<UniTask>();

            foreach (var target in injectActorList)
                taskList.Add(controller.Register(target.identifier, target.actor));

            await UniTask.WhenAll(taskList);
        }

        [System.Serializable]
        public class InjectActorTarget
        {
            public string identifier;
            public TActor actor;
        }
    }
}