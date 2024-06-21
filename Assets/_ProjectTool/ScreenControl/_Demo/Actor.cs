using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _ProjectTool
{
   public class Actor : MonoBehaviour,IActor
   {
      [Header("AnimationSetting")]
      public bool useCustomOpenAnimation;
      [SerializeField][ShowIf(nameof(useCustomOpenAnimation))]
      private ActorAnimationContainer openAnimation;
      public bool useCustomCloseAnimation;
      [SerializeField][ShowIf(nameof(useCustomCloseAnimation))]
      private ActorAnimationContainer  closeAnimation;
      
      public async UniTask Open(IActorAnimation defaultAnimation,bool useAnimation)
      {
         await OpenBefore();
         gameObject.SetActive(true);
         gameObject.transform.localScale=Vector3.one;

         if (useAnimation)
         {
            var anime = useCustomCloseAnimation ? 
               openAnimation: defaultAnimation;
            await anime.Play(gameObject);
         }
         await OpenAfter();
      }
   
      public async UniTask Close(IActorAnimation defaultAnimation,bool useAnimation)
      {
         await CloseBefore();
         if (useAnimation)
         {
            var anime= useCustomCloseAnimation ? 
               closeAnimation:defaultAnimation;
            await anime.Play(gameObject);
         }
         gameObject.transform.localScale= Vector3.zero;
         gameObject.SetActive(false);
      
         await CloseAfter();
      }

      public void CloseImmediate()
      {
         throw new NotImplementedException();
      }

      UniTask IActor.Initialize()
      {
         gameObject.transform.localScale=Vector3.zero;
         gameObject.SetActive(false);
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
      protected virtual UniTask OnInitialize(){return UniTask.CompletedTask;}
   
      /// <summary>
      /// 表示直前に処理される
      /// </summary>
      /// <returns></returns>
      protected virtual UniTask OpenBefore(){return UniTask.CompletedTask;}
   
      /// <summary>
      /// 表示直後に処理される
      /// </summary>
      /// <returns></returns>
      protected virtual UniTask OpenAfter() {return UniTask.CompletedTask;}
   
      /// <summary>
      /// 非表示直前に処理される
      /// </summary>
      /// <returns></returns>
      protected virtual UniTask CloseBefore(){return UniTask.CompletedTask;}
   
      /// <summary>
      /// 非表示直後に処理される
      /// </summary>
      /// <returns></returns>
      protected virtual UniTask CloseAfter() {return UniTask.CompletedTask;}
   
      /// <summary>
      /// 削除直前に処理される
      /// </summary>
      protected virtual void OnDispose(){}
   
      /// <summary>
      /// MainPanelControllerで生成処理か完了、
      /// ユーザーの入力が可能になった時の処理
      /// </summary>
      protected virtual void OnOpenComplete(){}
   }
}
