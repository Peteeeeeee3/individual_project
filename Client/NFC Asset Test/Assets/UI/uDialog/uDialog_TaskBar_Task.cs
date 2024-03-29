using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace UI.Dialogs
{            
    public class uDialog_TaskBar_Task : MonoBehaviour
    {
        [SerializeField]
        public uDialog Dialog;
        
        [SerializeField]
        public Text GO_TextComponent;

        [SerializeField]
        public bool IsTemplate = false;

        [SerializeField]
        public uDialog_TaskBar TaskBar;

        public void SetDialog(uDialog dialog)
        {
            Dialog = dialog;

            GO_TextComponent.text = dialog.TitleText;
            this.name = dialog.TitleText;
        }

        public void Clicked()
        {
            if (Dialog.isVisible && Dialog.gameObject.activeInHierarchy)
            {
                if (TaskBar.FocusDialogWhenClicked && !Dialog.IsFocused())
                {
                    // bring to the front
                    Dialog.Focus();
                }
                else 
                {
                    // otherwise, minimize
                    Dialog.Minimize();
                }
            }
            else
            {                
                Dialog.Show();
                TaskBar.SetFocusedTask(Dialog);
            }            
        }        
    }    
}
