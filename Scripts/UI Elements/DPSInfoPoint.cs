using UnityEngine;

namespace UIElements
{
    public class DPSInfoPoint : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI dpsFromTextComp, dpsToTextComp;

        public void SetDPSToFromText(float dpsFrom, float dpsTo)
        {
            gameObject.SetActive(true);

            dpsFromTextComp.text = dpsFrom.ToString("F1");
            dpsToTextComp.text = dpsTo.ToString("F1");
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}