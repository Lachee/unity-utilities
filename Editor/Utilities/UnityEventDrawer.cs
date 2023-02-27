#if UNITY_2020_1_OR_NEWER
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using System.Reflection;
using UnityEditorInternal;

namespace Lachee.Utilities.Editor
{
    [CustomPropertyDrawer(typeof(UnityEventBase), true)]
    public class UnityEventDrawer : UnityEditorInternal.UnityEventDrawer
    {
        #region Private Functions i steal from UnityEventDrawerer
        private static readonly FieldInfo fi_reoderableList;
        private static readonly FieldInfo fi_dummyEvent;
        private static readonly FieldInfo fi_text;
        private static readonly MethodInfo mi_RestoreState;
        private static readonly MethodInfo mi_GetDummyEvent;

        static UnityEventDrawer()
        {
            var T = typeof(UnityEditorInternal.UnityEventDrawer);
            fi_reoderableList = T.GetField("m_ReorderableList", BindingFlags.NonPublic | BindingFlags.Instance);
            fi_dummyEvent = T.GetField("m_DummyEvent", BindingFlags.NonPublic | BindingFlags.Instance);
            fi_text = T.GetField("m_Text", BindingFlags.NonPublic | BindingFlags.Instance);
            mi_GetDummyEvent = T.GetMethod("GetDummyEvent", BindingFlags.NonPublic | BindingFlags.Static);
            mi_RestoreState = T.GetMethod("RestoreState", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        private static UnityEventBase GetDummyEvent(SerializedProperty prop)
            => (UnityEventBase)mi_GetDummyEvent.Invoke(null, new object[] { prop });

        private ReorderableList reorderableList
        {
            get => (ReorderableList)fi_reoderableList.GetValue(this);
        }
        private UnityEventBase dummyEvent
        {
            get => (UnityEventBase)fi_dummyEvent.GetValue(this);
            set => fi_dummyEvent.SetValue(this, value);
        }
        private string text
        {
            get => (string)fi_text.GetValue(this);
            set => fi_text.SetValue(this, value);
        }
        private State RestoreState(SerializedProperty property)
            => (State)mi_RestoreState.Invoke(this, new[] { property });
        #endregion

        private readonly GUIContent iconToolbarPlus = EditorGUIUtility.TrIconContent("Toolbar Plus", "Add new event listener");
        private readonly GUIStyle headerBackground = "LODSliderBG";
        private readonly GUIStyle preButton = "RL FooterButton";
        private readonly Color headerColor = new Color(0.85f, 0.85f, 0.85f);

        private bool? _isFolded;
        private bool isFolded
        {
            get => _isFolded.GetValueOrDefault(EditorPrefs.GetBool($"com.lachee.utilities.UnityEventsFoldout", true));
            set
            {
                _isFolded = value;
                EditorPrefs.SetBool($"com.lachee.utilities.UnityEventsFoldout", value);
            }
        }

        private float CalculateBackgroundHeight(SerializedProperty property)
        {
            var list = reorderableList;
            if (list == null || list.count == 0 || isFolded)
                return EditorGUIUtility.singleLineHeight + 2f;

            return base.GetPropertyHeight(property, GUIContent.none);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            RestoreState(property);

            text = label.text;
            dummyEvent = GetDummyEvent(property);

            float lineHeight = EditorGUIUtility.singleLineHeight;

            // Draw the folder
            if (reorderableList.count > 0)
            {
                isFolded = !EditorGUI.Foldout(new Rect(position.x, position.y, position.width, lineHeight), !isFolded, GUIContent.none);
            }
            else
            {
                _isFolded = true;
            }

            // Draw the event box
            Rect headerRect = new Rect(position.x + 2, position.y, position.width - 2, CalculateBackgroundHeight(property));
            Rect headerLabel = new Rect(headerRect.x + 6, headerRect.y + 1, headerRect.width - 100, lineHeight);
            Rect headerAddButton = new Rect(headerRect.x + headerRect.width - 25, headerRect.y + 2, 24, headerRect.height + 4);

            // Draw the base GUI if we are unfolded
            if (!isFolded)
            {
                base.OnGUI(headerRect, property, label);
            }
            else
            {
                if (Event.current.type == EventType.Repaint)
                {
                    GUI.backgroundColor = headerColor;
                    headerBackground.Draw(headerRect, GUIContent.none, false, false, false, false);
                    GUI.backgroundColor = Color.white;
                }

                DrawEventHeader(headerLabel);
                if (GUI.Button(headerAddButton, iconToolbarPlus, preButton))
                {
                    isFolded = false;
                    OnAddEvent(reorderableList);
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return CalculateBackgroundHeight(property) + 5f;
        }

    }
}
#endif