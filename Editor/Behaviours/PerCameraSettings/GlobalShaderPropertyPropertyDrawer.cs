using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Lachee.Behaviours;
using Lachee.Editor.Icons;

namespace Lachee.Editor.Behaviours
{
    [CustomPropertyDrawer(typeof(PerCameraSettings.GlobalShaderProperty))]
    public class GlobalShaderPropertyPropertyDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            Rect rectName = new Rect(position.x, position.y, 150, position.height);
            Rect rectEq = new Rect(rectName.xMax + 5, position.y, 15, position.height);
            Rect rectValue = new Rect(rectEq.xMax + 5, position.y, 50, position.height);

            Rect rectResetLabel = new Rect(rectValue.xMax, position.y, 40, position.height);
            Rect rectReset = new Rect(rectResetLabel.xMax + 5, position.y, 15, position.height);
            Rect rectDefault = new Rect(rectReset.xMax + 5, position.y, 50, position.height);

            // Shift the right side so it sticks to the total width
            float diff = (position.width - rectDefault.xMax) + 50;
            rectResetLabel.x    = rectResetLabel.x + diff;
            rectReset.x         = rectReset.x + diff;
            rectDefault.x       = rectDefault.x + diff;

            EditorGUI.PropertyField(rectName, property.FindPropertyRelative("name"), GUIContent.none);
            EditorGUI.LabelField(rectEq, new GUIContent(Icon.equals));
            EditorGUI.PropertyField(rectValue, property.FindPropertyRelative("value"), GUIContent.none);
            EditorGUI.LabelField(rectResetLabel, new GUIContent("Resets"));

            var revertProperty = property.FindPropertyRelative("revert");
            EditorGUI.PropertyField(rectReset, revertProperty, GUIContent.none);
            EditorGUI.BeginDisabledGroup(!revertProperty.boolValue);
            {
                EditorGUI.PropertyField(rectDefault, property.FindPropertyRelative("revertValue"), GUIContent.none);
            }
            EditorGUI.EndDisabledGroup();

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }


    }
}