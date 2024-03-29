using UnityEngine;
using System.Collections;
using System.Linq;
using UI.Dialogs;

namespace UI.Dialogs.Examples
{
    public class uDialog_ExampleController : MonoBehaviour
    {
        public uDialog_NotificationPanel NotificationPanel;
        public eThemeImageSet ImageSet = eThemeImageSet.SharpEdges;
        public eOutlineMode OutlineMode = eOutlineMode.Shadow;
        public string ColorScheme = "Dark";

        public AudioClip OnButtonClickSound;
        public AudioClip OnShowSound;

        public RectTransform contentWindowContent;

        /// <summary>
        /// We're now using a container for the dialog elements so that uDialog.IsFocused() works correctly
        /// when a uDialog_NotificationPanel is present with "AlwaysOnTop" set to true
        /// (isFocused() checks if the uDialog is the last child in its parent transform, 
        /// which won't ever be true if the NotificationPanel has is always on top and shares the same parent)
        /// </summary>
        public RectTransform DialogContainer;

        public uDialog_TaskBar TaskBar;
        
        public void ShowSimpleDialogExample()
        {
            ShowSimpleDialog("Simple Dialog", "This is a simple <i>modal</i> Dialog.");
        }

        public void ShowConfirmDialogExample()
        {
            uDialog.NewDialog()
                   .SetTitleText("Confirmation Dialog")
                   .SetContentText("Would you like to continue?")
                   .SetThemeImageSet(ImageSet)
                   .SetColorScheme(ColorScheme)
                   .SetOutlineMode(OutlineMode)
                   .SetDimensions(468, 192)
                   .SetModal()
                   .SetShowTitleCloseButton(false)
                   .AddButton("Yes", () => { ShowSimpleDialog("Information", "You clicked <b><color=green>Yes</color></b>.", true, eIconType.Information); })
                   .AddButton("No", () => { ShowSimpleDialog("Information", "You clicked <b><color=red>No</color></b>.", true, eIconType.Information); })
                   .SetCloseWhenAnyButtonClicked(true)
                   .SetDestroyAfterClose(true)
                   .SetParent(DialogContainer);
        }

        private uDialog ShowSimpleDialog(string title, string text, bool closeButton = false, eIconType iconType = eIconType.None)
        {
            var dialog = uDialog.NewDialog()
                                .SetTitleText(title)
                                .SetContentText(text)
                                .SetThemeImageSet(ImageSet)
                                .SetColorScheme(ColorScheme)
                                .SetOutlineMode(OutlineMode)
                                .SetOnButtonClickSound(OnButtonClickSound)
                                .SetOnShowSound(OnShowSound)
                                .SetIcon(iconType)
                                .SetContentFontSize(18)
                                .SetDimensions(384, 192)
                                .SetModal(true, true)
                                .SetDestroyAfterClose(true)
                                .SetAllowDraggingViaTitle()
                                .SetParent(DialogContainer);

            if (closeButton)
            {
                dialog.AddButton("Close", (d) => d.Close());
            }

            return dialog;
        }

        public void ShowThemePresetMenuExample()
        {
            var menu = uDialog.NewMenu()
                              .SetTitleText("Example Presets")
                              .SetShowTitleCloseButton(true)
                              .SetThemeImageSet(ImageSet)
                              .SetColorScheme(ColorScheme)
                              .SetOutlineMode(OutlineMode)
                              .SetOnButtonClickSound(OnButtonClickSound)
                              .SetOnShowSound(OnShowSound)
                              .SetDimensions(384, 512)
                              .SetModal(false)
                              .SetDestroyAfterClose(true)
                              .AddToTaskBar(TaskBar)
                              .SetShowTitleMinimizeButton(true)
                              .SetParent(DialogContainer);

            menu.AddButton("Light - Rounded Edges", () => { SetThemeImageSet(eThemeImageSet.RoundedEdges); SetColorScheme("Light"); SetOutlineMode(eOutlineMode.Shadow); });
            menu.AddButton("Dark - Sharp Edges", () => { SetThemeImageSet(eThemeImageSet.SharpEdges); SetColorScheme("Dark"); SetOutlineMode(eOutlineMode.Shadow); });
            menu.AddButton("Fantasy", () => { SetThemeImageSet(eThemeImageSet.Fantasy); SetColorScheme("Fantasy"); SetOutlineMode(eOutlineMode.Shadow); });
            menu.AddButton("Angular - Blue Glow", () => { SetThemeImageSet(eThemeImageSet.Angular); SetColorScheme("Blue Glow"); SetOutlineMode(eOutlineMode.Glow); });
            menu.AddButton("Angular - Green Glow", () => { SetThemeImageSet(eThemeImageSet.Angular); SetColorScheme("Green Glow"); SetOutlineMode(eOutlineMode.Glow); });
            menu.AddButton("Sci-Fi - Green", () => { SetThemeImageSet(eThemeImageSet.SciFi); SetColorScheme("Green Highlight"); SetOutlineMode(eOutlineMode.Shadow); });
            menu.AddButton("Sci-Fi - Blue", () => { SetThemeImageSet(eThemeImageSet.SciFi); SetColorScheme("Blue Highlight"); SetOutlineMode(eOutlineMode.Shadow); });
            menu.AddButton("Sci-Fi - Orange/Red", () => { SetThemeImageSet(eThemeImageSet.SciFi); SetColorScheme("Orange Red"); SetOutlineMode(eOutlineMode.Shadow); });
        }

        public void ShowImageSetMenuExample()
        {
            var menu = uDialog.NewMenu()
                              .SetTitleText("Image Sets")
                              .SetContentText("Use built-in image sets - or use your own custom images!")
                              .SetShowTitleCloseButton(true)
                              .SetThemeImageSet(ImageSet)
                              .SetColorScheme(ColorScheme)
                              .SetOutlineMode(OutlineMode)
                              .SetOnButtonClickSound(OnButtonClickSound)
                              .SetOnShowSound(OnShowSound)
                              .SetDimensions(384, 512)
                              .SetModal(false)
                              .SetDestroyAfterClose(true)
                              .AddToTaskBar(TaskBar)
                              .SetShowTitleMinimizeButton(true)
                              .SetParent(DialogContainer);

            var imageSets = System.Enum.GetValues(typeof(eThemeImageSet)).Cast<eThemeImageSet>();
            foreach (var imageSet in imageSets)
            {
                if (imageSet == eThemeImageSet.Custom) continue;

                var _imageSet = imageSet; // extract the value as imageSet's value will have changed when the button is clicked

                menu.AddButton(imageSet.ToString(), () => { this.SetThemeImageSet(_imageSet); });
            }
        }

        public void ShowColorSchemeMenuExample()
        {
            var menu = uDialog.NewMenu()
                              .SetTitleText("Color Schemes")
                              .SetContentText("New color schemes can be created easily in the editor.\n<i><size=10>Some color schemes look better with specific image sets.</size></i>")
                              .SetShowTitleCloseButton(true)
                              .SetThemeImageSet(ImageSet)
                              .SetColorScheme(ColorScheme)
                              .SetOutlineMode(OutlineMode)
                              .SetOnButtonClickSound(OnButtonClickSound)
                              .SetOnShowSound(OnShowSound)
                              .SetDimensions(400, 550)
                              .SetModal(false)
                              .SetDestroyAfterClose(true)
                              .AddToTaskBar(TaskBar)
                              .SetShowTitleMinimizeButton(true)
                              .SetParent(DialogContainer);                              

            foreach (var colorScheme in uDialog_Utilities.ColorSchemeNames)
            {
                if (colorScheme == "Custom") continue;

                var _colorScheme = colorScheme;

                menu.AddButton(_colorScheme, () => { this.SetColorScheme(_colorScheme); });
            }
        }

        public void ShowOutlineSchemeMenuExample()
        {
            var menu = uDialog.NewMenu()
                              .SetTitleText("Outline")
                              .SetShowTitleCloseButton(true)
                              .SetThemeImageSet(ImageSet)
                              .SetColorScheme(ColorScheme)
                              .SetOutlineMode(OutlineMode)
                              .SetOnButtonClickSound(OnButtonClickSound)
                              .SetOnShowSound(OnShowSound)
                              .SetDimensions(384, 256)
                              .SetModal(false)
                              .SetDestroyAfterClose(true)
                              .AddToTaskBar(TaskBar)
                              .SetShowTitleMinimizeButton(true)
                              .SetParent(DialogContainer)
                              .SetShowAnimation(eShowAnimation.FadeIn);                              

            var outlineModes = System.Enum.GetValues(typeof(eOutlineMode)).Cast<eOutlineMode>();

            foreach (var outlineMode in outlineModes)
            {                
                var _outlineMode = outlineMode;

                menu.AddButton(outlineMode.ToString(), () => { this.SetOutlineMode(_outlineMode); });                
            }            
        }

        public void ShowNotificationsExample()
        {            
            uDialog.NewMenu()
                   .SetTitleText("Notifications")
                   .SetShowTitleCloseButton(true)
                   .SetThemeImageSet(ImageSet)
                   .SetColorScheme(ColorScheme)
                   .SetOutlineMode(OutlineMode)
                   .SetOnButtonClickSound(OnButtonClickSound)
                   .SetOnShowSound(OnShowSound)
                   .SetDimensions(320, 320)
                   .SetDestroyAfterClose(true)
                   .SetModal(false)
                   .SetAllowDraggingViaTitle()
                   .AddButton("Add Notification", () => { AddNotification("You clicked the button!"); })
                   .AddButton("Mode: " + (NotificationPanel.NotificationDirection == eNotificationDirection.BottomUp ? "Bottom Up" : "Top Down"),
                       (dialog, button) =>
                       {
                           // Toggle between Bottom Up and Top Down mode
                           NotificationPanel.SetNotificationDirection(
                               NotificationPanel.NotificationDirection == eNotificationDirection.BottomUp
                               ? eNotificationDirection.TopDown
                               : eNotificationDirection.BottomUp);

                           // Update button text to match
                           button.ButtonText = "Mode: " + (NotificationPanel.NotificationDirection == eNotificationDirection.BottomUp ? "Bottom Up" : "Top Down");                           
                       })
                   .AddButton("Icon: " + (NotificationPanel.NotificationTemplate.IconType == eIconType.None ? "<color=orange>Hidden</color>" : "<color=green>Visible</color>"),
                       (dialog, button) =>
                       {
                           var newIconType = NotificationPanel.NotificationTemplate.IconType == eIconType.None ? eIconType.Information : eIconType.None;
                           NotificationPanel.NotificationTemplate.SetIcon(newIconType);
                           button.ButtonText = "Icon: " + (newIconType == eIconType.None ? "<color=orange>Hidden</color>" : "<color=green>Visible</color>");                           
                       })
                 .SetShowTitleMinimizeButton(true)
                 .AddToTaskBar(TaskBar)
                 .SetParent(DialogContainer);                                                                 
        }

        public void ShowDraggingExample()
        {
            uDialog.NewMenu()
                   .SetThemeImageSet(ImageSet)
                   .SetColorScheme(ColorScheme)
                   .SetOutlineMode(OutlineMode)
                   .SetDimensions(400, 256)
                   .SetShowTitleCloseButton(true)
                   .SetTitleText("Dragging")
                   .SetContentText("uDialogs can optionally be draggable via the title, or via the entire dialog. Dragging can optionally be restricted the the bounds of the dialogs parent element.")
                   .SetContentFontSize(14)
                   .SetCloseWhenAnyButtonClicked(false)
                   .SetButtonFontSize(14)
                   .SetButtonFontStyle(FontStyle.Bold)
                   .SetAllowDraggingViaTitle(true)
                   .AddButton("Drag via Title: <color=green>Enabled</color>",
                        (dialog, button) =>
                        {
                            dialog.AllowDraggingViaTitle = !dialog.AllowDraggingViaTitle;
                            button.ButtonText = "Drag via Title: " + (dialog.AllowDraggingViaTitle ? "<color=green>Enabled</color>" : "<color=orange>Disabled</color>");                            
                        })
                   .AddButton("Drag via Dialog : <color=orange>Disabled</color>",
                        (dialog, button) =>
                        {
                            dialog.AllowDraggingViaDialog = !dialog.AllowDraggingViaDialog;
                            button.ButtonText = "Drag via Dialog: " + (dialog.AllowDraggingViaDialog ? "<color=green>Enabled</color>" : "<color=Orange>Disabled</color>");                            
                        })
                   .SetShowTitleMinimizeButton(true)
                   .AddToTaskBar(TaskBar)
                   .SetParent(DialogContainer);                               
        }

        public void ShowResizeExample()
        {
            var dialog = uDialog.NewMenu()
                                .SetThemeImageSet(ImageSet)
                                .SetColorScheme(ColorScheme)
                                .SetOutlineMode(OutlineMode)
                                .SetDimensions(500, 300)
                                .SetShowTitleCloseButton(true)
                                .SetTitleText("Resizing")
                                .SetContentText("uDialogs can optionally be resizeable from the left, right, and bottom sides (as well as bottom-left and bottom-right).\n\nWith optional Min and Max sizes.")
                                .SetContentFontSize(14)
                                .SetCloseWhenAnyButtonClicked(false)
                                .SetButtonFontSize(14)
                                .SetButtonFontStyle(FontStyle.Bold)
                                .SetResizeable(true, true, false)
                                .SetMinSize(350, 300)
                                .SetMaxSize(700, 500)
                                .SetAllowDraggingViaTitle(true)
                                .AddToTaskBar(TaskBar)
                                .SetParent(DialogContainer);

            var toggleResizeFromRight = new uDialog_Button_Data
            {
                ButtonText = "Resizeable From Right : <color=green>Enabled</color>"
            };

            toggleResizeFromRight.OnClick = new UnityEngine.Events.UnityAction(
                () =>
                {
                    if (dialog.AllowResizeFromRight)
                    {
                        dialog.SetResizeableFromDirection(eResizeDirection.Right, false);
                        toggleResizeFromRight.ButtonText = "Resizeable From Right: <color=orange>Disabled</color>";
                    }
                    else
                    {
                        dialog.SetResizeableFromDirection(eResizeDirection.Right, true);
                        toggleResizeFromRight.ButtonText = "Resizeable From Right: <color=green>Enabled</color>";
                    }

                    toggleResizeFromRight.Update();
                });

            var toggleResizeFromBottom = new uDialog_Button_Data
            {
                ButtonText = "Resizeable From Bottom: <color=green>Enabled</color>"
            };

            toggleResizeFromBottom.OnClick = new UnityEngine.Events.UnityAction(
                () =>
                {
                    if (dialog.AllowResizeFromBottom)
                    {
                        dialog.SetResizeableFromDirection(eResizeDirection.Bottom, false);
                        toggleResizeFromBottom.ButtonText = "Resizeable From Bottom: <color=orange>Disabled</color>";
                    }
                    else
                    {
                        dialog.SetResizeableFromDirection(eResizeDirection.Bottom, true);
                        toggleResizeFromBottom.ButtonText = "Resizeable From Bottom: <color=green>Enabled</color>";
                    }

                    toggleResizeFromBottom.Update();
                });

            var toggleResizeFromLeft = new uDialog_Button_Data
            {
                ButtonText = "Resizeable From Left: <color=orange>Disabled</color>"
            };

            toggleResizeFromLeft.OnClick = new UnityEngine.Events.UnityAction(
                () =>
                {
                    if (dialog.AllowResizeFromLeft)
                    {
                        dialog.SetResizeableFromDirection(eResizeDirection.Left, false);
                        toggleResizeFromLeft.ButtonText = "Resizeable From Left: <color=orange>Disabled</color>";
                    }
                    else
                    {
                        dialog.SetResizeableFromDirection(eResizeDirection.Left, true);
                        toggleResizeFromLeft.ButtonText = "Resizeable From Left: <color=green>Enabled</color>";
                    }

                    toggleResizeFromLeft.Update();
                });

            dialog.AddButton(toggleResizeFromRight);
            dialog.AddButton(toggleResizeFromBottom);
            dialog.AddButton(toggleResizeFromLeft);
        }

        public void ShowContentWindowExample()
        {
            // We could use contentWindowContent directly here, but it's best in this case if we clone it instead
            // (that way, we can use it over and over again for this example)
            var content = Instantiate(contentWindowContent) as RectTransform;
            uDialog_Utilities.FixInstanceTransform(contentWindowContent, content);

            var window = uDialog.NewDialog()
                                .SetThemeImageSet(ImageSet)
                                .SetColorScheme(ColorScheme)
                                .SetOutlineMode(OutlineMode)
                                .SetContentText(
                                    "uDialog.SetContent(RectTransform transform) allows you to place existing GUI elements within a uDialog window. "
                                    + "In this example, the nested uDialog has been created manually, in advance, and then set as the content of this dynamically created window."
                                    )
                                .SetContentFontSize(10, 22)
                                .SetButtonFontSize(10, 14)
                                .SetButtonSize(150, 64)
                                .SetTitleText("Content Window")
                                .SetDimensions(640, 480)
                                .SetAllowDragging(true, false)
                                .SetResizeable()
                                .SetMinSize(320, 240)
                                .SetContent(content, 400)
                                .SetDestroyAfterClose(true)
                                .SetCloseWhenAnyButtonClicked(false)
                                .SetShowTitleMinimizeButton(true)
                                .AddToTaskBar(TaskBar)
                                .SetParent(DialogContainer);                                
            
            window.AddButton("Hide this message", 
                () =>
                {
                    window.SetContentText("");
                    window.SetIcon(eIconType.None);
                    window.SetShowButtons(false);
                });            
        }

        public void SetThemeImageSet(eThemeImageSet imageSet)
        {            
            this.ImageSet = imageSet;

            var dialogs = GameObject.FindObjectsOfType<uDialog>();
            foreach (var dialog in dialogs)
            {
                dialog.SetThemeImageSet(imageSet);
            }

            NotificationPanel.NotificationTemplate.SetThemeImageSet(imageSet);
        }

        public void SetColorScheme(string colorScheme)
        {
            this.ColorScheme = colorScheme;

            var dialogs = GameObject.FindObjectsOfType<uDialog>();
            foreach (var dialog in dialogs)
            {
                dialog.SetColorScheme(colorScheme);
            }

            NotificationPanel.NotificationTemplate.SetColorScheme(colorScheme);
        }

        public void SetOutlineMode(eOutlineMode outlineMode)
        {
            this.OutlineMode = outlineMode;

            var dialogs = GameObject.FindObjectsOfType<uDialog>();
            foreach (var dialog in dialogs)
            {
                dialog.SetOutlineMode(outlineMode);
            }

            NotificationPanel.NotificationTemplate.SetOutlineMode(outlineMode);
        }

        public void AddNotification(string notificationText)
        {
            NotificationPanel.AddNotification(notificationText);            
        }
    }
}
