using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _ProjectTool
{
    [System.Serializable]
    public class ActorAnimationContainer : IActorAnimation
    {
        [SerializeField] private List<ActorAnimationData> animations=new ();

        public async UniTask Play(GameObject target)
        {
            animations.Sort((a, b) =>b.priorityOrder.CompareTo(a.priorityOrder));
            foreach (var anime in animations)
            {
                await anime.actorAnimationBehaviour.Play(target);
            }
        }
        [System.Serializable]
        public class ActorAnimationData
        {
            public int priorityOrder = 0;
            public ActorAnimationBehaviour actorAnimationBehaviour;
        }
    }
    
    [System.Serializable]
    public class ActorDefaultAnimation
    {
        [SerializeField]private ActorAnimationBehaviour defaultOpenAnimation;
        public ActorAnimationBehaviour DefaultOpenAnimation => defaultOpenAnimation;
   
        [SerializeField]private ActorAnimationBehaviour defaultCloseAnimation;
        public ActorAnimationBehaviour DefaultCloseAnimation => defaultCloseAnimation;
    }

    public abstract class ActorAnimationBehaviour : MonoBehaviour, IActorAnimation
    {
        public abstract UniTask Play(GameObject target);
    }

    public interface IActorAnimation　
    {
        UniTask Play(GameObject target);
    }
}