using Core;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class NextWaveButton : MonoBehaviour
    {
        [SerializeField] private Image buttonIcon, buttonInterior, buttonExterior;

        [SerializeField] private float fadeTime, innerFadeSpeed = 0.25f, minFadeValue = 0.5f;

        [SerializeField] TMPro.TextMeshProUGUI timerText;

        [SerializeField] private Button buttonComponent;

        [SerializeField] private GameObject elementsGroup;

        private bool buttonEnabled = false;
        private bool isInteractable = true;

        private EventBus eventBus;

        private void Start()
        {
            eventBus = EventBus.Instance;

            if (buttonInterior == null)
            {
                Debug.Log("No button interior assigned!");
            }

            if (buttonExterior == null)
            {
                Debug.Log("No button exterior assigned!");
            }

            if (buttonIcon == null)
            {
                Debug.Log("No button icon assigned!");
            }

            buttonComponent.onClick.AddListener(OnButtonClick);

            EnableButton();

            eventBus.Subscribe("NextWaveButtonDisabled", DisableButton);
            eventBus.Subscribe("NextWaveButtonPressed", DisableButton);
            eventBus.Subscribe("NextWaveButtonTimerSet", ActivateTimerText);
            eventBus.Subscribe("NextWaveButtonEnabled", EnableButton);
            eventBus.Subscribe("NextWaveButtonTimerDisabled", DisableCountdown);

            eventBus.Subscribe("GameWindowOpened", () =>
            {
                isInteractable = false;
            });

            eventBus.Subscribe("GameWindowClosed", () =>
            {
                isInteractable = true;
            });
        }

        private void OnButtonClick()
        {
            // Do not allow the button to be pressed if it is not interactable
            if (!isInteractable)
            {
                return;
            }

            // Disable the button to prevent spamming
            buttonComponent.interactable = false;

            eventBus.Publish("NextWaveButtonPressed");
        }

        /// <summary>
        /// Disable the countdown text and enable the button icon
        /// </summary>
        public void DisableCountdown()
        {
            if (buttonEnabled == false)
                return;

            timerText.gameObject.SetActive(false);
            buttonIcon.gameObject.SetActive(true);

            StopAllCoroutines();
        }

        public void EnableButton()
        {
            //Debug.Log("Enabling button");


            StopAllCoroutines();
            elementsGroup.gameObject.SetActive(true);
            buttonEnabled = true;
            buttonIcon.gameObject.SetActive(true);
            FadeInButton();
            buttonComponent.interactable = true;
        }

        public void DisableButton()
        {
            if (buttonEnabled == false)
                return;
          
            timerText.gameObject.SetActive(false);
            buttonIcon.gameObject.SetActive(true);

            StopAllCoroutines();

            buttonEnabled = false;
            FadeOutButton();
        }

        /// <summary>
        /// Start a countdown from the given value
        /// </summary>
        /// <param name="countdownFrom"></param>
        public void ActivateTimerText(object countdownFrom)
        {
            int countdownFromValue = (int)countdownFrom;

            EnableButton();

            // Activate the text and disable the icon graphic
            timerText.gameObject.SetActive(true);
            buttonIcon.gameObject.SetActive(false);

            StartCoroutine(BeginCountdown(countdownFromValue));
        }

        /// <summary>
        /// Coroutine to begin a countdown from the given value
        /// </summary>
        /// <param name="valueFrom"></param>
        /// <returns></returns>
        private IEnumerator BeginCountdown(int valueFrom)
        {
            while (valueFrom > 0)
            {
                timerText.text = valueFrom.ToString();
                valueFrom--;

                yield return new WaitForSeconds(1f);
            }

            DisableCountdown();

            yield return null;
        }
        public bool IsEnabled() => buttonEnabled;
        public void FadeInButton() => StartCoroutine(FadeInButtonCoroutine());
        public void FadeOutButton() => StartCoroutine(FadeOutButtonCoroutine());

        #region Visual Effects

        private void BeginInteriorFade()
        {
            StartCoroutine(DoInteriorFadeEffect());
        }

        /// <summary>
        /// Fade in the button graphic over time
        /// </summary>
        /// <returns></returns>
        private IEnumerator FadeInButtonCoroutine()
        {
            yield return new WaitForFixedUpdate();

            // Set the alpha of all the elements to 0
            buttonInterior.color = new Color(buttonInterior.color.r, buttonInterior.color.g, buttonInterior.color.b, 0);
            buttonExterior.color = new Color(buttonExterior.color.r, buttonExterior.color.g, buttonExterior.color.b, 0);
            buttonIcon.color = new Color(buttonIcon.color.r, buttonIcon.color.g, buttonIcon.color.b, 0);

            float time = 0;

            while (time < fadeTime)
            {
                float alpha = Mathf.Lerp(0, 1, time / fadeTime);

                buttonInterior.color = new Color(buttonInterior.color.r, buttonInterior.color.g, buttonInterior.color.b, alpha);
                buttonIcon.color = new Color(buttonIcon.color.r, buttonIcon.color.g, buttonIcon.color.b, alpha);
                buttonExterior.color = new Color(buttonExterior.color.r, buttonExterior.color.g, buttonExterior.color.b, alpha);

                time += Time.unscaledDeltaTime;

                yield return null;
            }

            // Fully fade in the button elements
            buttonInterior.color = new Color(buttonInterior.color.r, buttonInterior.color.g, buttonInterior.color.b, 1);
            buttonIcon.color = new Color(buttonIcon.color.r, buttonIcon.color.g, buttonIcon.color.b, 1);
            buttonExterior.color = new Color(buttonExterior.color.r, buttonExterior.color.g, buttonExterior.color.b, 1);

            BeginInteriorFade();

            yield return null;
        }

        /// <summary>
        /// Fade out the button graphic over time
        /// </summary>
        /// <returns></returns>
        private IEnumerator FadeOutButtonCoroutine()
        {
            // Set the alpha of all the elements to 1
            buttonInterior.color = new Color(buttonInterior.color.r, buttonInterior.color.g, buttonInterior.color.b, 1);
            buttonExterior.color = new Color(buttonExterior.color.r, buttonExterior.color.g, buttonExterior.color.b, 1);
            buttonIcon.color = new Color(buttonIcon.color.r, buttonIcon.color.g, buttonIcon.color.b, 1);

            float time = 0;

            while (time < fadeTime)
            {
                float alpha = Mathf.Lerp(1, 0, time / fadeTime);

                buttonInterior.color = new Color(buttonInterior.color.r, buttonInterior.color.g, buttonInterior.color.b, alpha);
                buttonIcon.color = new Color(buttonIcon.color.r, buttonIcon.color.g, buttonIcon.color.b, alpha);
                buttonExterior.color = new Color(buttonExterior.color.r, buttonExterior.color.g, buttonExterior.color.b, alpha);

                time += Time.unscaledDeltaTime;

                yield return null;
            }

            // Fully fade out the button elements
            buttonInterior.color = new Color(buttonInterior.color.r, buttonInterior.color.g, buttonInterior.color.b, 0);
            buttonIcon.color = new Color(buttonIcon.color.r, buttonIcon.color.g, buttonIcon.color.b, 0);
            buttonExterior.color = new Color(buttonExterior.color.r, buttonExterior.color.g, buttonExterior.color.b, 0);

            // Disable the elements group
            elementsGroup.gameObject.SetActive(false);

            yield return null;
        }

        /// <summary>
        /// Over time, fade the V value in of the HSV color of the button interior, when it reaches 0, fade it back in. This continuously loops.
        /// </summary>
        /// <returns></returns>
        private IEnumerator DoInteriorFadeEffect()
        {
            bool fadingOut = true;

            yield return new WaitForSeconds(1.5f);

            while (true)
            {
                Color.RGBToHSV(buttonInterior.color, out float h, out float s, out float v);

                if (fadingOut)
                {
                    v -= (innerFadeSpeed * Time.deltaTime);

                    if (v <= minFadeValue)
                    {
                        fadingOut = false;
                    }
                }
                else
                {
                    v += (innerFadeSpeed * Time.deltaTime);

                    if (v >= 1)
                    {
                        fadingOut = true;
                        yield return new WaitForSeconds(1.5f);
                    }
                }

                buttonInterior.color = Color.HSVToRGB(h, s, v);

                yield return new WaitForFixedUpdate();
            }
        }

        #endregion
    }
}
