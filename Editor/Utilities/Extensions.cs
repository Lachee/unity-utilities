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
    }
}