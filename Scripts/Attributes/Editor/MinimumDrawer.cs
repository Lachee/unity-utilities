#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Lachee.Attributes.Editor
{
	[CustomPropertyDrawer(typeof(MinimumAttribute))]
	public class MinimumDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			MinimumAttribute attr = (MinimumAttribute)attribute;  //The attribute data

			switch (property.propertyType)
			{
				default:
					Debug.LogError("Can only use Minimum attribute on integers or floats!");
					return;

				case SerializedPropertyType.Float:
					float floatValue = EditorGUI.FloatField(position, label, property.floatValue);
					property.floatValue = Mathf.Max(floatValue, attr.floatMin);
					break;

				case SerializedPropertyType.Integer:
					int intValue = EditorGUI.IntField(position, label, property.intValue);
					property.intValue = Mathf.Max(intValue, attr.intMin);
					break;
			}
		}
	}
}

#endif