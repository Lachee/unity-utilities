using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lachee.Attributes
{
	/// <summary>
	/// Gives attributes a minimum value they can be
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class MinimumAttribute : PropertyAttribute
	{
		/// <summary>
		/// Minimum float value
		/// </summary>
		public float floatMin;

		/// <summary>
		/// Minimum int value
		/// </summary>
		public int intMin;

		/// <summary>
		/// Stops the field from going bellow 0 in the editor.
		/// </summary>
		public MinimumAttribute()
		{
			this.floatMin = 0;
			this.intMin = 0;
		}

		/// <summary>
		/// Stops the field from going below the minimum value in the editor.
		/// </summary>
		/// <param name="minimum">The inclusive minimum value.</param>
		public MinimumAttribute(float minimum)
		{
			this.floatMin = minimum;
			this.intMin = Mathf.RoundToInt(minimum);
		}

		/// <summary>
		/// Stops the field from going below the minimum value in the editor.
		/// </summary>
		/// <param name="minimum">The inclusive minimum value.</param>
		public MinimumAttribute(int minimum)
		{
			this.floatMin = minimum;
			this.intMin = minimum;
		}
	}

}