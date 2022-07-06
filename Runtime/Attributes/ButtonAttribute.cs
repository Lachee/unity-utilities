using UnityEngine;

namespace Lachee.Attributes
{
	/// <summary>
	/// This attribute gives methods buttons within the editor inspector. This will allow for quick execution of the method within the editor. Similar to how ContexMenu works.
	/// </summary>
	[System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false)]
	public class ButtonAttribute : PropertyAttribute
	{
		/// <summary>
		/// The label to draw the button as.
		/// </summary>
		public string Label { get; private set; }

		/// <summary>
		/// The colour to draw the button in.
		/// </summary>
		public Color Color { get; private set; }

		/// <summary>
		/// Creates a new button in the inspector
		/// </summary>
		public ButtonAttribute()
		{
			this.Label = null;
			this.Color = Color.white;
		}

		/// <summary>
		/// Creates a new button in the inspector with specified label.
		/// </summary>
		/// <param name="label">The label to appear on the button</param>
		public ButtonAttribute(string label)
		{
			this.Label = label;
			this.Color = Color.white;
		}

		/// <summary>
		/// Creates a new button in the inspector with specified label and colour.
		/// </summary>
		/// <param name="label">The label to appear on the button</param>
		/// <param name="r">Red value of the colour</param>
		/// <param name="g">Green value of the colour</param>
		/// <param name="b">Blue value of the colour</param>
		public ButtonAttribute(string label, byte r, byte g, byte b)
		{
			this.Label = label;
			this.Color = new Color32(r, g, b, 255);
		}


	}

}