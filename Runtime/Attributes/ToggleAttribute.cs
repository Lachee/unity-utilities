using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Lachee.Attributes
{
    /// <summary>Marks an attribute as togglable.</summary>
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public sealed class ToggleAttribute : PropertyAttribute
    {
        /// <summary>The name of the field to store the toggle state under.</summary>
        /// <remarks>Prefix with # for the current variable name</remarks>
        /// <example>
        /// [Toggle(Field: "use_#")]
        /// public Color color;
        /// public bool use_color;
        /// </example>
        public string Field { get; private set; }

        public string Tooltip { get; private set; }

        /// <summary>
        /// Inverts the disable state of the UI element so that when ticked it is disabeld
        /// </summary>
        public bool Invert { get; private set; }

        /// <summary>
        /// Shows the checkbox to enable/disable he field
        /// </summary>
        public bool ShowCheckbox { get; set; } = true;

        /// <summary>Creates a toggle attribute with the default prefix of @Enabled</summary>
        public ToggleAttribute() 
        {
            Field = "#Enabled";
            Tooltip = "Toggle Attribute";
            Invert = false;
        }

        public ToggleAttribute(string field)
            : this()
        {
            Field = field;
        }

        public ToggleAttribute(string field, string tooltip)
            : this()
        {
            Field = field;
            Tooltip = tooltip;
        }


        public ToggleAttribute(string field, string tooltip, bool invert)
            : this()
        {
            Field = field;
            Tooltip = tooltip;
            Invert = invert;
        }
    }
}
