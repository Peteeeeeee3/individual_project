using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace UI.Dialogs
{    
    [RequireComponent(typeof(LayoutGroup))]
    public class uDialog_NotificationPanel : MonoBehaviour
    {
        public uDialog NotificationTemplate;        
        public eNotificationDirection NotificationDirection = eNotificationDirection.TopDown;

        public bool ShowNotificationTemplateInEditor = false;

        public bool AlwaysOnTop = true;

        private RectTransform _transform;
        private uDialog _lastNotificationAdded;

        void Start()
        {            
            _transform = this.gameObject.transform as RectTransform;            
        }

        void Update()
        {
            if (AlwaysOnTop)
            {
                this.transform.SetAsLastSibling();
            }
        }

        void OnValidate()
        {
            SetNotificationDirection(NotificationDirection);

            if (NotificationTemplate != null)
            {
                if (!Application.isPlaying && ShowNotificationTemplateInEditor)
                {
                    NotificationTemplate.gameObject.SetActive(true);
                }
                else
                {
                    NotificationTemplate.gameObject.SetActive(false);
                }
            }
        }

        public void SetNotificationDirection(eNotificationDirection newNotificationDirection)
        {
            NotificationDirection = newNotificationDirection;

            var rectTransform = (RectTransform)this.gameObject.transform;

            // This is needed for the content fitter
            switch (NotificationDirection)
            {
                case eNotificationDirection.BottomUp:
                    rectTransform.pivot = new Vector2(rectTransform.pivot.x, 0);
                    break;
                case eNotificationDirection.TopDown:
                    rectTransform.pivot = new Vector2(rectTransform.pivot.x, 1);
                    break;
            }
        }

        public uDialog GetLastNotificationAdded()
        {
            return _lastNotificationAdded;
        }

        public uDialog AddNotification(string notificationText)
        {
            var notification = Instantiate(NotificationTemplate) as uDialog;

            if (notificationText != null)
            {
                notification.SetContentText(notificationText);
            }

            return AddNotification(notification);
        }

        /// <summary>
        /// Add a notification based on the NotificationTemplate
        /// </summary>
        /// <returns></returns>
        public uDialog AddNotification(string notificationText, eIconType? iconType)
        {
            var notification = Instantiate(NotificationTemplate) as uDialog;

            if (notificationText != null)
            {
                notification.SetContentText(notificationText);
            }

            if (iconType.HasValue)
            {
                notification.SetIconType(iconType.Value);
            }

            return AddNotification(notification);
        }

        public uDialog AddNotification(string notificationText, Sprite icon)
        {
            var notification = Instantiate(NotificationTemplate) as uDialog;

            if (notificationText != null)
            {
                notification.SetContentText(notificationText);
            }

            notification.SetIcon(icon);

            return AddNotification(notification);
        }

        /// <summary>
        /// Add the specified notification to this NotificationPanel
        /// </summary>
        /// <param name="notification"></param>
        /// <returns></returns>
        public uDialog AddNotification(uDialog notification)
        {
            notification.SetParent(_transform);
            notification.transform.localScale = Vector3.one;
            notification.transform.localPosition = Vector3.zero;

            if (NotificationDirection == eNotificationDirection.BottomUp)
            {
                notification.transform.SetAsFirstSibling();
            }

            notification.Show();

            _lastNotificationAdded = notification;

            return notification;
        }
    } 
}
