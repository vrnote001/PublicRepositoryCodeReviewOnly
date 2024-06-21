using System;
using UnityEngine;

namespace _ProjectTool
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public class ShowIfAttribute : PropertyAttribute
    {
        public string ConditionName { get; }
        public bool? ConditionValue { get; }

        public ShowIfAttribute(string conditionName)
        {
            ConditionName = conditionName;
            ConditionValue = null; 
        }

        public ShowIfAttribute(string conditionName, bool conditionValue)
        {
            ConditionName = conditionName;
            ConditionValue = conditionValue;
        }
    }
}

