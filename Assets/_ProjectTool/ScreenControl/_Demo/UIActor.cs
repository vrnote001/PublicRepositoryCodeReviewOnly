using System;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace _ProjectTool
{
    public class UIActor : MonoBehaviour, IActor
    {
        protected CanvasGroup CanvasGroup;

        [Header("AnimationSetting")] public bool useCustomOpenAnimation;

        [SerializeField] [ShowIf(nameof(useCustomOpenAnimation))]
        private ActorAnimationContainer openAnimation;

        public bool useCustomCloseAnimation;

        [SerializeField] [ShowIf(nameof(useCustomCloseAnimation))]
        private ActorAnimationContainer closeAnimation;

        async UniTask IActor.Open(IActorAnimation defaultAnimation, bool useAnimation)
        {
            CanvasGroup.alpha = 0f;
            gameObject.SetActive(true);
            await OnOpenBefore();

            CanvasGroup.alpha = 1f;

            if (useAnimation)//Animationの処理を抽象インターフェースに委譲
            {
                var anime = useCustomCloseAnimation ? openAnimation : defaultAnimation;
                if (anime != null)
                    await anime.Play(gameObject);
            }

            await OnOpenAfter();
        }

        async UniTask IActor.Close(IActorAnimation defaultAnimation, bool useAnimation)
        {
            await OnCloseBefore();

            if (useAnimation)//Animationの処理を抽象インターフェースに委譲
            {
                var anime = useCustomCloseAnimation ? closeAnimation : defaultAnimation;
                if (anime != null)
                    await anime.Play(gameObject);
            }

            CanvasGroup.alpha = 0f;
            await OnCloseAfter();
        }

        void IActor.CloseImmediate()
        {
            CanvasGroup.alpha = 0f;
            gameObject.SetActive(false);
            OnCloseImmediate();
        }

        UniTask IActor.Initialize()
        {
            CanvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
            CanvasGroup.alpha = 0f;
            gameObject.SetActive(true);
            return OnInitialize();
        }

        void IActor.Dispose()
        {
            OnDispose();
        }

        public virtual void OpenComplete()
        {
            OnOpenComplete();
        }

        //*****継承先で実装*****

        /// <summary>
        /// 生成直後に処理される
        /// </summary>
        /// <returns></returns>
        protected virtual UniTask OnInitialize()
        {
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// 表示直前に処理される
        /// </summary>
        /// <returns></returns>
        protected virtual UniTask OnOpenBefore()
        {
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// 表示直後に処理される
        /// </summary>
        /// <returns></returns>
        protected virtual UniTask OnOpenAfter()
        {
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// 非表示直前に処理される
        /// </summary>
        /// <returns></returns>
        protected virtual UniTask OnCloseBefore()
        {
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// 非表示直後に処理される
        /// </summary>
        /// <returns></returns>
        protected virtual UniTask OnCloseAfter()
        {
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// 非表示直後に処理される
        /// </summary>
        /// <returns></returns>
        protected virtual void OnCloseImmediate()
        {
        }

        /// <summary>
        /// 削除直前に処理される
        /// </summary>
        protected virtual void OnDispose()
        {
        }

        /// <summary>
        /// MainPanelControllerで生成処理か完了、
        /// ユーザーの入力が可能になった時の処理
        /// </summary>
        protected virtual void OnOpenComplete()
        {
        }
    }
}