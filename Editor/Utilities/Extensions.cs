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
            var fi = property.GetSerializedFieldInfo();
            return fi?.FieldType;
        }

        /// <summary>
        /// Gets the FieldInfo of the underlying field
        /// </summary>
        /// <param name="property">The property to get the FieldInfo off</param>
        /// <returns></returns>
        public static FieldInfo GetSerializedFieldInfo(this SerializedProperty property)
        {
            BindingFlags flag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField;
            System.Type parentType = property.serializedObject.targetObject.GetType();
            return parentType.GetFieldInfoFromPath(property.propertyPath, flag);
        }

        /// <summary>
        /// Gets the field info from the given property path
        /// </summary>
        /// <param name="type"></param>
        /// <param name="path"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static FieldInfo GetFieldInfoFromPath(this System.Type type, string path, BindingFlags flag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField)
        {
            System.Type parentType = type;
            FieldInfo fi = type.GetField(path, flag);
            if (fi != null) return fi;

            string[] perDot = path.Split('.');
            foreach (string fieldName in perDot)
            {
                fi = parentType.GetField(fieldName, flag);
                if (fi != null)
                    parentType = fi.FieldType;
                else
                    return null;
            }
            if (fi != null)
                return fi;
            else return null;
        }

        /// <summary>
        /// Gets the raw value of the property
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
#if CSHARP_7_3_OR_NEWER && ENABLE_DYNAMIC
        public static dynamic GetSerializedValue(this SerializedProperty property) {
            dynamic result;
#else
        public static object GetSerializedValue(this SerializedProperty property) {
            object result;
#endif
            BindingFlags flag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField;
            result = property.serializedObject.targetObject;
            System.Type parentType = property.serializedObject.targetObject.GetType();

            // See if we can return the object directly
            string path     = property.propertyPath;
            FieldInfo fi    = null;//    = parentType.GetField(path, flag);
            
            // We need to delve deeper until we hit the final result.
            string[] perDot = path.Split('.');
            foreach (string fieldName in perDot)
            {
                fi = parentType.GetField(fieldName, flag);
                if (fi != null)
                {
                    parentType = fi.FieldType;
                    result = fi.GetValue(result);
                }
                else
                {
                    return null;
                }
            }

            return result;
        }

        /// <summary>
        /// Determines the best name for the given property
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static string GetReadableName(this SerializedProperty property)
        {
            switch(property.propertyType)
            {
                default:
                    return property.displayName;
                case SerializedPropertyType.ObjectReference:
                    return property.objectReferenceValue ? property.objectReferenceValue.name : "[ NULL ]";
            }
        }
    }
}