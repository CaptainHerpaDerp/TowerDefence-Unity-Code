using UnityEngine;

namespace Interactables
{
    /// <summary>
    /// A class that represents an interactable object in the game world.
    /// </summary>
    public abstract class InteractableObject : MonoBehaviour
    {
        public abstract void Interact();
    }
}