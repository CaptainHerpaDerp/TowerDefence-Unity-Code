using UnityEngine;
using UnityEngine.UI;

namespace Core.Character
{
    /// <summary>
    /// A local UI element that displays the health of a character.
    /// </summary>
    public class HealthBar : MonoBehaviour
    {
        public Image background;
        public Image foreground;

        private float maxHealth;
        private float currentHealth;

        private void UpdateHealthBar()
        {
            // Calculate the normalized width based on current health
            float normalizedWidth = Mathf.Clamp01(currentHealth / maxHealth);

            // Set the width of the foreground bar
            foreground.rectTransform.sizeDelta = new Vector2(normalizedWidth * background.rectTransform.sizeDelta.x, background.rectTransform.sizeDelta.y);
        }

        public void InitializeHealthBar(float pMaxHealth, float pCurrentHealth)
        {
            maxHealth = pMaxHealth;
            SetHealth(pCurrentHealth);
            UpdateHealthBar();
        }

        public void SetHealth(float newHealth)
        {
            // Clamp health to the valid range
            currentHealth = Mathf.Clamp(newHealth, 0f, maxHealth);

            // Update the health bar
            UpdateHealthBar();
        }

        public void Hide()
        {
            background.enabled = false;
            foreground.enabled = false;
        }

        public void Show()
        {
            background.enabled = true;
            foreground.enabled = true;
        }
    }
}