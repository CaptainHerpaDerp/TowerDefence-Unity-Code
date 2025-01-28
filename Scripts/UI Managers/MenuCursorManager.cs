using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Management
{

    public enum InteractionObject { UI, UIBlock, GameElement, None }

    /// <summary>
    /// A script to be added to menu screens (map menu and main menu) - Manages the cursor texture
    /// </summary>
    public class MenuCursorManager : MonoBehaviour
    {
        public static MenuCursorManager Instance;

        [Header("Cursor Textures")]
        public Texture2D defaultCursor;
        public Texture2D interactableCursor;

        [Header("Cursor Settings")]
        public Vector2 cursorHotspot = Vector2.zero;

        [Header("Layers")]
        public LayerMask interactableLayerMask;

        [Header("UI Tags")]
        [SerializeField] private string interactableUITag;
        [SerializeField] private string nonInteractableUITag;
        [SerializeField] private string interactableGameElementTag;
        [SerializeField] private List<string> interactableGameElementTags;
        [SerializeField] private List<string> exclusionTags;
        // Reference to the sound effect manager so that cursor sounds can be played
        SoundEffectManager soundEffectManager;

        // Record the last UI element that was hovered over so that the hover sound is not played repeatedly
        private GameObject lastHoveredUIElement;

        [SerializeField] private bool customCursorInEditor;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogError("Multiple MenuCursorManager instances found in the scene. Destroying the new instance.");
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            soundEffectManager = SoundEffectManager.Instance;
        }

        private void Update()
        {
            HandleCursorGraphic();
        }

        private void HandleCursorGraphic()
        {
            if (!customCursorInEditor && Application.isEditor)
            {
                return;
            }

            InteractionObject interactionObject = GetPointerOverElement();

            switch (interactionObject)
            {
                case InteractionObject.UI:
                    Cursor.SetCursor(interactableCursor, cursorHotspot, CursorMode.Auto);
                    break;
                case InteractionObject.UIBlock:
                    Cursor.SetCursor(defaultCursor, cursorHotspot, CursorMode.Auto);
                    lastHoveredUIElement = null;
                    break;
                case InteractionObject.GameElement:
                    Cursor.SetCursor(interactableCursor, cursorHotspot, CursorMode.Auto);
                    break;
                case InteractionObject.None:
                    Cursor.SetCursor(defaultCursor, cursorHotspot, CursorMode.Auto);
                    lastHoveredUIElement = null;
                    break;
            }
        }

        private InteractionObject GetPointerOverElement()
        {
            if (IsPointerOverTaggedUIElement())
            {
                return InteractionObject.UI;
            }
            else if (IsPointerOverTaggedUIBlockElement())
            {
                return InteractionObject.UIBlock;
            }
            else if (IsPointerOverGameElement())
            {
                return InteractionObject.GameElement;
            }

            return InteractionObject.None;
        }

        /// <summary>
        /// Checks if the mouse pointer is over a UI element with a specific tag
        /// </summary>
        /// <returns>True if the pointer is over a tagged UI element, false otherwise</returns>
        private bool IsPointerOverTaggedUIElement()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            foreach (RaycastResult result in results)
            {
                if (result.gameObject.tag == interactableUITag)
                {
                    if (lastHoveredUIElement != result.gameObject)
                    {
                        soundEffectManager.PlayHoverSound();
                    }

                    lastHoveredUIElement = result.gameObject;

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if the mouse pointer is over a game element that blocks ui interaction (like a blocking panel)
        /// </summary>
        private bool IsPointerOverTaggedUIBlockElement()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            foreach (RaycastResult result in results)
            {
                if (result.gameObject.tag == nonInteractableUITag)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if the mouse pointer is over a game element with a specific layer mask
        /// </summary>
        /// <returns>True if the pointer is over an interactable game element, false otherwise</returns>
        private bool IsPointerOverGameElement()
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null)
            {
                bool tagValid = true;

                foreach (string tag in exclusionTags)
                {
                    if (hit.collider != null && hit.collider.CompareTag(tag))
                    {
                        tagValid = false;
                    }
                }

                if (tagValid)
                {
                    foreach (string tag in interactableGameElementTags)
                    {
                        if (hit.collider.gameObject.tag == tag)
                        {
                            return true;
                        }
                    }
                }
            }

        
            PointerEventData eventData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            foreach (RaycastResult result in results)
            {
                if (result.gameObject.CompareTag(interactableGameElementTag))
                {
                    //if (lastHoveredUIElement != result.gameObject)
                    //{
                    //    soundEffectManager.PlayHoverSound();
                    //}

                    //lastHoveredUIElement = result.gameObject;

                    return true;
                }
            }
            
            return false;
        }
    }
}