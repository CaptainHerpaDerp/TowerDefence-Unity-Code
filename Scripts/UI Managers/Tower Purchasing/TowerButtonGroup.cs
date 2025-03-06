using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

/// <summary>
/// A an abstract class that represents a group of UI elements that are shown when a tower is pressed
/// </summary>
public abstract class TowerButtonGroup : MonoBehaviour
{
    [BoxGroup("Component References"), SerializeField] protected RectTransform rectTransform;
    [BoxGroup("Component References"), SerializeField] protected CanvasGroup canvasGroup;

    [Header("Fading Options")]
    [SerializeField] private float windowFadeTime;

    [BoxGroup("Settings"), SerializeField] protected int screenEdgePadding;

    protected float defaultY;
    protected bool isFadedIn;
    protected float defaultElementHeight;
    protected float defaultChildSpacing;

    protected int defaultBottomPadding, defaultTopPadding;

    protected Canvas canvas;

    [BoxGroup("Debug"), SerializeField] private bool doConstantBoundaryCheck;

    protected RectTransform parentRectTransform;

    protected virtual void Start()
    {
        parentRectTransform = transform.parent.GetComponent<RectTransform>();

        if (rectTransform == null) { rectTransform = GetComponent<RectTransform>(); }
        if (canvasGroup == null) { canvasGroup = GetComponent<CanvasGroup>(); }

        defaultY = rectTransform.localPosition.y;

        canvas = FindFirstObjectByType<Canvas>();
    }

    private void Update()
    {
        if (doConstantBoundaryCheck)
        {
            KeepWindowInBoundaries();
        }
    }

    public abstract void KeepWindowInBoundaries();

    protected float CanvasRefHeight()
    {
        Vector2 canvasRefResolution = canvas.GetComponent<CanvasScaler>().referenceResolution;
        return canvasRefResolution.y;
    }

    #region Visibility

    public void FadeInGroup()
    {
        if (isFadedIn)
            return;

        isFadedIn = true;

        StartCoroutine(Utils.FadeInCanvasGroup(canvasGroup, windowFadeTime));
    }

    public void FadeOutGroup()
    {
        if (!isFadedIn)
            return;

        isFadedIn = false;

        StartCoroutine(Utils.FadeOutCanvasGroup(canvasGroup, windowFadeTime));
    }

    public void QuickFadeInGroup()
    {
        isFadedIn = true;
        Utils.EnableCanvasGroup(canvasGroup);
    }

    public void QuickFadeOutGroup()
    {
        isFadedIn = false;
        Utils.DisableCanvasGroup(canvasGroup);
    }

    #endregion

    public bool IsVisible()
    {
        return canvasGroup.alpha > 0;
    }
}
