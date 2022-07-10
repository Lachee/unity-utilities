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
        /// <summary>Searches the parent for the component</summary>
        Parent = 8
    }

    /// <summary>
    /// Automatically fetches attached components
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public partial class AutoAttribute : PropertyAttribute
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
    }
}