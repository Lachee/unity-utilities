using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Lachee.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public sealed class SlideAttribute : PropertyAttribute
    {
        public float Min { get; }
        public float Max { get; }

        public GUIContent LeftLabel { get; }
        public GUIContent RightLabel { get; }

        public SlideAttribute(float min, float max)
        {
            this.Min = min;
            this.Max = max;
            LeftLabel = null;
            RightLabel = null;
        }

        public SlideAttribute(float min, float max, string leftLabel, string rightLabel)
            : this(min, max, new GUIContent(leftLabel), new GUIContent(rightLabel)) { }

        public SlideAttribute(float min, float max, GUIContent leftLabel, GUIContent rightLabel)
        {
            this.Min = min;
            this.Max = max;
            this.LeftLabel = leftLabel;
            this.RightLabel = rightLabel;
        }
    }
}
