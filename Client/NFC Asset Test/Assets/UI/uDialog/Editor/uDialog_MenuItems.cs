using UnityEngine;
using UnityEditor;

namespace UI.Dialogs
{    
    public class uDialog_MenuItems
    {
        [MenuItem("GameObject/UI/uDialog/uDialog Dialog")]
        public static void MenuItem_NewDialogMenu()
        {
            var gameObject = uDialog_Utilities.InstantiatePrefab("uDialog_Default");
            uDialog uDialog = gameObject.GetComponent<uDialog>();
            
            uDialog.Start();
        }

        [MenuItem("GameObject/UI/uDialog/uDialog Menu")]
        public static void MenuItem_NewMenu()
        {
            var gameObject = uDialog_Utilities.InstantiatePrefab("uDialog_Menu");
            uDialog uDialog = gameObject.GetComponent<uDialog>();

            uDialog.Start();
        }

        [MenuItem("GameObject/UI/uDialog/uDialog Notification Panel - Left")]
        public static void MenuItem_NotificationPanelLeft()
        {
            uDialog_Utilities.InstantiatePrefab("uDialog_NotificationPanel_Left");
        }

        [MenuItem("GameObject/UI/uDialog/uDialog Notification Panel - Right")]
        public static void MenuItem_NotificationPanelRight()
        {
            uDialog_Utilities.InstantiatePrefab("uDialog_NotificationPanel_Right");
        }

        [MenuItem("GameObject/UI/uDialog/Wrap Content")]
        public static void MenuItem_WrapContent()
        {
            uDialog_Utilities.WrapSelectedContent();
        }

        [MenuItem("GameObject/UI/uDialog/uDialog Task Bar")]
        public static void MenuItem_uDialogTaskBar()
        {
            uDialog_Utilities.InstantiatePrefab("uDialog_TaskBar");
        }
    }    
}
