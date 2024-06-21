using UnityEditor;
using UnityEngine;

namespace _ProjectTool
{
    [CustomPropertyDrawer(typeof(ShowIfListAttribute))]
    public class ShowIfListDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ShowIfListAttribute showIfAttribute = attribute as ShowIfListAttribute;
            string conditionBoolName = showIfAttribute.ConditionBoolName;

            SerializedProperty conditionBool = property.serializedObject.FindProperty(conditionBoolName);

            if (conditionBool == null || conditionBool.propertyType != SerializedPropertyType.Boolean)
            {
                EditorGUI.HelpBox(position, $"Invalid boolean condition property: {conditionBoolName}", MessageType.Error);
                return;
            }

            bool readOnly = !conditionBool.boolValue;

            GUI.enabled = !readOnly;

            EditorGUI.PropertyField(position, property, label, true);

            GUI.enabled = true;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}