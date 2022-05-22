using System;
using UnityEngine;

namespace Lachee.Attributes
{
    /// <summary>
    /// Makes object readonly in the editor. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
    public class ReadonlyAttribute : PropertyAttribute
    {
        /// <summary>
        /// Hide the property all together while in edit mode?
        /// </summary>
        public bool hideInEditmode = false;

        /// <summary>
        /// Allow the field to be editable in playmode?
        /// </summary>
        public bool editableInPlaymode = false;

        /// <summary>
        /// Marks this attribute as readonly in the inspector.
        /// </summary>
        /// <param name="hideInEditmode">Hide the property all together while in edit mode?</param>
        /// <param name="editableInPlaymode">Allow the field to be editable in playmode?</param>
        public ReadonlyAttribute(bool hideInEditmode = false, bool editableInPlaymode = false)
        {
            this.hideInEditmode = hideInEditmode;
            this.editableInPlaymode = editableInPlaymode;
        }

    }
}