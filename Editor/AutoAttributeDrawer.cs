using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Lachee.Utilities.Editor
{
    [CustomPropertyDrawer(typeof(AutoAttribute))]
    public class AutoAttributeDrawer : PropertyDrawer
    {
        public static dynamic FindComponent(SerializedProperty property, AutoAttribute options)
        {
            var baseComponent = property.serializedObject.targetObject as Component;
            if (baseComponent == null)
                throw new System.InvalidOperationException("Cannot find a component on a non-component object");

            var componentType = GetSerializedPropertyType(property);
            if (!typeof(Component).IsAssignableFrom(componentType))
                throw new System.InvalidOperationException("Type is not a componet and cannot be looked up");

            if (componentType.IsArray)
            {
                if (options.IncludeChildren)
                    return baseComponent.GetComponentsInChildren(componentType);
                return baseComponent.GetComponents(componentType);
            }
            else
            {
                var component = baseComponent.GetComponent(componentType);
                if (component != null)
                    return component;

                if (options.IncludeChildren)
                    return baseComponent.GetComponentsInChildren(componentType);
            }

            return null;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = attribute as AutoAttribute;

            // Null, so lets assign it
            if (property.objectReferenceValue == null)
            {
                var component = FindComponent(property, attr);
                if (component is Object comObject)
                {
                    property.objectReferenceValue = comObject;
                }
                else if (component is Object[] comArray)
                {
                    if (!property.isArray)
                        throw new System.InvalidOperationException("Cannot pass array to non-array field");
                    property.ClearArray();
                    property.arraySize = comArray.Length;
                    for (int i = 0; i < comArray.Length; i++)
                    {
                        property.InsertArrayElementAtIndex(i);
                        var element = property.GetArrayElementAtIndex(i);
                        element.objectReferenceValue = comArray[i];
                    }

                }
            }

            if (attr.Hidden)
            {
                // Hidden but we cannot find one!
                if (ShouldShow(property))
                {
                    if (property.objectReferenceValue == null)
                    {
                        GUI.color = Color.red;
                        label.tooltip = $"[BAD COMPONENT] " + label.tooltip;
                    }

                    EditorGUI.PropertyField(position, property, label);
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

        private bool ShouldShow(SerializedProperty property)
        {
            var attr = attribute as AutoAttribute;
            if (!attr.Hidden)
                return true;

            if (property.objectReferenceValue == null)
                return true;

            var componentReferenceValue = property.objectReferenceValue as Component;
            if (componentReferenceValue.gameObject != (property.serializedObject.targetObject as Component).gameObject)
                return true;

            return false;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return !ShouldShow(property) ? 0 : base.GetPropertyHeight(property, label);
        }

        private static System.Type GetSerializedPropertyType(SerializedProperty property)
        {
            System.Type parentType = property.serializedObject.targetObject.GetType();
            System.Reflection.FieldInfo fi = parentType.GetField(property.propertyPath);
            return fi.FieldType;
        }
    }


    /// <summary>Observes the Auto-Attribute to rebuild lists.</summary>
    [InitializeOnLoad]
    public static class AutoAttributeObserver
    {
        static AutoAttributeObserver()
        {
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
        }

        private static void OnHierarchyChanged()
        {

            var components = Resources.FindObjectsOfTypeAll(typeof(Component));
            var fieldAttribs = components.Select(component => (component, component.GetType()
                                                                                    .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic)
                                                                                    .Select(field => (field, field.GetCustomAttributes(typeof(AutoAttribute), true)))
                                                                                    .Where(touple => touple.Item2 != null)
                                                               )
                                                 );
        }
    }
}
