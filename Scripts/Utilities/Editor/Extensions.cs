#if UNITY_EDITOR
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
            System.Type parentType = property.serializedObject.targetObject.GetType();
            System.Reflection.FieldInfo fi = parentType.GetField(property.propertyPath);
            return fi.FieldType;
        }
    }
}
#endif