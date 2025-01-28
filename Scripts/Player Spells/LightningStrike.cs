using Enemies;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

namespace PlayerSpells
{

    // A spell that strikes all enemies in a given radius with a given amount of damage and then destroys itself.
    public class LightningStrike : PlayerSpell
    {
        [SerializeField] private float radius, damage;
        [SerializeField] private LayerMask enemyLayer;

        public float Radius => radius;

        [Header("How long after starting should the spell destroy itself")]
        [SerializeField] private float spellLifetime;

        [SerializeField] private LayerMask waterLayer;
        [SerializeField] private GameObject waterSplashPrefab;

        [SerializeField] private AudioClip lightningSound;
        [SerializeField] private AudioSource audioSource;

        private SoundEffectManager soundEffectManager;

        private GameObject waterSplash;

        // The effect will apply on instantiation.
        private void Start()
        {
            // Retrieve all enemies in the area of the spell.
            List<Enemy> enemiesInRange = GetEnemiesInArea();

            audioSource.PlayOneShot(lightningSound, volumeScale: SoundEffectManager.Instance.SoundEffectVolume / 2);

            // See if the spell hit any water.
            if (IsOverWater())
            {
                waterSplash = Instantiate(waterSplashPrefab, transform.position, Quaternion.identity);
            }

            // Apply damage to all enemies in range.
            foreach (Enemy enemy in enemiesInRange)
            {
                enemy.IntakeDamage(damage);
            }

            // Destroy the spell after a certain amount of time.
            StartCoroutine(DestroySelf());
        }

        /// <summary>
        /// Returns all enemies under a given layer mask in the given area of the spell.
        /// </summary>
        /// <returns></returns>
        private List<Enemy> GetEnemiesInArea()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, enemyLayer);
            List<Enemy> enemies = new List<Enemy>();

            foreach (Collider2D collider in colliders)
            {
                Enemy enemy = collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemies.Add(enemy);               
                }
            }

            return enemies;
        }

        private IEnumerator DestroySelf()
        {
            yield return new WaitForSeconds(spellLifetime);

            Destroy(waterSplash);
            Destroy(gameObject);
        }


        /// <summary>
        /// Returns true if the projectile is over water
        /// </summary>
        /// <returns></returns>
        protected bool IsOverWater()
        {
            return Physics2D.Raycast(transform.position, Vector2.down, 0.1f, waterLayer);
        }
    }
}