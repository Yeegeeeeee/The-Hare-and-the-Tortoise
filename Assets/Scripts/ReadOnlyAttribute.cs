using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ReadOnlyAttribute : PropertyAttribute { }

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        bool previousGUIState = GUI.enabled;

        GUI.enabled = false;

        if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue == null)
        {
            EditorGUI.LabelField(position, label.text, "Object destroyed or null");
        }
        else
        {
            EditorGUI.PropertyField(position, property, label);
        }

        GUI.enabled = previousGUIState;
    }
}
#endif