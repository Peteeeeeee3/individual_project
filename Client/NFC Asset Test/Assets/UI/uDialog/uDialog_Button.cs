using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace UI.Dialogs
{
    /// <summary>
    /// This class defines data used to create/update uDialog_Button objects
    /// </summary>
    [Serializable]
    public class uDialog_Button_Data
    {
        [SerializeField]
        public string ButtonText;

        [SerializeField]
        public bool Interactable = true;

        /// <summary>
        /// A UnityAction to be called when this button is clicked - Used for adding events through code.
        /// </summary>
        [SerializeField]
        public UnityAction OnClick;

        /// <summary>
        /// A Unity ButtonClickedEvent to be called when this button is clicked - Used for adding events in the editor.
        /// </summary>
        [SerializeField]
        public Button.ButtonClickedEvent OnClickEvent;

        [HideInInspector, NonSerialized]
        public uDialog_Button Button;

        [SerializeField,HideInInspector]
        private string _Guid;        
        public string Guid
        {
            get { return _Guid; }
            protected set { _Guid = value; }
        }        

        public uDialog_Button_Data()
        {
            if (String.IsNullOrEmpty(Guid))
            {
                Guid = System.Guid.NewGuid().ToString();
            }
        }

        public void Update()
        {
            if (this.Button != null) this.Button.SetData(this);
        }
    }

    [RequireComponent(typeof(Button))]
    public class uDialog_Button : MonoBehaviour
    {
        public uDialog_Button_Data ButtonData;
        public Button buttonComponent { get; protected set; }
        
        public bool IsTemplate = false;        

        public void SetData(uDialog_Button_Data data)
        {            
            if (buttonComponent == null) buttonComponent = this.GetComponent<Button>();

            this.buttonComponent.GetComponentInChildren<Text>().text = data.ButtonText;

            buttonComponent.onClick = data.OnClickEvent ?? new Button.ButtonClickedEvent();
            
            if (data.OnClick != null)
            {                
                this.buttonComponent.onClick.RemoveListener(data.OnClick);
                this.buttonComponent.onClick.AddListener(data.OnClick);
            }
            
            this.gameObject.name = data.ButtonText;

            this.ButtonData = data;

            this.buttonComponent.interactable = data.Interactable;

            ButtonData.Button = this;
        }        
    }
}
