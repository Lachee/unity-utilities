#if UNITY_EDITOR

// Initial Concept by http://www.reddit.com/user/zaikman
// Revised by http://www.reddit.com/user/quarkism

using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace Lachee.Attributes.Editor
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class EditorButton : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var mono = (MonoBehaviour)targets[0];
            var methods = mono.GetType()
                .GetMembers(BindingFlags.Instance | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                            BindingFlags.NonPublic)
                .Where(o => Attribute.IsDefined(o, typeof(ButtonAttribute)));


            foreach (var memberInfo in methods)
            {
                //Prepare the name
                string buttonText = memberInfo.Name;
                Color color = Color.white;

                //Get the button attributes
                ButtonAttribute[] attributes = (ButtonAttribute[])memberInfo.GetCustomAttributes(typeof(ButtonAttribute), false);
                if (attributes.Length != 0)
                {
                    if (!string.IsNullOrEmpty(attributes[0].Label))
                        buttonText = attributes[0].Label;

                    color = attributes[0].Color;
                }


                if (targets.Length > 1)
                    buttonText += " x" + targets.Length;

                //Store previous colour
                Color pc = GUI.color;
                GUI.color = color;

                //Draw a button
                if (GUILayout.Button(new GUIContent(buttonText)))
                {
                    var method = memberInfo as MethodInfo;
                    for (int i = 0; i < targets.Length; i++) method.Invoke(targets[i], null);
                }

                //Restore the colour
                GUI.color = pc;
            }
        }
    }
}
#endif