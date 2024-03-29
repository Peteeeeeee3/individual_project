using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace UI.Dialogs
{    
    [RequireComponent(typeof(LayoutGroup))]
    public class uDialog_TaskBar : MonoBehaviour
    {
        private RectTransform _transform = null;

        [Header("Focus"), Tooltip("If this is set, then windows which are not currently in front (but are active) will be brought to the front when the TaskBar button is clicked instead of being minimized. This is useful when you have several large windows.")]
        public bool FocusDialogWhenClicked = false;

        [Space, Header("Visibility")]
        public bool ShowActiveTask = true;
        public bool ShowFocusedTask = true;
        public bool ShowInactiveTasks = true;

        [Space, Header("Tasks")]
        public List<uDialog> Tasks = new List<uDialog>();

        [Space]
        public uDialog CurrentTask = null;        

        [Space, Header("References")]
        public uDialog_TaskBar_Task TaskTemplate_ActiveTask = null;
        public uDialog_TaskBar_Task TaskTemplate_FocusedTask = null;
        public uDialog_TaskBar_Task TaskTemplate_InactiveTask = null;                

        protected bool m_lateUpdateProcessed = false;

        void Awake()
        {            
            _transform = this.gameObject.transform as RectTransform;            
        }

        void Start()
        {
            UpdateDisplay();            
        }

        void OnValidate()
        {
            UpdateDisplay();
        }

        void LateUpdate()
        {
            if (m_lateUpdateProcessed) return;

            m_lateUpdateProcessed = true;

            UpdateDisplay();
        }

        public void AddTask(uDialog dialog, bool isActive = true)
        {
            if (Tasks.Any(t => t == dialog)) return;

            Tasks.Add(dialog);

            dialog.GO_TaskBar = this;

            SetFocusedTask(dialog);
            UpdateDisplay();
        }

        public void RemoveTask(uDialog dialog)
        {
            Tasks.RemoveAll(t => t == dialog);

            UpdateDisplay();
        }

        public void SetFocusedTask(uDialog dialog)
        {
            CurrentTask = dialog;
            UpdateDisplay();
        }

        public void UpdateDisplay()
        {
            //Tasks.ForEach(t => t.GO_TaskBar = this);
            foreach (var task in Tasks)
            {
                task.GO_TaskBar = this;
            }            

            var existingTasks = this.GetComponentsInChildren<uDialog_TaskBar_Task>(true);
            
            foreach (var task in existingTasks)
            {
                if (task.IsTemplate) continue;
                
                if (Application.isPlaying)
                {
                    GameObject.Destroy(task.gameObject);
                }
                else
                {
                    GameObject.DestroyImmediate(task.gameObject);
                }
            }            

            foreach (var uDialog in Tasks)
            {
                var state = (uDialog.isVisible && uDialog.gameObject.activeInHierarchy) 
                                ? ((uDialog == CurrentTask) ? TaskState.Focused : TaskState.Active)
                                : TaskState.Inactive;

                uDialog_TaskBar_Task template = null;
                switch (state)
                {
                    case TaskState.Active:
                        if(ShowActiveTask) template = TaskTemplate_ActiveTask;
                        break;
                    case TaskState.Focused:
                        if(ShowFocusedTask) template = TaskTemplate_FocusedTask;
                        break;
                    case TaskState.Inactive:
                        if(ShowInactiveTasks) template = TaskTemplate_InactiveTask;
                        break;
                }

                if (template == null) continue;

                var task = Instantiate(template);
                task.transform.SetParent(this._transform);
                task.gameObject.SetActive(true);
                task.transform.localScale = Vector3.one;
                task.transform.localPosition = Vector3.zero;

                task.IsTemplate = false;
                task.TaskBar = this;
                task.SetDialog(uDialog);                
            }
        }

        public void UpdateDisplayDelayed(int frames = 1)
        {
            StartCoroutine(_UpdateDisplayDelayed(frames));
        }

        protected IEnumerator _UpdateDisplayDelayed(int frames = 1)
        {
            for(var x = 0; x < frames; x++)
            {
                yield return null;
            }

            UpdateDisplay();
        }

        protected enum TaskState
        {            
            Active,
            Focused,
            Inactive
        }
    }    
}
