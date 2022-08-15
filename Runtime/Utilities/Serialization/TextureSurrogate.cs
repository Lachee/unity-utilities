using System.Runtime.Serialization;
using UnityEngine;

namespace Lachee.Utilities.Serialization
{

    public class Texture2DSurrogate : ISerializationSurrogate
    {
        private const string ValueName = "n";
        private const string ValueWidth = "w";
        private const string ValueHeight = "h";
        private const string ValueFormat = "f";
        private const string ValueMipmap = "m";
        private const string ValueFilter = "F";
        private const string ValueWrap = "w";
        private const string ValueAniso = "a";
        private const string ValueData = "D";

        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Texture2D texture = (Texture2D)obj;

#if UNITY_2018_3_OR_NEWER
            if (!texture.isReadable)
                throw new System.NotSupportedException("Textures must be readable to serialize");
#endif

            var data = texture.GetRawTextureData();
            info.AddValue(ValueName, texture.name);
            info.AddValue(ValueWidth, texture.width);
            info.AddValue(ValueHeight, texture.height);
            info.AddValue(ValueFormat, (int)texture.format);
            info.AddValue(ValueMipmap, texture.mipmapCount);
            info.AddValue(ValueFilter, (int)texture.filterMode);
            info.AddValue(ValueWrap, (int)texture.wrapMode);
            info.AddValue(ValueAniso, texture.anisoLevel);
            info.AddValue(ValueData, data);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
#if UNITY_2021_OR_NEWER
            Texture2D texture = new Texture2D(
                info.GetInt32(ValueWidth),
                info.GetInt32(ValueHeight),
                (TextureFormat) info.GetInt32(ValueFormat),
                info.GetInt32(ValueMipmap),
                false
            );
#else
            Texture2D texture = new Texture2D(
                info.GetInt32(ValueWidth),
                info.GetInt32(ValueHeight),
                (TextureFormat)info.GetInt32(ValueFormat),
                true,
                false
            );
#endif

            texture.filterMode = (FilterMode) info.GetInt32(ValueFilter);
            texture.wrapMode = (TextureWrapMode) info.GetInt32(ValueWrap);
            texture.anisoLevel = info.GetInt32(ValueAniso);
            texture.LoadRawTextureData((byte[]) info.GetValue(ValueData, typeof(byte[])));
            texture.Apply();
            obj = texture;
            return obj;
        }

    }
}