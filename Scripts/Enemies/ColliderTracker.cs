using System.Collections.Generic;
using UnityEngine;
using Core.Character;

namespace Enemies
{
    /// <summary>
    /// Keeps track of all the colliders that are currently inside the trigger collider
    /// </summary>
    public class ColliderTracker : MonoBehaviour
    {
        public List<Character> Colliders { get; private set; }

        void Start()
        {
            Colliders = new();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision == null)
                return;

            if (collision.gameObject.GetComponent<Character>() != null && !Colliders.Contains(collision.gameObject.GetComponent<Character>()))
            {
                Colliders.Add(collision.gameObject.GetComponent<Character>());
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision == null)
                return;

            if (collision.gameObject.GetComponent<Character>() != null && Colliders.Contains(collision.gameObject.GetComponent<Character>()))
            {
                Colliders.Remove(collision.gameObject.GetComponent<Character>());
            }
        }
    }
}