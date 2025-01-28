using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class WaypoinTrail : MonoBehaviour
    {
        [Header("The image component of the waypoint trail")]
        [SerializeField] private Image image;

        [Header("The color of the trail when it will be animating")]
        [SerializeField] private Color drawColor;

        [Header("The color of the trail when it will be animating")]
        [SerializeField] private Color finalColor;

        [SerializeField] private Animator animatorComponent;

        [Header("The time it takes for the trail to animate - before the final color is applied")]
        [SerializeField] private float animationTime;

        [Header("Fading Settings")]
        [SerializeField] private float fadeTime;

        [Header("The next waypoint group that will be enabled after this trail is animated")]
        [SerializeField] private WaypointGroup nextWaypointGroup;

        private void Start()
        {
            // By default, the trail is disabled
            image.enabled = false;
        }

        /// <summary>
        /// Enable the trail to the final color without animating it.
        /// </summary>
        public void EnableTrail()
        {
            image.enabled = true;
            image.color = finalColor;
        }

        /// <summary>
        /// Play the animation of the trail and fade its color to default.
        /// </summary>
        public void AnimateTrail()
        {
            nextWaypointGroup.Hide();

            image.enabled = true;

            image.color = drawColor;

            animatorComponent.SetTrigger("Animate");

            StartCoroutine(FadeTrailColor());
        }

        /// <summary>
        /// Over a period of time, switch the color of the trail from the draw color to the final color.
        /// </summary>
        /// <returns></returns>
        private IEnumerator FadeTrailColor()
        {
            yield return new WaitForSeconds(animationTime);

            nextWaypointGroup.AnimateActive();

            float elapsedTime = 0;

            while (elapsedTime < animationTime)
            {
                Color newColor = Color.Lerp(drawColor, finalColor, elapsedTime / animationTime);
                image.color = newColor;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }
}
