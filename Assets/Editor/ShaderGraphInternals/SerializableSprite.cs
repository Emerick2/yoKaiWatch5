using System;
using UnityEngine;

namespace UnityEditor.ShaderGraph.Internal
{
    /// <summary>
    /// Shader Graph serializes its graph data as JSON. A direct Sprite field is not
    /// enough for a sliced sub-asset, so keep the Unity object reference through
    /// EditorJsonUtility, exactly like Shader Graph's SerializableTexture does.
    /// </summary>
    [Serializable]
    public sealed class SerializableSprite : ISerializationCallbackReceiver
    {
        [SerializeField]
        private string m_SerializedSprite;

        [NonSerialized]
        private Sprite m_Sprite;

        [Serializable]
        private sealed class SpriteHelper
        {
#pragma warning disable 649
            public Sprite sprite;
#pragma warning restore 649
        }

        public Sprite sprite
        {
            get
            {
                if (!string.IsNullOrEmpty(m_SerializedSprite))
                {
                    var helper = new SpriteHelper();
                    EditorJsonUtility.FromJsonOverwrite(m_SerializedSprite, helper);
                    m_SerializedSprite = null;
                    m_Sprite = helper.sprite;
                }

                return m_Sprite;
            }
            set
            {
                m_Sprite = value;
                m_SerializedSprite = null;
            }
        }

        public void OnBeforeSerialize()
        {
            Sprite current = sprite;
            var helper = current != null
                ? new SpriteHelper { sprite = current }
                : null;

            m_SerializedSprite = EditorJsonUtility.ToJson(helper, false);
        }

        public void OnAfterDeserialize()
        {
        }
    }
}
