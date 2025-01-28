
using UnityEngine;
namespace Interactables
{
    /// <summary>
    /// A fire that can be turned on and off, purely for easter egg purposes
    /// </summary>
    public class InteractableFire : InteractableObject
    {
        private bool fireOn = true;

        private Animator animator;

        private const string FIRE_ON = "Fire On", FIRE_OFF = "Fire Off";

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        public override void Interact()
        {
            fireOn = !fireOn;

            if (fireOn)
            {
                animator.Play(FIRE_ON);
            }

            else
            {
                animator.Play(FIRE_OFF);
            }
        }
    }
}