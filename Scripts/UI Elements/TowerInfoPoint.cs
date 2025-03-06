using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UIElements
{
    public class TowerInfoPoint : MonoBehaviour
    {
        public TextMeshProUGUI textComponent;

        public string Text
        {
            set
            {
                Activate(); 
                textComponent.text = value;           
            }

            get
            {
                return textComponent.text;
            }
        }

        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
            textComponent.text = "";
        }
    }

}
