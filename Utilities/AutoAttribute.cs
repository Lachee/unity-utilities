using UnityEngine;

namespace Lachee.Utilities
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
    }
}