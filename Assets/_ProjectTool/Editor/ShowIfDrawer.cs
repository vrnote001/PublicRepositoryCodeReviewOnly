using UnityEditor;
using UnityEngine;

namespace _ProjectTool
{
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var showIf = attribute as ShowIfAttribute;
            var conditionProperty = property.serializedObject.FindProperty(showIf.ConditionName);
            if (conditionProperty == null)
                conditionProperty = GetConditionProperty(property, showIf.ConditionName);

            if (conditionProperty == null)
            {
                EditorGUI.HelpBox(position, $"Property '{showIf.ConditionName}' not found!",MessageType.Error);
                return;
            }

            bool conditionValue = conditionProperty.boolValue;

            // Show the property only if the condition is met
            if (conditionValue == showIf.ConditionValue)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
            if (conditionValue && showIf.ConditionValue==null)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var showIf = attribute as ShowIfAttribute;
            var conditionProperty = property.serializedObject.FindProperty(showIf.ConditionName);
            if (conditionProperty == null)
                conditionProperty = GetConditionProperty(property, showIf.ConditionName);

            if (conditionProperty == null)
            {
                return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            bool conditionValue = conditionProperty.boolValue;
        
            if (!conditionValue && (showIf.ConditionValue==null || showIf.ConditionValue==true))
            {
                return 0f;
            }
        
            if (conditionValue && showIf.ConditionValue==false)
            {
                return 0f;
            }

            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    
        private SerializedProperty GetConditionProperty(SerializedProperty property, string conditionName)
        {
            SerializedObject serializedObject = property.serializedObject;
            string[] pathComponents = property.propertyPath.Split('.');
            SerializedProperty conditionProperty = serializedObject.FindProperty(pathComponents[0]);
    
            for (int i = 1; i < pathComponents.Length - 1; i++)
            {
                conditionProperty = conditionProperty.FindPropertyRelative(pathComponents[i]);
            }
    
            SerializedProperty targetProperty = conditionProperty.FindPropertyRelative(conditionName);
            return targetProperty;
        }
    }
}




