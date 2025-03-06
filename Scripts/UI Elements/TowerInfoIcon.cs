
using TMPro;
using UnityEngine;
namespace UIElements
{
    public class TowerInfoIcon : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textComponent;
        public string Text
        {
            set
            {
                // Check if the input value contains non-numeric characters
                bool isNumeric = float.TryParse(value, out float floatValue);

                if (isNumeric)
                {
                    // Convert the float to a string with two decimal places
                    string formattedValue = floatValue.ToString("F2");

                    // Remove trailing zeros and set the text
                    textComponent.text = formattedValue.TrimEnd('0').TrimEnd('.');
                }
                else
                {
                    textComponent.text = value;
                }
            }
        }


        [Header("Text Colors")]
        [SerializeField] private Color defaultColor;
        [SerializeField] private Color upgradeColor;

        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        public void SetColorDefault()
        {
            textComponent.color = defaultColor;
        }

        public void SetColorUpgrade()
        {
            textComponent.color = upgradeColor;
        }
    }
}