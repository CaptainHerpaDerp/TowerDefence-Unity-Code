using UnityEngine;
using UnityEngine.UI;
using Core;
using UIElements;
using UnityEngine.EventSystems;

namespace UIManagement
{
    public class QuitConfirmation : Singleton<QuitConfirmation>, IPointerEnterHandler
    {
        [SerializeField] private Button confirmButton, denyButton;
        [SerializeField] private ExpandingScrollHorizontal expandingScrollHorizontal; 

        private EventBus eventBus;

        private bool confirmationEnabled = false;

        private void Start()
        {
            eventBus = EventBus.Instance;

            #region Null checks and event assignments
            if (confirmButton == null)
            {
                Debug.LogError("Error at " + this + " confirmButton not assigned!");
            }
            else
            {
                confirmButton.onClick.AddListener(ConfirmQuit);
            }

            if (denyButton == null)
            {
                Debug.LogError("Error at " + this + " denyButton not assigned!");
            }
            else
            {
                denyButton.onClick.AddListener(DisableConfirmation);
            }

            if (expandingScrollHorizontal == null)
            {
                Debug.LogError("Error at " + this + " expandingScrollHorizontal not assigned!");
            }
            #endregion
        }

        public void EnableConfirmation()
        {
            if (confirmationEnabled)
            {
                return;
            }

            eventBus.Publish("DisableMouseUsage");

            expandingScrollHorizontal.EnableScroll();
            confirmationEnabled = true;
        }

        public void QuickEnableConfirmation()
        {
            if (confirmationEnabled)
            {
                return;
            }

            eventBus.Publish("DisableMouseUsage");

            expandingScrollHorizontal.QuickEnableScroll();
            confirmationEnabled = true;
        }

        public void DisableConfirmation()
        {
            if (!confirmationEnabled)
            {
                return;
            }

            eventBus.Publish("OnQuitDeclined");

            expandingScrollHorizontal.DisableScroll();
            confirmationEnabled = false;
        }

        public void QuickDisableConfirmation()
        {
            if (!confirmationEnabled)
            {
                return;
            }

            eventBus.Publish("EnableMouseUsage");

            expandingScrollHorizontal.QuickDisableScroll();
            confirmationEnabled = false;
        }

        /// <summary>
        /// Exits the application.
        /// </summary>
        public void ConfirmQuit()
        {
            Application.Quit();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("Pointer entered");
        }
    }
}