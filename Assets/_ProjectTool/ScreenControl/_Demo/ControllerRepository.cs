using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace _ProjectTool
{
    /// <summary>
    /// IRegistrableControllerの取得クラス
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ControllerRepository<T> where T: class, IRegistrableController
    {
        //UIが入れ子になることも想定してListで定義
        private static readonly List<T> _instances = new List<T>();
        public static IReadOnlyList<T> Instances => _instances;

        public static void Register(T nameHolder)
        {
            //同じControllerが登録されているか
            if (_instances .Contains(nameHolder))
            {
                Debug.LogError($"ControllerRepository:{typeof(T).Name}.Register:Error: This controller[{nameHolder.Name}] is already in use");
                return;
            }

            //同じ名前が登録されているか
            if (_instances .Exists((target) => target.Name == nameHolder.Name))
            {
                Debug.LogError($"ControllerRepository:{typeof(T).Name}.Register:Error: This Name[{nameHolder.Name}] is already in use");
                return;
            }

            _instances .Add(nameHolder);
        }
        /// <summary>
        /// 登録されたPanelを全て破棄して自身のインスタンスを静的Listから除く
        /// </summary>
        public static void DeRegister(T nameHolder)
        {
            _instances .Remove(nameHolder);
        }
    
        /// <summary>
        /// 検索用の関数コンスラクタで登録した文字列のControllerを返却
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T NameOf(string name)
        {
            var nameHolder = _instances.FirstOrDefault((target) => target.Name == name);
            if (nameHolder != null) return nameHolder;
        
            Debug.LogError($"{typeof(T).Name}.NameOf:Error:[{name}].not contain");
            return default;
        }

        /// <summary>
        /// 検索用の関数コンスラクタで登録した文字列のControllerを返却
        /// </summary>
        /// <returns></returns>
        public static bool TryGetParentOf(GameObject parent,out T controller)
        {
            var nameHolder = _instances.FirstOrDefault((target) => target.Parent == parent);
            if (nameHolder != null)
            {
                controller = nameHolder; 
                return true;
            }
            Debug.LogError($"{typeof(T).Name}.NameOf:Error:[{parent}].not contain");
            controller = null;
            return false;
        }
        
       /// <summary>
       /// 一番近い親のコントローラーを取得
       /// </summary>
       /// <param name="target"></param>
       /// <returns></returns>
        public static T MyLayer(GameObject target)
        {
            var parent = target.transform;
            while (parent != null)
            {
                //重いのでLineQにはしないで
                foreach (var c in _instances)
                {
                    if (c.Parent == parent.gameObject)
                    {
                        return c;
                    }
                }
                parent = parent.parent;
            }
            return null;
        }
        
        /// <summary>
        /// 一番近い親のコントローラーを取得
        /// </summary>
        /// <param name="target"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool  TryGetMyLayer(GameObject target,out T result)
        {
            result = MyLayer(target);
            return result != null;
        }
    }
    
    /// <summary>
    /// ControllerRepositoryへ登録可能にする抽象インターフェース
    /// </summary>
    public interface IRegistrableController
    {
        public string Name { get; }
        public GameObject Parent { get; }
    }
    
    /// <summary>
    /// 制御されるオブジェクトの抽象インターフェース
    /// </summary>
    public interface IActor 
    {
        UniTask Initialize();
        UniTask Open(IActorAnimation defaultOpenAnimation,bool useAnimation);
        UniTask Close(IActorAnimation defaultCloseAnimation,bool useAnimation);
        void CloseImmediate();
        void Dispose();
    }
    
    /// <summary>
    /// コントローラーの抽象インターフェース
    /// </summary>
    /// <typeparam name="TActor">制御される対象</typeparam>
    public interface IActorController<out TActor> where TActor : MonoBehaviour, IActor
    {
        UniTask Open(string key, bool useAnimation = true, Action<TActor> setUpAction = null);
        UniTask Close(bool useAnimation = true);
        void CloseImmediateAll();
        void Dispose();
    }
}





