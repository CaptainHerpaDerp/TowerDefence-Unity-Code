using System.Collections;
using UnityEngine;

namespace UIElements
{
    public class ExpandingScrollVertical : ExpandingScroll
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

            // Fade the scroll in
            FadeInScrollElements();

            yield return null;
        }

        protected override void QuickExpandScroll()
        {
            scrollObject.GetComponent<RectTransform>().sizeDelta = new Vector2(scrollObject.GetComponent<RectTransform>().sizeDelta.x, scrollTargetHeight);

            // Fade in the scroll instantly as well as its elements
            FadeInScrollInstant();
        }
    }
}
