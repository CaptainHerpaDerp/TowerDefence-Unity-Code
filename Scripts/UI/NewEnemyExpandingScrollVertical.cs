using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A specialized version of the ExpandingScroll class that is used for the New Enemy scroll in the UI
/// </summary>
public class NewEnemyExpandingScrollVertical : ExpandingScroll
{
    public float scrollStartHeight, scrollTargetHeight;

    [SerializeField] private Image scrollImageComponent;

    #region Overriden Methods

    /// <summary>
    /// Enables and expands the scroll object
    /// </summary>
    public override void EnableScroll()
    {
        StopAllCoroutines();

        Debug.Log("Enabling scroll");

        // Disable the elements group parent
        soundEffectManager.PlayScrollOpenSound();
        StartCoroutine(FadeInScrollRoutine());
        StartCoroutine(ExpandScroll());
    }

    /// <summary>
    /// Expands the scroll object's Height to the target Height, then fades in the elements
    /// </summary>
    /// <returns></returns>
    protected override IEnumerator ExpandScroll()
    {
        float time = 0;

        // Set the scroll's starting Height
        scrollImageComponent.GetComponent<RectTransform>().sizeDelta = new Vector2(scrollImageComponent.GetComponent<RectTransform>().sizeDelta.x, scrollStartHeight);

        while (time < scrollExpandTime)
        {
            float newHeight = Mathf.Lerp(scrollStartHeight, scrollTargetHeight, time / scrollExpandTime);

            // increase the rect transform Height
            scrollImageComponent.GetComponent<RectTransform>().sizeDelta = new Vector2(scrollImageComponent.GetComponent<RectTransform>().sizeDelta.x, newHeight);

            time += Time.unscaledDeltaTime;
            yield return null;
        }

        // Fully expand the scroll
        scrollImageComponent.GetComponent<RectTransform>().sizeDelta = new Vector2(scrollImageComponent.GetComponent<RectTransform>().sizeDelta.x, scrollTargetHeight);

        StartCoroutine(FadeInScrollElements());
        StartCoroutine(FadeInTextElements());

        yield return null;
    }

    protected override void QuickExpandScroll()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    ///  Fades in the scroll image
    /// </summary>
    protected override IEnumerator FadeInScrollRoutine()
    {
        scrollImageComponent.color = new Color(scrollImageComponent.color.r, scrollImageComponent.color.g, scrollImageComponent.color.b, 0);

        float time = 0;

        while (time < scrollFadeTime)
        {
            float newAlpha = Mathf.Lerp(0, 1, time / scrollFadeTime);

            Color newColor = new(scrollImageComponent.color.r, scrollImageComponent.color.g, scrollImageComponent.color.b, newAlpha);
            scrollImageComponent.color = newColor;

            time += Time.unscaledDeltaTime;
            yield return null;
        }

        // Fully fade in the scroll
        scrollImageComponent.color = new Color(scrollImageComponent.color.r, scrollImageComponent.color.g, scrollImageComponent.color.b, 1);

        yield return null;
    }

    /// <summary>
    /// Gets image components of each element in the elements group parent and fade them in
    /// </summary>
    protected override IEnumerator FadeInScrollElements()
    {
        List<Image> images = new List<Image>();

        foreach (Transform child in elementsGroupParent)
        {
            // Try to get the image component of the child
            if (child.TryGetComponent<Image>(out Image drctImg))
            {
                images.Add(drctImg);
            }

            // Try to get the text component of the child
            if (child.TryGetComponent<TextMeshProUGUI>(out TextMeshProUGUI drctText))
            {
                drctText.color = new Color(drctText.color.r, drctText.color.g, drctText.color.b, 0);
            }

            // Get all images in the child
            foreach (Image image in child.GetComponentsInChildren<Image>())
            {
                images.Add(image);
            }

            // Get all text components in the child
            foreach (TextMeshProUGUI text in child.GetComponentsInChildren<TextMeshProUGUI>())
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
            }
        }

        float time = 0;

        while (time < scrollFadeTime)
        {
            float newAlpha = Mathf.Lerp(0, 1, time / scrollFadeTime);

            foreach (Image image in images)
            {
                Color newColor = new(image.color.r, image.color.g, image.color.b, newAlpha);
                image.color = newColor;
            }

            time += Time.unscaledDeltaTime;
            yield return null;
        }

        // Fully fade in the elements
        foreach (Image image in images)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
        }

        yield return null;
    }

    #endregion

    private IEnumerator FadeInTextElements()
    {
        // Instantiate a new list of text components
        List<TextMeshProUGUI> textComponents = new List<TextMeshProUGUI>();

        foreach (Transform child in elementsGroupParent)
        {
            // Try to get the text component of the child
            if (child.TryGetComponent<TextMeshProUGUI>(out TextMeshProUGUI drctText))
            {
                textComponents.Add(drctText);
            }

            // Get all text components in the child
            foreach (TextMeshProUGUI text in child.GetComponentsInChildren<TextMeshProUGUI>())
            {
                textComponents.Add(text);
            }
        }

        float time = 0;

        while (time < scrollFadeTime)
        {
            float newAlpha = Mathf.Lerp(0, 1, time / scrollFadeTime);

            foreach (TextMeshProUGUI textComponent in textComponents)
            {
                Color newColor = new(textComponent.color.r, textComponent.color.g, textComponent.color.b, newAlpha);
                textComponent.color = newColor;
            }

            time += Time.unscaledDeltaTime;
            yield return null;
        }

        // Fully fade in the elements
        foreach (TextMeshProUGUI textComponent in textComponents)
        {
            textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, 1);
        }

        yield return null;
    }
}
