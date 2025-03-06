using Core;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UIElements
{
    /// <summary>
    /// Manages the button that the player can click to select a power
    /// </summary>
    public class PlayerPowerButton : MonoBehaviour
    {
        [Header("The panel that covers the button to display the cooldown time")]
        [SerializeField] private Image cooldownOverlay;

        [SerializeField] private Button ButtonComponent;

        [SerializeField] private float cooldownTime;
        private bool onCooldown;

        public bool OnCooldown => onCooldown;

        public Action OnPress;


        private EventBus eventBus;

        private void Start()
        {
            // Grabs the event bus instance so that when the level is reset, the cooldown overlay is also reset
            eventBus = EventBus.Instance;
            eventBus.Subscribe("GameReset", ResetCooldownUI);

            if (cooldownOverlay == null)
            {
                Debug.LogError("Cooldown Overlay Not Assigned!");
                return;
            }

            // Set the height of the cooldown overlay to its maximum height
            DoMaxCooldownFill();

            ButtonComponent.onClick.AddListener(() =>
            {
                if (onCooldown)
                {
                    return;
                }

                OnPress?.Invoke();
            });
        }

        public void DoCooldown()
        {
            // Set the button to be on cooldown so that it cannot be clicked
            onCooldown = true;

            // Set the cooldown overlay to the maximum height
            DoMaxCooldownFill();

            StartCoroutine(ReduceOverlayHeight(cooldownTime));
        }

        /// <summary>
        /// Reduces the height of the cooldown overlay over time to show the cooldown progress
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private IEnumerator ReduceOverlayHeight(float time)
        {
            float elapsedTime = 0;

            while (elapsedTime < time)
            {
                  // Calculate the new height of the cooldown overlay
                float newHeight = Mathf.Lerp(1, 0, elapsedTime / time);

                // Set the new height of the cooldown overlay
                cooldownOverlay.fillAmount = newHeight;

                // Increment the elapsed time
                elapsedTime += Time.deltaTime;

                yield return null;
            }

            // Set the height of the cooldown overlay to 0
            cooldownOverlay.fillAmount = 0;

            // Set the button to be off cooldown
            onCooldown = false;

            yield return null;
        }

        private void ResetCooldownUI()
        {
            StopAllCoroutines();

            // Set the height of the cooldown overlay to 0
            cooldownOverlay.fillAmount = 0;

            // Set the button to be off cooldown
            onCooldown = false;
        }

        /// <summary>
        /// Sets the cooldown overlay to the maximum height
        /// </summary>
        public void DoMaxCooldownFill()
        {
            cooldownOverlay.fillAmount = 1;
        }

        /// <summary>
        /// Sets the cooldown overlay to the minimum height
        /// </summary>
        public void HideCooldownFill()
        {
            cooldownOverlay.fillAmount = 0;
        }
    }
}

