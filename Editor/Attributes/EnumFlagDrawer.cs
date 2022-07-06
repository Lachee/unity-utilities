using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Lachee.Attributes.Editor
{
    //src: http://wiki.unity3d.com/index.php/EnumFlagPropertyDrawer

    [CustomPropertyDrawer(typeof(EnumFlagAttribute))]
    public class EnumFlagDrawer : PropertyDrawer
    {      
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EnumFlagAttribute flagSettings = (EnumFlagAttribute)attribute;
            if (flagSettings.isReadonly)
                GUI.color = new Color(0.65f, 0.65f, 0.65f);

            if (flagSettings.buttonMode)
                DrawButtons(position, property, label);
            else
                DrawMask(position, property, label);


            GUI.color = Color.white;
        }

        public void DrawButtons(Rect position, SerializedProperty property, GUIContent label)
        {
            EnumFlagAttribute flagSettings = (EnumFlagAttribute)attribute;
            Enum targetEnum = GetBaseProperty<Enum>(property);

            string propName = flagSettings.displayName;
            if (string.IsNullOrEmpty(propName)) propName = label.text;

            EditorGUI.LabelField(new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height), label);
            EditorGUI.BeginProperty(position, label, property);

            var values = Enum.GetValues(targetEnum.GetType());
            string[] names = Enum.GetNames(targetEnum.GetType());

            float buttonWidth = (position.width - EditorGUIUtility.labelWidth) / names.Length;

            int propValue = property.intValue;
            int buttonValue = 0;

            int index = 0;

            foreach (int value in values)
            {
                //Calculate the position of the button
                Rect buttonPos = new Rect(position.x + EditorGUIUtility.labelWidth + buttonWidth * index, position.y, buttonWidth, position.height);

                //Calculate the correct styling
                GUIStyle style = EditorStyles.miniButtonMid;
                if (index == 0) style = EditorStyles.miniButtonLeft;
                if (index == values.Length - 1) style = EditorStyles.miniButtonRight;
                if (values.Length == 1) style = EditorStyles.miniButton;

                //The value isn't 0, so we are not the first element
                if (value != 0)
                {
                    if (GUI.Toggle(buttonPos, ((propValue & value) != 0), property.enumNames[index], style))
                        buttonValue |= value;
                }
                else
                {
                    //The value is 0, so this is a nothing element.
                    if (GUI.Toggle(buttonPos, propValue == value, property.enumNames[index], style))
                    {
                        buttonValue = 0;
                        propValue = 0;
                    }
                }

                index++;
            }

            if (!flagSettings.isReadonly)
                property.intValue = buttonValue;

            EditorGUI.EndProperty();
        }

        public void DrawMask(Rect position, SerializedProperty property, GUIContent label)
        {
            EnumFlagAttribute flagSettings = (EnumFlagAttribute)attribute;
            Enum targetEnum = GetBaseProperty<Enum>(property);

            string propName = flagSettings.displayName;
            if (string.IsNullOrEmpty(propName)) propName = property.name;

            EditorGUI.BeginProperty(position, label, property);

            Enum enumNew = EditorGUI.EnumFlagsField(position, propName, targetEnum);

            if (!flagSettings.isReadonly)
                property.intValue = (int)Convert.ChangeType(enumNew, targetEnum.GetType());


            EditorGUI.EndProperty();
        }

        public void DrawShitButtons(Rect position, SerializedProperty property, GUIContent label)
        {
            EnumFlagAttribute flagSettings = (EnumFlagAttribute)attribute;

            int buttonsIntValue = 0;
            int enumLength = property.enumNames.Length;
            bool[] buttonPressed = new bool[enumLength];
            float buttonWidth = (position.width - EditorGUIUtility.labelWidth) / enumLength;

            int propertyValue = property.intValue;

            label.text += " " + propertyValue;
            EditorGUI.LabelField(new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height), label);

            EditorGUI.BeginChangeCheck();

            if (flagSettings.isReadonly)
                GUI.color = new Color(0.65f, 0.65f, 0.65f);

            for (int i = 0; i < enumLength; i++)
            {

                // Check if the button is/was pressed 
                if ((property.intValue & (1 << i)) == 1 << i)
                {
                    buttonPressed[i] = true;
                }


                Rect buttonPos = new Rect(position.x + EditorGUIUtility.labelWidth + buttonWidth * i, position.y, buttonWidth, position.height);

                buttonPressed[i] = GUI.Toggle(buttonPos, buttonPressed[i], property.enumNames[i], "Button");

                if (buttonPressed[i])
                    buttonsIntValue += 1 << i;
            }

            if (EditorGUI.EndChangeCheck() && !flagSettings.isReadonly)
            {
                property.intValue = buttonsIntValue;
            }

            GUI.color = Color.white;
        }

        static T GetBaseProperty<T>(SerializedProperty prop)
        {
            // Separate the steps it takes to get to this property
            string[] separatedPaths = prop.propertyPath.Split('.');

            // Go down to the root of this serialized property
            System.Object reflectionTarget = prop.serializedObject.targetObject as object;
            // Walk down the path to get the target object
            foreach (var path in separatedPaths)
            {
                FieldInfo fieldInfo = reflectionTarget.GetType().GetField(path, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetField);
                reflectionTarget = fieldInfo.GetValue(reflectionTarget);
            }
            return (T)reflectionTarget;
        }
    }
}