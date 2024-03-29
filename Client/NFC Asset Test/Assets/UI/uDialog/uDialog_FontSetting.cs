using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

namespace UI.Dialogs
{
    [Serializable]
    public class uDialog_FontSetting
    {
        [SerializeField]
        private Font _Font;
        [SerializeField]
        private FontStyle _FontStyle;
        [SerializeField]
        private int _FontSize;
        [SerializeField]
        private eTextEffect _TextEffect;

        public Font Font
        {
            get
            {
                return _Font;
            }
            set
            {
                var changed = _Font != value;

                _Font = value;

                Update(changed);
            }
        }

        public FontStyle FontStyle
        {
            get
            {
                return _FontStyle;
            }
            set
            {
                var changed =_FontStyle != value;

                _FontStyle = value;

                Update(changed);
            }
        }

        public int FontSize
        {
            get
            {
                return _FontSize;
            }
            set
            {
                var changed = _FontSize != value;

                _FontSize = value;

                Update(changed);
            }
        }

        public eTextEffect TextEffect
        {
            get
            {
                return _TextEffect;
            }
            set
            {
                var changed = _TextEffect != value;

                _TextEffect = value;

                Update(changed);
            }
        }

        [SerializeField]
        public Text TextObject;
        
        [SerializeField]
        protected Shadow _Shadow;
        public Shadow Shadow
        {
            get
            {
                if (_Shadow == null)
                {
                    _Shadow = TextObject.GetComponent<Shadow>();
                    if (_Shadow == null)
                    {
                        _Shadow = TextObject.gameObject.AddComponent<Shadow>();
                    }
                }

                return _Shadow;
            }
        }

        [SerializeField]
        protected Outline _Outline;
        public Outline Outline
        {
            get
            {
                if (_Outline == null)
                {
                    _Outline = TextObject.GetComponent<Outline>();
                    if (_Outline == null)
                    {
                        _Outline = TextObject.gameObject.AddComponent<Outline>();
                    }
                }

                return _Outline;
            }
        }

        [SerializeField]
        public Action UpdateCallback;        

        public bool Initialised { get; protected set; }


        public void Update(bool changed = false)
        {
            if (TextObject == null) return;          

            TextObject.font = this.Font;
            TextObject.fontSize = this.FontSize;
            TextObject.fontStyle = this.FontStyle;

            switch (TextEffect)
            {
                case eTextEffect.None:
                    {
                        Shadow.enabled = false;
                        Outline.enabled = false;
                    }
                    break;

                case eTextEffect.Shadowed:
                    {
                        Shadow.enabled = true;
                        Outline.enabled = false;
                    }
                    break;

                case eTextEffect.Outline:
                    {
                        Shadow.enabled = false;
                        Outline.enabled = true;
                    }
                    break;
            }

            if (UpdateCallback != null && changed)
            {
                UpdateCallback();
            }
        }
        
        public uDialog_FontSetting(Text textObject)
        {            
            _Font = textObject.font;
            _FontSize = textObject.fontSize;
            _FontStyle = textObject.fontStyle;

            TextObject = textObject;

            TextEffect = Shadow.enabled ? eTextEffect.Shadowed : Outline.enabled ? eTextEffect.Outline : eTextEffect.None;

            Initialised = true;

            Update();            
        }

        public void SetTextEffectColor(Color newColor)
        {
            if (TextObject == null) return;

            Shadow.effectColor = newColor;
            Outline.effectColor = newColor;            
        }
    }
}
