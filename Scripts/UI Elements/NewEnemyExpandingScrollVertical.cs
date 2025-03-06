using System.Collections;
using UnityEngine;

namespace UIElements
{

    /// <summary>
    /// A specialized version of the ExpandingScroll class that is used for the New Enemy scroll in the UI
    /// </summary>
    public class NewEnemyExpandingScrollVertical : ExpandingScroll
    {
        public float scrollStartHeight, scrollTargetHeight;

        #region Overriden Methods

        /// <summary>
        /// Enables and expands the scroll object
        /// </summary>
        public override void EnableScroll()
        {
            StopAllCoroutines();

            // Disable the elements group parent
            PlayScrollOpenSound();

            FadeInScroll();

            StartCoroutine(ExpandScroll());
        }

        /// <summary>
        /// Expands the scroll object's Height to the target Height, then fades in the elements
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator ExpandScroll()
        {
            float time = 0;

            // Set the scroll's starting Height
            scrollImageComponent.GetComponent<RectTransform>().sizeDelta = new Vector2(scrollImageComponent.GetComponent<RectTransform>().sizeDelta.x, scrollStartHeight);

            while (time < scrollExpandTime)
            {
                float newHeight = Mathf.Lerp(scrollStartHeight, scrollTargetHeight, time / scrollExpandTime);

                // increase the rect transform Height
                scrollImageComponent.GetComponent<RectTransform>().sizeDelta = new Vector2(scrollImageComponent.GetComponent<RectTransform>().sizeDelta.x, newHeight);

                time += Time.unscaledDeltaTime;
                yield return null;
            }

            // Fully expand the scroll
            scrollImageComponent.GetComponent<RectTransform>().sizeDelta = new Vector2(scrollImageComponent.GetComponent<RectTransform>().sizeDelta.x, scrollTargetHeight);

            // Fade the scroll elements
            FadeInScrollElements();

            yield return null;
        }

        protected override void QuickExpandScroll()
        {
            throw new System.NotImplementedException();
        }


        #endregion
    }
}