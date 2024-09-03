using UnityEditor;
using UnityEngine;

namespace Lachee.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(LabelAttribute))]
    public class LabelDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            LabelAttribute attr = (LabelAttribute)attribute;
            if (label != GUIContent.none) label = attr.label;
            EditorGUI.PropertyField(position, property, label, true);
        }
    }
}