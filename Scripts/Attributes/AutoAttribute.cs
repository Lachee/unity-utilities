using System.Linq;
using UnityEngine;


namespace Lachee.Attributes
{
    /// <summary>
    /// Automatically fetches attached components
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class AutoAttribute : PropertyAttribute
    {
        /// <summary>Controls how to search with children</summary>
        public enum ChildMode
        {
            /// <summary>Ignores children when searching</summary>
            IgnoreChildren,
            /// <summary>Include children and ourselves when searching</summary>
            IncludeChildren,
            /// <summary>Only include children in the search</summary>
            OnlyChildren
        }

        /// <summary>
        /// Hides the field from the inspector if the value is set.
        /// </summary>
        public bool Hidden { get; set; } = true;

        /// <summary>
        /// Include children in the search for components 
        /// <para>If applied to an array, then all components found will be used</para>
        /// <para>If not applied to an array, then the first component found will be used</para>
        /// </summary>
        public ChildMode IncludeChildren { get; set; } = ChildMode.IgnoreChildren;

        #if UNITY_EDITOR
        /// <summary>
        /// Gets the component to link back to the serialized property
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public virtual dynamic FindReferenceForProperty(UnityEditor.SerializedProperty property)
        {
            // Get the base component attached to this property
            var baseComponent = property.serializedObject.targetObject as Component;
            if (baseComponent == null)
                throw new System.InvalidOperationException("Cannot find a component on a non-component object");

            // Get the component type.
            // Calling the extension directly here to avoid having to put scripting defines in the top
            var componentType = Utilities.Editor.SerializedPropertyExtensions.GetSerializedType(property);
            if (componentType.IsArray)
            {
                // Validate it is an array of components
                if (!typeof(Component).IsAssignableFrom(componentType.GetElementType()))
                    throw new System.InvalidOperationException("Type is not a componet and cannot be looked up");

                // Find the component in ourselves, then apply to the children
                switch(IncludeChildren)
                {
                    case ChildMode.OnlyChildren:
                        return baseComponent.GetComponentsInChildren(componentType.GetElementType());
                    case ChildMode.IgnoreChildren:
                        return baseComponent.GetComponents(componentType.GetElementType());
                    case ChildMode.IncludeChildren:
                        return baseComponent.GetComponents(componentType.GetElementType()).Union(baseComponent.GetComponentsInChildren(componentType.GetElementType()));
                }
            }
            else
            {
                // Validate it is a component
                if (!typeof(Component).IsAssignableFrom(componentType))
                    throw new System.InvalidOperationException("Type is not a componet and cannot be looked up");

                switch(IncludeChildren)
                {
                    case ChildMode.OnlyChildren:
                        return baseComponent.GetComponentsInChildren(componentType).FirstOrDefault();
                    case ChildMode.IgnoreChildren:
                        return baseComponent.GetComponent(componentType);
                    case ChildMode.IncludeChildren:
                        var component = baseComponent.GetComponent(componentType);
                        if (component) return component;
                        return baseComponent.GetComponentsInChildren(componentType).FirstOrDefault();
                }
            }

            // We found nothing
            return null;
        }
        #endif
    }
}