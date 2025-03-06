using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

namespace UIElements
{
    /// <summary>
    /// A button that will modify the text of the button when interacted with
    /// </summary>
    public class TextButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [BoxGroup("Component References"), SerializeField] private TextMeshProUGUI textComponent;
        [BoxGroup("Component References"), SerializeField] private Button buttonComponent;


        [BoxGroup("Text Settings"), SerializeField] private bool changeColorOnHover;
        [ShowIf("changeColorOnHover"), BoxGroup("Text Settings"), SerializeField] private Color defaultColor;
        [ShowIf("changeColorOnHover"), BoxGroup("Text Settings"), SerializeField] private Color hoverColor;

        private void Start()
        {
            // When the button is pressed, revert the text color to the default color
            buttonComponent.onClick.AddListener(() =>
            {
                if (changeColorOnHover)
                {
                    textComponent.color = defaultColor;
                }
            });

            // Set the color of the text to the default color
            if (changeColorOnHover)
            {
                textComponent.color = defaultColor;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (changeColorOnHover)
            {
                textComponent.color = hoverColor;   
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (changeColorOnHover)
            {
                textComponent.color = defaultColor;
            }
        }
    }
}
