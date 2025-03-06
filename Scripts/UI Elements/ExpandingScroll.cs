using System.Collections;
using UnityEngine;
using Core;
using Utilities;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using AudioManagement;

namespace UIElements
{
    /// <summary>
    /// Base class for expanding scroll UI objects
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class ExpandingScroll : MonoBehaviour
    {
        [BoxGroup("Visual Settings"), SerializeField] protected float scrollExpandTime = 1f, scrollFadeTime = 0.15f;

        [BoxGroup("Component References"), SerializeField] protected CanvasGroup scrollCanvasGroup, elementsCanvasGroup;

        [BoxGroup("Component References"), SerializeField] protected Image scrollImageComponent;
        protected GameObject scrollObject;

        // Singleton References
        private EventBus eventBus;
        protected AudioManager audioManager;
        protected FMODEvents fmodEvents;

        protected virtual void Start()
        {
            // Singleton Assignments
            eventBus = EventBus.Instance;
            audioManager = AudioManager.Instance;
            fmodEvents = FMODEvents.Instance;

            if (scrollCanvasGroup == null && !TryGetComponent(out scrollCanvasGroup))
            {
                Debug.LogError("Error at " + this + " scrollCanvasGroup not assigned!");
            }

            scrollObject = scrollImageComponent.gameObject;

            // Always start with the scroll disabled
            QuickDisableScroll();   
        }

        protected abstract IEnumerator ExpandScroll();
        protected abstract void QuickExpandScroll();

        #region Scroll Toggling

        /// <summary>
        /// Enables and expands the scroll object
        /// </summary>
        public virtual void EnableScroll()
        {
            StopAllCoroutines();

            PlayScrollOpenSound();

            // Ensure the elements are not visible
            Utils.DisableCanvasGroup(elementsCanvasGroup);

            StartCoroutine(Utils.FadeInCanvasGroup(scrollCanvasGroup, scrollFadeTime));
            StartCoroutine(ExpandScroll());
        }

        /// <summary>
        /// Expand the scroll object instantly
        /// </summary>
        public void QuickEnableScroll()
        {
            StopAllCoroutines();

            PlayScrollOpenSound();

            QuickExpandScroll();
            FadeInScrollInstant();
        }

        /// <summary>
        /// Fades out the scroll object and elements group parent
        /// </summary>
        public void DisableScroll()
        {
            StopAllCoroutines();

            PlayScrollCloseSound();

            // Fade out the scroll and elements
            FadeOutScroll();
        }

        /// <summary>
        /// Instantly disables the scroll object and elements group parent
        /// </summary>
        public void QuickDisableScroll()
        {
            // Disable the elements group parent
            scrollCanvasGroup.alpha = 0;
            scrollCanvasGroup.interactable = false;
            scrollCanvasGroup.blocksRaycasts = false;
        }


        #endregion

        #region Scroll Fading

        protected void FadeInScroll()
        {
            // Ensure the elements are not visible
            Utils.DisableCanvasGroup(elementsCanvasGroup);  

            // Fade in the scroll
            StartCoroutine(Utils.FadeInCanvasGroup(scrollCanvasGroup, scrollFadeTime));
        }
        
        /// <summary>
        /// Instantly enable the scroll and elements group parent
        /// </summary>
        protected void FadeInScrollInstant()
        {
            Utils.EnableCanvasGroup(scrollCanvasGroup);
            Utils.EnableCanvasGroup(elementsCanvasGroup);
        }

        protected void FadeInScrollElements()
        {
            StartCoroutine(Utils.FadeInCanvasGroup(elementsCanvasGroup, scrollFadeTime));
        }

        /// <summary>
        /// Fade out the scroll as well as the elements group parent
        /// </summary>
        protected void FadeOutScroll()
        {
            StartCoroutine(Utils.FadeOutCanvasGroup(scrollCanvasGroup, scrollFadeTime));
            StartCoroutine(Utils.FadeOutCanvasGroup(elementsCanvasGroup, scrollFadeTime));
        }

        protected void FadeOutScrollElements()
        {
            StartCoroutine(Utils.FadeOutCanvasGroup(elementsCanvasGroup, scrollFadeTime));
        }

        #endregion

        /// <summary>
        /// Returns true if the scroll is fully expanded, its alpha is 1, and the elements group parent is active
        /// </summary>
        public bool IsScrollExpanded()
        {
            return scrollCanvasGroup.alpha == 1;
        }

        #region Audio Methods

        protected void PlayScrollOpenSound()
        {
            audioManager.PlayOneShot(fmodEvents.scrollOpenSound, transform.position);
        }

        protected void PlayScrollCloseSound()
        {
            if (audioManager == null || fmodEvents == null)
            {
                audioManager = AudioManager.Instance;
                fmodEvents = FMODEvents.Instance;
            }

            audioManager.PlayOneShot(fmodEvents.scrollCloseSound, transform.position);
        }

        #endregion
    }
}
