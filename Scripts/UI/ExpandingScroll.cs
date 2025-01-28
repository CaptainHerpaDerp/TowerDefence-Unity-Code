using System.Collections;
using TMPro;
using UnityEngine;
using Core;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Base class for expanding scroll UI objects
    /// </summary>
    public abstract class ExpandingScroll : MonoBehaviour
    {
        [SerializeField] protected Transform elementsGroupParent;
        [SerializeField] protected GameObject scrollObject;
        [SerializeField] protected float scrollExpandTime = 1f, scrollFadeTime;

        // If true, the elements group parent will be disabled when the scroll is faded out
        [SerializeField] private bool disableElementsOnFadeOut = false;

        private EventBus eventBus;

        protected SoundEffectManager soundEffectManager;

        protected void Start()
        {
            soundEffectManager = SoundEffectManager.Instance;
            eventBus = EventBus.Instance;
        }

        protected abstract IEnumerator ExpandScroll();
        protected abstract void QuickExpandScroll();

        /// <summary>
        /// Enables and expands the scroll object
        /// </summary>
        public virtual void EnableScroll()
        {
            if (soundEffectManager == null)
            {
                soundEffectManager = SoundEffectManager.Instance;
            }

            StopAllCoroutines();

            // Disable the elements group parent
            soundEffectManager.PlayScrollOpenSound();
            elementsGroupParent.gameObject.SetActive(false);
            scrollObject.SetActive(true);
            StartCoroutine(FadeInScrollRoutine());
            StartCoroutine(ExpandScroll());
        }

        /// <summary>
        /// Expand the scroll object instantly
        /// </summary>
        public void QuickEnableScroll()
        {
            StopAllCoroutines();

            // Disable the elements group parent
            soundEffectManager.PlayScrollOpenSound();
            elementsGroupParent.gameObject.SetActive(false);
            scrollObject.SetActive(true);

            QuickExpandScroll();

            Image scrollImage = scrollObject.GetComponent<Image>();
            scrollImage.color = new Color(scrollImage.color.r, scrollImage.color.g, scrollImage.color.b, 1);

            QuickEnableScrollElements();
        }

        /// <summary>
        /// Instantly disables the scroll object and elements group parent
        /// </summary>
        public void DisableScroll()
        {
            // Disable the elements group parent
            elementsGroupParent.gameObject.SetActive(false);
            scrollObject.SetActive(false);
        }

        /// <summary>
        /// Fades out the scroll object and elements group parent
        /// </summary>
        public void FadeOutScroll()
        {
            StopAllCoroutines();

            soundEffectManager.PlayScrollCloseSound();
            StartCoroutine(FadeOutScrollRoutine());
            StartCoroutine(FadeOutScrollElements());
            StartCoroutine(FadeOutTextElements());
        }

        /// <summary>
        ///  Fades in the scroll object
        /// </summary>
        protected virtual IEnumerator FadeInScrollRoutine()
        {
            Image scrollImage = scrollObject.GetComponent<Image>();

            scrollImage.color = new Color(scrollImage.color.r, scrollImage.color.g, scrollImage.color.b, 0);

            float time = 0;

            while (time < scrollFadeTime)
            {
                float newAlpha = Mathf.Lerp(0, 1, time / scrollFadeTime);

                Color newColor = new(scrollImage.color.r, scrollImage.color.g, scrollImage.color.b, newAlpha);
                scrollImage.color = newColor;

                time += Time.unscaledDeltaTime;
                yield return null;
            }

            // Fully fade in the scroll
            scrollImage.color = new Color(scrollImage.color.r, scrollImage.color.g, scrollImage.color.b, 1);

            yield return null;
        }

        /// <summary>
        /// Fades out the scroll object
        /// </summary>
        protected virtual IEnumerator FadeOutScrollRoutine()
        {
            Image scrollImage = scrollObject.GetComponent<Image>();

            scrollImage.color = new Color(scrollImage.color.r, scrollImage.color.g, scrollImage.color.b, 0);

            float time = 0;

            while (time < scrollFadeTime)
            {
                float newAlpha = Mathf.Lerp(1, 0, time / scrollFadeTime);

                Color newColor = new(scrollImage.color.r, scrollImage.color.g, scrollImage.color.b, newAlpha);
                scrollImage.color = newColor;

                time += Time.unscaledDeltaTime;
                yield return null;
            }

            // Fully fade out the scroll
            scrollImage.color = new Color(scrollImage.color.r, scrollImage.color.g, scrollImage.color.b, 0);

            yield return null;
        }

        /// <summary>
        /// Gets image components of each element in the elements group parent and fade them in
        /// </summary>
        protected virtual IEnumerator FadeInScrollElements()
        {
            elementsGroupParent.gameObject.SetActive(true);

            Image[] images = elementsGroupParent.GetComponentsInChildren<Image>();

            foreach (Image image in images)
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
            }

            float time = 0;

            while (time < scrollFadeTime)
            {
                float newAlpha = Mathf.Lerp(0, 1, time / scrollFadeTime);

                foreach (Image image in images)
                {
                    Color newColor = new Color(image.color.r, image.color.g, image.color.b, newAlpha);

                    image.color = newColor;
                }


                time += Time.unscaledDeltaTime;
                yield return null;
            }

            // Fully fade in the elements
            foreach (Image image in images)
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
            }

            yield return null;
        }

        /// <summary>
        /// Instantly enables the scroll elements
        /// </summary>
        protected void QuickEnableScrollElements()
        {
            elementsGroupParent.gameObject.SetActive(true);

            Image[] images = elementsGroupParent.GetComponentsInChildren<Image>();

            foreach (Image image in images)
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
            }
        }

        /// <summary>
        /// Gets image components of each element in the elements group parent and fade them out
        /// </summary>
        /// <returns></returns>
        private IEnumerator FadeOutScrollElements()
        {
            Image[] images = elementsGroupParent.GetComponentsInChildren<Image>();         

            float time = 0;

            while (time < scrollFadeTime)
            {
                float newAlpha = Mathf.Lerp(1, 0, time / scrollFadeTime);

                foreach (Image image in images)
                {
                    Color newColor = new Color(image.color.r, image.color.g, image.color.b, newAlpha);

                    image.color = newColor;
                }

                time += Time.unscaledDeltaTime;
                yield return null;
            }

            // Fully fade out the elements
            foreach (Image image in images)
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
            }

            if (disableElementsOnFadeOut)
            {
                elementsGroupParent.gameObject.SetActive(false);
                scrollObject.SetActive(false);
            }

            yield return null;
        }

        protected void DisableTextElements()
        {
            TextMeshProUGUI[] texts = elementsGroupParent.GetComponentsInChildren<TextMeshProUGUI>();

            // disable all text components
            foreach (TextMeshProUGUI text in texts)
            {
                text.enabled = false;
            }
        }

        protected void QuickEnableTextElements()
        {
            TextMeshProUGUI[] texts = elementsGroupParent.GetComponentsInChildren<TextMeshProUGUI>();

            // enable all text components
            foreach (TextMeshProUGUI text in texts)
            {
                text.enabled = true;
            }
        }

        protected IEnumerator FadeInTextElements()
        {
            elementsGroupParent.gameObject.SetActive(true);

            TextMeshProUGUI[] textComponents = elementsGroupParent.GetComponentsInChildren<TextMeshProUGUI>();

            foreach (TextMeshProUGUI textComponent in textComponents)
            {
                textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, 0);
            }

            float time = 0;

            while (time < scrollFadeTime)
            {
                float newAlpha = Mathf.Lerp(0, 1, time / scrollFadeTime);

                foreach (TextMeshProUGUI textComponent in textComponents)
                {
                    Color newColor = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, newAlpha);
                    textComponent.color = newColor;
                }


                time += Time.unscaledDeltaTime;
                yield return null;
            }

            // Fully fade in the elements
            foreach (TextMeshProUGUI textComponent in textComponents)
            {
                textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, 1);
            }

            yield return null;
        }


        protected IEnumerator FadeOutTextElements()
        {
            elementsGroupParent.gameObject.SetActive(true);

            TextMeshProUGUI[] textComponents = elementsGroupParent.GetComponentsInChildren<TextMeshProUGUI>();

            foreach (TextMeshProUGUI textComponent in textComponents)
            {
                textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, 1);
            }

            float time = 0;

            while (time < scrollFadeTime)
            {
                float newAlpha = Mathf.Lerp(1, 0, time / scrollFadeTime);

                foreach (TextMeshProUGUI textComponent in textComponents)
                {
                    Color newColor = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, newAlpha);
                    textComponent.color = newColor;
                }


                time += Time.unscaledDeltaTime;
                yield return null;
            }

            // Fully fade out the elements
            foreach (TextMeshProUGUI textComponent in textComponents)
            {
                textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, 0);
            }

            yield return null;
        }

        /// <summary>
        /// Returns true if the scroll is fully expanded, its alpha is 1, and the elements group parent is active
        /// </summary>
        public bool IsScrollExpanded()
        {
            Image scrollImage = scrollObject.GetComponent<Image>();

            if (scrollImage.color.a == 1 && elementsGroupParent.gameObject.activeSelf)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
