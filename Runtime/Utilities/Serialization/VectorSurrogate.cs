using System.Runtime.Serialization;
using UnityEngine;

namespace Lachee.Utilities.Serialization {

    public class VectorSurrogate : ISerializationSurrogate
    {
        public int Dimensions { get; }

        public VectorSurrogate(int dimensions) {
            Dimensions = dimensions;
        }

        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            var type = obj.GetType();
            for(int i = 0; i < Dimensions; i++) {
                string name = GetDimensionName(i);
                info.AddValue(name, type.GetProperty(name).GetValue(obj, null));
            }
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            var type = obj.GetType();
            for(int i = 0; i < Dimensions; i++) {
                string name = GetDimensionName(i);
                type.GetProperty(name).SetValue(obj, info.GetValue(name, typeof(float)));
            }
            return obj;
        }

        public virtual string GetDimensionName(int index) {
            string dims = "xyzw";
            if (index < 0 || index >= dims.Length)
                throw new System.IndexOutOfRangeException();
            return dims[index].ToString();
        }
    }

    public class ColorSurrogate : VectorSurrogate {
        public ColorSurrogate() : base(4) {}

        public override string GetDimensionName(int index)
        {
            string dims = "rgba";
            if (index < 0 || index >= dims.Length)
                throw new System.IndexOutOfRangeException();
            return dims[index].ToString();
        }
    }

}