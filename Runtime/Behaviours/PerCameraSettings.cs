using Lachee.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lachee.Behaviours
{
    [RequireComponent(typeof(Camera))]
    [ExecuteInEditMode]
    public class PerCameraSettings : MonoBehaviour
    {
        [System.Serializable]
        public sealed class RenderSettingsState
        {
            private RenderSettingsState _previousState;

            #region Fog
            [Header("Fog Settings")]
            [Toggle("#Inherit", "Inherit", true)] public bool fog;
            [HideInInspector] public bool fogInherit = true;

            [Toggle("#Inherit", "Inherit ", true)] public Color fogColor;
            [HideInInspector] public bool fogColorInherit = true;

            [Toggle("#Inherit", "Inherit ", true)] public FogMode fogMode;
            [HideInInspector] public bool fogModeInherit = true;

            [Toggle("#Inherit", "Inherit ", true)] public float fogDensity;
            [HideInInspector] public bool fogDensityInherit = true;

            [Toggle("#Inherit", "Inherit ", true)] public float fogStartDistance;
            [HideInInspector] public bool fogStartDistanceInherit = true;

            [Toggle("#Inherit", "Inherit ", true)] public float fogEndDistance;
            [HideInInspector] public bool fogEndDistanceInherit = true;
            #endregion


            /// <summary>Creates a restore point for all the settings</summary>
            public void CreateState() {
                _previousState = FromCurrentRenderSettings();
            }

            /// <summary>Applies the current settings</summary>
            public void Apply()
                => Apply(false);

            /// <summary>Reverts the settings by forcefully reverting the state</summary>
            public void Revert()
            {
                _previousState.Apply(true);
            }

            private void Apply(bool force)
            {
                if (force || !fogInherit)               RenderSettings.fog = fog;
                if (force || !fogColorInherit)          RenderSettings.fogColor = fogColor;
                if (force || !fogModeInherit)           RenderSettings.fogMode = fogMode;
                if (force || !fogDensityInherit)        RenderSettings.fogDensity = fogDensity;
                if (force || !fogStartDistanceInherit)  RenderSettings.fogStartDistance = fogStartDistance;
                if (force || !fogEndDistanceInherit)    RenderSettings.fogEndDistance = fogEndDistance;
            }

            public static RenderSettingsState FromCurrentRenderSettings()
            {
                return new RenderSettingsState() {
                    fog                 = RenderSettings.fog,
                    fogColor            = RenderSettings.fogColor,
                    fogMode             = RenderSettings.fogMode,
                    fogDensity          = RenderSettings.fogDensity,
                    fogStartDistance    = RenderSettings.fogStartDistance,
                    fogEndDistance      = RenderSettings.fogEndDistance
                };
            }
        }


        [Auto, SerializeField]
        private new Camera camera;

        public bool active = true;
        public RenderSettingsState renderSettings;

        [System.Serializable]
        public class GlobalShaderProperty
        {
            public string name = "_Global_Variable";
            public float value = 1f;

            public bool revert = false;
            public float revertValue = 0f;

        }
        public GlobalShaderProperty[] properties;

        private void OnPreRender()
        {
            if (!active) return;
#if UNITY_EDITOR && false
            if (Application.isPlaying)
#endif
            {
                renderSettings.CreateState();
                renderSettings.Apply();
            }

            foreach(var property in properties)
            {
                Shader.SetGlobalFloat(property.name, property.value);
            }
        }

        private void OnPostRender()
        {
            if (!active) return;
#if UNITY_EDITOR && false
            if (Application.isPlaying)
#endif
            {
                renderSettings.Revert();
            }
            foreach (var property in properties)
            {
                if (!property.revert) continue;
                Shader.SetGlobalFloat(property.name, property.revertValue);
            }
        }
    }
}
