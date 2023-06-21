using Lachee.Utilities.Editor;
using System.Configuration;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Lachee.Attributes.Editor
{
    //TODO: Create a Optional<Component> drawer

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
                int indentation = 0;

                //Check if we should draw the property
                if (attr.ShowCheckbox)
                {
                    Rect iconBox = new Rect(position.x, position.y, 25, position.height);
                    EditorGUI.PropertyField(iconBox, toggleProperty, new GUIContent("", attr.Tooltip));
                    indentation += 1;
                }

                Rect propBox = new Rect(position.x, position.y, position.width, position.height);
                GUI.enabled = attr.Invert ? !toggleProperty.boolValue : toggleProperty.boolValue;
                {
                    EditorGUI.indentLevel += indentation;
                    {
                        EditorGUI.PropertyField(propBox, property, new GUIContent(label.text, label.tooltip), true);
                    }
                    EditorGUI.indentLevel -= indentation;
                }
                GUI.enabled = wasEnabled;

            }
        }
    }
}