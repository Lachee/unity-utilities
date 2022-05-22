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
        /// <summary>
        /// Hides the field from the inspector if the value is set.
        /// </summary>
        public bool Hidden { get; set; } = true;

        /// <summary>
        /// Search the tree for children with the same game object.
        /// <para>If this attribute is on an array, it will populate the values from only children</para>
        /// <para>If this attribute is not on an array, it will only check if the component cannot be found on the parent.</para>
        /// </summary>
        public bool IncludeChildren { get; set; } = true;

        /// <summary>
        /// Tag the children objects must have to be included. Leave blank or null for any tag.
        /// </summary>
        public string IncludeChildrenWithTag { get; set; } = "";

        #if UNITY_EDITOR
        /// <summary>
        /// Gets the component to link back to the serialized property
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public virtual dynamic FindReferenceForProperty(UnityEditor.SerializedProperty property)
        {
            var baseComponent = property.serializedObject.targetObject as Component;
            if (baseComponent == null)
                throw new System.InvalidOperationException("Cannot find a component on a non-component object");

            // Get the component type.
            // Calling the extension directly here to avoid having to put scripting defines in the top
            var componentType = Lachee.Utilities.Editor.SerializedPropertyExtensions.GetSerializedType(property);
            if (componentType.IsArray)
            {
                // Validate it is an array of components
                if (!typeof(Component).IsAssignableFrom(componentType.GetElementType()))
                    throw new System.InvalidOperationException("Type is not a componet and cannot be looked up");

                if (IncludeChildren)
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

                if (IncludeChildren)
                    return baseComponent.GetComponentsInChildren(componentType).FirstOrDefault();
            }

            return null;
        }
        #endif
    }
}