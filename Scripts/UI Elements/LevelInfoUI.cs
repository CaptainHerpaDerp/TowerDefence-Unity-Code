using Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
/// <summary>
/// A base class for UI that displays information about a level
/// </summary>

namespace UIElements
{
    public abstract class LevelInfoUI : MonoBehaviour // Have the singleton be overridable by the derived classes
    {
        [SerializeField] protected FadingPanelUI fadingPanelUI;
        [SerializeField] protected List<GameObject> starGroups;
        [SerializeField] private Image levelImagePreview;

        protected virtual void ShowStars(int starCount)
        {
            foreach (GameObject starGroup in starGroups)
            {
                starGroup.SetActive(false);
            }

            starGroups[starCount - 1].SetActive(true);
        }
    }
}