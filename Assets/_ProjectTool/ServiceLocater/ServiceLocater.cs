using System;
using System.Collections.Generic;
using UnityEngine;

namespace _ProjectTool
{
    public static class ServiceLocator 
    {
        /// <summary>
        /// 単一インスタンス用ディクショナリー
        /// </summary>
        private static Dictionary<Type, object> _instanceDictionary = new Dictionary<Type, object>();

        /// <summary>
        /// 都度インスタンス生成用ディクショナリー
        /// </summary>
        private static Dictionary<Type, Type> _typeDictionary = new Dictionary<Type, Type>();

        /// <summary>
        /// 単一インスタンスを登録する
        /// 呼び直すと上書き登録する
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <param name="instance">インスタンス</param>
        public static void Register<T>(T instance) where T : class
        {
            _instanceDictionary[typeof(T)] = instance;
        }

        /// <summary>
        /// 型を登録する
        /// このメソッドで登録するとResolveしたときに都度インスタンス生成する
        /// </summary>
        /// <typeparam name="TContract">抽象型</typeparam>
        /// <typeparam name="TConcrete">具現型</typeparam>
        public static void Register<TContract, TConcrete>() where TContract : class
        {
            _typeDictionary[typeof(TContract)] = typeof(TConcrete);
        }

        /// <summary>
        /// 型を指定して登録されているインスタンスを取得する
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <returns>インスタンス</returns>
        public static T Resolve<T>() where T : class
        {
            T instance = null;

            Type type = typeof(T);

            if (_instanceDictionary.TryGetValue(type, out var value))
            {
                // 事前に生成された単一インスタンスを返す
                instance = value as T;
                return instance;
            }

            if (_typeDictionary.TryGetValue(type, out var value1))
            {
                // インスタンスを生成して返す
                instance = Activator.CreateInstance(value1) as T;
                return instance;
            }

            if (instance == null)
            {
                Debug.LogWarning($"Locator: {typeof(T).Name} not found.");
            }

            return instance;
        }
    
        /// <summary>
        /// 指定された型の登録を解除する
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        public static void Unregister<T>()
        {
            Type type = typeof(T);

            if (_instanceDictionary.ContainsKey(type))
            {
                _instanceDictionary.Remove(type);
            }

            if (_typeDictionary.ContainsKey(type))
            {
                _typeDictionary.Remove(type);
            }
        }
    }
}