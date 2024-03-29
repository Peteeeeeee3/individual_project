using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Dialogs
{    
    public class uDialog_ScreenOverlay : MonoBehaviour
    {
        protected CanvasGroup _CanvasGroup;
        public CanvasGroup CanvasGroup
        {
            get
            {
                if (_CanvasGroup == null)
                {
                    _CanvasGroup = GetComponent<CanvasGroup>();
                    if (_CanvasGroup == null)
                    {
                        _CanvasGroup = this.gameObject.AddComponent<CanvasGroup>();
                    }
                }

                return _CanvasGroup;
            }
        }

        protected Image _Image;
        public Image Image
        {
            get
            {
                if (_Image == null)
                {
                    _Image = this.GetComponent<Image>();
                }

                return _Image;
            }
        }


        public Animator Animator { get; protected set; }
        public RuntimeAnimatorController RuntimeAnimatorController;

        void InitializeAnimator()
        {
            Animator = GetComponent<Animator>();

            if (Animator == null)
            {
                Animator = this.gameObject.AddComponent<Animator>();
            }
        }

        public void FadeIn(bool instant = false)
        {
            if (instant)
            {
                CanvasGroup.alpha = 1f;
            }
            else
            {
                PlayAnimation("FadeIn");
            }

            Image.raycastTarget = true;
        }

        public void FadeOut(bool instant = false)
        {
            if (instant)
            {
                CanvasGroup.alpha = 0f;
            }
            else
            {
                PlayAnimation("FadeOut");
            }

            // Allow click through
            Image.raycastTarget = false;
        }

        private void PlayAnimation(string animationName)
        {
            if (!Application.isPlaying) return;

            if (Animator == null)
            {
                InitializeAnimator();

                // shouldn't happen, but just in case
                if (Animator == null) return;
            }

            if (Animator.runtimeAnimatorController == null)
            {
                Animator.runtimeAnimatorController = RuntimeAnimatorController;
            }

            Animator.speed = 5f;
            Animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            Animator.enabled = true;
            Animator.StopPlayback();

            Animator.Play(animationName);
        }
    }
}