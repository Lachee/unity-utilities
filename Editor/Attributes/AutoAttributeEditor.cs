using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

using Lachee.Utilities;

namespace Lachee.Attributes.Editor
{

    /// <summary>Observes the Auto-Attribute to rebuild lists.</summary>
    [InitializeOnLoad]
    public static class AutoAttributeEditor
    {
        private const string PREF_PREVENT_PLAYMODE = "AUTO_ATTR_PREVENT_PLAYMODE";
        private const string PREF_AUTO_REFRESH = "AUTO_ATTR_AUTO_REFRESH";

        /// <summary>Error state about an attribute</summary>
        public struct Error
        {
            public readonly string message;
            public readonly bool blocks;
            public Error(string message, bool blocks)
            {
                this.message = message;
                this.blocks = blocks;
            }
        }
        /// <summary>Path of the attribute with a reference to the attribute itself</summary>
        public struct AttributeProperty
        {
            public string propertyPath;
            public AutoAttribute attribute;
        }
        /// <summary></summary>
        public class ObjectErrors : IReadOnlyDictionary<AttributeProperty, Error>
        {
            public Object Component => m_Component;
            private Object m_Component;
            private Dictionary<AttributeProperty, Error> m_Errors;

            public ObjectErrors(Object component, Dictionary<AttributeProperty, Error> errors)
            {
                m_Component = component;
                m_Errors = errors;
            }

            #region Dictionary Interface
            public Error this[AttributeProperty key] => m_Errors[key];
            public IEnumerable<AttributeProperty> Keys => m_Errors.Keys;
            public IEnumerable<Error> Values => m_Errors.Values;
            public int Count => m_Errors.Count;
            public bool ContainsKey(AttributeProperty key) => m_Errors.ContainsKey(key);
            public IEnumerator<KeyValuePair<AttributeProperty, Error>> GetEnumerator() => m_Errors.GetEnumerator();
            public bool TryGetValue(AttributeProperty key, out Error value) => m_Errors.TryGetValue(key, out value);
            IEnumerator IEnumerable.GetEnumerator() => m_Errors.GetEnumerator();
            #endregion
        }
        /// <summary>List of errors from the last check. Call CheckAttribute to update this list.</summary>
        public static IReadOnlyList<ObjectErrors> Errors => _errors;
        private static List<ObjectErrors> _errors = new List<ObjectErrors>();

        public static Texture2D IconAuto_32 { get; private set; }
        public static Texture2D IconAuto_16 { get; private set; }

        static AutoAttributeEditor()
        {
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
            EditorApplication.playModeStateChanged += OnPlayModeChange;

            IconAuto_32 = (Texture2D)AssetDatabase.LoadAssetAtPath("Packages/com.lachee.utilities/Editor Resources/auto_32.png", typeof(Texture2D));
            IconAuto_16 = (Texture2D)AssetDatabase.LoadAssetAtPath("Packages/com.lachee.utilities/Editor Resources/auto_16.png", typeof(Texture2D));
        }

        #region Event Listeners
        private static void OnPlayModeChange(PlayModeStateChange stateChange)
        {
            if (stateChange == PlayModeStateChange.ExitingEditMode)
            {
                CheckAttributes();
                if (Errors.Any(errors => errors.Values.Any(error => error.blocks)))
                {
                    AutoAttributeWindow.ShowWindow();
                    if (PreventPlayMode)
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
            CheckAttributes();
        }
        #endregion

        #region Properties
        /// <summary>Prevents PlayMode if there is an error</summary>
        public static bool PreventPlayMode
        {
            get => EditorPrefs.GetBool(PREF_PREVENT_PLAYMODE, true);
            set => EditorPrefs.SetBool(PREF_PREVENT_PLAYMODE, value);
        }
        /// <summary>Shoudl components auto refresh once per GUI frame when inspecting</summary>
        public static bool AutoRefreshInspector
        {
            get => EditorPrefs.GetBool(PREF_AUTO_REFRESH, true);
            set => EditorPrefs.SetBool(PREF_AUTO_REFRESH, value);
        }
        #endregion

        /// <summary>
        /// Applies the Attribute to the Serialized Property
        /// </summary>
        /// <param name="property"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static bool ApplyAttributeToSerializedProperty(SerializedProperty property, AutoAttribute attribute)
        {
            var component = FindReferenceForProperty(attribute, property);
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
        /// Checks if the serialized component is missing anything.
        /// <para>Note that this does not actually add the property to the error list.</para>
        /// </summary>
        /// <param name="property">The serialized property with the <see cref="AutoAttribute"/></param>
        /// <returns>The error, other null.</returns>
        public static Error? Validate(SerializedProperty property)
        {
            if (property == null)
                throw new System.Exception("Property is null");

            if (property.objectReferenceValue == null)
                return new Error("Component not found", true);

            var componentReferenceValue = property.objectReferenceValue as Component;
            if (componentReferenceValue.gameObject != (property.serializedObject.targetObject as Component).gameObject)
                return new Error($"Detatched Component found on {componentReferenceValue.gameObject.name}", false);

            if (property.isArray && property.arraySize == 0)
                return new Error("Components not found", true);

            return null;
        }

        /// <summary>
        /// Searches the entire scene and applies all attributes in the scene
        /// </summary>
        public static void CheckAttributes()
        {
            _errors.Clear();

            // Scan every MonoBehaviour for Auto Attribute fields
            var processQueue = new Dictionary<Object, List<AttributeProperty>>();
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
                            processQueue.Add(component, new List<AttributeProperty>(fields.Length));
                        }

                        // While the attribuite doesnt actually support multiple on one field, 
                        // we do this just to future proof. Performance is neglegitable.
                        foreach (var attribute in attributes)
                        {
                            processQueue[component].Add(new AttributeProperty()
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
                Dictionary<AttributeProperty, Error> errors = null;

                foreach (var propertyPair in kp.Value)
                {
                    var property = serializedObject.FindProperty(propertyPair.propertyPath);
                    if (ApplyAttributeToSerializedProperty(property, propertyPair.attribute))
                        hasMadeChanges = true;

                    // Get the error and append it to the list of all errors for thsi game object
                    if (Validate(property) is Error error)
                    {
                        if (errors == null)
                            errors = new Dictionary<AttributeProperty, Error>(kp.Value.Count);
                        errors.Add(propertyPair, error);
                    }
                }

                // Add all the errors
                if (errors != null)
                    _errors.Add(new ObjectErrors(kp.Key, errors));
            
                // if we modified, update the serialized object
                if (hasMadeChanges)
                    serializedObject.ApplyModifiedProperties();
            }

            // Tell the window we shoudl repaint if its open
            if (AutoAttributeWindow.Instance)
                AutoAttributeWindow.Instance.Repaint();
        }



        /// <summary>
        /// Gets the component to link back to the serialized property
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private static dynamic FindReferenceForProperty(AutoAttribute attribute, SerializedProperty property)
        {
            // Get the base component attached to this property
            var component = property.serializedObject.targetObject as Component;
            if (component == null)
                throw new System.InvalidOperationException("Cannot find a component on a non-component object");

            // Get the component type.
            // Calling the extension directly here to avoid having to put scripting defines in the top
            var propertyType = Utilities.Editor.SerializedPropertyExtensions.GetSerializedType(property);
            if (propertyType.IsArray)
            {
                // This version will pull all the components we find and unions them in a hashset first to avoid duplicates

                // Validate it is an array of components
                if (!typeof(Component).IsAssignableFrom(propertyType.GetElementType()))
                    throw new System.InvalidOperationException("Type is not a componet and cannot be looked up");

                HashSet<Object> results = new HashSet<Object>();

                // Find all the results
                Object[] found;
                if ((attribute.SearchFlag & AutoSearchFlag.GameObject) != 0)
                {
                    found = component.GetComponents(propertyType);
                    results.AddRange(found);
                }

                if ((attribute.SearchFlag & AutoSearchFlag.Children) != 0)
                {
                    found = component.GetComponentsInChildren(propertyType);
                    results.AddRange(found);
                }

                if ((attribute.SearchFlag & AutoSearchFlag.Scene) != 0)
                {
                    found = GameObject.FindObjectsOfType(propertyType);
                    results.AddRange(found);
                }

                // Convert to array
                return results.ToArray();
            }
            else
            {

                // This is a more optimised version.
                // Duplicated code for the most part, but it is done so we only call GetComponent once for non-arrrays


                // Validate it is a component
                if (!typeof(Component).IsAssignableFrom(propertyType))
                    throw new System.InvalidOperationException("Type is not a componet and cannot be looked up");

                Object found = null;
                if ((attribute.SearchFlag & AutoSearchFlag.GameObject) != 0)
                {
                    found = component.GetComponent(propertyType);
                    if (found) return found;
                }

                if ((attribute.SearchFlag & AutoSearchFlag.Children) != 0)
                {
                    found = component.GetComponentInChildren(propertyType);
                    if (found) return found;
                }

                if ((attribute.SearchFlag & AutoSearchFlag.Scene) != 0)
                {
                    found = GameObject.FindObjectOfType(propertyType);
                    if (found) return found;
                }
            }

            // We found nothing
            return null;
        }
    }
}