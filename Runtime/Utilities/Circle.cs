using Lachee.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lachee.Utilities
{
    [RequireComponent(typeof(LineRenderer))]
    [ExecuteInEditMode]
    public class Circle : MonoBehaviour
    {
        public enum Mode
        {
            XY, 
            XZ,
            ZY,
        }


        /// <summary>Current line renderer</summary>
        [Auto]
        public LineRenderer lineRenderer;

        [Tooltip("Radius of the circle")]
        [SerializeField]
        private float _radius = 0.5f;
        public float radius { get { return _radius; } set { _radius = value; UpdatePoints(); } }

        [Tooltip("How many points on the circle")]
        [Range(3, 360)]
        [SerializeField]
        private int _points = 16;
        public int points { get { return _points; } set { _points = value; UpdatePoints(); } }

        [Tooltip("Offset of the circle")]
        [SerializeField]
        private Vector3 _offset = Vector3.zero;
        public Vector3 offset { get { return _offset; } set { _offset = value; UpdatePoints(); } }

        [Tooltip("Orientation of the circle")]
        [SerializeField]
        private Mode _mode = Mode.XZ;
        public Mode mode { get { return _mode; } set { _mode = value; UpdatePoints(); } }

        private void Start()
        {
            UpdatePoints();
        }

#if UNITY_EDITOR
        private void Update() {
            if (!Application.isPlaying)
                UpdatePoints();
        }
#endif

        [ContextMenu("Update Points")]
        public void UpdatePoints()
        {
            if (lineRenderer == null)
                return;

            lineRenderer.positionCount = _points;
            lineRenderer.useWorldSpace = false;
            lineRenderer.loop = true;

            float angleOffset = 360f / _points;
            for (int i = 0; i < _points; i++)
            {
                float radians = Mathf.Deg2Rad * (angleOffset * i);
                float x = _radius * Mathf.Cos(radians);
                float y = _radius * Mathf.Sin(radians);

                Vector3 vector;
                switch(_mode)
                {
                    case Mode.XY:
                        vector = new Vector3(x, y, 0);
                        break;

                    default:
                    case Mode.XZ:
                        vector = new Vector3(x, 0, y);
                        break;

                    case Mode.ZY:
                        vector = new Vector3(0, x, y);
                        break;
                }

                lineRenderer.SetPosition(i, vector + _offset);
            }
        }
    }

}