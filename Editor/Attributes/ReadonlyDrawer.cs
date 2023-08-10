using UnityEditor;
using UnityEngine;

namespace Lachee.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(ReadonlyAttribute))]
    public class ReadonlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ReadonlyAttribute attr = (ReadonlyAttribute)attribute;  //The attribute data
            bool playMode = Application.isPlaying;                  //Are we currently in playmode?

            //If we are to be hidden, end here
            if (attr.hideInEditmode && !playMode) return;

            //Is the element enabled?
            bool enabled = (playMode && attr.editableInPlaymode) || (!playMode && attr.editableInEditmode);

            //Enable/disable the property
            bool wasEnabled = GUI.enabled;
            GUI.enabled = enabled;

            //Check if we should draw the property
            EditorGUI.PropertyField(position, property, label, true);

            //Ensure that the next property that is being drawn uses the correct settings
            GUI.enabled = wasEnabled;
        }
    }
}