using System.Collections;
using UnityEngine;

namespace UI
{
    public class ExpandingScrollVectical : ExpandingScroll
    {
        public float scrollStartHeight, scrollTargetHeight;

        /// <summary>
        /// Expands the scroll object's Height to the target Height, then fades in the elements
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator ExpandScroll()
        {
            float time = 0;

            // Set the scroll's starting Height
            scrollObject.GetComponent<RectTransform>().sizeDelta = new Vector2(scrollObject.GetComponent<RectTransform>().sizeDelta.x, scrollStartHeight);

            while (time < scrollExpandTime)
            {
                float newHeight = Mathf.Lerp(scrollStartHeight, scrollTargetHeight, time / scrollExpandTime);

                // increase the rect transform Height
                scrollObject.GetComponent<RectTransform>().sizeDelta = new Vector2(scrollObject.GetComponent<RectTransform>().sizeDelta.x, newHeight);

                time += Time.unscaledDeltaTime;
                yield return null;
            }

            // Fully expand the scroll
            scrollObject.GetComponent<RectTransform>().sizeDelta = new Vector2(scrollObject.GetComponent<RectTransform>().sizeDelta.x, scrollTargetHeight);

            StartCoroutine(FadeInScrollElements());
            StartCoroutine(FadeInTextElements());
            yield return null;
        }

        protected override void QuickExpandScroll()
        {
            scrollObject.GetComponent<RectTransform>().sizeDelta = new Vector2(scrollObject.GetComponent<RectTransform>().sizeDelta.x, scrollTargetHeight);
            QuickEnableScrollElements();
            QuickEnableTextElements();
        }
    }
}
