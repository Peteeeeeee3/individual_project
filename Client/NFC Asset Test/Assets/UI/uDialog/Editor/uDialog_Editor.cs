using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System;
using System.Text;
using System.Collections.Generic;

namespace UI.Dialogs
{
    [CustomEditor(typeof(uDialog))]
    public class uDialog_Editor : Editor
    {
        private GUIStyle boxStyle;

        private SerializedObject SO_Target;
        private uDialog uDialog;

        #region Basic
        SerializedProperty VisibleOnStart;        
        #endregion

        #region Title
        private SerializedProperty ShowTitle;
        private SerializedProperty TitleText;
        private SerializedProperty ShowTitleCloseButton;
        private SerializedProperty ShowTitleMinimizeButton;        
        #endregion

        #region Content
        private SerializedProperty ContentText;
        private SerializedProperty IconType;

        private SerializedProperty Icon_Information;
        private SerializedProperty Icon_Question;
        private SerializedProperty Icon_Warning;

        private SerializedProperty ShowButtons;
        private SerializedProperty CloseWhenAnyButtonClicked;
        private SerializedProperty Buttons;        
        #endregion
        
        #region Modal
        private SerializedProperty Modal;
        private SerializedProperty CloseWhenScreenOverlayIsClicked;        
        #endregion

        #region Auto Close
        private SerializedProperty AutoClose;
        private SerializedProperty AutoCloseTime;
        #endregion

        #region Theme
        private SerializedProperty ThemeImageSet;
        private SerializedProperty Image_Base;
        private SerializedProperty Image_Outline;
        private SerializedProperty Image_Title;
        private SerializedProperty Image_Viewport;
        private SerializedProperty Image_Button;
        private SerializedProperty Image_CloseButton;
        private SerializedProperty OutlineMode;
        private SerializedProperty TitleFontSettings;
        private SerializedProperty ContentFontSettings;
        private SerializedProperty ButtonFontSettings;
        private SerializedProperty ColorScheme;
        private SerializedProperty Color_TitleBackground;        
        private SerializedProperty Color_TitleText;
        private SerializedProperty Color_TitleTextEffect;
        private SerializedProperty Color_ViewportBackground;
        private SerializedProperty Color_ViewportText;
        private SerializedProperty Color_ViewportTextEffect;
        private SerializedProperty Color_ButtonBackground;
        private SerializedProperty Color_ButtonHighlight;
        private SerializedProperty Color_ButtonText;
        private SerializedProperty Color_ButtonTextEffect;
        private SerializedProperty Color_Shadow;
        private SerializedProperty Color_Glow;
        private SerializedProperty Color_ScreenOverlay;

        private SerializedProperty Icon_Information_Color;
        private SerializedProperty Icon_Warning_Color;
        private SerializedProperty Icon_Question_Color;

        private string saveColorSchemeAsName;
        private string colorSchemeSaveErrorMessageText;
        private bool showColorSchemeSavedMessage = false;

        private double timeToHideMessage = 0;
        private List<KeyValuePair<double, Action>> delayedActions = new List<KeyValuePair<double, Action>>();

        //private List<
        #endregion

        #region Events
        private SerializedProperty CloseWhenClicked;
        private SerializedProperty Event_OnShow;
        private SerializedProperty Event_OnClose;
        private SerializedProperty Event_OnClick;
        public SerializedProperty TriggerOnClickEventWhenOverlayIsClicked;        
        #endregion

        #region Animation
        private SerializedProperty ShowAnimation;
        private SerializedProperty CloseAnimation;
        #endregion

        #region Audio
        private SerializedProperty OnShowSound;
        private SerializedProperty OnCloseSound;
        private SerializedProperty OnButtonClickSound;
        private SerializedProperty AudioVolume;
        private SerializedProperty AudioMixerGroup;
        #endregion

        #region Dragging and Resizing
        private SerializedProperty AllowDraggingViaTitle;
        private SerializedProperty AllowDraggingViaDialog;
        private SerializedProperty RestrictToParentBounds;

        private SerializedProperty AllowResizeFromLeft;
        private SerializedProperty AllowResizeFromRight;
        private SerializedProperty AllowResizeFromBottom;

        private SerializedProperty MinSize;
        private SerializedProperty MaxSize;

        private SerializedProperty AllowResizeToAdjustPivot;
        #endregion

        #region Focus
        SerializedProperty FocusOnClick;
        SerializedProperty FocusOnMouseOver;
        SerializedProperty FocusOnShow;
        #endregion

        #region Misc
        private SerializedProperty DestroyAfterClose;
        #endregion

        #region References
        private SerializedProperty GO_Dialog;
        private SerializedProperty GO_Container;
        private SerializedProperty GO_Title;
        private SerializedProperty GO_TitleCloseButton;
        private SerializedProperty GO_TitleMinimizeButton;        
        private SerializedProperty GO_TitleText;
        private SerializedProperty GO_Viewport;
        private SerializedProperty GO_MessageContainer;
        private SerializedProperty GO_MessageText;
        private SerializedProperty GO_Icon;
        private SerializedProperty GO_Outline;
        private SerializedProperty GO_ScreenOverlayImage;
        private SerializedProperty GO_ScreenOverlay;
        private SerializedProperty GO_ButtonContainer;
        private SerializedProperty GO_ButtonTemplate;
        private SerializedProperty RuntimeAnimatorController;
        private SerializedProperty ResizeListeners;
        private SerializedProperty GO_Content;
        #endregion

        #region Editor Variables        
        private SerializedProperty editor_showReferencesSection;
        private SerializedProperty editor_showTitleSection;
        private SerializedProperty editor_showContentSection;
        private SerializedProperty editor_showIconDefinitionSection;
        private SerializedProperty editor_showButtonsSection;
        private SerializedProperty editor_showModalSection;
        private SerializedProperty editor_showAutoCloseSection;
        private SerializedProperty editor_showThemeSection;
        private SerializedProperty editor_showThemeColorsSection;
        private SerializedProperty editor_showEventsSection;
        private SerializedProperty editor_showAnimationSection;
        private SerializedProperty editor_showMiscSection;
        private SerializedProperty editor_showAudioSection;
        private SerializedProperty editor_showDragAndResizeSection;
        private SerializedProperty editor_showFocusSection;
        #endregion

        public void OnEnable()
        {
            uDialog = target as uDialog;
            SO_Target = new SerializedObject(target);

            CloseWhenClicked = SO_Target.FindProperty("CloseWhenClicked");
            VisibleOnStart = SO_Target.FindProperty("VisibleOnStart");

            ShowTitle = SO_Target.FindProperty("ShowTitle");
            TitleText = SO_Target.FindProperty("TitleText");
            ShowTitleCloseButton = SO_Target.FindProperty("ShowTitleCloseButton");
            ShowTitleMinimizeButton = SO_Target.FindProperty("ShowTitleMinimizeButton");            

            ContentText = SO_Target.FindProperty("ContentText");
            IconType = SO_Target.FindProperty("IconType");
            Icon_Information = SO_Target.FindProperty("Icon_Information");
            Icon_Question = SO_Target.FindProperty("Icon_Question");
            Icon_Warning = SO_Target.FindProperty("Icon_Warning");

            ShowButtons = SO_Target.FindProperty("_ShowButtons");
            CloseWhenAnyButtonClicked = SO_Target.FindProperty("CloseWhenAnyButtonClicked");
            Buttons = SO_Target.FindProperty("Buttons");

            Modal = SO_Target.FindProperty("Modal");
            CloseWhenScreenOverlayIsClicked = SO_Target.FindProperty("CloseWhenScreenOverlayIsClicked");

            AutoClose = SO_Target.FindProperty("AutoClose");
            AutoCloseTime = SO_Target.FindProperty("AutoCloseTime");

            ThemeImageSet = SO_Target.FindProperty("ThemeImageSet");
            Image_Base = SO_Target.FindProperty("Image_Base");
            Image_Button = SO_Target.FindProperty("Image_Button");
            Image_CloseButton = SO_Target.FindProperty("Image_CloseButton");
            Image_Outline = SO_Target.FindProperty("Image_Outline");
            Image_Title = SO_Target.FindProperty("Image_Title");
            Image_Viewport = SO_Target.FindProperty("Image_Viewport");
            OutlineMode = SO_Target.FindProperty("OutlineMode");

            TitleFontSettings = SO_Target.FindProperty("TitleFontSettings");
            ContentFontSettings = SO_Target.FindProperty("ContentFontSettings");
            ButtonFontSettings = SO_Target.FindProperty("ButtonFontSettings");
            
            ColorScheme = SO_Target.FindProperty("_ColorScheme");

            Color_TitleBackground = SO_Target.FindProperty("_Color_TitleBackground");
            Color_TitleText = SO_Target.FindProperty("_Color_TitleText");
            Color_TitleTextEffect = SO_Target.FindProperty("_Color_TitleTextEffect");
            Color_ViewportBackground = SO_Target.FindProperty("_Color_ViewportBackground");
            Color_ViewportText = SO_Target.FindProperty("_Color_ViewportText");            
            Color_ViewportTextEffect = SO_Target.FindProperty("_Color_ViewportTextEffect");
            Color_ButtonBackground = SO_Target.FindProperty("_Color_ButtonBackground");
            Color_ButtonHighlight = SO_Target.FindProperty("_Color_ButtonHighlight");
            Color_ButtonText = SO_Target.FindProperty("_Color_ButtonText");
            Color_ButtonTextEffect = SO_Target.FindProperty("_Color_ButtonTextEffect");
            Color_Shadow = SO_Target.FindProperty("_Color_Shadow");
            Color_Glow = SO_Target.FindProperty("_Color_Glow");
            Color_ScreenOverlay = SO_Target.FindProperty("_Color_ScreenOverlay");

            Icon_Information_Color = SO_Target.FindProperty("Icon_Information_Color");
            Icon_Warning_Color = SO_Target.FindProperty("Icon_Warning_Color");
            Icon_Question_Color = SO_Target.FindProperty("Icon_Question_Color");

            Event_OnShow = SO_Target.FindProperty("Event_OnShow");
            Event_OnClose = SO_Target.FindProperty("Event_OnClose");
            Event_OnClick = SO_Target.FindProperty("Event_OnClick");

            TriggerOnClickEventWhenOverlayIsClicked = SO_Target.FindProperty("TriggerOnClickEventWhenOverlayIsClicked");

            ShowAnimation = SO_Target.FindProperty("ShowAnimation");
            CloseAnimation = SO_Target.FindProperty("CloseAnimation");        

            OnShowSound = SO_Target.FindProperty("OnShowSound");
            OnCloseSound = SO_Target.FindProperty("OnCloseSound");
            OnButtonClickSound = SO_Target.FindProperty("OnButtonClickSound");
            AudioVolume = SO_Target.FindProperty("AudioVolume");
            AudioMixerGroup = SO_Target.FindProperty("AudioMixerGroup");

            AllowDraggingViaTitle = SO_Target.FindProperty("AllowDraggingViaTitle");
            AllowDraggingViaDialog = SO_Target.FindProperty("AllowDraggingViaDialog");
            RestrictToParentBounds = SO_Target.FindProperty("RestrictToParentBounds");
            AllowResizeFromLeft = SO_Target.FindProperty("AllowResizeFromLeft");
            AllowResizeFromRight = SO_Target.FindProperty("AllowResizeFromRight");
            AllowResizeFromBottom = SO_Target.FindProperty("AllowResizeFromBottom");            

            MinSize = SO_Target.FindProperty("MinSize");
            MaxSize = SO_Target.FindProperty("MaxSize");

            AllowResizeToAdjustPivot = SO_Target.FindProperty("AllowResizeToAdjustPivot");

            FocusOnClick = SO_Target.FindProperty("FocusOnClick");
            FocusOnMouseOver = SO_Target.FindProperty("FocusOnMouseOver");
            FocusOnShow = SO_Target.FindProperty("FocusOnShow");

            GO_Dialog = SO_Target.FindProperty("GO_Dialog");
            GO_Container = SO_Target.FindProperty("GO_Container");
            GO_Title = SO_Target.FindProperty("GO_Title");
            GO_TitleCloseButton = SO_Target.FindProperty("GO_TitleCloseButton");
            GO_TitleMinimizeButton = SO_Target.FindProperty("GO_TitleMinimizeButton");            
            GO_TitleText = SO_Target.FindProperty("GO_TitleText");
            GO_Viewport = SO_Target.FindProperty("GO_Viewport");
            GO_MessageContainer = SO_Target.FindProperty("GO_MessageContainer");
            GO_MessageText = SO_Target.FindProperty("GO_MessageText");
            GO_Icon = SO_Target.FindProperty("GO_Icon");
            GO_Outline = SO_Target.FindProperty("GO_Outline");
            GO_ScreenOverlayImage = SO_Target.FindProperty("GO_ScreenOverlayImage");
            GO_ScreenOverlay = SO_Target.FindProperty("GO_ScreenOverlay");
            GO_ButtonContainer = SO_Target.FindProperty("GO_ButtonContainer");
            GO_ButtonTemplate = SO_Target.FindProperty("GO_ButtonTemplate");
            RuntimeAnimatorController = SO_Target.FindProperty("RuntimeAnimatorController");
            ResizeListeners = SO_Target.FindProperty("ResizeListeners");
            GO_Content = SO_Target.FindProperty("GO_Content");

            DestroyAfterClose = SO_Target.FindProperty("DestroyAfterClose");

            editor_showReferencesSection = SO_Target.FindProperty("editor_showReferencesSection");
            editor_showTitleSection = SO_Target.FindProperty("editor_showTitleSection");
            editor_showContentSection = SO_Target.FindProperty("editor_showContentSection");
            editor_showIconDefinitionSection = SO_Target.FindProperty("editor_showIconDefinitionSection");
            editor_showButtonsSection = SO_Target.FindProperty("editor_showButtonsSection");
            editor_showModalSection = SO_Target.FindProperty("editor_showModalSection");
            editor_showAutoCloseSection = SO_Target.FindProperty("editor_showAutoCloseSection");
            editor_showThemeSection = SO_Target.FindProperty("editor_showThemeSection");
            editor_showThemeColorsSection = SO_Target.FindProperty("editor_showThemeColorsSection");
            editor_showEventsSection = SO_Target.FindProperty("editor_showEventsSection");
            editor_showAnimationSection = SO_Target.FindProperty("editor_showAnimationSection");
            editor_showMiscSection = SO_Target.FindProperty("editor_showMiscSection");
            editor_showAudioSection = SO_Target.FindProperty("editor_showAudioSection");
            editor_showDragAndResizeSection = SO_Target.FindProperty("editor_showDragAndResizeSection");
            editor_showFocusSection = SO_Target.FindProperty("editor_showFocusSection");

            EditorApplication.update += EditorUpdate;
        }

        public void OnDisable()
        {
            EditorApplication.update -= EditorUpdate;
        }

        public override void OnInspectorGUI()
        {            
            EditorGUI.BeginChangeCheck();
            
            InitStyles();

            EditorGUILayout.LabelField("Controls", EditorStyles.boldLabel);
            var text = "Hide";
            if (!uDialog.isVisible)
            {
                text = "Show";
            }

            if (GUILayout.Button(text, GUILayout.Width(150)))
            {
                if (uDialog.isVisible)
                {
                    uDialog.Close();
                }
                else
                {
                    uDialog.Show();
                }
            }
            EditorGUILayout.Space();

            Section_Basic();
            Section_Title();
            Section_Content();
            Section_Modal();
            Section_AutoClose();
            Section_Theme();
            Section_Events();
            Section_Animation();
            Section_Audio();
            Section_DraggingAndResizing();
            Section_Focus();
            Section_Misc();
            Section_References();

            ResetStyles();

            PreserveFoldoutSettings();

            if (GUI.changed)
            {
                SO_Target.ApplyModifiedProperties();
                
                uDialog.UpdateDisplay();

                if(!Application.isPlaying) UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(uDialog.gameObject.scene);
            }
        }

        private void Section_Basic()
        {
            EditorGUILayout.LabelField("Basic", EditorStyles.boldLabel);            
            EditorGUILayout.PropertyField(VisibleOnStart);

            if (VisibleOnStart.boolValue && !uDialog.gameObject.activeSelf)
            {
                EditorGUILayout.HelpBox("Please note, 'Visible On Start' will not function if the uDialog GameObject is inactive.", MessageType.Warning);
            }
        }

        private void Section_Title()
        {
            uDialog.editor_showTitleSection = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), uDialog.editor_showTitleSection, "Title", true);
            if (uDialog.editor_showTitleSection)
            {
                EditorGUILayout.PropertyField(ShowTitle);
                if (ShowTitle.boolValue)
                {
                    EditorGUILayout.PropertyField(TitleText);
                    EditorGUILayout.PropertyField(ShowTitleCloseButton);
                    EditorGUILayout.PropertyField(ShowTitleMinimizeButton);                    
                }
            }
        }

        private void Section_Content()
        {
            uDialog.editor_showContentSection = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), uDialog.editor_showContentSection, "Content", true);
            if (uDialog.editor_showContentSection)
            {
                EditorGUILayout.PropertyField(ContentText);
                EditorGUILayout.PropertyField(IconType);

                EditorGUI.indentLevel += 1;
                uDialog.editor_showIconDefinitionSection = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), uDialog.editor_showIconDefinitionSection, "Icons", true);
                if (uDialog.editor_showIconDefinitionSection)
                {
                    EditorGUILayout.HelpBox("You can change the sprite used for each of the icon types here.", MessageType.Info, false);
                    EditorGUILayout.PropertyField(Icon_Information);
                    EditorGUILayout.PropertyField(Icon_Warning);
                    EditorGUILayout.PropertyField(Icon_Question);
                }

                uDialog.editor_showButtonsSection = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), uDialog.editor_showButtonsSection, "Buttons", true);
                if (uDialog.editor_showButtonsSection)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(ShowButtons);

                    if (GUILayout.Button(new GUIContent("Force Button Update", "This button will force the buttons to be updated, e.g. if the template has been edited.")))
                    {
                        uDialog.ForceButtonUpdate();
                    }

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.PropertyField(CloseWhenAnyButtonClicked);

                    

                    EditorGUILayout.PropertyField(Buttons, true);
                }
                

                EditorGUI.indentLevel -= 1;
            }
        }

        private void Section_Modal()
        {
            uDialog.editor_showModalSection = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), uDialog.editor_showModalSection, "Modal", true);
            if (uDialog.editor_showModalSection)
            {
                EditorGUILayout.PropertyField(Modal);
                EditorGUILayout.PropertyField(CloseWhenScreenOverlayIsClicked);
            }
        }

        private void Section_AutoClose()
        {
            uDialog.editor_showAutoCloseSection = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), uDialog.editor_showAutoCloseSection, "Auto Close", true);
            if (uDialog.editor_showAutoCloseSection)
            {
                EditorGUILayout.PropertyField(AutoClose);
                EditorGUILayout.PropertyField(AutoCloseTime);
            }
        }

        private void Section_Theme()
        {
            uDialog.editor_showThemeSection = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), uDialog.editor_showThemeSection, "Theme", true);
            if (uDialog.editor_showThemeSection)
            {               
                EditorGUILayout.PropertyField(ThemeImageSet);

                if (uDialog.ThemeImageSet == eThemeImageSet.Custom)
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(Image_Base);
                    EditorGUILayout.PropertyField(Image_Viewport);
                    EditorGUILayout.PropertyField(Image_Title);
                    EditorGUILayout.PropertyField(Image_Outline);
                    EditorGUILayout.PropertyField(Image_Button);
                    EditorGUILayout.PropertyField(Image_CloseButton);
                    if (EditorGUI.EndChangeCheck())
                    {
                        EditorApplication.delayCall += uDialog.UpdateDisplay;
                    }
                }

                EditorGUILayout.PropertyField(OutlineMode);
                
                Section_FontSetting(new GUIContent("Title Font"), TitleFontSettings, uDialog.TitleFontSettings);                
                Section_FontSetting(new GUIContent("Content Font"), ContentFontSettings, uDialog.ContentFontSettings);

                EditorGUI.BeginChangeCheck();
                Section_FontSetting(new GUIContent("Button Font"), ButtonFontSettings, uDialog.ButtonFontSettings);
                if (EditorGUI.EndChangeCheck())
                {                    
                    uDialog.ForceButtonUpdate();
                }

                Section_Theme_Colors();
            }
        }

        private void Section_FontSetting(GUIContent label, SerializedProperty property, uDialog_FontSetting fontSettingObject)
        {
            SerializedProperty font = property.FindPropertyRelative("_Font");
            SerializedProperty fontStyle = property.FindPropertyRelative("_FontStyle");
            SerializedProperty fontSize = property.FindPropertyRelative("_FontSize");
            SerializedProperty textEffect = property.FindPropertyRelative("_TextEffect");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(label);
            fontSettingObject.Font = EditorGUILayout.ObjectField(fontSettingObject.Font, typeof(Font), false) as Font;
            fontSettingObject.FontStyle = (FontStyle)EditorGUILayout.EnumPopup(fontSettingObject.FontStyle, GUILayout.Width(100));
            fontSettingObject.TextEffect = (eTextEffect)EditorGUILayout.EnumPopup(fontSettingObject.TextEffect, GUILayout.Width(75));
            fontSettingObject.FontSize = EditorGUILayout.IntField(fontSettingObject.FontSize, GUILayout.Width(32));
            EditorGUILayout.EndHorizontal();

            font.objectReferenceValue = fontSettingObject.Font;
            fontStyle.enumValueIndex = (int)fontSettingObject.FontStyle;
            fontSize.intValue = fontSettingObject.FontSize;
            textEffect.enumValueIndex = (int)fontSettingObject.TextEffect;
        }

        private void Section_Theme_Colors()
        {
            var colorSchemeNames = uDialog_Utilities.ColorSchemeNames;
            var selectedIndex = colorSchemeNames.ToList().IndexOf(uDialog.ColorScheme);
            if (selectedIndex < 0) selectedIndex = colorSchemeNames.Count() - 1;

            var oldSelectedIndex = selectedIndex;
            selectedIndex = EditorGUILayout.Popup("Color Scheme", selectedIndex, uDialog_Utilities.ColorSchemeNames);
            ColorScheme.stringValue = colorSchemeNames[selectedIndex];
            uDialog.ColorScheme = ColorScheme.stringValue;

            // we'll need this to fire an event later
            bool colorSchemeChanged = oldSelectedIndex != selectedIndex;            

            EditorGUI.indentLevel += 1;
            
            uDialog.editor_showThemeColorsSection = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), uDialog.editor_showThemeColorsSection, "Colors", true);            

            if (uDialog.editor_showThemeColorsSection)
            {                
                if (uDialog.ColorScheme != "Custom")
                {
                    if (GUILayout.Button("Edit Color Schemes"))
                    {                        
                        Selection.activeObject = uDialog_Utilities.Config;
                    }
                }

                //if (uDialog.ColorScheme != "Custom") EditorGUI.BeginDisabledGroup(true);

                EditorGUILayout.LabelField("Title", EditorStyles.boldLabel);

                uDialog.Color_TitleBackground = EditorGUILayout.ColorField("Title Background", uDialog.Color_TitleBackground);
                uDialog.Color_TitleText = EditorGUILayout.ColorField("Title Text", uDialog.Color_TitleText);
                uDialog.Color_TitleTextEffect = EditorGUILayout.ColorField("Title Text Effect", uDialog.Color_TitleTextEffect);
                
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Viewport", EditorStyles.boldLabel);

                uDialog.Color_ViewportBackground = EditorGUILayout.ColorField("Viewport Background", uDialog.Color_ViewportBackground);
                uDialog.Color_ViewportText = EditorGUILayout.ColorField("Viewport Text", uDialog.Color_ViewportText);
                uDialog.Color_ViewportTextEffect = EditorGUILayout.ColorField("Viewport Text Effect", uDialog.Color_ViewportTextEffect);

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Buttons", EditorStyles.boldLabel);

                uDialog.Color_ButtonBackground = EditorGUILayout.ColorField("Button Background", uDialog.Color_ButtonBackground);
                uDialog.Color_ButtonHighlight = EditorGUILayout.ColorField("Button Highlight", uDialog.Color_ButtonHighlight);
                uDialog.Color_ButtonText = EditorGUILayout.ColorField("Button Text", uDialog.Color_ButtonText);
                EditorGUI.BeginChangeCheck();
                uDialog.Color_ButtonTextEffect = EditorGUILayout.ColorField("Button Text Effect", uDialog.Color_ButtonTextEffect);
                if (EditorGUI.EndChangeCheck())
                {
                    uDialog.ForceButtonUpdate();                    
                }
                
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Outline", EditorStyles.boldLabel);

                uDialog.Color_Shadow = EditorGUILayout.ColorField("Shadow", uDialog.Color_Shadow);                
                uDialog.Color_Glow = EditorGUILayout.ColorField("Glow", uDialog.Color_Glow);

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Modal", EditorStyles.boldLabel);

                uDialog.Color_ScreenOverlay = EditorGUILayout.ColorField("Modal Screen Overlay", uDialog.Color_ScreenOverlay);

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Icons", EditorStyles.boldLabel);
                uDialog.Icon_Information_Color = EditorGUILayout.ColorField("Information Icon", uDialog.Icon_Information_Color);
                uDialog.Icon_Warning_Color = EditorGUILayout.ColorField("Warning Icon", uDialog.Icon_Warning_Color);
                uDialog.Icon_Question_Color = EditorGUILayout.ColorField("Question Icon", uDialog.Icon_Question_Color);

                EditorGUILayout.Space();

                //if (uDialog.ColorScheme != "Custom") EditorGUI.EndDisabledGroup();

                EditorGUILayout.BeginVertical(boxStyle);
                EditorGUI.indentLevel = 0;
                EditorGUILayout.LabelField("Save Color Scheme", EditorStyles.boldLabel);

                if (colorSchemeChanged)
                {
                    saveColorSchemeAsName = ColorScheme.stringValue;
                    if (saveColorSchemeAsName == "Custom")
                    {
                        saveColorSchemeAsName = "Custom 1";
                    }
                }
                else if (String.IsNullOrEmpty(saveColorSchemeAsName))
                {
                    saveColorSchemeAsName = ColorScheme.stringValue;
                }

                saveColorSchemeAsName = EditorGUILayout.TextField("Color Scheme Name", saveColorSchemeAsName);                

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Save", EditorStyles.miniButton, GUILayout.Width(50)))
                {
                    SaveColorScheme();
                }

                if (uDialog_Utilities.DefaultColorSchemeNames.Contains(ColorScheme.stringValue))
                {
                    if (GUILayout.Button("Reset selected theme to default", EditorStyles.miniButton, GUILayout.Width(200)))
                    {
                        var themeName = ColorScheme.stringValue;

                        DeleteColorScheme(ColorScheme.stringValue, true);
                        uDialog_Utilities.InitializeDefaultThemes();
                        uDialog_Utilities.SaveColorSchemes();

                        ColorScheme.stringValue = themeName;
                        uDialog.ColorScheme = themeName;
                        uDialog.ColorSchemeChanged();

                        Repaint();
                    }
                }
                else
                {
                    if (ColorScheme.stringValue != "Custom")
                    {
                        if (GUILayout.Button("Delete selected theme", EditorStyles.miniButton, GUILayout.Width(150)))
                        {
                            DeleteColorScheme();
                            Repaint();
                        }
                    }
                }

                GUILayout.EndHorizontal();

                if (!String.IsNullOrEmpty(colorSchemeSaveErrorMessageText))
                {
                    EditorGUILayout.HelpBox(colorSchemeSaveErrorMessageText, MessageType.Error, false);
                }

                if (showColorSchemeSavedMessage)
                {
                    // hide the message in 2 seconds
                    timeToHideMessage = EditorApplication.timeSinceStartup + 2.00;
                    EditorGUILayout.HelpBox("Color Scheme saved.", MessageType.Info, false);
                }

                EditorGUILayout.EndVertical();                
            }

            // 
            Color_TitleBackground.colorValue = uDialog.Color_TitleBackground;
            Color_TitleText.colorValue = uDialog.Color_TitleText;
            Color_TitleTextEffect.colorValue = uDialog.Color_TitleTextEffect;
            Color_ViewportBackground.colorValue = uDialog.Color_ViewportBackground;
            Color_ViewportText.colorValue = uDialog.Color_ViewportText;
            Color_ViewportTextEffect.colorValue = uDialog.Color_ViewportTextEffect;
            Color_ButtonBackground.colorValue = uDialog.Color_ButtonBackground;
            Color_ButtonHighlight.colorValue = uDialog.Color_ButtonHighlight;
            Color_ButtonText.colorValue = uDialog.Color_ButtonText;
            Color_ButtonTextEffect.colorValue = uDialog.Color_ButtonTextEffect;

            Color_ScreenOverlay.colorValue = uDialog.Color_ScreenOverlay;
            Color_Glow.colorValue = uDialog.Color_Glow;
            Color_Shadow.colorValue = uDialog.Color_Shadow;

            Icon_Information_Color.colorValue = uDialog.Icon_Information_Color;
            Icon_Warning_Color.colorValue = uDialog.Icon_Warning_Color;
            Icon_Question_Color.colorValue = uDialog.Icon_Question_Color;

            EditorGUI.indentLevel = 0; 
        }

        private void Section_Events()
        {
            uDialog.editor_showEventsSection = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), uDialog.editor_showEventsSection, "Events", true);
            if (uDialog.editor_showEventsSection)
            {
                EditorGUILayout.PropertyField(CloseWhenClicked);
                EditorGUILayout.PropertyField(Event_OnShow);
                EditorGUILayout.PropertyField(Event_OnClose);
                EditorGUILayout.PropertyField(Event_OnClick);
                if (uDialog.Modal)
                {
                    EditorGUILayout.PropertyField(TriggerOnClickEventWhenOverlayIsClicked);
                }
            }
        }

        private void Section_Misc()
        {
            uDialog.editor_showMiscSection = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), uDialog.editor_showMiscSection, "Misc", true);
            if (uDialog.editor_showMiscSection)
            {
                EditorGUILayout.PropertyField(DestroyAfterClose);
            }
        }

        private void Section_Animation()
        {
            uDialog.editor_showAnimationSection = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), uDialog.editor_showAnimationSection, "Animation", true);
            if (uDialog.editor_showAnimationSection)
            {
                EditorGUILayout.PropertyField(ShowAnimation);
                EditorGUILayout.PropertyField(CloseAnimation);
            }
        }

        private void Section_Audio()
        {
            uDialog.editor_showAudioSection = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), uDialog.editor_showAudioSection, "Audio", true);
            if (uDialog.editor_showAudioSection)
            {
                EditorGUILayout.PropertyField(OnShowSound);
                EditorGUILayout.PropertyField(OnCloseSound);
                EditorGUILayout.PropertyField(OnButtonClickSound);
                EditorGUILayout.PropertyField(AudioVolume);
                EditorGUILayout.PropertyField(AudioMixerGroup);
            }            
        }

        private void Section_DraggingAndResizing()
        {
            uDialog.editor_showDragAndResizeSection = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), uDialog.editor_showDragAndResizeSection, "Dragging and Resizing", true);
            if (uDialog.editor_showDragAndResizeSection)
            {
                EditorGUILayout.LabelField("Dragging", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(AllowDraggingViaTitle);
                EditorGUILayout.PropertyField(AllowDraggingViaDialog);
                EditorGUILayout.PropertyField(RestrictToParentBounds);

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Resizing", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(AllowResizeFromLeft);
                EditorGUILayout.PropertyField(AllowResizeFromRight);
                EditorGUILayout.PropertyField(AllowResizeFromBottom);

                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(MinSize);
                EditorGUILayout.PropertyField(MaxSize);

                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(AllowResizeToAdjustPivot);
            }
        }

        private void Section_Focus()
        {
            uDialog.editor_showFocusSection = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), uDialog.editor_showFocusSection, "Focus", true);
            if (uDialog.editor_showFocusSection)
            {
                EditorGUILayout.PropertyField(FocusOnClick);
                EditorGUILayout.PropertyField(FocusOnMouseOver);
                EditorGUILayout.PropertyField(FocusOnShow);
            }
        }

        private void Section_References()
        {
            uDialog.editor_showReferencesSection = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), uDialog.editor_showReferencesSection, "References", true);
            if (uDialog.editor_showReferencesSection)
            {
                EditorGUILayout.HelpBox("Under normal circumstances, there should be no need to modify the following properties.", MessageType.Info, true);

                EditorGUILayout.PropertyField(GO_Dialog);
                EditorGUILayout.PropertyField(GO_Container);
                EditorGUILayout.PropertyField(GO_Title);
                EditorGUILayout.PropertyField(GO_TitleCloseButton);
                EditorGUILayout.PropertyField(GO_TitleMinimizeButton);                
                EditorGUILayout.PropertyField(GO_TitleText);
                EditorGUILayout.PropertyField(GO_Viewport);
                EditorGUILayout.PropertyField(GO_MessageContainer);
                EditorGUILayout.PropertyField(GO_MessageText);
                EditorGUILayout.PropertyField(GO_Icon);
                EditorGUILayout.PropertyField(GO_Outline);
                EditorGUILayout.PropertyField(GO_ScreenOverlayImage);
                EditorGUILayout.PropertyField(GO_ScreenOverlay);
                EditorGUILayout.PropertyField(GO_ButtonContainer);
                EditorGUILayout.PropertyField(GO_ButtonTemplate);
                EditorGUILayout.PropertyField(RuntimeAnimatorController);
                EditorGUILayout.PropertyField(ResizeListeners, true);
                EditorGUILayout.PropertyField(GO_Content);
            }            
        }

        private void SaveColorScheme()
        {
            showColorSchemeSavedMessage = false;

            if(String.IsNullOrEmpty(saveColorSchemeAsName))
            {
                colorSchemeSaveErrorMessageText = "Please provide a name for this color scheme!";
                return;
            }            

            if (saveColorSchemeAsName.Any(s => !char.IsLetterOrDigit(s) && s != ' ' && s != '-' && s != '_'))
            {
                colorSchemeSaveErrorMessageText = "Please provide a valid name for this color scheme! (Alphanumeric characters and spaces only)";
                return;
            }

            if (saveColorSchemeAsName == "Custom")
            {
                colorSchemeSaveErrorMessageText = "Please use a name other than 'Custom'";
                return;
            }

            var scheme = new uDialog_ColorScheme();
            scheme.Name = saveColorSchemeAsName;
            scheme.TitleBackground = Color_TitleBackground.colorValue;
            scheme.TitleText = Color_TitleText.colorValue;
            scheme.TitleTextEffect = Color_TitleTextEffect.colorValue;
            scheme.ViewportBackground = Color_ViewportBackground.colorValue;
            scheme.ViewportText = Color_ViewportText.colorValue;
            scheme.ViewportTextEffect = Color_ViewportTextEffect.colorValue;
            scheme.ButtonBackground = Color_ButtonBackground.colorValue;
            scheme.ButtonHighlight = Color_ButtonHighlight.colorValue;
            scheme.ButtonText = Color_ButtonText.colorValue;
            scheme.ButtonTextEffect = Color_ButtonTextEffect.colorValue;
            scheme.ScreenOverlay = Color_ScreenOverlay.colorValue;
            scheme.Shadow = Color_Shadow.colorValue;
            scheme.Glow = Color_Glow.colorValue;
            scheme.Icon_Information = Icon_Information_Color.colorValue;
            scheme.Icon_Question = Icon_Question_Color.colorValue;
            scheme.Icon_Warning = Icon_Warning_Color.colorValue;

            var result = uDialog_Utilities.AddColorScheme(saveColorSchemeAsName, scheme);

            colorSchemeSaveErrorMessageText = null;
            showColorSchemeSavedMessage = true;

            if (result == "") // success
            {
                ColorScheme.stringValue = saveColorSchemeAsName;
                uDialog.ColorScheme = saveColorSchemeAsName;
            }

            EditorUtility.SetDirty(target);
        }

        private void DeleteColorScheme(string name = null, bool force = false)
        {            
            if (name == null)
            {
                name = ColorScheme.stringValue;
            }

            if (!force && (name == "Custom" || uDialog_Utilities.DefaultColorSchemeNames.Contains(name)))
            {
                colorSchemeSaveErrorMessageText = "Sorry, you cannot delete the default colour schemes.";
                return;
            }

            uDialog_Utilities.DeleteColorScheme(name);

            if (ColorScheme.stringValue == name)
            {                
                ColorScheme.stringValue = "Custom";
                uDialog.ColorScheme = "Custom";
            }

            //Repaint();
        }

        void PreserveFoldoutSettings()
        {
            editor_showReferencesSection.boolValue = uDialog.editor_showReferencesSection;
            editor_showTitleSection.boolValue = uDialog.editor_showTitleSection;
            editor_showContentSection.boolValue = uDialog.editor_showContentSection;
            editor_showIconDefinitionSection.boolValue = uDialog.editor_showIconDefinitionSection;
            editor_showButtonsSection.boolValue = uDialog.editor_showButtonsSection;
            editor_showModalSection.boolValue = uDialog.editor_showModalSection;
            editor_showAutoCloseSection.boolValue = uDialog.editor_showAutoCloseSection;
            editor_showThemeSection.boolValue = uDialog.editor_showThemeSection;
            editor_showThemeColorsSection.boolValue = uDialog.editor_showThemeColorsSection;
            editor_showEventsSection.boolValue = uDialog.editor_showEventsSection;
            editor_showAnimationSection.boolValue = uDialog.editor_showAnimationSection;
            editor_showMiscSection.boolValue = uDialog.editor_showMiscSection;
            editor_showAudioSection.boolValue = uDialog.editor_showAudioSection;
            editor_showDragAndResizeSection.boolValue = uDialog.editor_showDragAndResizeSection;
            editor_showFocusSection.boolValue = uDialog.editor_showFocusSection;
        }

        void InitStyles()
        {
            EditorStyles.foldout.fontStyle = FontStyle.Bold;

            boxStyle = new GUIStyle(EditorStyles.textArea);
            boxStyle.padding = new RectOffset(5, 5, 5, 5);
            boxStyle.margin = new RectOffset(25, 25, 0, 0);
        }

        void ResetStyles()
        {
            EditorStyles.foldout.fontStyle = FontStyle.Normal;
        }

        void EditorUpdate()
        {
            if (showColorSchemeSavedMessage)
            {
                if (EditorApplication.timeSinceStartup >= timeToHideMessage)
                {
                    showColorSchemeSavedMessage = false;
                    Repaint();
                }
            }

            List<KeyValuePair<double, Action>> actionsToRemove = new List<KeyValuePair<double, Action>>();
            foreach (var kvp in delayedActions)
            {
                if (EditorApplication.timeSinceStartup >= kvp.Key)
                {
                    kvp.Value.Invoke();
                    actionsToRemove.Add(kvp);
                }
            }

            foreach (var kvp in actionsToRemove)
            {
                delayedActions.Remove(kvp);
            }
        }
    }    
}
