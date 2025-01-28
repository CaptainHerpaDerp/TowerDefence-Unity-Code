using UnityEngine;
using UnityEngine.UI;
using Core;

namespace UI.Management
{
    public class ResetConfirmation : MonoBehaviour
    {
        public static ResetConfirmation Instance;

        [SerializeField] private Button confirmButton, denyButton;
        [SerializeField] private ExpandingScrollHorizontal expandingScrollHorizontal;

        private EventBus eventBus;

        private bool confirmationEnabled = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogError("More than one ResetConfirmation instance in scene!");
                Destroy(this);
                return;
            }
        }

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
                confirmButton.onClick.AddListener(ConfirmReset);
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

            expandingScrollHorizontal.DisableScroll();
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

            eventBus.Publish("EnableMouseUsage");
            eventBus.Publish("GameWindowClosed");

            expandingScrollHorizontal.FadeOutScroll();
            confirmationEnabled = false;
        }

        public void QuickDisableConfirmation()
        {
            if (!confirmationEnabled)
            {
                return;
            }

            eventBus.Publish("EnableMouseUsage");

            expandingScrollHorizontal.DisableScroll();
            confirmationEnabled = false;
        }

        /// <summary>
        /// Resets the level
        /// </summary>
        public void ConfirmReset()
        {
            eventBus.Publish("ResetLevel");
            DisableConfirmation();
        }
    }
}