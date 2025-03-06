using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIElements
{
    
    /// <summary>
    /// An interactable object that is used to start levels in the map menu.
    /// </summary>
    public class WaypointGroup : MonoBehaviour
    {
        [SerializeField] private Animator animatorComponent;

        [SerializeField] private GameObject animationButton;
        [SerializeField] private Button buttonComponent;

        [SerializeField] private Color finalColor;

        [SerializeField] private TextMeshProUGUI textComponent;

        [SerializeField] private float animationDuration;

        public Action buttonPressed;

        private void Start()
        {
            buttonComponent.onClick.AddListener(() =>
            {
                buttonPressed?.Invoke();
            });
        }

        /// <summary>
        /// Instantly enable the waypoint group without any animations.
        /// </summary>
        public void Enable()
        {
            animatorComponent.enabled = false;

            animationButton.gameObject.SetActive(false);
            buttonComponent.gameObject.SetActive(true);

            textComponent.enabled = true;
        }

        public void Hide()
        {
            animationButton.gameObject.SetActive(false);
            buttonComponent.gameObject.SetActive(false);
            textComponent.enabled = false;

        }

        /// <summary>
        /// Called during the unlocking animation of the waypoint group.
        /// </summary>
        public void AnimateActive()
        {
            // disable the real button and enable the fake animation button
            buttonComponent.gameObject.SetActive(false);
            animationButton.gameObject.SetActive(true);

            textComponent.enabled = true;
            animatorComponent.enabled = true;
            animatorComponent.SetTrigger("Animate");

            StartCoroutine(EnableButtonAfterAnimation());
        }

        /// <summary>
        /// Disable the fake animation button and enable the real button.
        /// </summary>
        /// <returns></returns>
        private IEnumerator EnableButtonAfterAnimation()
        {
            yield return new WaitForSeconds(animationDuration);
            animationButton.gameObject.SetActive(false);
            buttonComponent.gameObject.SetActive(true);

            yield return null;
        }
    }
}