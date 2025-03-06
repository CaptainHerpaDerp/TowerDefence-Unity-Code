using AudioManagement;
using Core;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UIElements
{
    /// <summary>
    /// A screen overlay that fades in and out, can be used to transition between scenes or to create a fade effect.
    /// </summary>
    public class FadingPanelUI : Singleton<FadingPanelUI>   
    {
        [SerializeField] private float panelFadeTime;

        [SerializeField] private float windowDarknessLevel;

        [SerializeField] private Image image;

        private AudioManager audioManager;

        [BoxGroup("Debugging"), SerializeField] private bool WebGLMode;
        [BoxGroup("Debugging"), SerializeField] private float webGLCallbackDelay = 0.5f;


        private void Start()
        {
            audioManager = AudioManager.Instance;
        }

        public void FadePanel(float darkness, bool replaceDarkness = true)
        {
            StopAllCoroutines();
            StartCoroutine(FadePanelCR(darkness, replaceDarkness));
        }

        public void UnfadePanel(float fadeTime)
        {
            StopAllCoroutines();
            StartCoroutine(UnFadePanelCR(fadeTime)); 
        }

        public void UnfadePanelFromCurrent()
        {
            StopAllCoroutines();
            StartCoroutine(UnFadePanelCR(panelFadeTime, false));
        }

        /// <summary>
        /// Fade the panel and load a new scene.
        /// </summary>
        /// <param name="darkness"></param>
        /// <param name="levelName"></param>
        /// <param name="replaceDarkness"></param>
        public void FadePanelAndLoad(string levelName, bool replaceDarkness = true)
        {
            StopAllCoroutines();
            StartCoroutine(FadePanelAndLoadCR(levelName, replaceDarkness));
        }

        /// <summary>
        /// Sets the panel to dark and fades it out.
        /// </summary>
        /// <param name="fadeFromDark"></param>
        /// <returns></returns>
        private IEnumerator UnFadePanelCR(float fadeTime = 0, bool fadeFromDark = true)
        {
            float fromValue;

            if (fadeFromDark)
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
                fromValue = 1;
            }
            else
            {
                fromValue = image.color.a;
            }

            if (fadeTime == 0)
            {
                fadeTime = panelFadeTime;
            }

            float timeElapsed = 0;

            while (timeElapsed < fadeTime)
            {
                float fadeAmount = Mathf.Lerp(fromValue, 0, timeElapsed / fadeTime);

                Color newColor = new(image.color.r, image.color.g, image.color.b, fadeAmount);

                image.color = newColor;

                timeElapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            image.color = new Color(image.color.r, image.color.g, image.color.b, 0);

            yield return null;
        }

        /// <summary>
        /// Fades the panel with an optional callback action.
        /// </summary>
        /// <param name="darkness">The target darkness level.</param>
        /// <param name="replaceDarkness">Whether to replace the current darkness level.</param>
        /// <param name="callback">Optional callback action to execute after fading.</param>
        /// <returns></returns>
        private IEnumerator FadePanelCR(float darkness, bool replaceDarkness, Action callback = null)
        {
            if (replaceDarkness)
                image.color = new Color(image.color.r, image.color.g, image.color.b, 0);

            float timeElapsed = 0;

            while (timeElapsed < panelFadeTime)
            {
                float fadeAmount = Mathf.Lerp(0, darkness, timeElapsed / panelFadeTime);

                Color newColor = new(image.color.r, image.color.g, image.color.b, fadeAmount);

                image.color = newColor;

                timeElapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            image.color = new Color(image.color.r, image.color.g, image.color.b, darkness);

            callback?.Invoke(); // Invoke the callback if it's not null

            yield return null;
        }

        /// <summary>
        /// Fades the panel and loads a new scene.
        /// </summary>
        /// <param name="darkness">The target darkness level.</param>
        /// <param name="levelName">The name of the scene to load.</param>
        /// <param name="replaceDarkness">Whether to replace the current darkness level.</param>
        /// <returns></returns>
        private IEnumerator FadePanelAndLoadCR(string levelName, bool replaceDarkness)
        {
            // Use the generic FadePanelCR with a callback to load the scene
            yield return FadePanelCR(1, replaceDarkness, () => SceneManager.LoadScene(levelName));
        }
    }
}