using UnityEngine;

namespace _ProjectTool
{
    public class ShowIfListAttribute : PropertyAttribute
    {
        public string ConditionBoolName { get; }

        public ShowIfListAttribute(string conditionBoolName)
        {
            ConditionBoolName = conditionBoolName;
        }
    }
}