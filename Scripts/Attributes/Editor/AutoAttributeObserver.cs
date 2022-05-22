#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Lachee.Attributes.Editor
{

    /// <summary>Observes the Auto-Attribute to rebuild lists.</summary>
    [InitializeOnLoad]
    public static class AutoAttributeObserver
    {
        static AutoAttributeObserver()
        {
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
        }


        /// <summary>
        /// Applies the Attribute to the Serialized Property
        /// </summary>
        /// <param name="property"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static bool ApplyAttributeToSerializedProperty(SerializedProperty property, AutoAttribute attribute)
        {
            var component = attribute.FindReferenceForProperty(property);
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

        /// <summary>
        /// Called when hierarchy changed.
        /// </summary>
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
        }

        struct SearchRequest
        {
            public string propertyPath;
            public AutoAttribute attribute;
        }
    }
}
#endif