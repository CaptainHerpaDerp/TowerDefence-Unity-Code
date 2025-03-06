using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Manages visual effects for the tower purchase option button
/// </summary>
public class TowerPurchaseOption : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [field: SerializeField, BoxGroup("Component References")] public Button ButtonComponent { get; private set; }
    [BoxGroup("Component References"), SerializeField] private GameObject buttonSelectionUnderlay;

    public bool IsSelected { get; private set; }

    private void Start()
    {
        if (buttonSelectionUnderlay == null)
        {
            Debug.LogWarning("Button selection underlay not assigned to TowerPurchaseOption script");
        }

        if (ButtonComponent == null)
        {
            ButtonComponent = GetComponent<Button>();
        }

        // Hide the underlay by default
        buttonSelectionUnderlay.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {   
        buttonSelectionUnderlay.SetActive(true);
        IsSelected = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonSelectionUnderlay.SetActive(false);
        IsSelected = false;
    }
}
