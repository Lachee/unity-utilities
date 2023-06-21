using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Lachee.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(AutoAttribute))]
    public class AutoAttributeDrawer : PropertyDrawer
    {
        const float ICON_LABEL_OFFSET = 1.5f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            try
            {

                var attr = attribute as AutoAttribute;
                // Null, so lets assign it
                if (AutoAttributeEditor.AutoRefreshInspector)
                    AutoAttributeEditor.ApplyAttributeToSerializedProperty(property, attr);

                var optionalError = AutoAttributeEditor.Validate(property);

                if (!attr.Hidden || optionalError.HasValue)
                    PropertyGUI(position, property, label);

                // If we have an error, render the error stuff
                if (optionalError is AutoAttributeEditor.Error error)
                {
                    var baseHeight = base.GetPropertyHeight(property, label);

                    Rect errorBox = new Rect(position.x + EditorGUIUtility.labelWidth + 1f, position.y + baseHeight + 1, position.width - EditorGUIUtility.labelWidth, position.height - baseHeight - 1);
                    EditorGUI.HelpBox(errorBox, error.message, error.blocks ? MessageType.Error : MessageType.Warning);
                }
            }
            catch(System.Exception e)
            {
                EditorGUI.HelpBox(position, e.Message, MessageType.Error);
                Debug.Log(e);
            }
        }

        private void PropertyGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            try
            {
                // Ensure regardless of height we still have a regular sized box
                position.height = base.GetPropertyHeight(property, label);

                // Not hidden, lets just display it
                var iconLabel = new GUIContent(AutoAttributeEditor.IconAuto_16);
                iconLabel.tooltip = "Automatically Linked Component";
                Rect iconBox = new Rect(position.x + EditorGUIUtility.labelWidth - position.height - 1.5f, position.y + 1.5f, position.height - 3, position.height - 3);
                EditorGUI.LabelField(iconBox, iconLabel);

                EditorGUI.PropertyField(position, property, label);
                GUI.color = Color.white;

            }
            catch (System.Exception e)
            {
                EditorGUI.HelpBox(position, e.Message, MessageType.Error);
                Debug.Log(e);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            try
            {
                if (AutoAttributeEditor.Validate(property).HasValue)
                    return base.GetPropertyHeight(property, label) + 20;

                var attr = attribute as AutoAttribute;
                return attr.Hidden ? 0 : base.GetPropertyHeight(property, label);
            }
            catch(System.Exception e)
            {
                Debug.LogError("Failed to determine the best high for an auto attribute\n" + e.Message);
                return EditorGUIUtility.singleLineHeight;
            }
        }
    }
}