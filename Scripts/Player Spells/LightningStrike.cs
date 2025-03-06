using Enemies;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
using AudioManagement;

namespace PlayerSpells
{

    // A spell that strikes all enemies in a given radius with a given amount of damage and then destroys itself.
    public class LightningStrike : PlayerSpell
    {
        [SerializeField] private float radius, damage;
        private LayerMask enemyLayer;

        public float Radius => radius;

        [Header("How long after starting should the spell destroy itself")]
        [SerializeField] private float spellLifetime;

        [SerializeField] private GameObject waterSplashPrefab;

        private GameObject waterSplash;

        private void Start()
        {
            enemyLayer = GamePrefs.Instance.EnemyLayer;
        }

        public void DoLightningStrike(List<GameObject> enemies)
        {

            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.lightningSound, transform.position);

            // See if the spell hit any water.
            if (IsOverWater())
            {
                waterSplash = Instantiate(waterSplashPrefab, transform.position, Quaternion.identity);
            }

            // Apply damage to all enemies in range.
            foreach (GameObject enemyObject in enemies)
            {
                Debug.Log("Lightning strike hit enemy: " + enemyObject.name);

                if (enemyObject == null)
                    continue;

                if (!enemyObject.TryGetComponent(out Enemy enemyComponent))
                    continue;

                enemyComponent.IntakeDamage(damage);
            }

            // Destroy the spell after a certain amount of time.
            StartCoroutine(DestroySelf());
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
            return Physics2D.Raycast(transform.position, Vector2.down, 0.1f, GamePrefs.Instance.WaterLayer);
        }
    }
}