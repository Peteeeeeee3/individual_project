using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace UI.Dialogs
{
    public enum eIconType
    {
        None,
        Information,
        Question,
        Warning,
        Custom
    }

    public enum eThemeImageSet
    {
        RoundedEdges,
        SharpEdges,
        NoImages,
        Fantasy,
        SciFi,
        Angular,
        Custom
    }

    public enum eOutlineMode
    {
        Shadow,
        Glow,
        None
    }

    public enum eShowAnimation
    {
        SlideIn_Left,
        SlideIn_Right,
        SlideIn_Top,
        SlideIn_Bottom,
        FadeIn,
        Grow,
        None
    }

    public enum eCloseAnimation
    {
        SlideOut_Left,
        SlideOut_Right,
        SlideOut_Top,
        SlideOut_Bottom,
        FadeOut,
        Shrink,
        None
    }

    public enum eTextEffect
    {
        None,
        Shadowed,
        Outline
    }

    public enum eNotificationDirection
    {
        TopDown,
        BottomUp
    }

    public enum eResizeListenerType
    {
        Left,
        Right,
        Bottom,
        BottomLeft,
        BottomRight
    }

    public enum eResizeDirection
    {
        Left,
        Right,
        Bottom
    }

    public enum ePreviousContentAction
    {
        Nothing,
        Disable,
        Destroy        
    }
}
