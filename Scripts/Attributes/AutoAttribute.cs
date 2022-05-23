using Lachee.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Lachee.Attributes
{
    /// <summary>Flag that controls how searching is handled within the <see cref="AutoAttribute"/></summary>
    [System.Flags]
    public enum AutoSearchFlag
    {
        /// <summary>Search the GameObject</summary>
        GameObject = 1,
        /// <summary>Search the children for the component</summary>
        Children = 2,
        /// <summary>Search the scene for the component</summary>
        Scene = 4,
    }

    /// <summary>
    /// Automatically fetches attached components
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class AutoAttribute : PropertyAttribute
    {
        /// <summary>
        /// Hides the field from the inspector if the value is set.
        /// </summary>
        public bool Hidden { get; set; }

        /// <summary>
        /// Include children in the search for components 
        /// <para>If applied to an array, then all components found will be used</para>
        /// <para>If not applied to an array, then the first component found will be used</para>
        /// </summary>
        public AutoSearchFlag SearchFlag { get; set; }

        /// <summary>Automatically fetches components and hides them in the inspector from this game object</summary>
        public AutoAttribute() : this(true, AutoSearchFlag.GameObject) { }

        /// <summary>
        /// Automatically fetches components from this game object.
        /// </summary>
        /// <param name="hidden"></param>
        public AutoAttribute(bool hidden) : this(hidden, AutoSearchFlag.GameObject) { }

        /// <summary>
        /// Automatically fetches the components and hides them from the inspector
        /// </summary>
        /// <param name="mode"></param>
        public AutoAttribute(AutoSearchFlag mode) : this(true, mode) { }

        /// <summary>
        /// Automatically fetches the components
        /// </summary>
        /// <param name="hidden"></param>
        /// <param name="mode"></param>
        public AutoAttribute(bool hidden, AutoSearchFlag mode)
        {
            this.Hidden = hidden;
            this.SearchFlag = mode;
        }

        #if UNITY_EDITOR
        /// <summary>
        /// Gets the component to link back to the serialized property
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public virtual dynamic FindReferenceForProperty(UnityEditor.SerializedProperty property)
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
                if ((SearchFlag & AutoSearchFlag.GameObject) != 0) {
                    found = component.GetComponents(propertyType);
                    results.AddRange(found);
                }

                if ((SearchFlag & AutoSearchFlag.Children) != 0) {
                    found = component.GetComponentsInChildren(propertyType);
                    results.AddRange(found);
                }

                if ((SearchFlag & AutoSearchFlag.Scene) != 0) {
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
                if ((SearchFlag & AutoSearchFlag.GameObject) != 0) {
                    found = component.GetComponent(propertyType);
                    if (found) return found;
                }

                if ((SearchFlag & AutoSearchFlag.Children) != 0) {
                    found = component.GetComponentInChildren(propertyType);
                    if (found) return found;
                }

                if ((SearchFlag & AutoSearchFlag.Scene) != 0) {
                    found = GameObject.FindObjectOfType(propertyType);
                    if (found) return found;
                }
            }

            // We found nothing
            return null;
        }
        #endif
    }
}