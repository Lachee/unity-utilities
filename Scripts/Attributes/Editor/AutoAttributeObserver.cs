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
        public const string PREF_PREVENT_PLAYMODE = "AUTO_ATTR_PREVENT_PLAYMODE";

        public struct PropertyAttributePair
        {
            public string propertyPath;
            public AutoAttribute attribute;
        }

        public struct ObjectError
        {
            public Object component;
            public IReadOnlyDictionary<PropertyAttributePair, string> errors;
        }

        private static List<ObjectError> _errors = new List<ObjectError>();
        public static IReadOnlyList<ObjectError> errors => _errors;

        static AutoAttributeObserver()
        {
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
            EditorApplication.playModeStateChanged += OnPlayModeChange;
        }

        private static void OnPlayModeChange(PlayModeStateChange stateChange)
        {
            if (stateChange == PlayModeStateChange.ExitingEditMode)
            {
                ProcessAttributes();
                if (errors.Count > 0)
                {
                    AutoAttributeErrorWindow.ShowWindow();
                    if (EditorPrefs.GetBool(PREF_PREVENT_PLAYMODE, true))
                    {
                        Debug.LogError("Cannot enter play mode because the scene has missing auto components");
                        EditorApplication.ExitPlaymode();
                    } else
                    {
                        Debug.LogError("The scene contains missing components");
                    }
                }
            }
        }

        private static void OnHierarchyChanged()
        {
            ProcessAttributes();
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
        /// Checks if the serialized component is missing anything
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static string CheckError(SerializedProperty property)
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

        /// <summary>
        /// Searches the entire scene and applies all attributes in the scene
        /// </summary>
        public static void ProcessAttributes()
        {
            _errors.Clear();

            // Scan every MonoBehaviour for Auto Attribute fields
            var processQueue = new Dictionary<Object, List<PropertyAttributePair>>();
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
                            processQueue.Add(component, new List<PropertyAttributePair>(fields.Length));
                        }

                        // While the attribuite doesnt actually support multiple on one field, 
                        // we do this just to future proof. Performance is neglegitable.
                        foreach (var attribute in attributes)
                        {
                            processQueue[component].Add(new PropertyAttributePair()
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
                var serializedObject = new SerializedObject(kp.Key);

                // Iterate over every property in thsi component. 
                // we will keep track of property pairs and their errors
                bool hasMadeChanges = false;
                Dictionary<PropertyAttributePair, string> objectErrors = null;

                foreach (var propertyPair in kp.Value)
                {
                    var property = serializedObject.FindProperty(propertyPair.propertyPath);
                    if (ApplyAttributeToSerializedProperty(property, propertyPair.attribute))
                        hasMadeChanges = true;

                    // Get the error and append it to the list of all errors for thsi game object
                    string err = CheckError(property);
                    if (err != null)
                    {
                        if (objectErrors == null)
                            objectErrors = new Dictionary<PropertyAttributePair, string>(kp.Value.Count);

                        objectErrors.Add(propertyPair, err);
                    }
                }

                // Add all the errors
                if (objectErrors != null)
                {
                    _errors.Add(new ObjectError()
                    {
                        component = kp.Key,
                        errors = objectErrors
                    });
                }

                // if we modified, update the serialized object
                if (hasMadeChanges)
                    serializedObject.ApplyModifiedProperties();
            }

            // Finally, tell the editor to repaint
            if (AutoAttributeErrorWindow.Instance)
                AutoAttributeErrorWindow.Instance.Repaint();
        }
    }
}
#endif