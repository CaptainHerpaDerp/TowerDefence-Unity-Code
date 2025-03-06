using System.Collections;
using UnityEngine;

namespace UIElements
{
    public class ExpandingScrollHorizontal : ExpandingScroll
    {
        [SerializeField] private float scrollStartWidth, scrollTargetWidth;

        /// <summary>
        /// Expands the scroll object's width to the target width, then fades in the elements
        /// </summary>
        protected override IEnumerator ExpandScroll()
        {
            float time = 0;

            // Set the scroll's starting width
            scrollObject.GetComponent<RectTransform>().sizeDelta = new Vector2(scrollStartWidth, scrollObject.GetComponent<RectTransform>().sizeDelta.y);

            while (time < scrollExpandTime)
            {
                float newWidth = Mathf.Lerp(scrollStartWidth, scrollTargetWidth, time / scrollExpandTime);

                // increase the rect transform width
                scrollObject.GetComponent<RectTransform>().sizeDelta = new Vector2(newWidth, scrollObject.GetComponent<RectTransform>().sizeDelta.y);

                time += Time.unscaledDeltaTime;
                yield return null;
            }

            // Fully expand the scroll
            scrollObject.GetComponent<RectTransform>().sizeDelta = new Vector2(scrollTargetWidth, scrollObject.GetComponent<RectTransform>().sizeDelta.y);

            // Fade in the scroll elements
            FadeInScrollElements();

            yield return null;
        }

        protected override void QuickExpandScroll()
        {
            scrollObject.GetComponent<RectTransform>().sizeDelta = new Vector2(scrollTargetWidth, scrollObject.GetComponent<RectTransform>().sizeDelta.y);

            // Fade in the scroll instantly as well as its elements
            FadeInScrollInstant();
        }
    }
}
