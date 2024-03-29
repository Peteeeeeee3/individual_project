using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using UI.Dialogs;

public static class uDialog_Fluent
{
    /// <summary>
    /// Should this uDialog show a title?
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="showTitle"></param>
    /// <returns></returns>
    public static uDialog SetShowTitle(this uDialog uDialog, bool showTitle = true)
    {
        uDialog.ShowTitle = showTitle;
        uDialog.UpdateDisplay();

        return uDialog;
    }

    /// <summary>
    /// Should this uDialog show a close button on the title?
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="showTitleCloseButton"></param>
    /// <returns></returns>
    public static uDialog SetShowTitleCloseButton(this uDialog uDialog, bool showTitleCloseButton = true)
    {
        uDialog.ShowTitleCloseButton = showTitleCloseButton;
        uDialog.UpdateDisplay();

        return uDialog;
    }

    /// <summary>
    /// Should this uDialog show a minimize button on the title?
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="showTitleCloseButton"></param>
    /// <returns></returns>
    public static uDialog SetShowTitleMinimizeButton(this uDialog uDialog, bool showTitleMinimizeButton = true)
    {
        uDialog.ShowTitleMinimizeButton = showTitleMinimizeButton;
        uDialog.UpdateDisplay();

        return uDialog;
    }

    /// <summary>
    /// Sets the title text of this uDialog.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="title"></param>
    /// <param name="dontSetName"></param>
    /// <returns></returns>
    public static uDialog SetTitleText(this uDialog uDialog, string title, bool dontSetName = false)
    {
        uDialog.TitleText = title;
        uDialog.UpdateDisplay();

        if (!dontSetName)
        {
            uDialog.name = title;
        }

        return uDialog;
    }

    /// <summary>
    /// Sets whether or not this uDialog is initially visible.
    /// Please note: if the uDialog GameObject is not active, the uDialog will not be shown regardless of the value of this setting.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="visibleOnStart"></param>
    /// <returns></returns>
    public static uDialog SetVisibleOnStart(this uDialog uDialog, bool visibleOnStart = true)
    {
        uDialog.VisibleOnStart = visibleOnStart;
        return uDialog;
    }

    /// <summary>
    /// Set the content text of this uDialog.
    /// Set to null or empty to disable the text component (allowing the space it takes up to be used by other elements)
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    public static uDialog SetContentText(this uDialog uDialog, string text)
    {
        uDialog.ContentText = text;
        uDialog.UpdateDisplay();

        return uDialog;
    }

    /// <summary>
    /// Set the Icon Type of this uDialog.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="iconType"></param>
    /// <returns></returns>
    public static uDialog SetIcon(this uDialog uDialog, eIconType iconType)
    {
        uDialog.SetIconType(iconType);
        return uDialog;
    }

    /// <summary>
    /// Set a specific sprite as the icon for this uDialog.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="newIcon"></param>
    /// <returns></returns>
    public static uDialog SetIcon(this uDialog uDialog, Sprite newIcon)
    {
        uDialog.SetIconSprite(newIcon);
        return uDialog;
    }

    /// <summary>
    /// Set a specific sprite as the icon for this uDialog.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="newIcon"></param>
    /// <returns></returns>
    public static uDialog SetIcon(this uDialog uDialog, Sprite newIcon, Color newColor)
    {
        uDialog.SetIconSprite(newIcon, newColor);
        return uDialog;
    }

    /// <summary>
    /// Specify whether or not this uDialog should show any buttons.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="showButtons"></param>
    /// <returns></returns>
    public static uDialog SetShowButtons(this uDialog uDialog, bool showButtons =  true)
    {
        uDialog.ShowButtons = showButtons;
        uDialog.UpdateDisplay();

        return uDialog;
    }

    /// <summary>
    /// Specify whether or not this uDialog should close when any of its buttons are clicked.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="closeWhenAnyButtonClicked"></param>
    /// <returns></returns>
    public static uDialog SetCloseWhenAnyButtonClicked(this uDialog uDialog, bool closeWhenAnyButtonClicked = true)
    {
        uDialog.CloseWhenAnyButtonClicked = closeWhenAnyButtonClicked;

        return uDialog;
    }

    /// <summary>
    /// Add a new button to this uDialog.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="button"></param>
    /// <param name="showButtons"></param>
    /// <returns></returns>
    public static uDialog AddButton(this uDialog uDialog, uDialog_Button_Data button, bool showButtons = true, bool focusThisButton = false)
    {
        uDialog.Buttons.Add(button);
        uDialog.SetShowButtons(showButtons);

        uDialog.UpdateDisplay();

        if (focusThisButton)
        {
            uDialog.SetFocusedButton(button.Button);
        }

        return uDialog;
    }

    /// <summary>
    /// Add a new button to this uDialog.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="buttonText"></param>
    /// <param name="buttonAction"></param>
    /// <param name="showButtons"></param>
    /// <returns></returns>
    public static uDialog AddButton(this uDialog uDialog, string buttonText, UnityAction buttonAction, bool showButtons = true, bool focusThisButton = false)
    {
        return uDialog.AddButton(new uDialog_Button_Data { ButtonText = buttonText, OnClick = buttonAction }, showButtons, focusThisButton);
    }

    /// <summary>
    /// Add a new button to this uDialog.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="buttonText"></param>
    /// <param name="buttonAction"></param>
    /// <param name="showButtons"></param>
    /// <returns></returns>
    public static uDialog AddButton(this uDialog uDialog, string buttonText, UnityAction<uDialog> buttonAction, bool showButtons = true, bool focusThisButton = false)
    {
        return uDialog.AddButton(new uDialog_Button_Data { ButtonText = buttonText, OnClick = () => { buttonAction(uDialog); } }, showButtons, focusThisButton);
    }

    /// <summary>
    /// Add a new button to this uDialog.
    /// Will automatically call button.Update() after buttonAction is complete to carry over any changes to the button.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="buttonText"></param>
    /// <param name="buttonAction"></param>
    /// <param name="showButtons"></param>
    /// <returns></returns>
    public static uDialog AddButton(this uDialog uDialog, string buttonText, UnityAction<uDialog, uDialog_Button_Data> buttonAction, bool showButtons = true, bool focusThisButton = false)
    {
        var button = new uDialog_Button_Data { ButtonText = buttonText };
        button.OnClick = () =>
            {
                buttonAction(uDialog, button);
                button.Update();
            };

        return uDialog.AddButton(button, showButtons, focusThisButton);
    }

    /// <summary>
    /// Set the buttons for this uDialog (replacing any existing buttons)
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="showButtons"></param>
    /// <param name="buttons"></param>
    /// <returns></returns>
    public static uDialog SetButtons(this uDialog uDialog, bool showButtons, params uDialog_Button_Data[] buttons)
    {
        uDialog.Buttons = buttons.ToList();
        uDialog.SetShowButtons(showButtons);

        uDialog.UpdateDisplay();

        return uDialog;
    }

    /// <summary>
    /// Set the buttons for this uDialog (replacing any existing buttons)
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="buttons"></param>
    /// <returns></returns>
    public static uDialog SetButtons(this uDialog uDialog, params uDialog_Button_Data[] buttons)
    {
        return uDialog.SetButtons(true, buttons);
    }

    /// <summary>
    /// Specify whether or not this uDialog should be modal. If the uDialog is modal, the Screen Overlay will be shown.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="modal"></param>
    /// <param name="closeWhenOverlayIsClicked"></param>
    /// <returns></returns>
    public static uDialog SetModal(this uDialog uDialog, bool modal = true, bool closeWhenOverlayIsClicked = false)
    {
        uDialog.Modal = modal;
        uDialog.CloseWhenScreenOverlayIsClicked = closeWhenOverlayIsClicked;
        uDialog.UpdateDisplay();

        return uDialog;
    }

    /// <summary>
    /// Specify whether or not this uDialog should automatically close on its own after a specified duration.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="autoClose"></param>
    /// <param name="autoCloseTime"></param>
    /// <returns></returns>
    public static uDialog SetAutoClose(this uDialog uDialog, bool autoClose = true, float autoCloseTime = 10f)
    {
        uDialog.AutoClose = true;
        uDialog.AutoCloseTime = autoCloseTime;

        return uDialog;
    }

    /// <summary>
    /// Set the Theme Image Set used by this uDialog.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="imageSet"></param>
    /// <returns></returns>
    public static uDialog SetThemeImageSet(this uDialog uDialog, eThemeImageSet imageSet)
    {
        uDialog.ThemeImageSet = imageSet;
        uDialog.UpdateDisplay();

        return uDialog;
    }

    /// <summary>
    /// Set the Outline Mode used by this uDialog (Shadow / Glow / None)
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="outlineMode"></param>
    /// <returns></returns>
    public static uDialog SetOutlineMode(this uDialog uDialog, eOutlineMode outlineMode)
    {
        uDialog.OutlineMode = outlineMode;
        uDialog.UpdateDisplay();

        return uDialog;
    }

    /// <summary>
    /// Set the Color Scheme used by this uDialog.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="newColorScheme"></param>
    /// <returns></returns>
    public static uDialog SetColorScheme(this uDialog uDialog, string newColorScheme)
    {
        if (uDialog_Utilities.ColorSchemeNames.Contains(newColorScheme))
        {
            uDialog.ColorScheme = newColorScheme;
        }
        else
        {
            Debug.LogError("Attempted to call uDialog.SetColorScheme(" + newColorScheme + ") - but this color scheme does not exist.");
        }

        return uDialog;
    }

    /// <summary>
    /// Set a custom color scheme, specifying the colors in a uDialog_ColorScheme object.
    /// This will set the ColorScheme used by this uDialog to 'Custom' to prevent the values from being overriden by any other color scheme.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="newColorScheme"></param>
    /// <returns></returns>
    public static uDialog SetCustomColorScheme(this uDialog uDialog, uDialog_ColorScheme newColorScheme)
    {
        uDialog.ColorScheme = "Custom";
        uDialog.LoadColorScheme(newColorScheme);

        return uDialog;
    }

    /// <summary>
    /// Save the colors currently used by this uDialog as a color scheme which can then be used later (via SetColorScheme)
    /// This can be used to overwrite existing schemes
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="colorSchemeName"></param>
    /// <returns>True if the save was successful, false otherwise</returns>
    public static bool SaveCurrentColorScheme(this uDialog uDialog, string colorSchemeName)
    {
        var scheme = new uDialog_ColorScheme();
        scheme.Name = colorSchemeName;
        scheme.TitleBackground = uDialog.Color_TitleBackground;
        scheme.TitleText = uDialog.Color_TitleText;
        scheme.ViewportBackground = uDialog.Color_ViewportBackground;
        scheme.ViewportText = uDialog.Color_ViewportText;
        scheme.ButtonBackground = uDialog.Color_ButtonBackground;
        scheme.ButtonText = uDialog.Color_ButtonText;
        scheme.ScreenOverlay = uDialog.Color_ScreenOverlay;
        scheme.Glow = uDialog.Color_Glow;
        scheme.Shadow = uDialog.Color_Shadow;
        scheme.Icon_Information = uDialog.Icon_Information_Color;
        scheme.Icon_Question = uDialog.Icon_Question_Color;
        scheme.Icon_Warning = uDialog.Icon_Warning_Color;

        var result = uDialog_Utilities.AddColorScheme(colorSchemeName, scheme);

        if (result != "") // blank string == success
        {
            Debug.LogError("Error saving color scheme: " + result);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Specify whether or not this uDialog should close when it is clicked (anywhere)
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="closeWhenClicked"></param>
    /// <returns></returns>
    public static uDialog SetCloseWhenClicked(this uDialog uDialog, bool closeWhenClicked = true)
    {
        uDialog.CloseWhenClicked = closeWhenClicked;

        return uDialog;
    }

    /// <summary>
    /// Add an event to be triggered when this uDialog is shown.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="onShowEvent"></param>
    /// <returns></returns>
    public static uDialog AddOnShowEvent(this uDialog uDialog, UnityAction<uDialog> onShowEvent)
    {
        if (uDialog.Event_OnShow == null)
        {
            uDialog.Event_OnShow = new uDialog.uDialog_Event();
        }

        uDialog.Event_OnShow.AddListener(onShowEvent);

        return uDialog;
    }

    /// <summary>
    /// Add an event to be triggered when this uDialog is shown.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="onShowEvent"></param>
    /// <returns></returns>
    public static uDialog AddOnShowEvent(this uDialog uDialog, UnityAction onShowEvent)
    {
        var wrappedOnShowEvent = new UnityAction<uDialog>((e) => { onShowEvent(); });

        return uDialog.AddOnShowEvent(wrappedOnShowEvent);
    }

    /// <summary>
    /// Add an event to be triggered when this uDialog is closed.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="onCloseEvent"></param>
    /// <returns></returns>
    public static uDialog AddOnCloseEvent(this uDialog uDialog, UnityAction<uDialog> onCloseEvent)
    {
        if (uDialog.Event_OnClose == null)
        {
            uDialog.Event_OnClose = new uDialog.uDialog_Event();
        }

        uDialog.Event_OnClose.AddListener(onCloseEvent);

        return uDialog;
    }

    /// <summary>
    /// Add an event to be triggered when this uDialog is closed.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="onCloseEvent"></param>
    /// <returns></returns>
    public static uDialog AddOnCloseEvent(this uDialog uDialog, UnityAction onCloseEvent)
    {
        var wrappedOnCloseEvent = new UnityAction<uDialog>((e) => { onCloseEvent(); });

        return uDialog.AddOnCloseEvent(wrappedOnCloseEvent);
    }

    /// <summary>
    /// Add an event to be triggered when this uDialog is clicked.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="onClickEvent"></param>
    /// <returns></returns>
    public static uDialog AddOnClickEvent(this uDialog uDialog, UnityAction<uDialog> onClickEvent)
    {
        if (uDialog.Event_OnClick == null)
        {
            uDialog.Event_OnClick = new uDialog.uDialog_Event();
        }

        uDialog.Event_OnClick.AddListener(onClickEvent);

        return uDialog;
    }

    /// <summary>
    /// Add an event to be triggered when this uDialog is clicked.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="onClickEvent"></param>
    /// <returns></returns>
    public static uDialog AddOnClickEvent(this uDialog uDialog, UnityAction onClickEvent)
    {
        var wrappedOnClickEvent = new UnityAction<uDialog>((e) => { onClickEvent(); });

        return uDialog.AddOnClickEvent(wrappedOnClickEvent);
    }

    public static uDialog AddOnMinimizeEvent(this uDialog uDialog, UnityAction<uDialog> onMinimizeEvent)
    {
        if (uDialog.Event_OnMinimize == null)
        {
            uDialog.Event_OnMinimize = new uDialog.uDialog_Event();
        }

        uDialog.Event_OnMinimize.AddListener(onMinimizeEvent);

        return uDialog;
    }

    public static uDialog AddOnMinimizeEvent(this uDialog uDialog, UnityAction onMinimizeEvent)
    {
        var wrappedOnMinimizeEvent = new UnityAction<uDialog>((e) => { onMinimizeEvent(); });

        return uDialog.AddOnMinimizeEvent(wrappedOnMinimizeEvent);
    }

    public static uDialog AddOnMaximizeEvent(this uDialog uDialog, UnityAction<uDialog> onMaximizeEvent)
    {
        if (uDialog.Event_OnMaximize == null)
        {
            uDialog.Event_OnMaximize = new uDialog.uDialog_Event();
        }

        uDialog.Event_OnMaximize.AddListener(onMaximizeEvent);

        return uDialog;
    }

    public static uDialog AddOnMaximizeEvent(this uDialog uDialog, UnityAction onMaximizeEvent)
    {
        var wrappedOnMaximizeEvent = new UnityAction<uDialog>((e) => { onMaximizeEvent(); });

        return uDialog.AddOnMaximizeEvent(wrappedOnMaximizeEvent);
    }

    /// <summary>
    /// Add this uDialog window to a uDialog_TaskBar
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="taskBar"></param>
    /// <param name="isActive"></param>
    /// <returns></returns>
    public static uDialog AddToTaskBar(this uDialog uDialog, uDialog_TaskBar taskBar, bool isActive = true)
    {
        taskBar.AddTask(uDialog, isActive);

        return uDialog;
    }

    /// <summary>
    /// Set the animation to be used when this uDialog is shown.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="showAnimation"></param>
    /// <returns></returns>
    public static uDialog SetShowAnimation(this uDialog uDialog, eShowAnimation showAnimation)
    {
        uDialog.ShowAnimation = showAnimation;

        return uDialog;
    }

    /// <summary>
    /// Specify the animation to be used when this uDialog is closed.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="closeAnimation"></param>
    /// <returns></returns>
    public static uDialog SetCloseAnimation(this uDialog uDialog, eCloseAnimation closeAnimation)
    {
        uDialog.CloseAnimation = closeAnimation;

        return uDialog;
    }

    /// <summary>
    /// Set the width of this uDialog.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="width"></param>
    /// <returns></returns>
    public static uDialog SetWidth(this uDialog uDialog, float width)
    {
        var rectTransform = (uDialog.transform as RectTransform);

        return uDialog.SetDimensions(width, rectTransform.sizeDelta.y);
    }

    /// <summary>
    /// Set the height of this uDialog.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public static uDialog SetHeight(this uDialog uDialog, float height)
    {
        var rectTransform = (uDialog.transform as RectTransform);

        return uDialog.SetDimensions(rectTransform.sizeDelta.x, height);
    }

    /// <summary>
    /// Set the width and height of this uDialog.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public static uDialog SetDimensions(this uDialog uDialog, float width, float height)
    {
        var rectTransform = (uDialog.transform as RectTransform);
        rectTransform.sizeDelta = new Vector2(width, height);

        var layoutComponent = uDialog.GetComponent<LayoutElement>();
        if (layoutComponent == null)
        {
            layoutComponent = uDialog.gameObject.AddComponent<LayoutElement>();
        }

        layoutComponent.preferredWidth = width;
        layoutComponent.preferredHeight = height;

        return uDialog;
    }

    /// <summary>
    /// Set the parent of this uDialog (e.g. so that it can be contained within another RectTransform)
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static uDialog SetParent(this uDialog uDialog, RectTransform parent)
    {
        uDialog.gameObject.transform.SetParent(parent);

        var t = uDialog.transform as RectTransform;
        t.anchoredPosition = new Vector2();

        return uDialog;
    }

    /// <summary>
    /// Set the parent of this uDialog (e.g. so that it can be contained within another RectTransform)
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static uDialog SetParent(this uDialog uDialog, GameObject parent)
    {
        var t = parent.transform as RectTransform;
        if (t == null)
        {
            Debug.LogWarning("Warning: SetParent called, but parent " + parent.name + " does not have a rectTransform!");
        }
        else
        {
            uDialog.SetParent(t);
        }

        return uDialog;
    }

    /// <summary>
    /// Set the font, size, style, and effect of the title text.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="newFont"></param>
    /// <param name="newSize"></param>
    /// <param name="newStyle"></param>
    /// <param name="newEffect"></param>
    /// <returns></returns>
    public static uDialog SetTitleFont(this uDialog uDialog, Font newFont, int? newSize = null, FontStyle? newStyle = null, eTextEffect? newEffect = null)
    {
        uDialog.TitleFontSettings.Font = newFont;
        if (newSize.HasValue) uDialog.SetTitleFontSize(newSize.Value);
        if (newStyle.HasValue) uDialog.SetTitleFontStyle(newStyle.Value);
        if (newEffect.HasValue) uDialog.SetTitleTextEffect(newEffect.Value);

        return uDialog;
    }

    /// <summary>
    /// Set the font, size, style, and effect of the content font.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="newFont"></param>
    /// <param name="newSize"></param>
    /// <param name="newStyle"></param>
    /// <param name="newEffect"></param>
    /// <returns></returns>
    public static uDialog SetContentFont(this uDialog uDialog, Font newFont, int? newSize = null, FontStyle? newStyle = null, eTextEffect? newEffect = null)
    {
        uDialog.ContentFontSettings.Font = newFont;
        if (newSize.HasValue) uDialog.SetContentFontSize(newSize.Value);
        if (newStyle.HasValue) uDialog.SetContentFontStyle(newStyle.Value);
        if (newEffect.HasValue) uDialog.SetContentTextEffect(newEffect.Value);

        return uDialog;
    }

    /// <summary>
    /// Set the size of the title font.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="newSize"></param>
    /// <returns></returns>
    public static uDialog SetTitleFontSize(this uDialog uDialog, int newSize)
    {
        uDialog.GO_TitleText.resizeTextForBestFit = false;
        uDialog.TitleFontSettings.FontSize = newSize;

        return uDialog;
    }

    /// <summary>
    /// Set the size of the title font - it will use standard 'resizeTextForBestFit' behaviour between minSize and maxSize
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="minSize"></param>
    /// <param name="maxSize"></param>
    /// <returns></returns>
    public static uDialog SetTitleFontSize(this uDialog uDialog, int minSize, int maxSize)
    {
        uDialog.GO_TitleText.resizeTextForBestFit = true;
        uDialog.GO_TitleText.resizeTextMinSize = minSize;
        uDialog.GO_TitleText.resizeTextMaxSize = maxSize;

        return uDialog;
    }

    /// <summary>
    /// Set the size of the content font.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="newSize"></param>
    /// <returns></returns>
    public static uDialog SetContentFontSize(this uDialog uDialog, int newSize)
    {
        uDialog.GO_MessageText.resizeTextForBestFit = false;
        uDialog.ContentFontSettings.FontSize = newSize;

        return uDialog;
    }

    /// <summary>
    /// Set the size of the content font - it will use standard 'resizeTextForBestFit' behaviour between minSize and maxSize
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="minSize"></param>
    /// <param name="maxSize"></param>
    /// <returns></returns>
    public static uDialog SetContentFontSize(this uDialog uDialog, int minSize, int maxSize)
    {
        uDialog.GO_MessageText.resizeTextForBestFit = true;
        uDialog.GO_MessageText.resizeTextMinSize = minSize;
        uDialog.GO_MessageText.resizeTextMaxSize = maxSize;

        return uDialog;
    }

    /// <summary>
    /// Set the style of the title font.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="newStyle"></param>
    /// <returns></returns>
    public static uDialog SetTitleFontStyle(this uDialog uDialog, FontStyle newStyle)
    {
        uDialog.TitleFontSettings.FontStyle = newStyle;

        return uDialog;
    }

    /// <summary>
    /// Set the style of the content font.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="newStyle"></param>
    /// <returns></returns>
    public static uDialog SetContentFontStyle(this uDialog uDialog, FontStyle newStyle)
    {
        uDialog.ContentFontSettings.FontStyle = newStyle;

        return uDialog;
    }

    /// <summary>
    /// Set the text effect used by the title text.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="newEffect"></param>
    /// <returns></returns>
    public static uDialog SetTitleTextEffect(this uDialog uDialog, eTextEffect newEffect)
    {
        uDialog.TitleFontSettings.TextEffect = newEffect;

        return uDialog;
    }

    /// <summary>
    /// Set the text effect used by the content text.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="newEffect"></param>
    /// <returns></returns>
    public static uDialog SetContentTextEffect(this uDialog uDialog, eTextEffect newEffect)
    {
        uDialog.ContentFontSettings.TextEffect = newEffect;

        return uDialog;
    }

    /// <summary>
    /// Set the font used by the buttons.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="font"></param>
    /// <returns></returns>
    public static uDialog SetButtonFont(this uDialog uDialog, Font font)
    {
        uDialog.ButtonFontSettings.Font = font;

        return uDialog;
    }

    /// <summary>
    /// Set the text effect used by the buttons.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="newEffect"></param>
    /// <returns></returns>
    public static uDialog SetButtonTextEffect(this uDialog uDialog, eTextEffect newEffect)
    {
        uDialog.ButtonFontSettings.TextEffect = newEffect;

        return uDialog;
    }

    /// <summary>
    /// Set the font style used by the buttons.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="newStyle"></param>
    /// <returns></returns>
    public static uDialog SetButtonFontStyle(this uDialog uDialog, FontStyle newStyle)
    {
        uDialog.ButtonFontSettings.FontStyle = newStyle;

        return uDialog;
    }

    /// <summary>
    /// Set the font size used by the buttons.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="newSize"></param>
    /// <returns></returns>
    public static uDialog SetButtonFontSize(this uDialog uDialog, int newSize)
    {
        uDialog.ButtonFontSettings.FontSize = newSize;

        var text = uDialog.GO_ButtonTemplate.GetComponentInChildren<Text>();
        if (text.resizeTextForBestFit)
        {
            text.resizeTextForBestFit = false;
            uDialog.ForceButtonUpdate();
        }

        return uDialog;
    }

    /// <summary>
    /// Set the font size used by the buttons - they will use standard 'resizeTextForBestFit' behaviour between minSize and maxSize
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="minSize"></param>
    /// <param name="maxSize"></param>
    /// <returns></returns>
    public static uDialog SetButtonFontSize(this uDialog uDialog, int minSize, int maxSize)
    {
        var text = uDialog.GO_ButtonTemplate.GetComponentInChildren<Text>();

        text.resizeTextForBestFit = true;
        text.resizeTextMinSize = minSize;
        text.resizeTextMaxSize = maxSize;

        uDialog.ForceButtonUpdate();

        return uDialog;
    }

    public static uDialog SetButtonSize(this uDialog uDialog, float width, float height)
    {
        var layoutElement = uDialog.GO_ButtonTemplate.GetComponent<LayoutElement>();
        layoutElement.preferredWidth = width;
        layoutElement.preferredHeight = height;

        uDialog.ForceButtonUpdate();

        return uDialog;
    }

    /// <summary>
    /// Specify whether or not this uDialog should be destroyed after closing.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="destroyAfterClose"></param>
    /// <returns></returns>
    public static uDialog SetDestroyAfterClose(this uDialog uDialog, bool destroyAfterClose = true)
    {
        uDialog.DestroyAfterClose = destroyAfterClose;

        return uDialog;
    }

    /// <summary>
    /// Remove all buttons from this uDialog.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <returns></returns>
    public static uDialog ClearButtons(this uDialog uDialog)
    {
        uDialog.Buttons = new List<uDialog_Button_Data>();
        uDialog.UpdateButtons();

        return uDialog;
    }

    /// <summary>
    /// Specify whether or not this uDialog should close when the screen overlay is clicked (Only applicable to modal dialogs)
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="close"></param>
    /// <returns></returns>
    public static uDialog SetCloseWhenOverlayClicked(this uDialog uDialog, bool close = true)
    {
        uDialog.CloseWhenScreenOverlayIsClicked = close;

        return uDialog;
    }

    /// <summary>
    /// Specify the sound to be played when this uDialog is shown.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="onShowSound"></param>
    /// <returns></returns>
    public static uDialog SetOnShowSound(this uDialog uDialog, AudioClip onShowSound)
    {
        uDialog.OnShowSound = onShowSound;

        return uDialog;
    }

    /// <summary>
    /// Specify the sound to be played when this uDialog is closed.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="onCloseSound"></param>
    /// <returns></returns>
    public static uDialog SetOnCloseSound(this uDialog uDialog, AudioClip onCloseSound)
    {
        uDialog.OnCloseSound = onCloseSound;

        return uDialog;
    }

    /// <summary>
    /// Specify the sound to be played when any of this uDialogs buttons are clicked.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="onButtonClickSound"></param>
    /// <returns></returns>
    public static uDialog SetOnButtonClickSound(this uDialog uDialog, AudioClip onButtonClickSound)
    {
        uDialog.OnButtonClickSound = onButtonClickSound;

        return uDialog;
    }

    /// <summary>
    /// Set the audio volume for all sounds played by this uDialog.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="audioVolume"></param>
    /// <returns></returns>
    public static uDialog SetAudioVolume(this uDialog uDialog, float audioVolume)
    {
        uDialog.AudioVolume = audioVolume;

        return uDialog;
    }

    /// <summary>
    /// Set the AudioMixerGroup to be used by this uDialog - this is completely optional, but can be used so that you can assign this uDialog to a Mixer Group for volume control.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="audioMixerGroup"></param>
    /// <returns></returns>
    public static uDialog SetAudioMixerGroup(this uDialog uDialog, UnityEngine.Audio.AudioMixerGroup audioMixerGroup)
    {
        uDialog.AudioMixerGroup = audioMixerGroup;

        return uDialog;
    }

    /// <summary>
    /// Set the size of the icon used by this uDialog.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="iconSize"></param>
    /// <returns></returns>
    public static uDialog SetIconSize(this uDialog uDialog, float iconSize)
    {
        var layoutElement = uDialog.GO_Icon.GetComponent<LayoutElement>();

        layoutElement.preferredHeight = iconSize;
        layoutElement.preferredWidth = iconSize;

        return uDialog;
    }

    /// <summary>
    /// Specify whether or not this uDialog can be dragged by its title.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="allowDragging"></param>
    /// <returns></returns>
    public static uDialog SetAllowDraggingViaTitle(this uDialog uDialog, bool allowDragging = true)
    {
        uDialog.AllowDraggingViaTitle = allowDragging;

        return uDialog;
    }

    /// <summary>
    /// Specify whether or not this uDialog can be dragged from anywhere.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="allowDragging"></param>
    /// <returns></returns>
    public static uDialog SetAllowDraggingViaDialog(this uDialog uDialog, bool allowDragging = true)
    {
        uDialog.AllowDraggingViaDialog = allowDragging;

        return uDialog;
    }

    /// <summary>
    /// Specify whether or not this uDialog can be dragged by its title and/or the rest of the dialog.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="allowDraggingViaTitle"></param>
    /// <param name="allowDraggingViaDialog"></param>
    /// <returns></returns>
    public static uDialog SetAllowDragging(this uDialog uDialog, bool allowDraggingViaTitle = true, bool allowDraggingViaDialog = true)
    {
        uDialog.AllowDraggingViaTitle = allowDraggingViaTitle;
        uDialog.AllowDraggingViaDialog = allowDraggingViaDialog;

        return uDialog;
    }

    /// <summary>
    /// Specify whether or not this uDialog is resizeable from a specific direction.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="direction"></param>
    /// <param name="resizeable"></param>
    /// <returns></returns>
    public static uDialog SetResizeableFromDirection(this uDialog uDialog, eResizeDirection direction, bool resizeable = true)
    {
        switch (direction)
        {
            case eResizeDirection.Left:
                uDialog.AllowResizeFromLeft = resizeable;
                break;
            case eResizeDirection.Right:
                uDialog.AllowResizeFromRight = resizeable;
                break;
            case eResizeDirection.Bottom:
                uDialog.AllowResizeFromBottom = resizeable;
                break;
        }

        uDialog.UpdateResizeListeners();

        return uDialog;
    }

    /// <summary>
    /// Specify whether or not this uDialog is resizeable from the right, bottom, and left sides.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="fromRight"></param>
    /// <param name="fromBottom"></param>
    /// <param name="fromLeft"></param>
    /// <returns></returns>
    public static uDialog SetResizeable(this uDialog uDialog, bool fromRight = true, bool fromBottom = true, bool fromLeft = false)
    {
        uDialog.AllowResizeFromLeft = fromLeft;
        uDialog.AllowResizeFromRight = fromRight;
        uDialog.AllowResizeFromBottom = fromBottom;

        uDialog.UpdateResizeListeners();

        return uDialog;
    }

    /// <summary>
    /// Set the minimum size of this uDialog when resizing.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="newSize"></param>
    /// <returns></returns>
    public static uDialog SetMinSize(this uDialog uDialog, Vector2 newSize)
    {
        uDialog.MinSize = newSize;

        return uDialog;
    }

    /// <summary>
    /// Set the minimum size of this uDialog when resizing.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="minX"></param>
    /// <param name="minY"></param>
    /// <returns></returns>
    public static uDialog SetMinSize(this uDialog uDialog, float minX, float minY)
    {
        uDialog.MinSize = new Vector2(minX, minY);

        return uDialog;
    }

    /// <summary>
    /// Set the maximum size of this uDialog when resizing.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="newSize"></param>
    /// <returns></returns>
    public static uDialog SetMaxSize(this uDialog uDialog, Vector2 newSize)
    {
        uDialog.MaxSize = newSize;

        return uDialog;
    }

    /// <summary>
    /// Set the maximum size of this uDialog when resizing.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="maxX"></param>
    /// <param name="maxY"></param>
    /// <returns></returns>
    public static uDialog SetMaxSize(this uDialog uDialog, float maxX, float maxY)
    {
        uDialog.MaxSize = new Vector2(maxX, maxY);

        return uDialog;
    }

    /// <summary>
    /// Set the content of this uDialog window - primarily used to add custom content that doesn't fit into the standard icon/message/button layout.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="contentTransform"></param>
    /// <returns></returns>
    public static uDialog SetContent(   this uDialog uDialog,
                                        RectTransform contentTransform,
                                        float? preferredHeight = null,
                                        ePreviousContentAction previousContentAction = ePreviousContentAction.Nothing)
    {
        if (uDialog.GO_Content.transform.childCount > 0)
        {
            foreach(RectTransform rt in uDialog.GO_Content.transform)
            {
                switch (previousContentAction)
                {
                    case ePreviousContentAction.Disable:
                        {
                            rt.gameObject.SetActive(false);
                        }
                        break;
                    case ePreviousContentAction.Destroy:
                        {
                            GameObject.DestroyImmediate(rt.gameObject);
                        }
                        break;
                }
            }
        }

        var sizeDelta = contentTransform.sizeDelta;
        var anchoredPosition = contentTransform.anchoredPosition;

#if UNITY_EDITOR
        UnityEditor.Undo.SetTransformParent(contentTransform, uDialog.GO_Content.transform, "uDialog - Wrap Content " + contentTransform.name);
#else
        contentTransform.SetParent(uDialog.GO_Content.transform);
#endif
        uDialog.GO_Content.gameObject.SetActive(contentTransform != null);

        contentTransform.sizeDelta = sizeDelta;
        contentTransform.localPosition = Vector3.zero;
        contentTransform.anchoredPosition = anchoredPosition;
        contentTransform.localScale = Vector3.one;

        if (contentTransform != null)
        {
            contentTransform.gameObject.SetActive(true);
        }

        if (preferredHeight.HasValue)
        {
            var layoutElement = uDialog.GO_Content.GetComponent<LayoutElement>();
            layoutElement.preferredHeight = preferredHeight.Value;
        }

        return uDialog;
    }

    /// <summary>
    /// Specify whether or not this uDialog should be clamped within the bounds of its parent.
    /// If this is set to false, it is possible to drag the uDialog outside of its parent or even off-screen.
    /// </summary>
    /// <param name="uDialog"></param>
    /// <param name="restrict"></param>
    /// <returns></returns>
    public static uDialog SetRestrictDraggingToParentBounds(this uDialog uDialog, bool restrict = true)
    {
        uDialog.RestrictToParentBounds = restrict;

        return uDialog;
    }

    public static uDialog SetTriggerOnClickEventWhenOverlayClicked(this uDialog uDialog, bool trigger = true)
    {
        uDialog.TriggerOnClickEventWhenOverlayIsClicked = trigger;

        return uDialog;
    }

    public static uDialog SetFocusedButton(this uDialog uDialog, uDialog_Button button)
    {
        EventSystem.current.SetSelectedGameObject(button.gameObject);

        return uDialog;
    }

    public static uDialog SetFocusedButton(this uDialog uDialog, string buttonText)
    {
        var button = uDialog.Buttons.FirstOrDefault(b => b.ButtonText.Equals(buttonText, StringComparison.OrdinalIgnoreCase));

        if (button != null)
        {
            uDialog.SetFocusedButton(button.Button);
        }

        return uDialog;
    }

    public static uDialog SetMessagePadding(this uDialog uDialog, RectOffset padding)
    {
        var contentLayout = uDialog.GO_MessageContainer.GetComponent<LayoutGroup>();
        contentLayout.padding = padding;

        return uDialog;
    }

    public static uDialog SetMessagePadding(this uDialog uDialog, int left, int right, int top, int bottom)
    {
        return SetMessagePadding(uDialog, new RectOffset(left, right, top, bottom));
    }

    public static uDialog SetViewportPadding(this uDialog uDialog, RectOffset padding)
    {
        var contentLayout = uDialog.GO_Viewport.GetComponent<LayoutGroup>();
        contentLayout.padding = padding;

        return uDialog;
    }

    public static uDialog SetViewportPadding(this uDialog uDialog, int left, int right, int top, int bottom)
    {
        return SetViewportPadding(uDialog, new RectOffset(left, right, top, bottom));
    }
}
