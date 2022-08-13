using Lachee.Utilities.Editor;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Lachee.Attributes.Editor
{
    //TODO: Implement this for HideInInspector
    //  https://github.com/NdubuisiJr/TypeExtender/blob/main/TypeExtender/TypeExtender/TypeExtender.cs#L360-L379
    //  https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.customattributebuilder?view=net-6.0                

    [CustomPropertyDrawer(typeof(ToggleAttribute))]
    public class ToggleDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            ToggleAttribute attr = (ToggleAttribute)attribute;  //The attribute data
            string fieldName = attr.Field.Replace("#", property.propertyPath);

            SerializedProperty toggleProperty = property.serializedObject.FindProperty(fieldName);
            if (toggleProperty == null)
            {
                EditorGUI.HelpBox(position, $"Cannot find '{fieldName}'", MessageType.Error);

                Rect smolRect = new Rect(position.x + 200, position.y, position.width - 200, position.height);
                EditorGUI.PropertyField(smolRect, property, label, true);
            }
            else
            {
                bool wasEnabled = GUI.enabled;

                //Check if we should draw the property
                Rect iconBox = new Rect(position.x - 20, position.y, 25, position.height);
                EditorGUI.PropertyField(iconBox, toggleProperty, new GUIContent("", attr.Tooltip));

                GUI.enabled = attr.Invert ? !toggleProperty.boolValue : toggleProperty.boolValue;
                EditorGUI.PropertyField(position, property, label, true);
                GUI.enabled = wasEnabled;
            }
        }
    }
}