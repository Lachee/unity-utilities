using UnityEditor;
using UnityEngine;

namespace Lachee.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(SlideAttribute))]
    public class SlideDrawer : PropertyDrawer
    {
        const float miniLabelVerticalOffset = -7;

        static GUIStyle miniLabelStyle;
        static SlideDrawer()
        {
            miniLabelStyle = new GUIStyle(EditorStyles.miniLabel);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Lock the position to the bottom
            position.y = position.y + position.height - EditorGUIUtility.singleLineHeight;

            SlideAttribute attr = (SlideAttribute)attribute;

            // Draw the left and right
            if (attr.LeftLabel != null)
            {
                miniLabelStyle.alignment = TextAnchor.UpperLeft;
                EditorGUI.LabelField(new Rect(position.x + EditorGUIUtility.labelWidth, position.y + miniLabelVerticalOffset, position.width, position.height), attr.LeftLabel, miniLabelStyle);
            }

            //style.alignment = TextAnchor.LowerRight;
            if (attr.RightLabel != null)
            {
                float padding = property.propertyType == SerializedPropertyType.Float || property.propertyType == SerializedPropertyType.Integer ? -EditorGUIUtility.fieldWidth - 5 : 0;
                miniLabelStyle.alignment = TextAnchor.UpperRight;
                EditorGUI.LabelField(new Rect(position.x + EditorGUIUtility.labelWidth, position.y + miniLabelVerticalOffset, position.width - EditorGUIUtility.labelWidth + padding, position.height), attr.RightLabel, miniLabelStyle);
            }

            // Draw the slider
            Rect sliderPosition = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            switch (property.propertyType)
            {
                case SerializedPropertyType.Float:
                    EditorGUI.Slider(sliderPosition, property, attr.Min, attr.Max, label);
                    break;
                case SerializedPropertyType.Integer:
                    EditorGUI.IntSlider(sliderPosition, property, (int) attr.Min, (int) attr.Max, label);
                    break;
                case SerializedPropertyType.Vector2:
                case SerializedPropertyType.Vector2Int:
                    MinMaxSlider(sliderPosition, property, attr.Min, attr.Max, label);
                    break;
                default:
                    EditorGUI.LabelField(position, label, new GUIContent("Slider must be of type float, int, Vector2, or Vector2Int"));
                    return;
            }

        }

        private static void MinMaxSlider(Rect position, SerializedProperty property, float min, float max, GUIContent label)
        {
            //TODO: Look at how unity draws its MinMaxSlider attribute

            float minValue = 0;
            float maxValue = 0;

            if (property.propertyType == SerializedPropertyType.Vector2)
            {
                minValue = property.vector2Value.x;
                maxValue = property.vector2Value.y;
                EditorGUI.MinMaxSlider(position, label, ref minValue, ref maxValue, min, max);
                property.vector2Value = new Vector2(minValue, maxValue);
            } 
            else if (property.propertyType == SerializedPropertyType.Vector2Int)
            {
                minValue = property.vector2IntValue.x;
                maxValue = property.vector2IntValue.y;
                EditorGUI.MinMaxSlider(position, label, ref minValue, ref maxValue, min, max);
                property.vector2IntValue = new Vector2Int(Mathf.RoundToInt(minValue), Mathf.RoundToInt(maxValue));
            }
            else
            {
                throw new System.ArgumentException("The property must be a vector2 type");
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = base.GetPropertyHeight(property, label);

            SlideAttribute attr = attribute as SlideAttribute;
            if (attr != null && (attr.LeftLabel != null || attr.RightLabel != null))
                height += -miniLabelVerticalOffset;

            return height;
        }
    }
}