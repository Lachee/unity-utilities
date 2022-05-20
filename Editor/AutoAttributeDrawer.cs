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
        const string PREF_ALWAYS_SCAN = "autoattr_scan";

        public static dynamic FindComponent(SerializedProperty property, AutoAttribute options)
        {
            var baseComponent = property.serializedObject.targetObject as Component;
            if (baseComponent == null)
                throw new System.InvalidOperationException("Cannot find a component on a non-component object");

            var componentType = GetSerializedPropertyType(property);
            if (componentType.IsArray)
            {
                // Validate it is an array of components
                if (!typeof(Component).IsAssignableFrom(componentType.GetElementType()))
                    throw new System.InvalidOperationException("Type is not a componet and cannot be looked up");


                if (options.IncludeChildren)
                    return baseComponent.GetComponentsInChildren(componentType.GetElementType());
                return baseComponent.GetComponents(componentType.GetElementType());
            }
            else
            {
                // Validate it is a component
                if (!typeof(Component).IsAssignableFrom(componentType))
                    throw new System.InvalidOperationException("Type is not a componet and cannot be looked up");

                var component = baseComponent.GetComponent(componentType);
                if (component != null)
                    return component;

                if (options.IncludeChildren)
                    return baseComponent.GetComponentsInChildren(componentType);
            }

            return null;
        }

        /// <summary>Searches and applies components for the serialized proeprty</summary>
        public static bool ApplyToSerialziedProperty(SerializedProperty property, AutoAttribute attribute)
        {
            var component = FindComponent(property, attribute);
            if (component is Object comObject)
            {
                property.objectReferenceValue = comObject;
                return true;
            }
            else if (component is Object[] comArray)
            {
                if (!property.isArray)
                    throw new System.InvalidOperationException("Cannot pass array to non-array field");
                property.ClearArray();
                property.arraySize = comArray.Length;
                for (int i = 0; i < comArray.Length; i++)
                {
                    var element = property.GetArrayElementAtIndex(i);
                    element.objectReferenceValue = comArray[i];
                }
                return comArray.Length > 0;
            }
            return false;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = attribute as AutoAttribute;

            // Null, so lets assign it
            if (EditorPrefs.GetBool(PREF_ALWAYS_SCAN, false))
            {
                if (property.objectReferenceValue == null || (property.isArray && property.arraySize == 0))
                    ApplyToSerialziedProperty(property, attr);
            }

            if (attr.Hidden)
            {
                // Hidden but we cannot find one!
                if (ShouldShow(property))
                {
                    string errmsg;
                    if (property.objectReferenceValue == null)
                    {
                        errmsg = $" {label.text} [Missing Component]";
                        EditorGUI.HelpBox(position, errmsg, MessageType.Error);
                    } 
                    else
                    {
                        errmsg = $" {label.text} [Wrong GameObject]";
                        EditorGUI.HelpBox(position, errmsg, MessageType.Warning);
                    }

                    EditorGUI.PropertyField(position, property);
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
            // Scan every MonoBehaviour for Auto Attribute fields
            var processQueue = new Dictionary<Object, List<SearchRequest>>();
            var components = Resources.FindObjectsOfTypeAll(typeof(MonoBehaviour)); // Use MonoBehaviour as a slightly more optimised search

            foreach (var component in components)
            {

                bool addedComponentToProcess = false;
                var fields = component.GetType().GetFields();
                foreach (var field in fields)
                {
                    var attributes = field.GetCustomAttributes(typeof(AutoAttribute), true);
                    if (attributes.Length > 0)
                    {
                        if (!addedComponentToProcess)
                        {
                            addedComponentToProcess = true;
                            processQueue.Add(component, new List<SearchRequest>(fields.Length));
                        }

                        // While the attribuite doesnt actually support multiple on one field, 
                        // we do this just to future proof. Performance is neglegitable.
                        foreach (var attribute in attributes)
                        {
                            processQueue[component].Add(new SearchRequest()
                            {
                                propertyPath = field.Name,
                                attribute = (AutoAttribute)attribute
                            });
                        }
                    }
                }
            }

            // Now process all the components
            // We are going to create a serialized object then call the ApplyToSerialized the drawer has to all the 
            //  appropriate properites of that serialized object
            foreach (var kp in processQueue)
            {
                bool hasMadeChanges = false;
                var serializedObject = new SerializedObject(kp.Key);
                foreach (var request in kp.Value)
                {
                    var property = serializedObject.FindProperty(request.propertyPath);
                    if (AutoAttributeDrawer.ApplyToSerialziedProperty(property, request.attribute))
                        hasMadeChanges = true;
                }

                if (hasMadeChanges)
                    serializedObject.ApplyModifiedProperties();
            }

            // Done
        }

        struct SearchRequest
        {
            public string propertyPath;
            public AutoAttribute attribute;
        }
    }
}
