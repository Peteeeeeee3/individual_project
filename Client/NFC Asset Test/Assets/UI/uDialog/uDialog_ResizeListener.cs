using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UI.Dialogs
{    
    [RequireComponent(typeof(RectTransform))]
    public class uDialog_ResizeListener : MonoBehaviour
    {
        public eResizeListenerType ResizeListenerType;

        public bool Vertical = true;
        public bool Horizontal = true;
        public bool InverseHorizontal = false;        

        public uDialog uDialog;

        protected RectTransform _RectTransform;
        public RectTransform RectTransform
        {
            get
            {
                if (_RectTransform == null)
                {
                    _RectTransform = uDialog.RectTransform;
                }

                return _RectTransform;
            }
        }        

        public void OnDrag(BaseEventData data)
        {
            if (RectTransform == null) return;
            var pointerData = data as PointerEventData;
            if (pointerData == null) return;

            // Adjusting the pivot allows us to maintain the illusion of only moving the side we're dragging
            // (and is easier than attempting to calculate x/y differences for various different pivot settings)
            if (uDialog.AllowResizeToAdjustPivot)
            {
                uDialog.SetPivot(this.ResizeListenerType);
            }

            var sizeDelta = RectTransform.sizeDelta;            

            sizeDelta += new Vector2
                (
                    Horizontal ? InverseHorizontal ? -pointerData.delta.x : pointerData.delta.x : 0,
                    Vertical ? -pointerData.delta.y : 0
                );

            sizeDelta = new Vector2
                (
                    Mathf.Clamp(sizeDelta.x, uDialog.MinSize.x, uDialog.MaxSize.x > 0 ? uDialog.MaxSize.x : float.MaxValue),
                    Mathf.Clamp(sizeDelta.y, uDialog.MinSize.y, uDialog.MaxSize.y > 0 ? uDialog.MaxSize.y : float.MaxValue)
                );
            
            RectTransform.sizeDelta = sizeDelta;            
        }      
    }    
}
