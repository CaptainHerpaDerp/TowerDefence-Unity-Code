using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIElements
{
    public class PauseOverlay : MonoBehaviour
    {
        [SerializeField] private float panelFadeSpeed;
        [SerializeField] private Image panelImage;
        [SerializeField] private TMPro.TextMeshProUGUI pauseText;

        public void FadePanel(float darkness, bool replaceDarkness = true)
        {
            StopAllCoroutines();
            StartCoroutine(FadePanelCR(darkness, replaceDarkness, panelFadeSpeed));
        }

        public void UnfadePanel()
        {
            StopAllCoroutines();
            StartCoroutine(UnFadePanelCR());
        }

        /// <summary>
        /// Fades the panel with an optional callback action.
        /// </summary>
        /// <param name="darkness">The target darkness level.</param>
        /// <param name="replaceDarkness">Whether to replace the current darkness level.</param>
        /// <param name="callback">Optional callback action to execute after fading.</param>
        /// <returns></returns>
        private IEnumerator FadePanelCR(float darkness, bool replaceDarkness, float fadeSpeed)
        {
            if (replaceDarkness)
                panelImage.color = new Color(panelImage.color.r, panelImage.color.g, panelImage.color.b, 0);

            float timeElapsed = 0;

            while (panelImage.color.a < darkness)
            {
                timeElapsed += Time.deltaTime;

                float fadeAmount = Mathf.Lerp(0, 1, timeElapsed);

                Color newColor = new(panelImage.color.r, panelImage.color.g, panelImage.color.b, panelImage.color.a + fadeAmount * fadeSpeed);

                pauseText.color = newColor;
                panelImage.color = newColor;

                yield return null;
            }

            yield return null;
        }

        /// <summary>
        /// Sets the panel to dark and fades it out.
        /// </summary>
        /// <param name="fadeFromDark"></param>
        /// <returns></returns>
        private IEnumerator UnFadePanelCR(bool fadeFromDark = true)
        {
            if (fadeFromDark)
                panelImage.color = new Color(panelImage.color.r, panelImage.color.g, panelImage.color.b, 1);

            float timeElapsed = 0;

            while (panelImage.color.a > 0)
            {
                timeElapsed += Time.deltaTime;

                float fadeAmount = Mathf.Lerp(0, 1, timeElapsed);

                Color newColor = new Color(panelImage.color.r, panelImage.color.g, panelImage.color.b, panelImage.color.a - fadeAmount * panelFadeSpeed);

                pauseText.color = newColor;
                panelImage.color = newColor;

                yield return null;
            }

            yield return null;
        }
    }
}