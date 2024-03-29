using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace UI.Dialogs
{
    [Serializable]
    public class uDialog_ColorScheme
    {
        [SerializeField]
        public string Name;
        [SerializeField]
        public SerializableColor TitleBackground;
        [SerializeField]
        public SerializableColor TitleText;
        [SerializeField]
        public SerializableColor TitleTextEffect;
        [SerializeField]
        public SerializableColor ViewportBackground;
        [SerializeField]
        public SerializableColor ViewportText;
        [SerializeField]
        public SerializableColor ViewportTextEffect;
        [SerializeField]
        public SerializableColor ButtonBackground;
        [SerializeField]
        public SerializableColor ButtonHighlight;
        [SerializeField]
        public SerializableColor ButtonText;
        [SerializeField]
        public SerializableColor ButtonTextEffect;        
        [SerializeField]
        public SerializableColor Shadow;
        [SerializeField]
        public SerializableColor Glow;
        [SerializeField]
        public SerializableColor ScreenOverlay;
        [SerializeField]
        public SerializableColor Icon_Information;
        [SerializeField]
        public SerializableColor Icon_Question;
        [SerializeField]
        public SerializableColor Icon_Warning;
    }

    [Serializable]
    public class SerializableColor
    {
        [SerializeField]
        float r, g, b, a;
        
        public Color Color
        {
            get
            {
                return new Color(r, g, b, a);
            }
            set
            {
                r = value.r;
                g = value.g;
                b = value.b;
                a = value.a;
            }
        }

        public SerializableColor(Color color)
        {
            this.Color = color;
        }

        public SerializableColor(float r, float g, float b, float a = 1f)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public SerializableColor()
        {
            this.Color = Color.black;
        }

        // These operators allow us to use SerializableColor and Color interchangeably
        public static implicit operator Color(SerializableColor c)
        {
            if (c == null) return Color.black;

            return c.Color;
        }

        public static implicit operator SerializableColor(Color c)
        {
            return new SerializableColor(c);
        }
    }

}
