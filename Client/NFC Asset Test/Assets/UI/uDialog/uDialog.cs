using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UI.Dialogs
{
    [ExecuteInEditMode]
    public class uDialog : MonoBehaviour
    {
        #region Basic
        public bool VisibleOnStart = true;
        
        [SerializeField]
        private bool _isVisible = true;
        public bool isVisible 
        {
            get 
            { 
                return _isVisible; 
            }
            protected set
            {
                _isVisible = value;
            }
        }
        #endregion

        #region Title
        public bool ShowTitle = true;
        public bool ShowTitleCloseButton = true;
        public bool ShowTitleMinimizeButton = false;        
        public string TitleText = "Dialog Title";        
        #endregion

        #region Content
        [TextArea]
        public string ContentText = "Message Dialog Text";        
        #endregion

        #region Icon
        public eIconType IconType = eIconType.Information;
        public Sprite Icon_Information = null;
        public Color Icon_Information_Color = new Color(1, 1, 1);
        public Sprite Icon_Warning = null;
        public Color Icon_Warning_Color = new Color(1, 0.75f, 0.75f);
        public Sprite Icon_Question = null;
        public Color Icon_Question_Color = new Color(0.9f, 0.75f, 0.5f);
        #endregion        

        #region Buttons
        [SerializeField]
        private bool _ShowButtons = false;
        public bool ShowButtons
        {
            get
            {
                return _ShowButtons;
            }
            set
            {
                _ShowButtons = value;

                UpdateButtons();
            }
        }

        public bool CloseWhenAnyButtonClicked = true;
        protected bool closeOnNextUpdate = false;
        
        public List<uDialog_Button_Data> Buttons = new List<uDialog_Button_Data>();
        #endregion

        #region Modal
        public bool Modal = false;                
        public bool CloseWhenScreenOverlayIsClicked = false;
        #endregion        

        #region Auto Close
        [Tooltip("Should this dialog automatically close itself?")]
        public bool AutoClose = false;
        [Tooltip("How long should this dialog wait before closing itself?")]            
        public float AutoCloseTime = 10f;
        private float TimeLeftUntilClose = 10f;
        #endregion

        #region Theme
        public eThemeImageSet ThemeImageSet = eThemeImageSet.RoundedEdges;

        public Sprite Image_Base;
        public Sprite Image_Outline;
        public Sprite Image_Title;
        public Sprite Image_Viewport;
        public Sprite Image_Button;
        public Sprite Image_CloseButton;

        public eOutlineMode OutlineMode = eOutlineMode.Glow;

        [SerializeField]
        public uDialog_FontSetting TitleFontSettings;
        [SerializeField]
        public uDialog_FontSetting ContentFontSettings;
        [SerializeField]
        public uDialog_FontSetting ButtonFontSettings;

        [SerializeField]
        private string _ColorScheme = "Light";        
        public string ColorScheme
        {
            get
            {
                return _ColorScheme;
            }
            set
            {
                if (_ColorScheme != value)
                {
                    _ColorScheme = value;

                    ColorSchemeChanged();                    
                }                
            }
        }
        
        private string previousColorScheme = "";

        [SerializeField]
        private Color _Color_TitleBackground = Color.black;        
        public Color Color_TitleBackground 
        {
            get
            {                    
                return _Color_TitleBackground;
            }
            set
            {
                _Color_TitleBackground = value;
                if (GO_Title != null) GO_Title.color = value;                 
            }
        }

        [SerializeField]
        private Color _Color_TitleText = Color.black;
        public Color Color_TitleText 
        {
            get
            {
                return _Color_TitleText;
            }
            set
            {
                _Color_TitleText = value;
                if (GO_TitleText != null) GO_TitleText.color = value;                
            }
        }

        [SerializeField]
        private Color _Color_TitleTextEffect = Color.black;
        public Color Color_TitleTextEffect
        {
            get
            {
                return _Color_TitleTextEffect;
            }
            set
            {
                _Color_TitleTextEffect = value;
                if (TitleFontSettings != null) TitleFontSettings.SetTextEffectColor(value);                
            }
        }

        [SerializeField]
        private Color _Color_ViewportBackground = Color.black;
        public Color Color_ViewportBackground 
        {
            get
            {
                return _Color_ViewportBackground;
            }
            set
            {
                _Color_ViewportBackground = value;
                if (GO_Viewport != null)
                {
                    GO_Viewport.color = value;
                    GO_Container.color = value;
                }
            }
        }

        [SerializeField]
        public Color _Color_ViewportText;
        public Color Color_ViewportText 
        {
            get
            {
                return _Color_ViewportText;
            }
            set
            {
                _Color_ViewportText = value;
                if(GO_MessageText != null) GO_MessageText.color = value;
            }
        }

        [SerializeField]
        private Color _Color_ViewportTextEffect = Color.black;
        public Color Color_ViewportTextEffect
        {
            get
            {
                return _Color_ViewportTextEffect;
            }
            set
            {
                _Color_ViewportTextEffect = value;
                if (ContentFontSettings != null) ContentFontSettings.SetTextEffectColor(value);
            }
        }

        [SerializeField]
        private Color _Color_ButtonBackground = Color.white;
        public Color Color_ButtonBackground 
        {
            get
            {
                return _Color_ButtonBackground;
            }
            set
            {
                _Color_ButtonBackground = value;
                
                UpdateButtonColors();
            }
        }

        [SerializeField]
        private Color _Color_ButtonHighlight = Color.white;
        public Color Color_ButtonHighlight
        {
            get
            {
                return _Color_ButtonHighlight;
            }
            set
            {
                _Color_ButtonHighlight = value;

                UpdateButtonColors();
            }
        }

        [SerializeField]
        private Color _Color_ButtonText = Color.black;
        public Color Color_ButtonText 
        {
            get
            {
                return _Color_ButtonText;
            }
            set
            {
                _Color_ButtonText = value;

                UpdateButtonColors();
            }
        }

        [SerializeField]
        private Color _Color_ButtonTextEffect = Color.black;
        public Color Color_ButtonTextEffect
        {
            get
            {
                return _Color_ButtonTextEffect;
            }
            set
            {
                _Color_ButtonTextEffect = value;

                if (ButtonFontSettings != null) ButtonFontSettings.SetTextEffectColor(value);
            }
        }

        [SerializeField]
        private Color _Color_Glow = Color.white;
        public Color Color_Glow 
        {
            get
            {
                return _Color_Glow;
            }
            set
            {
                _Color_Glow = value;

                UpdateOutlineColor();
            }
        }

        [SerializeField]
        private Color _Color_Shadow = Color.black;
        public Color Color_Shadow 
        {
            get
            {
                return _Color_Shadow;
            }
            set
            {
                _Color_Shadow = value;

                UpdateOutlineColor();
            }
        }

        [SerializeField]
        private Color _Color_ScreenOverlay = new Color(0, 0, 0, 0.25f);
        public Color Color_ScreenOverlay
        {
            get
            {
                return _Color_ScreenOverlay;
            }
            set
            {
                _Color_ScreenOverlay = value;

                if (GO_ScreenOverlayImage != null) GO_ScreenOverlayImage.color = value;
            }
        }
        #endregion

        #region Events
        [Tooltip("If this is set, then this uDialog will be closed if it is clicked anywhere (with the exception of buttons)")]
        public bool CloseWhenClicked = false;

        [SerializeField]
        public uDialog_Event Event_OnShow = null;
        [SerializeField]
        public uDialog_Event Event_OnClose = null;
        [SerializeField]
        public uDialog_Event Event_OnClick = null;
        [SerializeField]
        public uDialog_Event Event_OnMinimize = null;
        [SerializeField]
        public uDialog_Event Event_OnMaximize = null;

        public bool TriggerOnClickEventWhenOverlayIsClicked = false;

        [Serializable]
        public class uDialog_Event : UnityEvent<uDialog> { };
        #endregion

        #region Animation
        public eShowAnimation ShowAnimation = eShowAnimation.None;
        public eCloseAnimation CloseAnimation = eCloseAnimation.None;
        public Animator Animator { get; protected set; }

        protected Vector3 initialPosition { get; set; }

        protected CanvasGroup _CanvasGroup;
        public CanvasGroup CanvasGroup
        {
            get
            {
                if (_CanvasGroup == null)
                {
                    _CanvasGroup = GO_Dialog.GetComponent<CanvasGroup>();
                    if (_CanvasGroup == null)
                    {
                        _CanvasGroup = GO_Dialog.AddComponent<CanvasGroup>();
                    }
                }

                return _CanvasGroup;
            }
        }
        #endregion

        #region Audio
        public AudioClip OnShowSound;
        public AudioClip OnCloseSound;
        public AudioClip OnButtonClickSound;
        public float AudioVolume = 1f;
        [Tooltip("Optional - use this if you wish the UI sounds for this uDialog to be part of an Audio Mixer Group.")]
        public UnityEngine.Audio.AudioMixerGroup AudioMixerGroup;
        protected AudioSource _AudioSource;        
        #endregion

        #region Dragging and Resizing
        public bool AllowDraggingViaTitle = false;
        public bool AllowDraggingViaDialog = false;
        public bool RestrictToParentBounds = true;

        public bool AllowResizeFromLeft = false;
        public bool AllowResizeFromRight = false;
        public bool AllowResizeFromBottom = false;

        public Vector2 MinSize = new Vector2();
        public Vector2 MaxSize = new Vector2();

        public bool AllowResizeToAdjustPivot = true;
        #endregion

        #region Focus
        public bool FocusOnClick = true;
        public bool FocusOnMouseOver = false;
        public bool FocusOnShow = true;
        #endregion

        #region Editor Variables
        [SerializeField]
        public bool editor_showReferencesSection = false;
        [SerializeField]
        public bool editor_showTitleSection = true;
        [SerializeField]
        public bool editor_showContentSection = true;
        [SerializeField]
        public bool editor_showIconDefinitionSection = false;
        [SerializeField]
        public bool editor_showButtonsSection = false;
        [SerializeField]
        public bool editor_showModalSection = true;
        [SerializeField]
        public bool editor_showAutoCloseSection = false;
        [SerializeField]
        public bool editor_showOutlineSection = true;
        [SerializeField]
        public bool editor_showThemeSection = true;
        [SerializeField]
        public bool editor_showThemeColorsSection = true;
        [SerializeField]
        public bool editor_showEventsSection = false;
        [SerializeField]
        public bool editor_showAnimationSection = false;
        [SerializeField]
        public bool editor_showMiscSection = false;
        [SerializeField]
        public bool editor_showAudioSection = false;
        [SerializeField]
        public bool editor_showDragAndResizeSection = false;
        [SerializeField]
        public bool editor_showFocusSection = false;
        #endregion

        #region Misc
        public bool DestroyAfterClose = false;
        public bool ShowCalledThisFrame { get; protected set; }
        private bool started = false;
        private bool closing = false;        

        protected RectTransform _RectTransform;
        public RectTransform RectTransform
        {
            get
            {
                if (_RectTransform == null)
                {
                    _RectTransform = this.transform as RectTransform;
                }

                return _RectTransform;
            }
        }
        #endregion

        #region References
        public GameObject GO_Dialog = null;        
        public Image GO_Container = null;
        public Image GO_Title = null;
        public Button GO_TitleCloseButton = null;
        public Button GO_TitleMinimizeButton = null;        
        public Text GO_TitleText = null;        
        public Image GO_Viewport = null;
        public GameObject GO_MessageContainer = null;
        public Text GO_MessageText = null;
        public Image GO_Icon = null;
        public Image GO_Outline = null;
        public Image GO_ScreenOverlayImage = null;
        public uDialog_ScreenOverlay GO_ScreenOverlay = null;
        public GameObject GO_ButtonContainer = null;
        public uDialog_Button GO_ButtonTemplate = null;
        public RuntimeAnimatorController RuntimeAnimatorController = null;
        public List<uDialog_ResizeListener> ResizeListeners = null;
        public GameObject GO_Content = null;

        public uDialog_TaskBar GO_TaskBar = null;

        private Canvas m_canvas;
        protected Canvas canvas
        {
            get
            {                
                if (m_canvas == null) m_canvas = GetComponentInParent<Canvas>();
                return m_canvas;
            }
        }
        #endregion        

        #region Unity Functions
        void OnValidate()
        {
            if (Application.isPlaying)
            {
                if (gameObject.activeInHierarchy)
                {
                    StartCoroutine(DelayedCall(0.01f, UpdateDisplay));
                }
            }
            else
            {   
#if UNITY_EDITOR
                UnityEditor.EditorApplication.delayCall += UpdateDisplay;
#endif
            }
        }

        void Update()
        {
            if (!Application.isPlaying) return;

            if (this.isVisible)
            {
                if (AutoClose)
                {
                    TimeLeftUntilClose -= Time.deltaTime;
                    if (TimeLeftUntilClose <= 0)
                    {
                        this.Close();
                    }
                }

                if (closeOnNextUpdate)
                {
                    this.Close();

                    closeOnNextUpdate = false;
                }
            }

            ShowCalledThisFrame = false;
        }        

        public void Start()
        {
            if (started) return;

            started = true;

            if (!TitleFontSettings.Initialised) TitleFontSettings = new uDialog_FontSetting(GO_TitleText);
            if (!ContentFontSettings.Initialised) ContentFontSettings = new uDialog_FontSetting(GO_MessageText);
            if (!ButtonFontSettings.Initialised) ButtonFontSettings = new uDialog_FontSetting(GO_ButtonTemplate.GetComponentInChildren<Text>());

            // If the button font settings change, force a full button update
            ButtonFontSettings.UpdateCallback = ForceButtonUpdate;

            if (!Application.isPlaying) return;           

            Animator = GO_Dialog.GetComponent<Animator>();

            if (Animator == null)
            {                
                Animator = GO_Dialog.AddComponent<Animator>();                   
            }            

            initialPosition = this.transform.localPosition;            

            if (VisibleOnStart || ShowCalledThisFrame)
            {
                Show();
            }
            else
            {
                Close(false, false);
            }            
        }
        #endregion

        public void UpdateDisplay()
        {
            if (this == null) return; // if this object has been destroyed            
            
            UpdateTitle();
            UpdateModal();
            UpdateOutlineMode();
            UpdateText();
            UpdateIcon();
            UpdateThemeImages();
            
            if (previousColorScheme != ColorScheme)
            {
                ColorSchemeChanged();
            }
            else
            {
                UpdateColorScheme();            
            }

            if (ButtonFontSettings != null)
            {
                // Apply the settings to the button template
                ButtonFontSettings.Update();                                
            }
            
            UpdateButtons();

            if (TitleFontSettings != null) TitleFontSettings.Update();
            if (ContentFontSettings != null) ContentFontSettings.Update();

            // hide the MessageContainer entirely if there is no icon set and no message text
            if (GO_MessageContainer != null)
            {
                GO_MessageContainer.gameObject.SetActive(!(IconType == eIconType.None && String.IsNullOrEmpty(ContentText)));
            }

            UpdateResizeListeners();            
        }

        void UpdateTitle()
        {
            if (GO_Title != null)
            {
                if (GO_TitleText != null)
                {
                    GO_TitleText.text = TitleText;
                }                

                GO_Title.gameObject.SetActive(ShowTitle);

                if (GO_TitleCloseButton != null)
                {
                    GO_TitleCloseButton.gameObject.SetActive(ShowTitleCloseButton);
                }

                if (GO_TitleMinimizeButton != null)
                {
                    GO_TitleMinimizeButton.gameObject.SetActive(ShowTitleMinimizeButton);
                }                

                var viewportImageComponent = GO_Viewport.GetComponent<Image>();
                if (ShowTitle)
                {
                    viewportImageComponent.enabled = true;
                }
                else
                {
                    viewportImageComponent.enabled = false;
                }
            }
        }

        void UpdateOutlineMode()
        {
            if (GO_Outline != null)
            {                
                GO_Outline.gameObject.SetActive(OutlineMode != eOutlineMode.None);

                var outlineTransform = GO_Outline.rectTransform;

                switch (OutlineMode)
                {
                    case eOutlineMode.Glow:
                        outlineTransform.offsetMin = new Vector2(-8, -8);
                        outlineTransform.offsetMax = new Vector2(8, 8);
                        break;
                    case eOutlineMode.Shadow:
                        outlineTransform.offsetMin = new Vector2(0, -12);
                        outlineTransform.offsetMax = new Vector2(12, 0);
                        break;
                }
            }
        }

        void UpdateModal()
        {
            if (GO_ScreenOverlayImage != null)
            {
                GO_ScreenOverlayImage.gameObject.SetActive(this.Modal);

                var screenOverlayTransform = GO_ScreenOverlayImage.transform as RectTransform;
                // Basically, make the Screen overlay _really_ really big - so that we never see the edges of it
                screenOverlayTransform.sizeDelta = new Vector2(Screen.currentResolution.width * 10, Screen.currentResolution.height * 10);
                
                GO_ScreenOverlayImage.GetComponent<Image>().raycastTarget = this.Modal;
            }
        }

        void UpdateText()
        {
            if (GO_MessageText != null)
            {
                GO_MessageText.text = this.ContentText;

                if (String.IsNullOrEmpty(this.ContentText))
                {
                    GO_MessageText.gameObject.SetActive(false);
                }
                else
                {
                    GO_MessageText.gameObject.SetActive(true);
                }
            }
        }

        void UpdateIcon()
        {
            if (GO_Icon != null)
            {
                switch (IconType)
                {
                    case eIconType.None:
                        {
                            GO_Icon.gameObject.SetActive(false);
                        }
                        break;

                    default:
                        {
                            GO_Icon.gameObject.SetActive(true);
                            switch (IconType)
                            {
                                case eIconType.Information:
                                    GO_Icon.sprite = Icon_Information;
                                    GO_Icon.color = Icon_Information_Color;
                                    break;
                                case eIconType.Question:
                                    GO_Icon.sprite = Icon_Question;
                                    GO_Icon.color = Icon_Question_Color;
                                    break;
                                case eIconType.Warning:
                                    GO_Icon.sprite = Icon_Warning;
                                    GO_Icon.color = Icon_Warning_Color;
                                    break;

                                // If type == Custom, then do nothing (leave the image alone)
                            }
                        }
                        break;
                }
            }
        }

        void UpdateThemeImages()
        {            
            var currentImages = new Dictionary<string, Sprite>()
            {
                {"Base", GO_Container.sprite},
                {"Outline", GO_Outline.sprite},
                {"Title", GO_Title.sprite},
                {"Viewport", GO_Viewport.sprite}
            };

            var newImages = new Dictionary<string, Sprite>();
            var imageTypes = new List<string>() { "Base", "Outline", "Title", "Viewport", "Button", "CloseButton" };
            var imagePathBase = "";

            switch (ThemeImageSet)
            {
                case eThemeImageSet.RoundedEdges:
                    imagePathBase = "Rounded Edges";
                    break;
                case eThemeImageSet.SharpEdges:
                    imagePathBase += "Sharp Edges";
                    break;
                case eThemeImageSet.NoImages:
                case eThemeImageSet.Custom:
                    // do nothing
                    break;                
                default:
                    imagePathBase += ThemeImageSet.ToString();
                    break;

            }

            if (ThemeImageSet == eThemeImageSet.Custom)
            {
                if (Image_Base != null) newImages.Add("Base", Image_Base);
                if (Image_Viewport != null) newImages.Add("Viewport", Image_Viewport);
                if (Image_Title != null) newImages.Add("Title", Image_Title);
                if (Image_Outline != null) newImages.Add("Outline", Image_Outline);
                if (Image_Button != null) newImages.Add("Button", Image_Button);
                if (Image_CloseButton != null)
                {
                    newImages.Add("CloseButton", Image_CloseButton);
                }
                else if (Image_Button != null)
                {
                    newImages.Add("CloseButton", Image_Button);
                }

            }
            else
            {
                foreach (var imageType in imageTypes)
                {
                    if (ThemeImageSet == eThemeImageSet.NoImages)
                    {
                        newImages.Add(imageType, null);
                    }
                    else
                    {
                        var sprite = uDialog_Utilities.LoadResource<Sprite>(string.Format("{0}/uDialog_{1}_{2}", imagePathBase, ThemeImageSet.ToString(), imageType));

                        if (sprite != null)
                        {
                            newImages.Add(imageType, sprite);
                        }
                    }
                }
            }

            if (newImages.Any())
            {                
                foreach (var imageType in newImages)
                {
                    if (currentImages.ContainsKey(imageType.Key) && imageType.Value == currentImages[imageType.Key]) continue;

                    switch (imageType.Key)
                    {
                        case "Base":
                            GO_Container.sprite = imageType.Value;
                            GO_TitleCloseButton.GetComponent<Image>().sprite = imageType.Value;
                            if (GO_TitleMinimizeButton != null) GO_TitleMinimizeButton.GetComponent<Image>().sprite = imageType.Value;
                            break;
                        case "Outline":
                            GO_Outline.sprite = imageType.Value;
                            break;
                        case "Title":
                            GO_Title.sprite = imageType.Value;
                            break;
                        case "Viewport":
                            GO_Viewport.sprite = imageType.Value;                            
                            break;
                        case "Button":
                            var buttons = this.GetComponentsInChildren<Button>(true);
                            foreach (var button in buttons)
                            {
                                button.GetComponent<Image>().sprite = imageType.Value;
                            }
                            break;
                        case "CloseButton":
                            GO_TitleCloseButton.GetComponent<Image>().sprite = imageType.Value;
                            if (GO_TitleMinimizeButton != null) GO_TitleMinimizeButton.GetComponent<Image>().sprite = imageType.Value;
                            break;
                    }
                }                
            }            
        }

        public void ColorSchemeChanged()
        {            
            // don't change anything if the color scheme is custom
            if (ColorScheme != "Custom")
            {
                if (uDialog_Utilities.ColorSchemes.ContainsKey(ColorScheme))
                {
                    var colorScheme = uDialog_Utilities.ColorSchemes[ColorScheme];

                    LoadColorScheme(colorScheme);
                }
            }

            previousColorScheme = ColorScheme;

            UpdateColorScheme();
        }

        public void LoadColorScheme(uDialog_ColorScheme colorScheme)
        {
            Color_TitleBackground = colorScheme.TitleBackground;
            Color_TitleText = colorScheme.TitleText;
            _Color_TitleTextEffect = colorScheme.TitleTextEffect;
            Color_ViewportBackground = colorScheme.ViewportBackground;
            Color_ViewportText = colorScheme.ViewportText;
            Color_ViewportTextEffect = colorScheme.ViewportTextEffect;
            Color_Glow = colorScheme.Glow;
            Color_Shadow = colorScheme.Shadow;
            Color_ButtonBackground = colorScheme.ButtonBackground;
            Color_ButtonHighlight = colorScheme.ButtonHighlight;
            Color_ButtonText = colorScheme.ButtonText;
            Color_ButtonTextEffect = colorScheme.ButtonTextEffect;
            Color_ScreenOverlay = colorScheme.ScreenOverlay;

            Icon_Information_Color = colorScheme.Icon_Information;
            Icon_Warning_Color = colorScheme.Icon_Warning;
            Icon_Question_Color = colorScheme.Icon_Question;
        }

        void UpdateColorScheme()
        {
            // this fires the set events of each of these
            Color_TitleBackground = Color_TitleBackground;
            Color_TitleText = Color_TitleText;
            Color_ViewportBackground = Color_ViewportBackground;
            Color_ViewportText = Color_ViewportText;
            Color_Glow = Color_Glow;
            Color_Shadow = Color_Shadow;
            Color_ButtonBackground = Color_ButtonBackground;
            Color_ButtonHighlight = Color_ButtonHighlight;
            Color_ButtonText = Color_ButtonText;            

            UpdateOutlineMode();
            UpdateButtonColors();
        }

        void UpdateOutlineColor()
        {
            switch (this.OutlineMode)
            {
                case eOutlineMode.Glow:
                    this.GO_Outline.color = Color_Glow;
                    break;
                case eOutlineMode.Shadow:
                    this.GO_Outline.color = Color_Shadow;
                    break;
            }
        }

        void UpdateButtonColors()
        {
            var buttons = this.GetComponentsInChildren<Button>(true);
            foreach (var button in buttons)
            {                                
                var colors = button.colors;

                button.GetComponent<Image>().color = Color.white;

                colors.normalColor = Color_ButtonBackground;
                colors.highlightedColor = Color_ButtonHighlight;                                
                colors.pressedColor = Color_ButtonBackground;
                colors.disabledColor = new Color(Color_ButtonBackground.r, Color_ButtonBackground.g, Color_ButtonBackground.b, 0.5f);

                button.colors = colors;                
                
                button.GetComponentInChildren<Text>().color = Color_ButtonText;
            }
        }

        public void UpdateButtons()
        {
            if (!this.gameObject.activeInHierarchy) return;

            GO_ButtonContainer.SetActive(ShowButtons);

            if (!ShowButtons) return;
            
            var existingButtonComponents = GO_ButtonContainer.GetComponentsInChildren<uDialog_Button>(true).Where(b => !b.IsTemplate).ToList();
            foreach (var buttonData in Buttons)
            {                
                var button = existingButtonComponents.FirstOrDefault(b => b.ButtonData.Guid == buttonData.Guid);

                if (button == null)
                {
                    // Instantiate
                    button = Instantiate(GO_ButtonTemplate);
                    button.transform.SetParent(GO_ButtonContainer.transform);
                    button.IsTemplate = false;
                    button.gameObject.SetActive(true);
                    button.transform.localScale = Vector3.one;
                    button.transform.localPosition = Vector3.zero;
                }                
                
                button.SetData(buttonData);
                if (button.buttonComponent.onClick == null)
                {
                    button.buttonComponent.onClick = new Button.ButtonClickedEvent();
                }

                button.buttonComponent.onClick.RemoveListener(ButtonClicked); // if this event handler has already been added, remove it
                button.buttonComponent.onClick.AddListener(ButtonClicked); // now add it (this helps ensure the event handler will only be called once)
                                  
                existingButtonComponents.Remove(button);
            }
            
            // remove any button components that weren't found in Buttons
            foreach (var button in existingButtonComponents)
            {
                if (Application.isPlaying)
                {
                    GameObject.Destroy(button.gameObject);
                }
                else
                {                    
                    GameObject.DestroyImmediate(button.gameObject);
                }
            }

            UpdateThemeImages();
            UpdateButtonColors();
        }

        /// <summary>
        /// Set the Icon used by this uDialog
        /// </summary>
        /// <param name="newIcon"></param>
        internal void SetIconSprite(Sprite newIcon)
        {
            this.IconType = eIconType.Custom;
            GO_Icon.sprite = newIcon;

            UpdateIcon();
        }

        /// <summary>
        /// Set the Icon (and its color) for this uDialog
        /// </summary>
        /// <param name="newIcon"></param>
        /// <param name="newColor"></param>
        internal void SetIconSprite(Sprite newIcon, Color newColor)
        {
            GO_Icon.color = newColor;
            SetIconSprite(newIcon);
        }

        public void SetIconType(eIconType newIconType)
        {
            this.IconType = newIconType;
            UpdateIcon();
        }

        public void Show(bool fireOnShowEvent = true)
        {
            ShowCalledThisFrame = true;
            isVisible = true;
            this.gameObject.SetActive(true);

            // bring to front
            if(FocusOnShow) Focus();

            if (ShowAnimation != eShowAnimation.None)
            {
                FadeInOverlay();
                PlayAnimation(ShowAnimation.ToString());
            }
            else
            {
                FadeInOverlay(true);
            }            

            if (AutoClose)
            {
                TimeLeftUntilClose = AutoCloseTime;
            }

            if (fireOnShowEvent && Event_OnShow != null)
            {
                Event_OnShow.Invoke(this);
            }

            UpdateButtons();

            PlaySound(this.OnShowSound, this.AudioVolume);

            if (GO_TaskBar != null)
            {
                GO_TaskBar.UpdateDisplayDelayed();
            }
        }

        public void Close()
        {
            Close(true);
        }

        public void Close(bool fireOnCloseEvent)
        {
            Close(fireOnCloseEvent, true);
        }

        public void Close(bool fireOnCloseEvent, bool animate, bool minimize = false)
        {
            isVisible = false;

            if (Application.isPlaying && animate &&  CloseAnimation != eCloseAnimation.None)
            {
                closing = true;
                foreach (var button in this.gameObject.GetComponentsInChildren<uDialog_Button>())
                {
                    if (button.IsTemplate) continue;

                    button.buttonComponent.interactable = false;
                }

                FadeOutOverlay();
                PlayAnimation(CloseAnimation.ToString());
                
                if (this.gameObject.activeInHierarchy)
                {
                    initialPosition = gameObject.transform.localPosition;

                    StartCoroutine(DisableWhenAnimationIsComplete(minimize));
                }
            }
            else
            {
                FadeOutOverlay(true);
                this.gameObject.SetActive(false);                
            }
            
            if (!minimize && fireOnCloseEvent && Event_OnClose != null)
            {
                Event_OnClose.Invoke(this);
            }
            
            PlaySound(this.OnCloseSound, this.AudioVolume);

            if (!minimize && GO_TaskBar != null)
            {
                GO_TaskBar.RemoveTask(this);
            }
        }

        public void Minimize()
        {
            if (Event_OnMinimize != null) Event_OnMinimize.Invoke(this);

            if (GO_TaskBar != null)
            {
                GO_TaskBar.UpdateDisplayDelayed();
            }

            this.Close(false, true, true);
        }

        public void Maximize()
        {
            if (Event_OnMaximize != null) Event_OnMaximize.Invoke(this);

        }        

        protected IEnumerator DisableWhenAnimationIsComplete(bool minimize = false)
        {
            yield return new WaitForSeconds(0.5f);

            if (!this.isVisible)
            {
                this.gameObject.SetActive(false);

                ResetPositionAndAlpha();

                closing = false;

                if (DestroyAfterClose && !minimize) Destroy(this.gameObject, 0.5f);
            }
        }

        private void PlayAnimation(string animationName)
        {
            if (!Application.isPlaying) return;

            if (Animator == null) return;

            if (Animator.runtimeAnimatorController == null)
            {
                Animator.runtimeAnimatorController = this.RuntimeAnimatorController;                
            }

            //Animator.speed = 0.1f;
            Animator.updateMode = AnimatorUpdateMode.UnscaledTime;            
            Animator.enabled = true;
            Animator.StopPlayback();

            Animator.Play(animationName);            
        }

        public void ResetPositionAndAlpha()
        {
            this.transform.localPosition = initialPosition;

            // reset alpha values too
            this.CanvasGroup.alpha = 1;
        }

        private void FadeInOverlay(bool instant = false)
        {
            if (Modal)
            {
                GO_ScreenOverlay.FadeIn(instant);
            }
        }

        private void FadeOutOverlay(bool instant = false)
        {
            if (Modal)
            {
                GO_ScreenOverlay.FadeOut(instant);
            }
        }

        public void ForceButtonUpdate()
        {
            if (!this.gameObject.activeInHierarchy) return;

            RemoveExistingButtons();

            if (Application.isPlaying)
            {                
                StartCoroutine(DelayedCall(0, UpdateButtons));
            }
            else
            {
                UpdateButtons();
            }
        }

        private void RemoveExistingButtons()
        {
            var buttonComponents = this.gameObject.GetComponentsInChildren<uDialog_Button>();

            foreach (var button in buttonComponents)
            {
                if (button.IsTemplate) continue;

                if (Application.isPlaying)
                {
                    Destroy(button.gameObject);
                }
                else
                {
                    DestroyImmediate(button.gameObject);
                }
            }
        }

        private IEnumerator DelayedCall(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);

            action.Invoke();
        }

        public void UpdateResizeListeners()
        {
            if (ResizeListeners == null) return;

            Dictionary<eResizeListenerType, uDialog_ResizeListener> resizeListeners = ResizeListeners.Where(r => r != null).ToDictionary(k => k.ResizeListenerType, v => v);

            if(resizeListeners.ContainsKey(eResizeListenerType.Bottom)) resizeListeners[eResizeListenerType.Bottom].gameObject.SetActive(AllowResizeFromBottom);
            if(resizeListeners.ContainsKey(eResizeListenerType.Left)) resizeListeners[eResizeListenerType.Left].gameObject.SetActive(AllowResizeFromLeft);
            if(resizeListeners.ContainsKey(eResizeListenerType.Right)) resizeListeners[eResizeListenerType.Right].gameObject.SetActive(AllowResizeFromRight);

            if (AllowResizeFromBottom)
            {
                if (resizeListeners.ContainsKey(eResizeListenerType.BottomLeft)) resizeListeners[eResizeListenerType.BottomLeft].gameObject.SetActive(AllowResizeFromLeft);
                if (resizeListeners.ContainsKey(eResizeListenerType.BottomRight)) resizeListeners[eResizeListenerType.BottomRight].gameObject.SetActive(AllowResizeFromRight);
            }
            else
            {
                if (resizeListeners.ContainsKey(eResizeListenerType.BottomLeft)) resizeListeners[eResizeListenerType.BottomLeft].gameObject.SetActive(false);
                if (resizeListeners.ContainsKey(eResizeListenerType.BottomRight)) resizeListeners[eResizeListenerType.BottomRight].gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Make this uDialog the Last Sibling in its parent container (Bring it to the front)
        /// </summary>
        public void Focus()
        {
            this.RectTransform.SetAsLastSibling();

            if (GO_TaskBar != null)
            {
                GO_TaskBar.SetFocusedTask(this);
            }
        }

        public bool IsFocused()
        {
            if (transform.parent == null) return true;

            Transform lastActiveChild = null;
            foreach (Transform t in transform.parent)
            {
                if (t.gameObject.activeInHierarchy) lastActiveChild = t;
            }

            return lastActiveChild == transform;
        }

        // Thank you jmorhart: http://answers.unity3d.com/questions/976201/set-a-recttranforms-pivot-without-changing-its-pos.html
        public void SetPivot(Vector2 pivot, RectTransform rectTransform = null)
        {
            if (rectTransform == null) rectTransform = RectTransform;
            if (rectTransform == null) return;

            Vector2 size = rectTransform.rect.size;
            Vector2 deltaPivot = rectTransform.pivot - pivot;
            Vector3 deltaPosition = new Vector3(deltaPivot.x * size.x, deltaPivot.y * size.y);
            rectTransform.pivot = pivot;
            rectTransform.localPosition -= deltaPosition;
        }

        public void SetPivot(eResizeListenerType resizeListenerType)
        {
            SetPivot(GetPivot(resizeListenerType));
        }

        protected Vector2 GetPivot(eResizeListenerType resizeListenerType)
        {
            switch (resizeListenerType)
            {
                case eResizeListenerType.Right:
                    return new Vector2(0, 0.5f);
                case eResizeListenerType.Left:
                    return new Vector2(1, 0.5f);
                case eResizeListenerType.Bottom:
                    return new Vector2(0.5f, 1);
                case eResizeListenerType.BottomRight:
                    return new Vector2(0f, 1f);
                case eResizeListenerType.BottomLeft:
                    return new Vector2(1f, 1f);
            }

            return new Vector2(0, 0);
        }  

        #region Audio
        protected void PlaySound(AudioClip Sound, float volume = 1f)
        {
            if (Sound == null)
            {
                return;
            }

            if (_AudioSource == null)
            {
                InitAudioSource(this.AudioMixerGroup);
            }

            _AudioSource.volume = volume;
            _AudioSource.clip = Sound;
            _AudioSource.Play();
        }

        protected void InitAudioSource(UnityEngine.Audio.AudioMixerGroup audioMixerGroup = null)
        {
            if (this._AudioSource == null)
            {
                _AudioSource = this.gameObject.AddComponent<AudioSource>();
            }

            _AudioSource.playOnAwake = false;
            _AudioSource.loop = false;
            _AudioSource.outputAudioMixerGroup = audioMixerGroup;
        }
        #endregion

        #region Events
        public void ScreenOverlayClicked()
        {
            if (closing) return;

            if (TriggerOnClickEventWhenOverlayIsClicked)
            {
                Clicked();
            }

            if (CloseWhenScreenOverlayIsClicked)
            {
                Close();
            }
        }

        public void ButtonClicked()
        {
            if (closing) return;

            if (CloseWhenAnyButtonClicked)
            {
                // Don't close immediately, give the button's event-handler (if there is one) a chance to execute first
                closeOnNextUpdate = true;
            }

            PlaySound(OnButtonClickSound, this.AudioVolume);
        }

        /// <summary>
        /// Called when the dialog is clicked anywhere
        /// </summary>
        public void Clicked()
        {
            if(Event_OnClick != null)
            {
                Event_OnClick.Invoke(this);
            }

            if (closing) return;
            
            if (CloseWhenClicked)
            {
                Close();
            } 
            else if (FocusOnClick)
            {
                Focus();
            }            
        }        

        public void OnMouseEnter()
        {
            if (closing) return;

            if (FocusOnMouseOver)
            {
                Focus();
            }
        }

        public void OnDialogDrag(UnityEngine.EventSystems.BaseEventData eventData)
        {
            if (!AllowDraggingViaDialog) return;

            var pointerData = eventData as UnityEngine.EventSystems.PointerEventData;

            DragUpdate(pointerData);
        }

        public void OnTitleDrag(UnityEngine.EventSystems.BaseEventData eventData)
        {
            if (!AllowDraggingViaTitle) return;

            var pointerData = eventData as UnityEngine.EventSystems.PointerEventData;

            DragUpdate(pointerData);
        }                

        protected void DragUpdate(PointerEventData eventData)
        {            
            if (eventData == null) return;

            Focus();

            RectTransform.anchoredPosition += eventData.delta;            

            // Clamp within parent bounds
            if (RestrictToParentBounds)
            {
                var parentTransform = (RectTransform)this.RectTransform.parent;

                Vector3 pos = parentTransform.localPosition;

                Vector3 minPosition = parentTransform.rect.min - RectTransform.rect.min;
                Vector3 maxPosition = parentTransform.rect.max - RectTransform.rect.max;

                pos.x = Mathf.Clamp(RectTransform.localPosition.x, minPosition.x, maxPosition.x);
                pos.y = Mathf.Clamp(RectTransform.localPosition.y, minPosition.y, maxPosition.y);

                RectTransform.localPosition = pos;
            }            
        }
        #endregion

        #region Instantiation
        public static uDialog NewDialog(RectTransform parent)
        {
            return NewDialog("uDialog_Default", parent);
        }

        public static uDialog NewDialog(string dialogPrefabType = "uDialog_Default", RectTransform parent = null)
        {            
            var gameObject = uDialog_Utilities.InstantiatePrefab(dialogPrefabType, true, true, parent);
            uDialog uDialog = gameObject.GetComponent<uDialog>();

            uDialog_Timer.DelayedCall(0f, () => uDialog.Start(), uDialog);

            return uDialog;
        }

        public static uDialog NewNotification()
        {
            return NewDialog("uDialog_Notification");
        }

        public static uDialog NewMenu()
        {
            return NewDialog("uDialog_Menu").ClearButtons();
        }
        #endregion        
    }    
}
