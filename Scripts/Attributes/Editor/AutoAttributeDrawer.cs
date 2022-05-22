#if UNITY_EDITOR
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
        const string PREF_ALWAYS_SCAN = "autoattr_scan";

        /// <summary>Gets the error associated with the property</summary>
        private string GetError(SerializedProperty property)
        {
            if (property.objectReferenceValue == null)
                return "Component not found";

            var componentReferenceValue = property.objectReferenceValue as Component;
            if (componentReferenceValue.gameObject != (property.serializedObject.targetObject as Component).gameObject)
                return "Component is not attached";

            if (property.isArray && property.arraySize == 0)
                return "No children";

            return null;
        }

        /// <summary>Should the property be showing</summary>
        private bool ShouldShow(SerializedProperty property)
        {
            var attr = attribute as AutoAttribute;
            if (!attr.Hidden) return true;
            return GetError(property) != null;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = attribute as AutoAttribute;

            // Null, so lets assign it
            if (EditorPrefs.GetBool(PREF_ALWAYS_SCAN, false))
            {
                if (property.objectReferenceValue == null || (property.isArray && property.arraySize == 0))
                    AutoAttributeObserver.ApplyAttributeToSerializedProperty(property, attr);
            }

            if (attr.Hidden)
            {
                // Hidden but we cannot find one!
                if (ShouldShow(property))
                {
                    string error = label.text + "\n" + GetError(property);                    
                    EditorGUI.HelpBox(position, error, property.objectReferenceValue == null ? MessageType.Error : MessageType.Warning);

                    Rect centerBox = new Rect(position);
                    centerBox.height = base.GetPropertyHeight(property, label);
                    centerBox.width -= 5;
                    centerBox.y += position.height / 2f - centerBox.height / 2f;
                    EditorGUI.PropertyField(centerBox, property);
                    GUI.color = Color.white;
                }
            }
            else
            {
                // Not hidden, lets just display it
                GUI.color = Color.gray;
                label.tooltip = $"[Automatically Fetched] " + label.tooltip;
                EditorGUI.PropertyField(position, property, label);
                GUI.color = Color.white;
            }

        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!ShouldShow(property))
                return 0;

            return base.GetPropertyHeight(property, label) + 20;
        }
    }
}
#endif