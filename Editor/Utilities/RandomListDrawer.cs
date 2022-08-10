using Lachee.Editor.Icons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Lachee.Utilities.Editor
{
    internal class RandomListStyle
    {
        public static readonly Color[] Colors =
        {
            new Color(0.4831376f, 0.6211768f, 0.0219608f, 1.0f),
            new Color(0.3827448f, 0.2886272f, 0.5239216f, 1.0f),
            new Color(0.8000000f, 0.4423528f, 0.0000000f, 1.0f),
            new Color(0.5333336f, 0.1600000f, 0.0282352f, 1.0f),
            new Color(0.2070592f, 0.5333336f, 0.6556864f, 1.0f),
            new Color(0.2792160f, 0.4078432f, 0.5835296f, 1.0f),
            new Color(0.4486272f, 0.4078432f, 0.0501960f, 1.0f),
            new Color(0.7749016f, 0.6368624f, 0.0250984f, 1.0f)
        };

        public readonly GUIStyle sliderBG = "LODSliderBG";
        public readonly GUIStyle sliderRange = "LODSliderRange";
        public readonly GUIStyle sliderRangeSelected = "LODSliderRangeSelected";
        public readonly GUIStyle sliderText = "LODSliderText";

        public readonly GUIStyle listFooterBackground = "RL Footer";
        public readonly GUIStyle listPreButton = "RL FooterButton";
        public readonly GUIContent listIconToolbarMinus = EditorGUIUtility.TrIconContent("Toolbar Minus", "Remove selection from the list");
        public readonly GUIContent listIconToolbarPlus = EditorGUIUtility.TrIconContent("Toolbar Plus", "Add to the list");
    }

    [CustomPropertyDrawer(typeof(RandomList<>))]
    class RandomListDrawer : UnityEditor.PropertyDrawer
    {
        private const int WEIGHT_HEIGHT = 32;
        private const int WEIGHT_RANGE_PADDING = 2;
        private static readonly int WEIGHT_SLIDER_ID = "RISTSliderIDHash".GetHashCode();

        private int _selectedSlider = -1;
        private int _selectedWeight = -1;
        private ReorderableList _list;

        private static RandomListStyle _style;
        public static RandomListStyle Style
        {
            get
            {
                if (_style == null)
                    _style = new RandomListStyle();
                return _style;
            }
        }

        private bool isListFoldout
        {
            get => EditorPrefs.GetBool($"com.lachee.utilities.RISTFoldout", false);
            set => EditorPrefs.SetBool($"com.lachee.utilities.RISTFoldout", value);
        }

        private static ReorderableList CreateReoderableList(SerializedProperty property)
        {
            SerializedProperty itemListProperty = property.FindPropertyRelative("_list");
            SerializedProperty weightListProperty = property.FindPropertyRelative("_weights");

            // Fix lists with this https://answers.unity.com/questions/1479572/how-to-properly-use-reorderablelist-inside-customp.html
            var list = new ReorderableList(property.serializedObject, itemListProperty, true, false, true, true);
            list.drawElementCallback += (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                Rect objectRect = new Rect(rect.x, rect.y + 2, rect.width * 2 / 3, rect.height - 4);
                EditorGUI.PropertyField(objectRect, itemListProperty.GetArrayElementAtIndex(index), GUIContent.none);

                Rect weightRect = new Rect(objectRect.xMax + 5, objectRect.y, rect.width - objectRect.width - 5, objectRect.height);
                EditorGUI.PropertyField(weightRect, weightListProperty.GetArrayElementAtIndex(index), GUIContent.none);
            };

            list.drawElementBackgroundCallback += (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                Event evt = Event.current;
                if (evt.type != EventType.Repaint) return;
                var previous = GUI.backgroundColor;
                
                if (index >= 0)
                    GUI.backgroundColor = RandomListStyle.Colors[index % RandomListStyle.Colors.Length];

                if (!isActive)
                    GUI.backgroundColor *= 0.6f;

                Style.sliderRange.Draw(rect, GUIContent.none, false, false, false, false);
                GUI.backgroundColor = previous;
            };

            list.onAddCallback += (_) =>
            {
                itemListProperty.arraySize++;
                weightListProperty.arraySize++;
                weightListProperty.GetArrayElementAtIndex(weightListProperty.arraySize-1).floatValue = 1f;
            };

            list.onRemoveCallback += (_) =>
            {
                int index = list.selectedIndices.First();
                itemListProperty.DeleteArrayElementAtIndex(index);
                weightListProperty.DeleteArrayElementAtIndex(index);
            };

            list.onCanRemoveCallback += (_) =>
            {
                return list.selectedIndices.Count == 1;
            };

            // For fancy footer that allows adjustment of the total sum on the fly, see the following gist:
            // https://i.lu.je/2022/Unity_HoZtERJPs6.png
            // https://gist.github.com/Lachee/e76299f00b334b741a6bccd82229e907

            list.onReorderCallbackWithDetails += (_, from, too) =>
            {
                //itemListProperty.MoveArrayElement(from, too);
                weightListProperty.MoveArrayElement(from, too);
            };

            return list;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            isListFoldout = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), isListFoldout, label);

            EditorGUI.indentLevel++;
            if (isListFoldout)
            {
                
                // Draw the regular UI
                Rect weightRect = EditorGUI.IndentedRect(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, WEIGHT_HEIGHT));
                List<WeightSlider> sliders = WeightSlider.Create(property, weightRect);
                OnSliderBoxGUI(weightRect, property, sliders);

                // Draw the list
                if (_list == null)
                    _list = CreateReoderableList(property);
                
                Rect listRect = EditorGUI.IndentedRect(new Rect(position.x, weightRect.yMax + EditorGUIUtility.singleLineHeight / 2f, position.width, position.height - weightRect.yMax));
                _list.DoList(listRect);

                Recalculate(property);
            } 
            else
            {
                // Manually draw the slider, but make it really small and sqished
                Event evt = Event.current;
                if (evt.type == EventType.Repaint)
                {
                    Rect miniBoxSliderRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, position.height);
                    List<WeightSlider> sliders = WeightSlider.Create(property, miniBoxSliderRect);
                    DrawSliderBox(miniBoxSliderRect, sliders, -1, true);
                }
            }
            EditorGUI.indentLevel--;
        }
    
        private void Recalculate(SerializedProperty property)
        {
            var context = property.GetSerializedValue();
            var method = context.GetType().GetMethod("RecalculateWeights", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.InvokeMethod);
            method.Invoke(context, new object[0]);
        }

        private void OnSliderBoxGUI(Rect groupRect, SerializedProperty property, List<WeightSlider> sliders)
        {
            int sliderId = GUIUtility.GetControlID(WEIGHT_SLIDER_ID, FocusType.Passive);
            Event evt = Event.current;

            switch (evt.GetTypeForControl(sliderId))
            {
                case EventType.Repaint:
                    DrawSliderBox(groupRect, sliders, _selectedWeight, false);
                    break;

                case EventType.MouseDown:
                    {
                        // Slightly grow position on the x because edge buttons overflow by 5 pixels
                        var barPosition = groupRect;
                        barPosition.x -= 5;
                        barPosition.width += 10;

                        if (barPosition.Contains(evt.mousePosition))
                        {
                            evt.Use();
                            GUIUtility.hotControl = sliderId;

                            // Check for button click
                            var clickedButton = false;

             
                            foreach (var weight in sliders)
                            {
                                if (weight.buttonPosition.Contains(evt.mousePosition))
                                {
                                    _selectedSlider = weight.index;
                                    clickedButton = true;
                                    break;
                                }
                            }

                            /*
                            // We dont care about selecting boxes
                            if (!clickedButton)
                            {
                                // Check for range click
                                foreach (var weight in sliders)
                                {
                                    if (weight.rangePosition.Contains(evt.mousePosition))
                                    {
                                        _selectedSlider = -1;
                                        _selectedWeight = weight.index;
                                        break;
                                    }
                                }
                            }
                            */
                        }
                        break;
                    }

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == sliderId && _selectedSlider >= 0 && sliders[_selectedSlider] != null)
                    {
                        evt.Use();


                        Debug.Assert(_selectedSlider > 0);
                        var right = sliders[_selectedSlider];
                        var left = sliders[_selectedSlider - 1];

                        var percentage = GetMouseProgressThroughRect(evt.mousePosition, groupRect);
                        percentage = Mathf.Clamp(percentage, left.startPercentage, right.endPercentage);

                        right.startPercentage = percentage;
                        right.ApplyWeightProperty();

                        left.endPercentage = percentage;
                        left.ApplyWeightProperty();

                        Recalculate(property);
                    }
                    break;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == sliderId)
                    {
                        GUIUtility.hotControl = 0;
                        _selectedSlider = -1;
                        evt.Use();
                    }
                    break;

                case EventType.DragUpdated:
                case EventType.DragPerform:
                    break;

                case EventType.DragExited:
                     evt.Use();
                    break;

                default:
                    break;
            }
        }
        private void DrawSliderBox(Rect area, List<WeightSlider> weights, int activeWeight, bool isCompact)
        {
            Style.sliderBG.Draw(area, GUIContent.none, false, false, false, false);

            var tempColor = GUI.backgroundColor;
            for (int i = 0; i < weights.Count; i++)
            {
                WeightSlider weight   = weights[i];
                Color color         = RandomListStyle.Colors[i % RandomListStyle.Colors.Length];
                string label        = weight.name;
                if (!isCompact)
                    label = string.Format("{0}\n{1:0}%", weight.name, weight.weightPercentage * 100);

                if (activeWeight == i)
                {
                    var foreground = weight.rangePosition;
                    foreground.width -= WEIGHT_RANGE_PADDING * 2;
                    foreground.height -= WEIGHT_RANGE_PADDING * 2;
                    foreground.center += new Vector2(WEIGHT_RANGE_PADDING, WEIGHT_RANGE_PADDING);

                    GUI.backgroundColor = color;
                    Style.sliderRangeSelected.Draw(weight.rangePosition, GUIContent.none, false, false, false, false);
                    if (foreground.width > 0)
                        Style.sliderRange.Draw(foreground, GUIContent.none, false, false, false, false);

                    if (!isCompact)
                        Style.sliderText.Draw(weight.rangePosition, label, false, false, false, false);
                }
                else
                {
                    //DrawLODRAnge
                    GUI.backgroundColor = color;
                    GUI.backgroundColor *= 0.6f;
                    Style.sliderRange.Draw(weight.rangePosition, GUIContent.none, false, false, false, false);
                    Style.sliderText.Draw(weight.rangePosition, label, false, false, false, false);
                }

                //DrawLODButton
                if (!isCompact && i > 0)
                {
                    GUI.DrawTexture(weight.buttonPosition, Icon.ristBar);
                    EditorGUIUtility.AddCursorRect(weight.buttonPosition, MouseCursor.ResizeHorizontal);
                }

            }
            GUI.backgroundColor = tempColor;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!isListFoldout) 
                return EditorGUIUtility.singleLineHeight;

            if (_list == null)
                _list = CreateReoderableList(property);
            return WEIGHT_HEIGHT + EditorGUIUtility.singleLineHeight*2 + _list.GetHeight();
        }

        private static float GetMouseProgressThroughRect(Vector2 position, Rect sliderRect)
        {
            var percentage = Mathf.Clamp((position.x - sliderRect.x) / sliderRect.width, 0.01f, 1.0f);
            return percentage;
        }

        class ItemWeightSerializedPair
        {
            public SerializedProperty itemProperty;
            public SerializedProperty weightProperty;

            public string name => itemProperty.GetValueName();
        }

        class WeightSlider : ItemWeightSerializedPair
        {
            public int index;

            public Rect rangePosition;
            public Rect buttonPosition;

            public float startWeight;
            public float startPercentage 
            {
                get => startWeight / sumWeight;
                set => startWeight = value * sumWeight;
            }

            public float endWeight;
            public float endPercentage
            {
                get => endWeight / sumWeight;
                set => endWeight = value * sumWeight;
            }

            public float weight => endWeight - startWeight;
            public float weightPercentage => weight / sumWeight;
            
            public float sumWeight;

            public void ApplyWeightProperty()
                => weightProperty.floatValue = weight;

            public static List<WeightSlider> Create(SerializedProperty property, Rect sliderRect)
            {
                var infos = new List<WeightSlider>();

                SerializedProperty totalWeightProperty = property.FindPropertyRelative("_sumWeight");
                SerializedProperty itemListProperty = property.FindPropertyRelative("_list");
                SerializedProperty weightListProperty = property.FindPropertyRelative("_weights");

                float weightOffset = 0;
                for (int i = 0; i < itemListProperty.arraySize; i++)
                {
                    SerializedProperty itemProperty = itemListProperty.GetArrayElementAtIndex(i);
                    SerializedProperty weightProperty = weightListProperty.GetArrayElementAtIndex(i);
                    

                    // Setup the initial info
                    var info = new WeightSlider()
                    {
                        index               = i,
                        itemProperty        = itemProperty,
                        weightProperty      = weightProperty,
                        startWeight         = weightOffset,
                        endWeight           = weightOffset + weightProperty.floatValue,
                        sumWeight           = totalWeightProperty.floatValue,
                    };

                    // Calculate the rect
                    float buttonSize = 10;
                    info.rangePosition = new Rect(sliderRect.x + info.startPercentage * sliderRect.width, sliderRect.y, info.weightPercentage * sliderRect.width, sliderRect.height);
                    info.buttonPosition = new Rect(info.rangePosition.x - (buttonSize / 2), info.rangePosition.y, buttonSize, info.rangePosition.height);
                    infos.Add(info);

                    weightOffset += info.weight;
                }

                return infos;
            }

        }
    
    }




    [Obsolete, CustomPropertyDrawer(typeof(Rist<>))]
    class RistDrawer : RandomListDrawer { }
}
