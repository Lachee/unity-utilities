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