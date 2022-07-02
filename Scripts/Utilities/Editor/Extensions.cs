#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;

namespace Lachee.Utilities.Editor
{
    public static class SerializedPropertyExtensions
    {
        /// <summary>
        /// Gets the type of the underlying field the SerializedProperty is of.
        /// </summary>
        /// <param name="property">The property to get the type from</param>
        /// <returns></returns>
        public static System.Type GetSerializedType(this SerializedProperty property)
        {
            BindingFlags flag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField;

            System.Type parentType = property.serializedObject.targetObject.GetType();
            FieldInfo fi = parentType.GetField(property.propertyPath, flag);
            return fi.FieldType;
        }
    }
}
#endif