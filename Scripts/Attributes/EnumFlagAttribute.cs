using UnityEngine;

namespace Lachee.Attributes
{
	/// <summary>
	/// Assign this attribute to <see cref="System.FlagsAttribute"/> enums to give them a mask popup within unity.
	/// </summary>
	public class EnumFlagAttribute : PropertyAttribute
	{
		public string displayName = "";
		public bool isReadonly = false;
		public bool buttonMode = true;

		public EnumFlagAttribute() { }

		/// <summary>
		/// Make the enum a flag display
		/// </summary>
		/// <param name="name">The display name</param>
		/// <param name="isReadonly">Should it be just text instead (readonly)?</param>
		public EnumFlagAttribute(string name = "", bool isReadonly = false, bool isButtons = true)
		{
			this.displayName = name;
			this.isReadonly = isReadonly;
			this.buttonMode = isButtons;
		}
	}
}