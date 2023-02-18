using System;
using UnityEngine;

namespace Lachee.Attributes
{
    /// <summary>
    /// Makes object readonly in the editor. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
    public class LabelAttribute : PropertyAttribute
    {
        /// <summary>
        /// The label and tooltip for the property.
        /// </summary>
        public GUIContent label;

        /// <summary>
        /// Creates a label
        /// </summary>
        /// <param name="label">The name of the property</param>
        /// <param name="tooltip">The tooltip for the property</param>
        public LabelAttribute(string label, string tooltip = "")
        {
            this.label = new GUIContent(label, tooltip);
        }

        /// <summary>
        /// Creates a new label from the GUIContent
        /// </summary>
        /// <param name="content"></param>
        public LabelAttribute(GUIContent content)
        {
            this.label = content;
        }
    }
}